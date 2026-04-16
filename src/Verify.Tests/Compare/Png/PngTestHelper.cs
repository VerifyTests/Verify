static class PngTestHelper
{
    static readonly byte[] signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    static readonly uint[] crcTable = BuildCrcTable();

    public static byte[] EncodeRgba(int width, int height, byte[] rgba)
    {
        if (rgba.Length != width * height * 4)
        {
            throw new ArgumentException("rgba length mismatch");
        }

        var raw = AddFilterBytes(rgba, width * 4, height);
        var ihdr = BuildIhdr(width, height, colorType: 6);
        return BuildPng(ihdr, plte: null, trns: null, raw: raw);
    }

    public static byte[] EncodeRgb(int width, int height, byte[] rgb)
    {
        var raw = AddFilterBytes(rgb, width * 3, height);
        var ihdr = BuildIhdr(width, height, colorType: 2);
        return BuildPng(ihdr, null, null, raw);
    }

    public static byte[] EncodeGray(int width, int height, byte[] gray)
    {
        var raw = AddFilterBytes(gray, width, height);
        var ihdr = BuildIhdr(width, height, colorType: 0);
        return BuildPng(ihdr, null, null, raw);
    }

    public static byte[] EncodePaletted(int width, int height, byte[] indices, byte[] palette, byte[]? trns = null)
    {
        var raw = AddFilterBytes(indices, width, height);
        var ihdr = BuildIhdr(width, height, colorType: 3);
        return BuildPng(ihdr, palette, trns, raw);
    }

    static byte[] AddFilterBytes(byte[] data, int stride, int height)
    {
        var result = new byte[(stride + 1) * height];
        for (var y = 0; y < height; y++)
        {
            result[y * (stride + 1)] = 0; // filter None
            Buffer.BlockCopy(data, y * stride, result, y * (stride + 1) + 1, stride);
        }

        return result;
    }

    static byte[] BuildIhdr(int width, int height, byte colorType)
    {
        var data = new byte[13];
        WriteUInt32Be(data, 0, (uint)width);
        WriteUInt32Be(data, 4, (uint)height);
        data[8] = 8; // bit depth
        data[9] = colorType;
        data[10] = 0; // compression
        data[11] = 0; // filter method
        data[12] = 0; // interlace
        return data;
    }

    static byte[] BuildPng(byte[] ihdr, byte[]? plte, byte[]? trns, byte[] raw)
    {
        var compressed = ZlibCompress(raw);
        using var stream = new MemoryStream();
        stream.Write(signature, 0, signature.Length);
        WriteChunk(stream, "IHDR", ihdr);
        if (plte is not null)
        {
            WriteChunk(stream, "PLTE", plte);
        }

        if (trns is not null)
        {
            WriteChunk(stream, "tRNS", trns);
        }

        WriteChunk(stream, "IDAT", compressed);
        WriteChunk(stream, "IEND", []);
        return stream.ToArray();
    }

    static void WriteChunk(Stream stream, string type, byte[] data)
    {
        var header = new byte[4];
        WriteUInt32Be(header, 0, (uint)data.Length);
        stream.Write(header, 0, 4);
        var typeBytes = new[] { (byte)type[0], (byte)type[1], (byte)type[2], (byte)type[3] };
        stream.Write(typeBytes, 0, 4);
        stream.Write(data, 0, data.Length);

        var combined = new byte[4 + data.Length];
        Buffer.BlockCopy(typeBytes, 0, combined, 0, 4);
        Buffer.BlockCopy(data, 0, combined, 4, data.Length);
        var crc = Crc32(combined);
        var crcBytes = new byte[4];
        WriteUInt32Be(crcBytes, 0, crc);
        stream.Write(crcBytes, 0, 4);
    }

    static byte[] ZlibCompress(byte[] data)
    {
        using var output = new MemoryStream();
        // zlib header (deflate, 32K window, default level, no dict)
        output.WriteByte(0x78);
        output.WriteByte(0x9C);
        using (var deflate = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            deflate.Write(data, 0, data.Length);
        }

        var adler = Adler32(data);
        output.WriteByte((byte)((adler >> 24) & 0xFF));
        output.WriteByte((byte)((adler >> 16) & 0xFF));
        output.WriteByte((byte)((adler >> 8) & 0xFF));
        output.WriteByte((byte)(adler & 0xFF));
        return output.ToArray();
    }

    static uint Adler32(byte[] data)
    {
        const uint modAdler = 65521;
        uint a = 1;
        uint b = 0;
        foreach (var item in data)
        {
            a = (a + item) % modAdler;
            b = (b + a) % modAdler;
        }

        return (b << 16) | a;
    }

    static void WriteUInt32Be(byte[] buffer, int offset, uint value)
    {
        buffer[offset] = (byte)((value >> 24) & 0xFF);
        buffer[offset + 1] = (byte)((value >> 16) & 0xFF);
        buffer[offset + 2] = (byte)((value >> 8) & 0xFF);
        buffer[offset + 3] = (byte)(value & 0xFF);
    }

    static uint[] BuildCrcTable()
    {
        var table = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            var c = n;
            for (var k = 0; k < 8; k++)
            {
                c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
            }

            table[n] = c;
        }

        return table;
    }

    static uint Crc32(byte[] data)
    {
        var c = 0xFFFFFFFF;
        foreach (var item in data)
        {
            c = crcTable[(c ^ item) & 0xFF] ^ (c >> 8);
        }

        return c ^ 0xFFFFFFFF;
    }
}
