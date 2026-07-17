public class PngImageTests
{
    [Fact]
    public void Valid()
    {
        var rgba = new byte[2 * 3 * 4];
        var image = new PngImage(2, 3, rgba);
        Assert.Equal(2, image.Width);
        Assert.Equal(3, image.Height);
        Assert.Same(rgba, image.Rgba);
    }

    [Fact]
    public void Zero_Width_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PngImage(0, 3, []));

    [Fact]
    public void Zero_Height_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PngImage(3, 0, []));

    [Fact]
    public void Buffer_Length_Mismatch_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() => new PngImage(2, 2, new byte[15]));
        Assert.Contains("Expected: 16", exception.Message);
        Assert.Contains("Actual: 15", exception.Message);
    }
}
