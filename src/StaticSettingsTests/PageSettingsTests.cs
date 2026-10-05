// The settings a converter of a paged document reads, set for every verification. Lives here since
// they are static, and this project runs serially with BaseTest resetting between tests.
public class PageSettingsTests :
    BaseTest
{
    [Fact]
    public void NothingSetIsEveryPageWithItsTextInTheInfo()
    {
        var context = new VerifySettings().Context;

        Assert.Equal(PageTextPlacement.InInfo, context.PageTextPlacement());
        Assert.True(context.IsPageIncluded(1));
        Assert.True(context.IsPageIncluded(500));
        Assert.False(context.IsDerivedTargetExcluded("png"));
    }

    [Fact]
    public void GlobalSettingsAreReadFromAnyContext()
    {
        VerifierSettings.PageText(PageTextPlacement.PerPage);
        VerifierSettings.PagesToInclude(2);
        VerifierSettings.ExcludeDerivedTargets("png");

        var context = new VerifySettings().Context;

        Assert.Equal(PageTextPlacement.PerPage, context.PageTextPlacement());
        Assert.True(context.IsPageIncluded(2));
        Assert.False(context.IsPageIncluded(3));
        Assert.True(context.IsDerivedTargetExcluded("png"));
        Assert.True(context.IsDerivedTargetExcluded("PNG"));
        Assert.False(context.IsDerivedTargetExcluded("txt"));
        // Only what was derived. A png that is the thing being verified is not excluded
        Assert.False(context.IsTargetExcluded("png"));
    }

    [Fact]
    public void AGlobalFilterCanBeAnyPredicate()
    {
        VerifierSettings.PagesToInclude(_ => _ % 2 == 0);

        var context = new VerifySettings().Context;

        Assert.False(context.IsPageIncluded(1));
        Assert.True(context.IsPageIncluded(2));
    }

    /// <summary>
    /// The verification's own setting is the one used, and its exclusions add to the global ones.
    /// </summary>
    [Fact]
    public void TheVerificationsSettingWins()
    {
        VerifierSettings.PageText(PageTextPlacement.PerPage);
        VerifierSettings.PagesToInclude(1);
        VerifierSettings.ExcludeDerivedTargets("png");

        var settings = new VerifySettings();
        settings.PageText(PageTextPlacement.None);
        settings.PagesToInclude(3);
        settings.ExcludeDerivedTargets("csv");
        var context = settings.Context;

        Assert.Equal(PageTextPlacement.None, context.PageTextPlacement());
        Assert.True(context.IsPageIncluded(3));
        Assert.True(context.IsDerivedTargetExcluded("png"));
        Assert.True(context.IsDerivedTargetExcluded("csv"));
    }

    /// <summary>
    /// Excluding an extension outright excludes what was derived with it too, so a converter
    /// asking about a derived target hears of either.
    /// </summary>
    [Fact]
    public void ExcludingAnExtensionExcludesWhatIsDerivedWithIt()
    {
        VerifierSettings.ExcludeTargets("png");

        Assert.True(new VerifySettings().Context.IsDerivedTargetExcluded("png"));
    }

    [Fact]
    public void ACopyOfSettingsKeepsThem()
    {
        var settings = new VerifySettings();
        settings.PageText(PageTextPlacement.PerPage);
        settings.PagesToInclude(1);
        settings.ExcludeDerivedTargets("png");

        var context = new VerifySettings(settings).Context;

        Assert.Equal(PageTextPlacement.PerPage, context.PageTextPlacement());
        Assert.False(context.IsPageIncluded(2));
        Assert.True(context.IsDerivedTargetExcluded("png"));
    }

    [Fact]
    public void AfterVerifyHasBeenRunThrows()
    {
        InnerVerifier.verifyHasBeenRun = true;

        Assert.Throws<Exception>(() => VerifierSettings.PageText(PageTextPlacement.PerPage));
        Assert.Throws<Exception>(() => VerifierSettings.PagesToInclude(1));
        Assert.Throws<Exception>(() => VerifierSettings.PagesToInclude(_ => true));
        Assert.Throws<Exception>(() => VerifierSettings.ExcludeDerivedTargets("png"));
    }
}
