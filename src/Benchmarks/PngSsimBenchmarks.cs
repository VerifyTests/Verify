using System.IO.Compression;

[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
public class PngSsimBenchmarks
{
    byte[] smallPng = null!;
    byte[] mediumPng = null!;
    byte[] largePng = null!;

    PngImage smallImage;
    PngImage smallImageCopy;
    PngImage mediumImage;
    PngImage mediumImageCopy;
    PngImage largeImage;
    PngImage largeImageCopy;

    static readonly IReadOnlyDictionary<string, object> emptyContext = new Dictionary<string, object>();

    [GlobalSetup]
    public void Setup()
    {
        smallPng = BuildPng(16, 16, seed: 1);
        mediumPng = BuildPng(128, 128, seed: 2);
        largePng = BuildPng(512, 512, seed: 3);

        smallImage = PngDecoder.Decode(new MemoryStream(smallPng));
        smallImageCopy = PngDecoder.Decode(new MemoryStream(smallPng));
        mediumImage = PngDecoder.Decode(new MemoryStream(mediumPng));
        mediumImageCopy = PngDecoder.Decode(new MemoryStream(mediumPng));
        largeImage = PngDecoder.Decode(new MemoryStream(largePng));
        largeImageCopy = PngDecoder.Decode(new MemoryStream(largePng));
    }

    [Benchmark]
    public int Decode_Small() => PngDecoder.Decode(new MemoryStream(smallPng)).Width;

    [Benchmark]
    public int Decode_Medium() => PngDecoder.Decode(new MemoryStream(mediumPng)).Width;

    [Benchmark]
    public int Decode_Large() => PngDecoder.Decode(new MemoryStream(largePng)).Width;

    [Benchmark]
    public double Ssim_Small() => Ssim.Compare(smallImage, smallImageCopy);

    [Benchmark]
    public double Ssim_Medium() => Ssim.Compare(mediumImage, mediumImageCopy);

    [Benchmark]
    public double Ssim_Large() => Ssim.Compare(largeImage, largeImageCopy);

    [Benchmark]
    public async Task Compare_Small() =>
        await PngSsimComparer.Compare(new MemoryStream(smallPng), new MemoryStream(smallPng), emptyContext);

    [Benchmark]
    public async Task Compare_Medium() =>
        await PngSsimComparer.Compare(new MemoryStream(mediumPng), new MemoryStream(mediumPng), emptyContext);

    [Benchmark]
    public async Task Compare_Large() =>
        await PngSsimComparer.Compare(new MemoryStream(largePng), new MemoryStream(largePng), emptyContext);

    static byte[] BuildPng(int width, int height, int seed)
    {
        var rgba = new byte[width * height * 4];
        new Random(seed).NextBytes(rgba);
        for (var i = 3; i < rgba.Length; i += 4)
        {
            rgba[i] = 255;
        }

        // Inline a minimal RGBA PNG builder so this file has no test-project dependency.
        var raw = new byte[(width * 4 + 1) * height];
        for (var y = 0; y < height; y++)
        {
            raw[y * (width * 4 + 1)] = 0; // filter None
            Buffer.BlockCopy(rgba, y * width * 4, raw, y * (width * 4 + 1) + 1, width * 4);
        }

        var compressed = ZlibCompress(raw);
        using var stream = new MemoryStream();
        stream.Write([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], 0, 8);

        var ihdr = new byte[13];
        WriteBe(ihdr, 0, (uint)width);
        WriteBe(ihdr, 4, (uint)height);
        ihdr[8] = 8;
        ihdr[9] = 6;
        WriteChunk(stream, "IHDR"u8.ToArray(), ihdr);
        WriteChunk(stream, "IDAT"u8.ToArray(), compressed);
        WriteChunk(stream, "IEND"u8.ToArray(), []);
        return stream.ToArray();
    }

    static void WriteChunk(Stream stream, byte[] type, byte[] data)
    {
        var header = new byte[4];
        WriteBe(header, 0, (uint)data.Length);
        stream.Write(header, 0, 4);
        stream.Write(type, 0, 4);
        stream.Write(data, 0, data.Length);

        var combined = new byte[type.Length + data.Length];
        Buffer.BlockCopy(type, 0, combined, 0, type.Length);
        Buffer.BlockCopy(data, 0, combined, type.Length, data.Length);
        var crc = Crc32(combined);
        var crcBytes = new byte[4];
        WriteBe(crcBytes, 0, crc);
        stream.Write(crcBytes, 0, 4);
    }

    static byte[] ZlibCompress(byte[] data)
    {
        using var output = new MemoryStream();
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
        const uint mod = 65521;
        uint a = 1;
        uint b = 0;
        foreach (var item in data)
        {
            a = (a + item) % mod;
            b = (b + a) % mod;
        }

        return (b << 16) | a;
    }

    static void WriteBe(byte[] buffer, int offset, uint value)
    {
        buffer[offset] = (byte)((value >> 24) & 0xFF);
        buffer[offset + 1] = (byte)((value >> 16) & 0xFF);
        buffer[offset + 2] = (byte)((value >> 8) & 0xFF);
        buffer[offset + 3] = (byte)(value & 0xFF);
    }

    static readonly uint[] crcTable = BuildCrcTable();

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
