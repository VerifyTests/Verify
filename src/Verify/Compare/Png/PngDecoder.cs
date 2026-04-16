static class PngDecoder
{
    static ReadOnlySpan<byte> Signature => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    const uint ihdr = ('I' << 24) | ('H' << 16) | ('D' << 8) | 'R';
    const uint plte = ('P' << 24) | ('L' << 16) | ('T' << 8) | 'E';
    const uint idatType = ('I' << 24) | ('D' << 16) | ('A' << 8) | 'T';
    const uint iend = ('I' << 24) | ('E' << 16) | ('N' << 8) | 'D';
    const uint trns = ('t' << 24) | ('R' << 16) | ('N' << 8) | 'S';

    public static PngImage Decode(Stream stream)
    {
        Span<byte> sig = stackalloc byte[8];
        ReadExact(stream, sig);
        if (!sig.SequenceEqual(Signature))
        {
            throw new("Not a PNG (bad signature).");
        }

        var width = 0;
        var height = 0;
        byte colorType = 0;
        byte[]? palette = null;
        byte[]? transparency = null;
        using var idat = new MemoryStream();
        var seenIhdr = false;

        Span<byte> header = stackalloc byte[8];
        Span<byte> crc = stackalloc byte[4];
        Span<byte> ihdrData = stackalloc byte[13];

        while (true)
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
                    width = (int)ReadUInt32BigEndian(ihdrData);
                    height = (int)ReadUInt32BigEndian(ihdrData[4..]);
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
                    break;

                case plte:
                    if (intLength % 3 != 0)
                    {
                        throw new("Invalid PLTE length.");
                    }

                    palette = new byte[intLength];
                    ReadExact(stream, palette);
                    break;

                case trns:
                    transparency = new byte[intLength];
                    ReadExact(stream, transparency);
                    break;

                case idatType:
                    CopyExact(stream, idat, intLength);
                    break;

                case iend:
                    if (!seenIhdr)
                    {
                        throw new("PNG missing IHDR.");
                    }

                    ReadExact(stream, crc);
                    idat.Position = 0;
                    return Reconstruct(idat, width, height, colorType, palette, transparency);

                default:
                    // unknown chunk — skip
                    Skip(stream, intLength);
                    break;
            }

            ReadExact(stream, crc);
        }

        if (!seenIhdr)
        {
            throw new("PNG missing IHDR.");
        }

        idat.Position = 0;
        return Reconstruct(idat, width, height, colorType, palette, transparency);
    }

    static PngImage Reconstruct(Stream idat, int width, int height, byte colorType, byte[]? palette, byte[]? trns)
    {
        var rawChannels = colorType switch
        {
            0 => 1,
            2 => 3,
            3 => 1,
            4 => 2,
            6 => 4,
            _ => throw new("Unreachable.")
        };
        var stride = width * rawChannels;

        using var inflate = OpenInflate(idat);

        var rgba = new byte[width * height * 4];

        if (colorType == 6)
        {
            // Unfilter directly in the output buffer.
            var filterByte = new byte[1];
            var prevRow = new byte[stride];
            for (var y = 0; y < height; y++)
            {
                ReadExact(inflate, filterByte.AsSpan());
                var rowSpan = rgba.AsSpan(y * stride, stride);
                ReadExact(inflate, rowSpan);
                Unfilter(filterByte[0], rowSpan, prevRow, rawChannels);
                rowSpan.CopyTo(prevRow);
            }

            return new(width, height, rgba);
        }

        // Non-RGBA: one row scratch buffer, expand into rgba row-by-row.
        var curr = new byte[stride];
        var prev = new byte[stride];
        var filter = new byte[1];

        for (var y = 0; y < height; y++)
        {
            ReadExact(inflate, filter.AsSpan());
            ReadExact(inflate, curr.AsSpan());
            Unfilter(filter[0], curr, prev, rawChannels);
            ExpandRow(curr, rgba, y, width, colorType, palette, trns);
            (prev, curr) = (curr, prev);
        }

        return new(width, height, rgba);
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

    static void ExpandRow(byte[] src, byte[] rgba, int y, int width, byte colorType, byte[]? palette, byte[]? trns)
    {
        var dstRow = y * width * 4;
        switch (colorType)
        {
            case 0: // gray
                for (var x = 0; x < width; x++)
                {
                    var g = src[x];
                    var o = dstRow + x * 4;
                    rgba[o] = g;
                    rgba[o + 1] = g;
                    rgba[o + 2] = g;
                    rgba[o + 3] = 255;
                }

                break;
            case 2: // rgb
                for (var x = 0; x < width; x++)
                {
                    var o = dstRow + x * 4;
                    rgba[o] = src[x * 3];
                    rgba[o + 1] = src[x * 3 + 1];
                    rgba[o + 2] = src[x * 3 + 2];
                    rgba[o + 3] = 255;
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

                    var o = dstRow + x * 4;
                    rgba[o] = palette[index * 3];
                    rgba[o + 1] = palette[index * 3 + 1];
                    rgba[o + 2] = palette[index * 3 + 2];
                    rgba[o + 3] = trns is not null && index < trns.Length ? trns[index] : (byte)255;
                }

                break;
            case 4: // gray + alpha
                for (var x = 0; x < width; x++)
                {
                    var g = src[x * 2];
                    var o = dstRow + x * 4;
                    rgba[o] = g;
                    rgba[o + 1] = g;
                    rgba[o + 2] = g;
                    rgba[o + 3] = src[x * 2 + 1];
                }

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

    static void CopyExact(Stream source, Stream destination, int count)
    {
        Span<byte> buffer = stackalloc byte[4096];
        while (count > 0)
        {
            var toRead = Math.Min(buffer.Length, count);
            var read = source.Read(buffer[..toRead]);
            if (read == 0)
            {
                throw new("Unexpected end of PNG stream.");
            }

            destination.Write(buffer[..read]);
            count -= read;
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
}
