public class CounterTests
{

    #region CounterTryConvert

    [Fact]
    public Task CounterTryConvert()
    {
        var settings = new VerifySettings();
        settings.AddScrubber((builder, counter) =>
        {
            var values = builder.ToString().Split();
            builder.Clear();
            foreach (var value in values)
            {
                if (counter.TryConvert(value, out var result))
                {
                    builder.Append(result);
                }
                else
                {
                    builder.Append(value);
                }

                builder.Append(' ');
            }
        });

        return Verify("The user with id c9cb98bf-3def-415e-a009-7d58055f5ffc was created on 2022-10-12", settings);
    }

    #endregion
}