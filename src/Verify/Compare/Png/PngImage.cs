namespace VerifyTests;

public readonly struct PngImage
{
    public PngImage(int width, int height, byte[] rgba)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "Must be at least 1.");
        }

        if (height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "Must be at least 1.");
        }

        Ensure.NotNull(rgba);
        if (rgba.Length != (long)width * height * 4)
        {
            throw new ArgumentException($"Length must be width * height * 4. Expected: {(long)width * height * 4}. Actual: {rgba.Length}.", nameof(rgba));
        }

        Width = width;
        Height = height;
        Rgba = rgba;
    }

    public int Width { get; }
    public int Height { get; }
    public byte[] Rgba { get; }
}
