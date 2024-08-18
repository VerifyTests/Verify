public class ReadStringBuilderWithFixedLinesTests(ITestOutputHelper output)
{
    [Fact]
    public async Task All()
    {
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        var enumerateFiles = Directory.EnumerateFiles(
            solutionDirectory,
            "*.verified.txt",
            SearchOption.AllDirectories)
            .ToList();
        var startNew = Stopwatch.StartNew();
        foreach (var file in enumerateFiles)
        {
            using var reader = IoHelpers.OpenRead(file);
            await IoHelpers.ReadStringBuilderWithFixedLines(reader);
        }

        output.WriteLine(startNew.ElapsedMilliseconds + "ms");
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("\t", "\t")]
    [InlineData("\r", "\n")]
    [InlineData("\n", "\n")]
    [InlineData("\n\n", "\n\n")]
    [InlineData("\n\n\n", "\n\n\n")]
    [InlineData("\r\n", "\n")]
    [InlineData("\r\r", "\n\n")]
    [InlineData("\r\r\r", "\n\n\n")]
    [InlineData("\r\r\n", "\n\n")]
    [InlineData("\r\n\n", "\n\n")]
    [InlineData("\r\n\r\n", "\n\n")]
    public async Task Run(string input, string expected)
    {
        using var reader = new StringReader(input);
        var result = await IoHelpers.ReadStringBuilderWithFixedLines(reader);
        Assert.Equal(expected, result.ToString());

        using var withPrefixReader = new StringReader($" {input}");
        var withPrefixResult = await IoHelpers.ReadStringBuilderWithFixedLines(withPrefixReader);
        Assert.Equal($" {expected}", withPrefixResult.ToString());

        using var withSuffixReader = new StringReader($"{input} ");
        var withSuffixResult = await IoHelpers.ReadStringBuilderWithFixedLines(withSuffixReader);
        Assert.Equal($"{expected} ", withSuffixResult.ToString());

        using var withPaddedReader = new StringReader($" {input} ");
        var withPaddedResult = await IoHelpers.ReadStringBuilderWithFixedLines(withPaddedReader);
        Assert.Equal($" {expected} ", withPaddedResult.ToString());
    }

    [Theory]
    [InlineData(1022)]
    [InlineData(1023)]
    [InlineData(1024)]
    [InlineData(1025)]
    [InlineData(1026)]
    [InlineData(2046)]
    [InlineData(2047)]
    [InlineData(2048)]
    [InlineData(2049)]
    [InlineData(2050)]
    public async Task Edges(int length)
    {
        var input = new string(' ', length);

        using var returnReader = new StringReader(input + "\r");
        var returnResult = await IoHelpers.ReadStringBuilderWithFixedLines(returnReader);
        Assert.Equal(input + "\n", returnResult.ToString());

        using var newLineReader = new StringReader(input + "\n");
        var newlineResult = await IoHelpers.ReadStringBuilderWithFixedLines(newLineReader);
        Assert.Equal(input + "\n", newlineResult.ToString());

         using var returnNewLineReader = new StringReader(input + "\r\n");
         var returnNewLineResult = await IoHelpers.ReadStringBuilderWithFixedLines(returnNewLineReader);
         Assert.Equal(input + "\n", returnNewLineResult.ToString());
    }
}