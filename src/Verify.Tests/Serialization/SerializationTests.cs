using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerifyTests;
using VerifyXunit;
using Xunit;
#if !NET461
using System.Linq.Expressions;
using System.Security.Claims;
#endif
// ReSharper disable UnusedParameter.Local
// ReSharper disable MemberCanBeMadeStatic.Local

// Non-nullable field is uninitialized.
#pragma warning disable CS8618

[UsesVerify]
public class SerializationTests
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.AddExtraDatetimeFormat("F");
        VerifierSettings.AddExtraDatetimeOffsetFormat("F");
    }

    [Fact]
    public Task PathInfos()
    {
        return Verifier.Verify(
            new
            {
                file = new FileInfo(@"c:/foo\bar.txt"),
                directory = new DirectoryInfo(@"c:/foo\bar/")
            });
    }

    class DescendingComparer<T> :
        IComparer<T>
        where T : IComparable<T>
    {
#pragma warning disable 8767
        public int Compare(T x, T y)
#pragma warning restore 8767
        {
            return y.CompareTo(x);
        }
    }

    [Fact]
    public Task SortedDictionaryInt()
    {
        var dictionary = new SortedDictionary<int, string>(new DescendingComparer<int>())
        {
            {1, "1234"},
            {2, "5678"}
        };

        return Verifier.Verify(dictionary);
    }

    [Fact]
    public Task SymbolOrdering1()
    {
        var target = new Dictionary<string, int>
        {
            {"#", 1},
            {"@", 2},
        };

        return Verifier.Verify(target);
    }

    [Fact]
    public Task SymbolOrdering2()
    {
        var target = new Dictionary<string, int>
        {
            {"@", 2},
            {"#", 1},
        };

        return Verifier.Verify(target);
    }

    [Fact]
    public Task JObjectOrdering1()
    {
        var obj = new JObject(
            new JProperty("@xmlns", 2),
            new JProperty("#text", 1)
        );

        return Verifier.Verify(obj);
    }

    [Fact]
    public Task JObjectOrdering2()
    {
        var obj = new JObject(
            new JProperty("#text", 1),
            new JProperty("@xmlns", 2)
        );

        return Verifier.Verify(obj);
    }

    [Fact]
    public Task SortedDictionaryOrder()
    {
        var dictionary = new SortedDictionary<string, string>(new DescendingComparer<string>())
        {
            {"Entry_1", "1234"},
            {"ignored", "1234"},
            {"Entry_2", "5678"}
        };

        return Verifier.Verify(dictionary)
            .ModifySerialization(_ => _.IgnoreMember("ignored"));
    }

    [Fact]
    public Task DictionaryOrderInt()
    {
        var dictionary = new Dictionary<int, string>();

        if (DateTime.UtcNow.Ticks % 2 == 0)
        {
            dictionary.Add(1, "1234");
            dictionary.Add(2, "5678");
        }
        else
        {
            dictionary.Add(2, "5678");
            dictionary.Add(1, "1234");
        }

        return Verifier.Verify(dictionary);
    }

    public class NonComparableKey
    {
        string member;

        public NonComparableKey(string member)
        {
            this.member = member;
        }

        public override string ToString()
        {
            return member;
        }

        public override int GetHashCode()
        {
            return member.GetHashCode();
        }
    }

    [Fact]
    public Task DictionaryOrderNonComparable()
    {
        var dictionary = new Dictionary<NonComparableKey, string>
        {
            [new("Foo1")] = "Bar",
            [new("Foo2")] = "Bar"
        };

        return Verifier.Verify(dictionary);
    }

    [Fact]
    public Task DictionaryOrderString()
    {
        var dictionary = new Dictionary<string, string>
        {
            {"ignored", "1234"}
        };

        if (DateTime.UtcNow.Ticks % 2 == 0)
        {
            dictionary.Add("Entry_1", "1234");
            dictionary.Add("Entry_2", "5678");
        }
        else
        {
            dictionary.Add("Entry_2", "5678");
            dictionary.Add("Entry_1", "1234");
        }

        return Verifier.Verify(dictionary)
            .ModifySerialization(_ => _.IgnoreMember("ignored"));
    }

    [Fact]
    public Task DatetimeOffsetScrubbingDisabled()
    {
        return Verifier.Verify(
                new
                {
                    noTime = new DateTimeOffset(new DateTime(2000, 1, 1), TimeSpan.FromHours(1)),
                    withTime = new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 1), TimeSpan.FromHours(1))
                })
            .ModifySerialization(settings => settings.DontScrubDateTimes());
    }

    [Fact]
    public Task DatetimeScrubbingDisabled()
    {
        return Verifier.Verify(
                new
                {
                    noTime = new DateTime(2000, 1, 1),
                    withTime = new DateTime(2000, 1, 1, 1, 1, 1)
                })
            .ModifySerialization(settings => settings.DontScrubDateTimes());
    }
#if NET6_0_OR_GREATER

#region AddExtraSettings

    [Fact]
    public Task AddExtraSettings()
    {
        var target = new DateOnly(2000, 1, 1);

        var verifySettings = new VerifySettings();
        verifySettings.ModifySerialization(settings =>
            settings.AddExtraSettings(serializerSettings =>
                serializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat));
        return Verifier.Verify(target, verifySettings);
    }

#endregion

#region AddExtraSettingsFluent

    [Fact]
    public Task AddExtraSettingsFluent()
    {
        var target = new DateOnly(2000, 1, 1);

        return Verifier.Verify(target)
            .ModifySerialization(settings =>
                settings.AddExtraSettings(serializerSettings =>
                    serializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat));
    }

#endregion

#endif

    void AddExtraSettingsGlobal()
    {
        #region AddExtraSettingsGlobal

        VerifierSettings.ModifySerialization(settings =>
            settings.AddExtraSettings(serializerSettings =>
                serializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat));

        #endregion
    }

    #region TreatAsNumericId

    [Fact]
    public Task TreatAsNumericId()
    {
        var target = new IdConventionTarget
        {
            TheProperty = 5
        };
        return Verifier.Verify(target)
            .ModifySerialization(
                settings => settings.TreatAsNumericId(
                    member => member.Name == "TheProperty"));
    }

    #endregion

    #region TreatAsNumericIdGlobal

    [Fact]
    public Task TreatAsNumericIdGlobal()
    {
        VerifierSettings.ModifySerialization(
            settings => settings.TreatAsNumericId(
                member => member.Name == "TheProperty"));
        var target = new IdConventionTarget
        {
            TheProperty = 5
        };
        return Verifier.Verify(target);
    }

    #endregion

    public class IdConventionTarget
    {
        public int TheProperty { get; set; }
    }

    #region DisableNumericIdGlobal

    [Fact]
    public Task NumericIdScrubbingDisabledGlobal()
    {
        VerifierSettings.ModifySerialization(settings => settings.DontScrubNumericIds());
        return Verifier.Verify(
            new
            {
                Id = 5,
                OtherId = 5,
                YetAnotherId = 4,
                PossibleNullId = (int?) 5,
                ActualNullId = (int?) null
            });
    }

    #endregion
    #region DisableNumericId

    [Fact]
    public Task NumericIdScrubbingDisabled()
    {
        var target = new
        {
            Id = 5,
            OtherId = 5,
            YetAnotherId = 4,
            PossibleNullId = (int?)5,
            ActualNullId = (int?)null
        };
        return Verifier.Verify(target)
            .ModifySerialization(settings => settings.DontScrubNumericIds());
    }

    #endregion

    #region NumericId

    [Fact]
    public Task NumericIdScrubbing()
    {
        var target = new
        {
            Id = 5,
            OtherId = 5,
            YetAnotherId = 4,
            PossibleNullId = (int?) 5,
            ActualNullId = (int?) null
        };

        return Verifier.Verify(target);
    }

    #endregion

    [Fact]
    public void SettingsIsCloned()
    {
        var settings = new SerializationSettings();

        var ignoredMemberList = new List<string>();
        settings.ignoredMembers.Add(GetType(), ignoredMemberList);

        var ignoredInstances = new List<Func<object, bool>>();
        settings.ignoredInstances.Add(GetType(), ignoredInstances);

        var memberConverterList = new Dictionary<string, ConvertMember>();
        settings.membersConverters.Add(GetType(), memberConverterList);

        settings.ignoredByNameMembers.Add("ignored");

        var clone = settings.Clone();

        Assert.NotSame(settings, clone);
        Assert.NotSame(settings.ignoredMembers, clone.ignoredMembers);
        Assert.NotSame(settings.ignoredMembers.First().Value, clone.ignoredMembers.First().Value);
        Assert.NotSame(settings.ignoredInstances, clone.ignoredInstances);
        Assert.NotSame(settings.ignoredInstances.First().Value, clone.ignoredInstances.First().Value);
        Assert.NotSame(settings.ignoredByNameMembers, clone.ignoredByNameMembers);
        Assert.NotSame(settings.membersConverters, clone.membersConverters);
        Assert.NotSame(settings.membersConverters.First().Value, clone.membersConverters.First().Value);
    }

    [Fact]
    public async Task GuidScrubbingDisabled()
    {
        var target = Guid.Parse("b6993f86-c1b9-44db-bfc5-33ed9e5c048e");

        #region DontScrubGuids

        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.DontScrubGuids());
        await Verifier.Verify(target, settings);

        #endregion
    }

    [Fact]
    public async Task GuidScrubbingDisabledFluent()
    {
        var target = Guid.Parse("b6993f86-c1b9-44db-bfc5-33ed9e5c048e");

        #region DontScrubGuidsFluent

        await Verifier.Verify(target)
            .ModifySerialization(_ => _.DontScrubGuids());

        #endregion
    }

    [Fact]
    public Task GuidScrubbingDisabledNested()
    {
        return Verifier.Verify(
                new
                {
                    value = Guid.Parse("b6993f86-c1b9-44db-bfc5-33ed9e5c048e")
                })
            .ModifySerialization(_ => _.DontScrubGuids());
    }

    [Fact]
    public Task ScrubberWithBadNewLine()
    {
        return Verifier.Verify("a")
            .AddScrubber(s =>
            {
                s.AppendLine("b");
                s.AppendLine("c");
            });
    }

    [Fact]
    public Task ExtensionAwareScrubbers()
    {
        var settings = new VerifySettings();
        settings.UseExtension("html");
        settings.AddScrubber("html", builder => builder.Replace("a", "b"));
        return Verifier.Verify("a", settings);
    }

    [Fact]
    public Task NameValueCollection()
    {
        return Verifier.Verify(
            new
            {
                item1 = new NameValueCollection {{null, null}},
                item2 = new NameValueCollection {{"key", null}},
                item3 = new NameValueCollection {{null, "value"}},
                item4 = new NameValueCollection {{"key", "value"}},
                item5 = new NameValueCollection {{"key", "value1"}, {"key", "value2"}},
                item6 = new NameValueCollection {{"key", null}, {"key", "value2"}},
                item7 = new NameValueCollection {{"key", "value1"}, {"key", null}},
                item8 = new NameValueCollection {{"key1", "value1"}, {"key2", "value2"}},
            });
    }

    [Fact]
    public Task Timespan()
    {
        return Verifier.Verify(
            new
            {
                timespan = TimeSpan.FromDays(1)
            });
    }

    [Fact]
    public Task EmptyDictionaryProperty()
    {
        return Verifier.Verify(new
        {
            property = new Dictionary<string, string>()
        });
    }


    [Fact]
    public Task ByteArray()
    {
        return Verifier.Verify(
            new
            {
                bytes = new byte[] {1}
            });
    }

    [Fact]
    public Task ExampleNonDefaults()
    {
        var person = new Person
        {
            Id = new("ebced679-45d3-4653-8791-3d969c4a986c"),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new() {"Sam", "Mary"},
            Address = new()
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        var settings = new VerifySettings();
        settings.ModifySerialization(_ =>
        {
            _.DontScrubDateTimes();
            _.DontIgnoreFalse();
            _.DontScrubGuids();
            _.DontIgnoreEmptyCollections();
        });
        settings.AddScrubber(s => s.Replace("Lane", "Street"));
        return Verifier.Verify(person, settings);
    }

    [Fact]
    public Task TypeNameHandlingAll()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new() {"Sam", "Mary"},
            Address = new()
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        return Verifier.Verify(person)
            .AddExtraSettings(_ => { _.TypeNameHandling = TypeNameHandling.All; });
    }

#if NET6_0_OR_GREATER

    [Fact]
    public Task TimeOnlyNested()
    {
        return Verifier.Verify(new { value = new TimeOnly(10, 10) });
    }

    [Fact]
    public Task ShouldIgnoreDatetimeDefaults()
    {
        var target = new DateTimeTarget();

        return Verifier.Verify(target);
    }

    [Fact]
    public async Task ShouldReUseDatetime()
    {
#region Date

        var dateTime = DateTime.Now;
        var dateTimeOffset = DateTimeOffset.Now;
        var target = new DateTimeTarget
        {
            DateTime = dateTime,
            DateOnly = new(dateTime.Year, dateTime.Month, dateTime.Day),
            DateOnlyNullable = new(dateTime.Year, dateTime.Month, dateTime.Day),
            DateOnlyString = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day).ToString(),
            DateTimeNullable = dateTime,
            DateTimeString = dateTime.ToString("F"),
            DateTimeOffset = dateTimeOffset,
            DateTimeOffsetNullable = dateTimeOffset,
            DateTimeOffsetString = dateTimeOffset.ToString("F"),
        };

        await Verifier.Verify(target);

#endregion
    }

    [Fact]
    public async Task DatetimeMin()
    {
        var dateTime = DateTime.MinValue;
        var dateTimeOffset = DateTimeOffset.MinValue;
        var target = new DateTimeTarget
        {
            DateTime = dateTime,
            DateOnly = DateOnly.MinValue,
            DateOnlyNullable = DateOnly.MinValue,
            DateOnlyString = DateOnly.MinValue.ToString(),
            DateTimeNullable = dateTime,
            DateTimeString = dateTime.ToString("F"),
            DateTimeOffset = dateTimeOffset,
            DateTimeOffsetNullable = dateTimeOffset,
            DateTimeOffsetString = dateTimeOffset.ToString("F"),
        };

        await Verifier.Verify(target);
    }

    [Fact]
    public async Task DatetimeMax()
    {
        var dateTime = DateTime.MaxValue;
        var dateTimeOffset = DateTimeOffset.MaxValue;
        var target = new DateTimeTarget
        {
            DateTime = dateTime,
            DateOnly = DateOnly.MaxValue,
            DateOnlyNullable = DateOnly.MaxValue,
            DateOnlyString = DateOnly.MaxValue.ToString(),
            DateTimeNullable = dateTime,
            DateTimeString = dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"),
            DateTimeOffset = dateTimeOffset,
            DateTimeOffsetNullable = dateTimeOffset,
            DateTimeOffsetString = dateTimeOffset.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"),
        };

        await Verifier.Verify(target);
    }

    [Fact]
    public Task ShouldScrubDatetime()
    {
        var dateTime = DateTime.Now;
        var dateTimeOffset = DateTimeOffset.Now;
        var target = new DateTimeTarget
        {
            DateTime = dateTime,
            DateTimeNullable = dateTime.AddDays(1),
            DateTimeString = dateTime.AddDays(2).ToString("F"),
            DateTimeOffset = dateTimeOffset,
            DateTimeOffsetNullable = dateTimeOffset.AddDays(1),
            DateTimeOffsetString = dateTimeOffset.AddDays(2).ToString("F"),
            DateOnly = new(2020, 10, 10),
            DateOnlyNullable = new(2020, 10, 12),
            DateOnlyString = new DateOnly(2020, 10, 12).ToString()
        };

        return Verifier.Verify(target);
    }

    class DateTimeTarget
    {
        public DateTime DateTime;
        public DateTime? DateTimeNullable;
        public DateOnly DateOnly;
        public DateOnly? DateOnlyNullable;
        public DateTimeOffset DateTimeOffset;
        public DateTimeOffset? DateTimeOffsetNullable;
        public string DateTimeString;
        public string DateTimeOffsetString;
        public string DateOnlyString;
    }

#endif

    [Fact]
    public Task VerifyBytes()
    {
        return Verifier.Verify(File.ReadAllBytes("sample.jpg"))
            .UseExtension("jpg");
    }

    [Fact]
    public Task ShouldNotScrubInlineGuidsByDefault()
    {
        Guid id = new("ebced679-45d3-4653-8791-3d969c4a986c");
        var product = new
        {
            Title = $"item {id} - (ID={{{id}}})",
            Variant = new
            {
                Id = "variant id: " + id
            }
        };

        return Verifier.Verify(product);
    }

    [Fact]
    public Task ShouldBeAbleToExcludeInlineGuidsInString()
    {
        var id = Guid.NewGuid();
        return Verifier.Verify($"The string {id} ")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldBeAbleToExcludeInlineGuidsWrappedInSymbols()
    {
        var id = Guid.NewGuid();
        return Verifier.Verify($"({id})")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldNotExcludeInlineGuidsWrappedInDash()
    {
        return Verifier.Verify("-087ea433-d83b-40b6-9e37-465211d9508-")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldNotExcludeInlineGuidsWrappedInLetters()
    {
        return Verifier.Verify("before087ea433-d83b-40b6-9e37-465211d9508cafter")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldNotExcludeInlineGuidsWrappedInNumber()
    {
        return Verifier.Verify("1087ea433-d83b-40b6-9e37-465211d95081")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldBeAbleToExcludeInlineGuids()
    {
        var id = Guid.NewGuid();
        var product = new
        {
            Title = $"item {id} - (ID={{{id}}})",
            Variant = new
            {
                Id = "variant id: " + id
            }
        };

        return Verifier.Verify(product)
            .ScrubInlineGuids();
    }

    void DontIgnoreEmptyCollections()
    {
        #region DontIgnoreEmptyCollections

        VerifierSettings.ModifySerialization(_ => _.DontIgnoreEmptyCollections());

        #endregion
    }

    void DontScrubGuids()
    {
        #region DontScrubGuidsGlobal

        VerifierSettings.ModifySerialization(_ => _.DontScrubGuids());

        #endregion
    }

    void DontScrubProjectDirectory()
    {
        #region DontScrubProjectDirectory

        VerifierSettings.DontScrubProjectDirectory();

        #endregion
    }

    void DontScrubSolutionDirectory()
    {
        #region DontScrubSolutionDirectory

        VerifierSettings.DontScrubSolutionDirectory();

        #endregion
    }

    void ScrubInlineGuids()
    {
        #region ScrubInlineGuids

        VerifierSettings.ScrubInlineGuids();

        #endregion
    }

    Task DontScrubDateTimes()
    {
        #region DontScrubDateTimes

        var target = new
        {
            Date = DateTime.Now
        };

        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.DontScrubDateTimes());

        return Verifier.Verify(target, settings);

        #endregion
    }

    Task DontScrubDateTimesFluent()
    {
        #region DontScrubDateTimesFluent

        var target = new
        {
            Date = DateTime.Now
        };

        return Verifier.Verify(target)
            .ModifySerialization(_ => _.DontScrubDateTimes());

        #endregion
    }

    void DontScrubDateTimesGlobal()
    {
        #region DontScrubDateTimesGlobal

        VerifierSettings.ModifySerialization(_ => _.DontScrubDateTimes());

        #endregion
    }

    void DontIgnoreFalse()
    {
        #region DontIgnoreFalse

        VerifierSettings.ModifySerialization(_ => _.DontIgnoreFalse());

        #endregion
    }

    [Fact]
    public Task NewLineNotEscapedInProperty()
    {
        return Verifier.Verify(new {Property = "a\r\nb\\nc"});
    }

    void List()
    {
        var verifySettings = new VerifySettings();

        #region ScrubLines

        verifySettings.ScrubLines(line => line.Contains("text"));

        #endregion

        #region ScrubLinesContaining

        verifySettings.ScrubLinesContaining("text1", "text2");

        #endregion

        #region ScrubLinesContainingOrdinal

        verifySettings.ScrubLinesContaining(StringComparison.Ordinal, "text1", "text2");

        #endregion

        #region ScrubLinesWithReplace

        verifySettings.ScrubLinesWithReplace(line => line.ToUpper());

        #endregion

        #region ScrubMachineName

        verifySettings.ScrubMachineName();

        #endregion

        #region AddScrubber

        verifySettings.AddScrubber(fullText => fullText.Remove(0, 100));

        #endregion
    }

    [Fact]
    public Task ScrubTempPath()
    {
        var tempPath = Path.GetTempPath().TrimEnd('/', '\\');
        var altTempPath = tempPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verifier.Verify(new {tempPath, altTempPath});
    }

    [Fact]
    public Task ScrubCurrentDirectory()
    {
        var currentDirectory = Environment.CurrentDirectory.TrimEnd('/', '\\');
        var altCurrentDirectory = currentDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verifier.Verify(new {currentDirectory, altCurrentDirectory});
    }

#if !NET5_0_OR_GREATER
    [Fact]
    public Task ScrubCodeBaseLocation()
    {
        var codeBaseLocation = CodeBaseLocation.CurrentDirectory!.TrimEnd('/', '\\');
        var altCodeBaseLocation = codeBaseLocation.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verifier.Verify(new {codeBaseLocation, altCodeBaseLocation});
    }
#endif

    [Fact]
    public Task ScrubBaseDirectory()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('/', '\\');
        var altBaseDirectory = baseDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verifier.Verify(new {baseDirectory, altBaseDirectory});
    }

    class TypeTarget
    {
        public Type Type;
        public Type Dynamic;
    }

    [Fact]
    public Task QuoteEscaping()
    {
        return Verifier.Verify(
            new
            {
                singleQuote = "'",
                doubleQuote = "\"",
                mixed = "\"'",
            });
    }

    [Fact]
    public async Task ShouldUseShortTypeName()
    {
        #region type

        var foo = new {x = 1};
        var target = new TypeTarget
        {
            Type = GetType(),
            Dynamic = foo.GetType(),
        };

        await Verifier.Verify(target);

        #endregion
    }

#if (!NETSTANDARD2_0 && !NET461)
    [Fact]
    public async Task NamedTuple()
    {
#region VerifyTuple

        await Verifier.VerifyTuple(() => MethodWithNamedTuple());

#endregion
    }

#region MethodWithNamedTuple

    static (bool Member1, string Member2, string Member3) MethodWithNamedTuple()
    {
        return (true, "A", "B");
    }

#endregion
#if !NET461
    [Fact]
    public async Task PartialNamedTuple()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verifier.VerifyTuple(() => MethodWithPartialNamedTuple()));
        PrefixUnique.Clear();
        await Verifier.Verify(exception.Message);
    }
#endif

    static (bool, string Member2, string Member3) MethodWithPartialNamedTuple()
    {
        return (true, "A", "B");
    }

    [Fact]
    public Task NamedTupleWithNull()
    {
        return Verifier.VerifyTuple(() => MethodWithNamedTupleWithNull());
    }

    [Fact]
    public Task Claim()
    {
        return Verifier.Verify(new Claim("TheType", "TheValue"));
    }

    [Fact]
    public Task ClaimWithClaimType()
    {
        return Verifier.Verify(new Claim(ClaimTypes.Email, "TheValue"));
    }

    [Fact]
    public Task ClaimsPrincipal()
    {
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new("TheType", "TheValue"));
        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(claimsIdentity);
        return Verifier.Verify(claimsPrincipal);
    }

    [Fact]
    public Task ClaimsIdentity()
    {
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new("TheType", "TheValue"));
        return Verifier.Verify(claimsIdentity);
    }

    static (string Member1, string? Member2) MethodWithNamedTupleWithNull()
    {
        return ("A", null);
    }

    [Fact]
    public async Task ShouldReUseGuid()
    {
#region guid

        var guid = Guid.NewGuid();
        var target = new GuidTarget
        {
            Guid = guid,
            GuidNullable = guid,
            GuidString = guid.ToString(),
            OtherGuid = Guid.NewGuid(),
        };

        await Verifier.Verify(target);

#endregion
    }

    [Fact]
    public async Task ShouldRespectEmptyGuid()
    {
        var guid = Guid.Empty;
        var target = new GuidTarget
        {
            Guid = guid,
            GuidNullable = guid,
            GuidString = guid.ToString(),
            OtherGuid = Guid.NewGuid(),
        };

        await Verifier.Verify(target);
    }

    [Fact]
    public Task ShouldIgnoreGuidDefaults()
    {
        var target = new GuidTarget();
        return Verifier.Verify(target);
    }

    [Fact]
    public Task ShouldScrubProjectDirectory()
    {
        var projectDirectory = GetProjectDirectory();
        var path = Path.GetFullPath(Path.Combine(projectDirectory, "Foo"));
        var altPath = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verifier.Verify(
            new
            {
                path,
                trimmed = path.TrimEnd('/', '\\'),
                altPath,
                altPathTrimmed = altPath.TrimEnd('/', '\\'),
            });
    }

    static string GetProjectDirectory([CallerFilePath] string file = "")
    {
        return new FileInfo(file).Directory!.Parent!.FullName;
    }

    [Fact]
    public Task ShouldScrubSolutionDirectory()
    {
        var solutionDirectory = GetSolutionDirectory();
        var path = Path.GetFullPath(Path.Combine(solutionDirectory, "Foo"));
        var altPath = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verifier.Verify(
            new
            {
                path,
                trimmed = path.TrimEnd('/', '\\'),
                altPath,
                altPathTrimmed = altPath.TrimEnd('/', '\\'),
            });
    }

    static string GetSolutionDirectory([CallerFilePath] string file = "")
    {
        return new FileInfo(file).Directory!.Parent!.Parent!.FullName;
    }

    [Fact]
    public Task ShouldScrubGuid()
    {
        var target = new GuidTarget
        {
            Guid = Guid.NewGuid(),
            GuidNullable = Guid.NewGuid(),
            GuidString = Guid.NewGuid().ToString(),
        };
        return Verifier.Verify(target);
    }

    class GuidTarget
    {
        public Guid Guid;
        public Guid? GuidNullable;
        public string GuidString;
        public Guid OtherGuid;
    }

    [Fact]
    public Task Escaping()
    {
        var target = new EscapeTarget
        {
            Property = @"\"
        };
        return Verifier.Verify(target);
    }

    class EscapeTarget
    {
        public string Property;
    }

    [Fact]
    public Task OnlySpecificDates()
    {
        var target = new NotDatesTarget
        {
            NotDate = "1.2.3"
        };
        return Verifier.Verify(target);
    }

    class NotDatesTarget
    {
        public string NotDate;
    }

    [Fact]
    public Task ShouldScrubDictionaryKey()
    {
        return Verifier.Verify(
            new
            {
                guid = new Dictionary<Guid, string>
                {
                    { Guid.NewGuid(), "value" }
                },
                dateTime = new Dictionary<DateTime, string>
                {
                    { DateTime.Now, "value" }
                },
                dateTimeOffset = new Dictionary<DateTimeOffset, string>
                {
                    { DateTimeOffset.Now, "value" }
                },
                type = new Dictionary<Type, string>
                {
                    { typeof(SerializationTests), "value" }
                }
            });
    }

    [Fact]
    public Task ShouldIgnoreEmptyList()
    {
        var target = new CollectionTarget
        {
            DictionaryProperty = new(),
            IReadOnlyDictionary = new Dictionary<int, string>(),
            ReadOnlyList = new List<string>(),
            ListProperty = new(),
            ReadOnlyCollection = new List<string>(),
            Array = Array.Empty<string>()
        };
        return Verifier.Verify(target);
    }

    class CollectionTarget
    {
        public Dictionary<int, string> DictionaryProperty;
        public List<string> ListProperty;
        public string[] Array;
        public IReadOnlyList<string> ReadOnlyList;
        public IReadOnlyCollection<string> ReadOnlyCollection;
        public IReadOnlyDictionary<int, string> IReadOnlyDictionary;
    }
#pragma warning disable 612

    void ExceptionMessagePropGlobal()
    {
#region IgnoreMembersThatThrowExpressionGlobal

        VerifierSettings.ModifySerialization(
            _ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));

#endregion
    }

#region IgnoreMembersThatThrowExpression

    [Fact]
    public Task ExceptionMessageProp()
    {
        var target = new WithExceptionIgnoreMessage();

        var settings = new VerifySettings();
        settings.ModifySerialization(
            _ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task ExceptionMessagePropFluent()
    {
        var target = new WithExceptionIgnoreMessage();

        return Verifier.Verify(target)
            .ModifySerialization(
                _ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));
    }

#endregion

    [Fact]
    public Task ExpressionString()
    {
        var parameter = Expression.Parameter(typeof(Exception), "source");
        var property = Expression.Property(parameter, "Message");
        var convert = Expression.Convert(property, typeof(object));
        var expression = Expression.Lambda<Func<Exception, object>>(convert, parameter);
        return Verifier.Verify(expression)
            .UniqueForRuntime();
    }

    class WithExceptionIgnoreMessage
    {
        public Guid ExceptionMessageProperty => throw new("Ignore");
    }

    [Fact]
    public Task NotImplementedExceptionProp()
    {
        var target = new WithNotImplementedException();
        return Verifier.Verify(target);
    }

    class WithNotImplementedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotImplementedException();
    }

    void AddIgnoreInstanceGlobal()
    {
#region AddIgnoreInstanceGlobal

        VerifierSettings.ModifySerialization(
            _ => { _.IgnoreInstance<Instance>(x => x.Property == "Ignore"); });

#endregion
    }

#region AddIgnoreInstance

    [Fact]
    public Task AddIgnoreInstance()
    {
        var target = new IgnoreInstanceTarget
        {
            ToIgnore = new()
            {
                Property = "Ignore"
            },
            ToInclude = new()
            {
                Property = "Include"
            }
        };
        var settings = new VerifySettings();
        settings.ModifySerialization(
            _ => { _.IgnoreInstance<Instance>(x => x.Property == "Ignore"); });
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task AddIgnoreInstanceFluent()
    {
        var target = new IgnoreInstanceTarget
        {
            ToIgnore = new()
            {
                Property = "Ignore"
            },
            ToInclude = new()
            {
                Property = "Include"
            }
        };
        return Verifier.Verify(target)
            .ModifySerialization(
                _ => { _.IgnoreInstance<Instance>(x => x.Property == "Ignore"); });
    }

#endregion

    class IgnoreInstanceTarget
    {
        public Instance ToIgnore;
        public Instance ToInclude;
    }

    class Instance
    {
        public string Property;
    }

    void AddIgnoreTypeGlobal()
    {
#region AddIgnoreTypeGlobal

        VerifierSettings.ModifySerialization(
            _ => _.IgnoreMembersWithType<ToIgnore>());

#endregion
    }

#region AddIgnoreType

    [Fact]
    public Task IgnoreType()
    {
        var target = new IgnoreTypeTarget
        {
            ToIgnore = new()
            {
                Property = "Value"
            },
            ToInclude = new()
            {
                Property = "Value"
            }
        };
        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.IgnoreMembersWithType<ToIgnore>());
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task IgnoreTypeFluent()
    {
        var target = new IgnoreTypeTarget
        {
            ToIgnore = new()
            {
                Property = "Value"
            },
            ToInclude = new()
            {
                Property = "Value"
            }
        };
        return Verifier.Verify(target)
            .ModifySerialization(_ => _.IgnoreMembersWithType<ToIgnore>());

    }

#endregion

    [Fact]
    public Task Type()
    {
        return Verifier.Verify(GetType());
    }

    [Fact]
    public Task Field()
    {
        var target = Info.OfField<SerializationTests>("MyField");
        return Verifier.Verify(target);
    }

    public string MyField;

    [Fact]
    public Task GetProperty()
    {
        var target = Info.OfPropertyGet<SerializationTests>("MyProperty");
        return Verifier.Verify(target);
    }

    [Fact]
    public Task SetProperty()
    {
        var target = Info.OfPropertySet<SerializationTests>("MyProperty");
        return Verifier.Verify(target);
    }

    [Fact]
    public Task Property()
    {
        var target = typeof(SerializationTests).GetProperty("MyProperty");
        return Verifier.Verify(target);
    }

    public string MyProperty { get; set; }

    [Fact]
    public Task Method()
    {
        return Verifier.Verify(Info.OfMethod<SerializationTests>("Method"));
    }

    [Fact]
    public Task Constructor()
    {
        return Verifier.Verify(Info.OfConstructor<SerializationTests>());
    }

    [Fact]
    public Task Parameter()
    {
        var method = Info.OfMethod<SerializationTests>("MyMethodWithParameters");
        return Verifier.Verify(method.GetParameters().First());
    }

    [Fact]
    public Task MethodWithParameters()
    {
        return Verifier.Verify(Info.OfMethod<SerializationTests>("MyMethodWithParameters"));
    }

    // ReSharper disable UnusedParameter.Local
    void MyMethodWithParameters(int x, string y)
        // ReSharper restore UnusedParameter.Local
    {
    }

    class IgnoreTypeTarget
    {
        public ToIgnore ToIgnore;
        public ToInclude ToInclude;
    }

    class ToInclude
    {
        public string Property;
    }

    class ToIgnore
    {
        public string Property;
    }

    void IgnoreMemberByExpressionGlobal()
    {
#region IgnoreMemberByExpressionGlobal

        VerifierSettings.ModifySerialization(_ =>
        {
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyWithPropertyName);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);
        });

#endregion
    }

#region IgnoreMemberByExpression

    [Fact]
    public Task IgnoreMemberByExpression()
    {
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value",
            PropertyWithPropertyName = "Value"
        };
        var settings = new VerifySettings();
        settings.ModifySerialization(_ =>
        {
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyWithPropertyName);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);
        });
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task IgnoreMemberByExpressionFluent()
    {
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        return Verifier.Verify(target)
            .ModifySerialization(_ =>
            {
                _.IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
                _.IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
                _.IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
                _.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);
            });
    }

#endregion


#region MemberConverter

    [Fact]
    public Task MemberConverterByExpression()
    {
        var input = new MemberConverterTarget
        {
            Field = "Value",
            Property = "Value"
        };
        var settings = new VerifySettings();
        settings.ModifySerialization(_ =>
        {
            _.MemberConverter<MemberConverterTarget, string>(x => x.Property, (target, value) => value + "Suffix");
            _.MemberConverter<MemberConverterTarget, string>(x => x.Field, (target, value) => value + "Suffix");
        });
        return Verifier.Verify(input, settings);
    }

    [Fact]
    public Task MemberConverterByExpressionFluent()
    {
        var input = new MemberConverterTarget
        {
            Field = "Value",
            Property = "Value"
        };
        return Verifier.Verify(input)
            .ModifySerialization(_ =>
            {
                _.MemberConverter<MemberConverterTarget, string>(
                    x => x.Property,
                    (target, value) => value + "Suffix");
                _.MemberConverter<MemberConverterTarget, string>(
                    x => x.Field,
                    (target, value) => value + "Suffix");
            });
    }

#endregion

    void MemberConverterGlobal()
    {
#region MemberConverterGlobal

        VerifierSettings.ModifySerialization(_ =>
        {
            _.MemberConverter<MemberConverterTarget, string>(
                x => x.Property,
                (target, value) => value + "Suffix");
            _.MemberConverter<MemberConverterTarget, string>(
                x => x.Field,
                (target, value) => value + "Suffix");
        });

#endregion
    }

    class MemberConverterTarget
    {
        public string Property { get; set; }
        public string Field;
    }

    void IgnoreMemberByNameGlobal()
    {
#region IgnoreMemberByNameGlobal

        VerifierSettings.ModifySerialization(_ =>
        {
            _.IgnoreMember("PropertyByName");
            var type = typeof(IgnoreExplicitTarget);
            _.IgnoreMember(type, "Property");
            _.IgnoreMember(type, "Field");
            _.IgnoreMember(type, "GetOnlyProperty");
            _.IgnoreMember(type, "PropertyThatThrows");
        });

#endregion
    }

#region IgnoreMemberByName

    [Fact]
    public Task IgnoreMemberByName()
    {
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value",
            PropertyByName = "Value"
        };
        var settings = new VerifySettings();
        settings.ModifySerialization(_ =>
        {
            _.IgnoreMember("PropertyByName");
            var type = typeof(IgnoreExplicitTarget);
            _.IgnoreMember(type, "Property");
            _.IgnoreMember(type, "Field");
            _.IgnoreMember(type, "GetOnlyProperty");
            _.IgnoreMember(type, "PropertyThatThrows");
        });
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task IgnoreMemberByNameFluent()
    {
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value",
            PropertyByName = "Value"
        };
        return Verifier.Verify(target)
            .ModifySerialization(_ =>
            {
                _.IgnoreMember("PropertyByName");
                var type = typeof(IgnoreExplicitTarget);
                _.IgnoreMember(type, "Property");
                _.IgnoreMember(type, "Field");
                _.IgnoreMember(type, "GetOnlyProperty");
                _.IgnoreMember(type, "PropertyThatThrows");
            });
    }

#endregion

    [Fact]
    public Task IgnoreJTokenByName()
    {
        var json = @"{
  'short': {
    'key': {
      'code': 0,
      'msg': 'No action taken'
    },
    'Ignore1': {
      'code': 2,
      'msg': 'ignore this'
    }
  }
}";
        var target = JToken.Parse(json);
        return Verifier.Verify(target)
            .ModifySerialization(_ => { _.IgnoreMember("Ignore1"); });
    }

    [Fact]
    public Task VerifyJsonGuid()
    {
        var json = "{'key': 'c572ff75-e1a2-49bd-99b9-4550697946c3'}";
        return Verifier.VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonDateTime()
    {
        var json = $"{{'key': '{DateTime.Now:yyyy-MM-ddTHH:mm:ss}'}}";
        return Verifier.VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonWithArray()
    {
        var json = @"{
    commitments: [
      {
        id: '9585dadf-551a-43eb-960c-18b935993cc3',
        title: 'Commitment1'
      }
    ]
    }";
        return Verifier.VerifyJson(json);
    }

#region VerifyJson

    [Fact]
    public Task VerifyJsonString()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        return Verifier.VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonStream()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        MemoryStream stream = new(Encoding.UTF8.GetBytes(json));
        return Verifier.VerifyJson(stream);
    }

    [Fact]
    public Task VerifyJsonJToken()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        var target = JToken.Parse(json);
        return Verifier.VerifyJson(target);
    }

#endregion

    [Fact]
    public Task IgnoreDictionaryKeyByName()
    {
        var target = new Dictionary<string, object>
        {
            {
                "Include", new Dictionary<string, string>
                {
                    { "Ignore", "Value1" },
                    { "Key1", "Value2" }
                }
            },
            { "Ignore", "Value3" },
            { "Key2", "Value4" },
        };
        return Verifier.Verify(target)
            .ModifySerialization(_ => { _.IgnoreMember("Ignore"); });
    }

    class IgnoreExplicitTarget
    {
        public string Include;
        public string Property { get; set; }

        [JsonProperty(PropertyName = "_Custom")]
        public string PropertyWithPropertyName { get; set; }

        public string PropertyByName { get; set; }
        public string GetOnlyProperty => "asd";
        public string PropertyThatThrows => throw new();
        public string Field;
    }

    void CustomExceptionPropGlobal()
    {
#region IgnoreMembersThatThrowGlobal

        VerifierSettings.ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());

#endregion
    }

#region IgnoreMembersThatThrow

    [Fact]
    public Task CustomExceptionProp()
    {
        var target = new WithCustomException();
        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task CustomExceptionPropFluent()
    {
        var target = new WithCustomException();
        return Verifier.Verify(target)
            .ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());
    }

#endregion

    class WithCustomException
    {
        public Guid CustomExceptionProperty => throw new CustomException();
    }

    [Fact]
    public Task ExceptionProps()
    {
        try
        {
            throw new();
        }
        catch (Exception exception)
        {
            return Verifier.Verify(exception);
        }
    }

    [Fact]
    public Task ExceptionProp()
    {
        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());

        var target = new WithException();

        return Assert.ThrowsAsync<JsonSerializationException>(() => Verifier.Verify(target, settings));
    }

    class WithException
    {
        public Guid ExceptionProperty => throw new();
    }

    [Fact]
    public Task ExceptionNotIgnoreMessageProp()
    {
        var settings = new VerifySettings();
        settings.ModifySerialization(
            _ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));
        var target = new WithExceptionNotIgnoreMessage();

        return Assert.ThrowsAsync<JsonSerializationException>(() => Verifier.Verify(target, settings));
    }

    class WithExceptionNotIgnoreMessage
    {
        public Guid ExceptionMessageProperty => throw new("NotIgnore");
    }

    [Fact]
    public Task DelegateProp()
    {
        var target = new WithDelegate();
        return Verifier.Verify(target);
    }

    class WithDelegate
    {
        public Action DelegateProperty => () => { };
    }

    class CustomException :
        Exception
    {
    }

    [Fact]
    public Task NotSupportedExceptionProp()
    {
        var target = new WithNotSupportedException();
        return Verifier.Verify(target);
    }

    class WithNotSupportedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotSupportedException();
    }


    void WithObsoletePropIncludedGlobally()
    {
#region WithObsoletePropIncludedGlobally

        VerifierSettings.ModifySerialization(_ => { _.IncludeObsoletes(); });

#endregion
    }

#region WithObsoletePropIncluded

    [Fact]
    public Task WithObsoletePropIncluded()
    {
        var target = new WithObsolete
        {
            ObsoleteProperty = "value1",
            OtherProperty = "value2"
        };
        var settings = new VerifySettings();
        settings.ModifySerialization(_ => { _.IncludeObsoletes(); });
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task WithObsoletePropIncludedFluent()
    {
        var target = new WithObsolete
        {
            ObsoleteProperty = "value1",
            OtherProperty = "value2"
        };
        return Verifier.Verify(target)
            .ModifySerialization(_ => { _.IncludeObsoletes(); });
    }

#endregion

#region WithObsoleteProp

    class WithObsolete
    {
        [Obsolete]
        public string ObsoleteProperty { get; set; }

        public string OtherProperty { get; set; }
    }

    [Fact]
    public Task WithObsoleteProp()
    {
        var target = new WithObsolete
        {
            ObsoleteProperty = "value1",
            OtherProperty = "value2"
        };
        return Verifier.Verify(target);
    }

#endregion

#pragma warning restore 612
    [Fact]
    public async Task Tuple()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verifier.VerifyTuple(() => MethodWithTuple()));
        PrefixUnique.Clear();
        await Verifier.Verify(exception.Message);
    }

    static (bool, string, string) MethodWithTuple()
    {
        return (true, "A", "B");
    }

#endif

    #region ScopedSerializer

    [Fact]
    public Task ScopedSerializer()
    {
        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith"
        };
        var settings = new VerifySettings();
        settings.AddExtraSettings(
            _ => { _.TypeNameHandling = TypeNameHandling.All; });
        return Verifier.Verify(person, settings);
    }

    [Fact]
    public Task ScopedSerializerFluent()
    {
        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith"
        };
        return Verifier.Verify(person)
            .AddExtraSettings(
                _ => { _.TypeNameHandling = TypeNameHandling.All; });
    }

    #endregion
}