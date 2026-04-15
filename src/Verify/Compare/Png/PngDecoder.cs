namespace VerifyTests;

static class PngDecoder
{
    static readonly byte[] signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    const uint ihdr = ('I' << 24) | ('H' << 16) | ('D' << 8) | 'R';
    const uint plte = ('P' << 24) | ('L' << 16) | ('T' << 8) | 'E';
    const uint idatType = ('I' << 24) | ('D' << 16) | ('A' << 8) | 'T';
    const uint iend = ('I' << 24) | ('E' << 16) | ('N' << 8) | 'D';
    const uint trns = ('t' << 24) | ('R' << 16) | ('N' << 8) | 'S';

    static uint ChunkType(byte a, byte b, byte c, byte d) =>
        ((uint)a << 24) | ((uint)b << 16) | ((uint)c << 8) | d;

    public static PngImage Decode(Stream stream)
    {
        var sig = new byte[8];
        ReadExact(stream, sig, 0, 8);
        for (var i = 0; i < 8; i++)
        {
            if (sig[i] != signature[i])
            {
                throw new("Not a PNG (bad signature).");
            }
        }

        var width = 0;
        var height = 0;
        byte colorType = 0;
        byte[]? palette = null;
        byte[]? transparency = null;
        using var idat = new MemoryStream();
        var seenIhdr = false;

        while (true)
        {
            var header = new byte[8];
            ReadExact(stream, header, 0, 8);
            var length = ReadUInt32BigEndian(header, 0);
            var type = ChunkType(header[4], header[5], header[6], header[7]);

            if (length > int.MaxValue)
            {
                throw new("PNG chunk too large.");
            }

            var data = new byte[length];
            if (length > 0)
            {
                ReadExact(stream, data, 0, (int)length);
            }

            // skip CRC
            ReadExact(stream, new byte[4], 0, 4);

            switch (type)
            {
                case ihdr:
                    if (length != 13)
                    {
                        throw new("Invalid IHDR length.");
                    }

                    width = (int)ReadUInt32BigEndian(data, 0);
                    height = (int)ReadUInt32BigEndian(data, 4);
                    var bitDepth = data[8];
                    colorType = data[9];
                    var compression = data[10];
                    var filter = data[11];
                    var interlace = data[12];
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
                    if (length % 3 != 0)
                    {
                        throw new("Invalid PLTE length.");
                    }

                    palette = data;
                    break;
                case trns:
                    transparency = data;
                    break;
                case idatType:
                    idat.Write(data, 0, data.Length);
                    break;
                case iend:
                    if (!seenIhdr)
                    {
                        throw new("PNG missing IHDR.");
                    }

                    idat.Position = 0;
                    var raw = Inflate(idat);
                    var pixels = Reconstruct(raw, width, height, colorType, palette, transparency);
                    return new(width, height, pixels);
            }
        }
    }

    static byte[] Inflate(MemoryStream zlibData)
    {
#if NET6_0_OR_GREATER
        using var inflate = new System.IO.Compression.ZLibStream(zlibData, System.IO.Compression.CompressionMode.Decompress, leaveOpen: true);
        using var output = new MemoryStream();
        inflate.CopyTo(output);
        return output.ToArray();
#else
        // Skip 2-byte zlib header, trailing Adler-32 ignored by DeflateStream EOF.
        zlibData.ReadByte();
        zlibData.ReadByte();
        using var inflate = new System.IO.Compression.DeflateStream(zlibData, System.IO.Compression.CompressionMode.Decompress, leaveOpen: true);
        using var output = new MemoryStream();
        inflate.CopyTo(output);
        return output.ToArray();
#endif
    }

    static byte[] Reconstruct(byte[] raw, int width, int height, byte colorType, byte[]? palette, byte[]? trns)
    {
        // channels in the raw stream (pre-expansion for palette)
        var rawChannels = colorType switch
        {
            0 => 1, // gray
            2 => 3, // rgb
            3 => 1, // palette index
            4 => 2, // gray + alpha
            6 => 4, // rgba
            _ => throw new("Unreachable.")
        };
        var stride = width * rawChannels;
        var expected = (stride + 1) * height;
        if (raw.Length < expected)
        {
            throw new($"PNG data too short: expected {expected}, got {raw.Length}.");
        }

        var unfiltered = new byte[stride * height];
        var prevRow = new byte[stride];
        var currRow = new byte[stride];
        var rawPos = 0;
        for (var y = 0; y < height; y++)
        {
            var filter = raw[rawPos++];
            Buffer.BlockCopy(raw, rawPos, currRow, 0, stride);
            rawPos += stride;
            Unfilter(filter, currRow, prevRow, rawChannels);
            Buffer.BlockCopy(currRow, 0, unfiltered, y * stride, stride);
            (prevRow, currRow) = (currRow, prevRow);
        }

        // Expand to RGBA8
        var rgba = new byte[width * height * 4];
        switch (colorType)
        {
            case 0: // gray
                for (var i = 0; i < width * height; i++)
                {
                    var g = unfiltered[i];
                    rgba[i * 4] = g;
                    rgba[i * 4 + 1] = g;
                    rgba[i * 4 + 2] = g;
                    rgba[i * 4 + 3] = 255;
                }

                break;
            case 2: // rgb
                for (var i = 0; i < width * height; i++)
                {
                    rgba[i * 4] = unfiltered[i * 3];
                    rgba[i * 4 + 1] = unfiltered[i * 3 + 1];
                    rgba[i * 4 + 2] = unfiltered[i * 3 + 2];
                    rgba[i * 4 + 3] = 255;
                }

                break;
            case 3: // palette
                if (palette is null)
                {
                    throw new("Paletted PNG missing PLTE chunk.");
                }

                var paletteEntries = palette.Length / 3;
                for (var i = 0; i < width * height; i++)
                {
                    var index = unfiltered[i];
                    if (index >= paletteEntries)
                    {
                        throw new("PNG palette index out of range.");
                    }

                    rgba[i * 4] = palette[index * 3];
                    rgba[i * 4 + 1] = palette[index * 3 + 1];
                    rgba[i * 4 + 2] = palette[index * 3 + 2];
                    rgba[i * 4 + 3] = trns is not null && index < trns.Length ? trns[index] : (byte)255;
                }

                break;
            case 4: // gray + alpha
                for (var i = 0; i < width * height; i++)
                {
                    var g = unfiltered[i * 2];
                    rgba[i * 4] = g;
                    rgba[i * 4 + 1] = g;
                    rgba[i * 4 + 2] = g;
                    rgba[i * 4 + 3] = unfiltered[i * 2 + 1];
                }

                break;
            case 6: // rgba
                Buffer.BlockCopy(unfiltered, 0, rgba, 0, unfiltered.Length);
                break;
        }

        return rgba;
    }

    static void Unfilter(byte filter, byte[] curr, byte[] prev, int bpp)
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
                    var left = i >= bpp ? (int)curr[i - bpp] : 0;
                    int up = prev[i];
                    var upLeft = i >= bpp ? (int)prev[i - bpp] : 0;
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

    static void ReadExact(Stream stream, byte[] buffer, int offset, int count)
    {
        while (count > 0)
        {
            var read = stream.Read(buffer, offset, count);
            if (read == 0)
            {
                throw new("Unexpected end of PNG stream.");
            }

            offset += read;
            count -= read;
        }
    }

    static uint ReadUInt32BigEndian(byte[] data, int offset) =>
        ((uint)data[offset] << 24) |
        ((uint)data[offset + 1] << 16) |
        ((uint)data[offset + 2] << 8) |
        data[offset + 3];
}

readonly struct PngImage(int width, int height, byte[] rgba)
{
    public int Width { get; } = width;
    public int Height { get; } = height;
    public byte[] Rgba { get; } = rgba;
}
