namespace VerifyTests;

// Streaming PNG row reader shared by PngDecoder (full decode) and the streaming SSIM compare.
// Parses IHDR/PLTE/tRNS, then inflates IDAT directly (no full compressed buffer) and yields
// unfiltered, RGBA-expanded rows on demand. Row scratch is rented from ArrayPool so repeated
// decodes do not churn the heap. The decoded output buffer, when one is needed, is owned by the
// caller (PngDecoder allocates it because it escapes into a PngImage).
sealed class PngReader : IDisposable
{
    static ReadOnlySpan<byte> Signature => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    const uint ihdr = ('I' << 24) | ('H' << 16) | ('D' << 8) | 'R';
    const uint plte = ('P' << 24) | ('L' << 16) | ('T' << 8) | 'E';
    const uint idatType = ('I' << 24) | ('D' << 16) | ('A' << 8) | 'T';
    const uint iend = ('I' << 24) | ('E' << 16) | ('N' << 8) | 'D';
    const uint trns = ('t' << 24) | ('R' << 16) | ('N' << 8) | 'S';

    public int Width { get; }
    public int Height { get; }

    readonly byte colorType;
    readonly byte[]? palette;
    readonly byte[]? transparency;
    readonly int rawChannels;
    readonly int stride;

    readonly IdatStream idatStream;
    readonly Stream inflate;
    byte[] prev;
    byte[] curr;
    bool disposed;

    public PngReader(Stream stream)
    {
        Span<byte> sig = stackalloc byte[8];
        ReadExact(stream, sig);
        if (!sig.SequenceEqual(Signature))
        {
            throw new("Not a PNG (bad signature).");
        }

        var seenIhdr = false;
        var foundIdat = false;
        var firstIdatLength = 0;
        Span<byte> header = stackalloc byte[8];
        Span<byte> crc = stackalloc byte[4];
        Span<byte> ihdrData = stackalloc byte[13];

        while (!foundIdat)
        {
            if (stream.ReadAtLeast(header, header.Length, throwOnEndOfStream: false) < header.Length)
            {
                break;
            }

            var length = ReadUInt32BigEndian(header);
            var type = ((uint)header[4] << 24) | ((uint)header[5] << 16) | ((uint)header[6] << 8) | header[7];

            if (length > int.MaxValue)
            {
                break;
            }

            var intLength = (int)length;

            switch (type)
            {
                case ihdr:
                    if (intLength != 13)
                    {
                        throw new("Invalid IHDR length.");
                    }

                    ReadExact(stream, ihdrData);
                    Width = (int)ReadUInt32BigEndian(ihdrData);
                    Height = (int)ReadUInt32BigEndian(ihdrData[4..]);
                    var bitDepth = ihdrData[8];
                    colorType = ihdrData[9];
                    var compression = ihdrData[10];
                    var filter = ihdrData[11];
                    var interlace = ihdrData[12];
                    if (compression != 0 || filter != 0)
                    {
                        throw new("Unsupported PNG compression/filter method.");
                    }

                    if (interlace != 0)
                    {
                        throw new("Unsupported PNG variant: Adam7 interlacing not supported.");
                    }

                    if (bitDepth != 8)
                    {
                        throw new($"Unsupported PNG bit depth: {bitDepth}. Only 8-bit supported.");
                    }

                    if (colorType != 0 && colorType != 2 && colorType != 3 && colorType != 4 && colorType != 6)
                    {
                        throw new($"Unsupported PNG color type: {colorType}.");
                    }

                    seenIhdr = true;
                    ReadExact(stream, crc);
                    break;

                case plte:
                    if (intLength % 3 != 0)
                    {
                        throw new("Invalid PLTE length.");
                    }

                    palette = new byte[intLength];
                    ReadExact(stream, palette);
                    ReadExact(stream, crc);
                    break;

                case trns:
                    transparency = new byte[intLength];
                    ReadExact(stream, transparency);
                    ReadExact(stream, crc);
                    break;

                case idatType:
                    // Hand off to IdatStream mid-chunk: it serves this payload then any subsequent
                    // IDAT chunks. Deliberately do not read the payload or CRC here.
                    firstIdatLength = intLength;
                    foundIdat = true;
                    break;

                case iend:
                    throw new(seenIhdr ? "PNG missing IDAT." : "PNG missing IHDR.");

                default:
                    Skip(stream, intLength);
                    ReadExact(stream, crc);
                    break;
            }
        }

        if (!seenIhdr)
        {
            throw new("PNG missing IHDR.");
        }

        if (!foundIdat)
        {
            throw new("PNG missing IDAT.");
        }

        rawChannels = colorType switch
        {
            0 => 1,
            2 => 3,
            3 => 1,
            4 => 2,
            6 => 4,
            _ => throw new("Unreachable.")
        };
        stride = Width * rawChannels;

        idatStream = new(stream, firstIdatLength);
        inflate = OpenInflate(idatStream);

        prev = ArrayPool<byte>.Shared.Rent(stride);
        curr = ArrayPool<byte>.Shared.Rent(stride);
        Array.Clear(prev, 0, stride);
    }

    // Reads the next scanline, unfilters it, and expands it into dest as RGBA. dest must be Width * 4.
    public void ReadRgbaRow(Span<byte> dest)
    {
        var filter = inflate.ReadByte();
        if (filter < 0)
        {
            throw new("Unexpected end of PNG stream.");
        }

        var currSpan = curr.AsSpan(0, stride);
        ReadExact(inflate, currSpan);
        Unfilter((byte)filter, currSpan, prev.AsSpan(0, stride), rawChannels);
        ExpandRow(currSpan, dest, Width, colorType, palette, transparency);
        (prev, curr) = (curr, prev);
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        inflate.Dispose();
        idatStream.Dispose();
        ArrayPool<byte>.Shared.Return(prev);
        ArrayPool<byte>.Shared.Return(curr);
    }

    static Stream OpenInflate(Stream zlibData)
    {
#if NET6_0_OR_GREATER
        var inflate = new ZLibStream(zlibData, CompressionMode.Decompress, leaveOpen: true);
#else
        zlibData.ReadByte();
        zlibData.ReadByte();
        var inflate = new DeflateStream(zlibData, CompressionMode.Decompress, leaveOpen: true);
#endif
        return new BufferedStream(inflate, 8192);
    }

    static void ExpandRow(ReadOnlySpan<byte> src, Span<byte> dest, int width, byte colorType, byte[]? palette, byte[]? trns)
    {
        switch (colorType)
        {
            case 0: // gray
                for (var x = 0; x < width; x++)
                {
                    var g = src[x];
                    var o = x * 4;
                    dest[o] = g;
                    dest[o + 1] = g;
                    dest[o + 2] = g;
                    dest[o + 3] = 255;
                }

                break;
            case 2: // rgb
                for (var x = 0; x < width; x++)
                {
                    var o = x * 4;
                    dest[o] = src[x * 3];
                    dest[o + 1] = src[x * 3 + 1];
                    dest[o + 2] = src[x * 3 + 2];
                    dest[o + 3] = 255;
                }

                break;
            case 3: // palette
                if (palette is null)
                {
                    throw new("Paletted PNG missing PLTE chunk.");
                }

                var paletteEntries = palette.Length / 3;
                for (var x = 0; x < width; x++)
                {
                    var index = src[x];
                    if (index >= paletteEntries)
                    {
                        throw new("PNG palette index out of range.");
                    }

                    var o = x * 4;
                    dest[o] = palette[index * 3];
                    dest[o + 1] = palette[index * 3 + 1];
                    dest[o + 2] = palette[index * 3 + 2];
                    dest[o + 3] = trns is not null && index < trns.Length ? trns[index] : (byte)255;
                }

                break;
            case 4: // gray + alpha
                for (var x = 0; x < width; x++)
                {
                    var g = src[x * 2];
                    var o = x * 4;
                    dest[o] = g;
                    dest[o + 1] = g;
                    dest[o + 2] = g;
                    dest[o + 3] = src[x * 2 + 1];
                }

                break;
            case 6: // rgba
                src[..(width * 4)].CopyTo(dest);
                break;
        }
    }

    static void Unfilter(byte filter, Span<byte> curr, ReadOnlySpan<byte> prev, int bpp)
    {
        switch (filter)
        {
            case 0:
                return;
            case 1: // Sub
                for (var i = bpp; i < curr.Length; i++)
                {
                    curr[i] = (byte)(curr[i] + curr[i - bpp]);
                }

                return;
            case 2: // Up
                for (var i = 0; i < curr.Length; i++)
                {
                    curr[i] = (byte)(curr[i] + prev[i]);
                }

                return;
            case 3: // Average
                for (var i = 0; i < curr.Length; i++)
                {
                    var left = i >= bpp ? curr[i - bpp] : 0;
                    curr[i] = (byte)(curr[i] + (left + prev[i]) / 2);
                }

                return;
            case 4: // Paeth
                for (var i = 0; i < curr.Length; i++)
                {
                    var left = i >= bpp ? curr[i - bpp] : 0;
                    int up = prev[i];
                    var upLeft = i >= bpp ? prev[i - bpp] : 0;
                    curr[i] = (byte)(curr[i] + Paeth(left, up, upLeft));
                }

                return;
            default:
                throw new($"Unknown PNG filter type: {filter}.");
        }
    }

    static int Paeth(int a, int b, int c)
    {
        var p = a + b - c;
        var pa = Math.Abs(p - a);
        var pb = Math.Abs(p - b);
        var pc = Math.Abs(p - c);
        if (pa <= pb && pa <= pc)
        {
            return a;
        }

        return pb <= pc ? b : c;
    }

    static void ReadExact(Stream stream, Span<byte> buffer)
    {
        while (buffer.Length > 0)
        {
            var read = stream.Read(buffer);
            if (read == 0)
            {
                throw new("Unexpected end of PNG stream.");
            }

            buffer = buffer[read..];
        }
    }

    static void Skip(Stream stream, int count)
    {
        Span<byte> buffer = stackalloc byte[1024];
        while (count > 0)
        {
            var toRead = Math.Min(buffer.Length, count);
            var read = stream.Read(buffer[..toRead]);
            if (read == 0)
            {
                throw new("Unexpected end of PNG stream.");
            }

            count -= read;
        }
    }

    static uint ReadUInt32BigEndian(ReadOnlySpan<byte> data) =>
        ((uint)data[0] << 24) |
        ((uint)data[1] << 16) |
        ((uint)data[2] << 8) |
        data[3];

    // Presents the concatenated payloads of consecutive IDAT chunks as one continuous stream,
    // transparently skipping chunk CRCs and headers, so the zlib/deflate layer can inflate
    // directly from the source without the whole compressed image being buffered first.
    sealed class IdatStream(Stream source, int firstChunkLength) : Stream
    {
        int chunkRemaining = firstChunkLength;
        bool ended;

        public override int Read(byte[] buffer, int offset, int count) =>
            ReadCore(buffer.AsSpan(offset, count));

#if NET6_0_OR_GREATER
        public override int Read(Span<byte> buffer) =>
            ReadCore(buffer);
#endif

        int ReadCore(Span<byte> buffer)
        {
            var total = 0;
            while (buffer.Length > 0 && !ended)
            {
                if (chunkRemaining == 0 && !AdvanceToNextIdat())
                {
                    ended = true;
                    break;
                }

                var toRead = Math.Min(buffer.Length, chunkRemaining);
                var read = source.Read(buffer[..toRead]);
                if (read == 0)
                {
                    ended = true;
                    break;
                }

                chunkRemaining -= read;
                buffer = buffer[read..];
                total += read;
            }

            return total;
        }

        bool AdvanceToNextIdat()
        {
            Span<byte> temp = stackalloc byte[8];
            // Skip the CRC of the chunk just consumed.
            if (source.ReadAtLeast(temp[..4], 4, throwOnEndOfStream: false) < 4)
            {
                return false;
            }

            // Read the next chunk header; continue only while it is another IDAT.
            if (source.ReadAtLeast(temp, temp.Length, throwOnEndOfStream: false) < temp.Length)
            {
                return false;
            }

            var length = ((uint)temp[0] << 24) | ((uint)temp[1] << 16) | ((uint)temp[2] << 8) | temp[3];
            var type = ((uint)temp[4] << 24) | ((uint)temp[5] << 16) | ((uint)temp[6] << 8) | temp[7];
            if (type != idatType || length > int.MaxValue)
            {
                return false;
            }

            chunkRemaining = (int)length;
            return true;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
