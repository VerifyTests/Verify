#if !NET461
using System.Security.Claims;
#endif

// ReSharper disable RedundantSuppressNullableWarningExpression
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
    public async Task Tasks()
    {
        var withResult = Task.FromResult("Value");
        var withException = Task.FromException(new("the exception"));
        var withExceptionAndResult = Task.FromException<string>(new("the exception"));
        var genericCompletionSource = new TaskCompletionSource<int>();
        genericCompletionSource.TrySetCanceled();
        var canceledAndResult = genericCompletionSource.Task;
        var finished = Task.Delay(0);
        var running = Task.Delay(10000);
        await Verify(
            new
            {
                finished,
                running,
                withResult,
                withException,
                withExceptionAndResult,
                canceledAndResult
            });
    }

#if NET5_0_OR_GREATER || net48
    [Fact]
    public async Task ValueTasks()
    {
        var withResult = ValueTask.FromResult("Value");
        var withException = ValueTask.FromException(new("the exception"));
        var withExceptionAndResult = ValueTask.FromException<string>(new("the exception"));
        var genericCompletionSource = new TaskCompletionSource<int>();
        genericCompletionSource.TrySetCanceled();
        var canceledAndResult = genericCompletionSource.Task;
        await Verify(
            new
            {
                withResult,
                withException,
                withExceptionAndResult,
                canceledAndResult
            }).UniqueForRuntime();
    }
#endif

    [Fact]
    public Task PathInfos() =>
        Verify(
            new
            {
                file = new FileInfo(@"c:/foo\bar.txt"),
                directory = new DirectoryInfo(@"c:/foo\bar/")
            });

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

        return Verify(dictionary);
    }

    [Fact]
    public Task SymbolOrdering1()
    {
        var target = new Dictionary<string, int>
        {
            {"#", 1},
            {"@", 2}
        };

        return Verify(target);
    }

    [Fact]
    public Task SymbolOrdering2()
    {
        var target = new Dictionary<string, int>
        {
            {"@", 2},
            {"#", 1}
        };

        return Verify(target);
    }

    [Fact]
    public Task JObjectOrdering1()
    {
        var obj = new JObject(
            new JProperty("@xmlns", 2),
            new JProperty("#text", 1)
        );

        return Verify(obj);
    }

    [Fact]
    public Task JTokenIgnore()
    {
        var jToken = JToken.Parse(@"{
  Include: 1,
  Ignore: 2,
  ""Memory Info"": {
          fragmentedBytes: 208,
          heapSizeBytes: 2479536,
          highMemoryLoadThresholdBytes: 30821986713,
          memoryLoadBytes: 14041127280,
          totalAvailableMemoryBytes: 34246651904
        }
}");
        return Verify(jToken)
            .IgnoreMembers("Ignore", "Memory Info");
    }

    [Fact]
    public Task JObjectIgnore()
    {
        var obj = new JObject(
            new JProperty("Include", 2),
            new JProperty("Ignore", 1)
        );

        return Verify(obj)
            .IgnoreMember("Ignore");
    }

    [Fact]
    public Task JObjectOrdering2()
    {
        var obj = new JObject(
            new JProperty("#text", 1),
            new JProperty("@xmlns", 2)
        );

        return Verify(obj);
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

        return Verify(dictionary)
            .IgnoreMember("ignored");
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

        return Verify(dictionary);
    }

    public class NonComparableKey
    {
        string member;

        public NonComparableKey(string member) =>
            this.member = member;

        public override string ToString() =>
            member;

        public override int GetHashCode() =>
            member.GetHashCode();
    }

    [Fact]
    public Task DictionaryOrderNonComparable()
    {
        var dictionary = new Dictionary<NonComparableKey, string>
        {
            [new("Foo1")] = "Bar",
            [new("Foo2")] = "Bar"
        };

        return Verify(dictionary);
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

        return Verify(dictionary)
            .IgnoreMember("ignored");
    }

    [Fact]
    public Task DatetimeOffsetScrubbingDisabled() =>
        Verify(
                new
                {
                    noTime = new DateTimeOffset(new DateTime(2000, 1, 1), TimeSpan.FromHours(1)),
                    withTime = new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 1), TimeSpan.FromHours(1))
                })
            .DontScrubDateTimes();

    [Fact]
    public Task DatetimeScrubbingDisabled() =>
        Verify(
                new
                {
                    noTime = new DateTime(2000, 1, 1),
                    withTime = new DateTime(2000, 1, 1, 1, 1, 1)
                })
            .DontScrubDateTimes();
#if NET6_0_OR_GREATER

    #region AddExtraSettings

    [Fact]
    public Task AddExtraSettings()
    {
        var target = new DateOnly(2000, 1, 1);

        var settings = new VerifySettings();
        settings.AddExtraSettings(_ => _.DefaultValueHandling = DefaultValueHandling.Include);
        return Verify(target, settings);
    }

    #endregion

    #region AddExtraSettingsFluent

    [Fact]
    public Task AddExtraSettingsFluent()
    {
        var target = new DateOnly(2000, 1, 1);

        return Verify(target)
            .AddExtraSettings(_ => _.DefaultValueHandling = DefaultValueHandling.Include);
    }

    #endregion

#endif

    void AddExtraSettingsGlobal()
    {
        #region AddExtraSettingsGlobal

        VerifierSettings
            .AddExtraSettings(_ =>
                _.TypeNameHandling = TypeNameHandling.All);

        #endregion
    }

    [Fact]
    public void SettingsIsCloned()
    {
        var settings = new SerializationSettings();

        var ignoredMemberList = new List<string>();
        settings.ignoredMembers.Add(GetType(), ignoredMemberList);

        var ignoredInstances = new List<ShouldIgnore>();
        settings.ignoredInstances.Add(GetType(), ignoredInstances);

        settings.ignoredByNameMembers.Add("ignored");

        var clone = new SerializationSettings(settings);

        Assert.NotSame(settings, clone);
        Assert.NotSame(settings.ignoredMembers, clone.ignoredMembers);
        Assert.NotSame(settings.ignoredMembers.First().Value, clone.ignoredMembers.First().Value);
        Assert.NotSame(settings.ignoredInstances, clone.ignoredInstances);
        Assert.NotSame(settings.ignoredInstances.First().Value, clone.ignoredInstances.First().Value);
        Assert.NotSame(settings.ignoredByNameMembers, clone.ignoredByNameMembers);
    }

    [Fact]
    public async Task GuidScrubbingDisabled()
    {
        var target = Guid.Parse("b6993f86-c1b9-44db-bfc5-33ed9e5c048e");

        #region DontScrubGuids

        var settings = new VerifySettings();
        settings.DontScrubGuids();
        await Verify(target, settings);

        #endregion
    }

    [Fact]
    public async Task GuidScrubbingDisabledFluent()
    {
        var target = Guid.Parse("b6993f86-c1b9-44db-bfc5-33ed9e5c048e");

        #region DontScrubGuidsFluent

        await Verify(target)
            .DontScrubGuids();

        #endregion
    }

    [Fact]
    public Task GuidScrubbingDisabledNested() =>
        Verify(
                new
                {
                    value = Guid.Parse("b6993f86-c1b9-44db-bfc5-33ed9e5c048e")
                })
            .DontScrubGuids();

    [Fact]
    public Task ScrubberWithBadNewLine() =>
        Verify("a")
            .AddScrubber(s =>
            {
                s.AppendLine("b");
                s.AppendLine("c");
            });

    [Fact]
    public Task ExtensionAwareScrubbers()
    {
        var settings = new VerifySettings();
        settings.UseExtension("html");
        settings.AddScrubber("html", builder => builder.Replace("a", "b"));
        return Verify("a", settings);
    }

    [Fact]
    public Task NameValueCollection() =>
        Verify(
            new
            {
                item1 = new NameValueCollection {{null, null}},
                item2 = new NameValueCollection {{"key", null}},
                item3 = new NameValueCollection {{null, "value"}},
                item4 = new NameValueCollection {{"key", "value"}},
                item5 = new NameValueCollection {{"key", "value1"}, {"key", "value2"}},
                item6 = new NameValueCollection {{"key", null}, {"key", "value2"}},
                item7 = new NameValueCollection {{"key", "value1"}, {"key", null}},
                item8 = new NameValueCollection {{"key1", "value1"}, {"key2", "value2"}}
            });

    [Fact]
    public Task Timespan() =>
        Verify(
            new
            {
                timespan = TimeSpan.FromDays(1)
            });

    [Fact]
    public Task EmptyDictionaryProperty() =>
        Verify(new
        {
            property = new Dictionary<string, string>()
        });


    [Fact]
    public Task ByteArray() =>
        Verify(
            new
            {
                bytes = new byte[] {1}
            });

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
        return Verify(person, settings);
    }

    [Theory]
    [MemberData(nameof(GetBoolData))]
    public Task Bools(bool boolean, bool? nullableBoolean, bool dontIgnoreFalse, bool includeDefault)
    {
        var target = new BoolModel
        {
            BoolMember = boolean,
            NullableBoolMember = nullableBoolean
        };

        var settings = new VerifySettings();
        settings.UseParameters(boolean, nullableBoolean, dontIgnoreFalse, includeDefault);
        settings.ModifySerialization(serialization =>
        {
            if (dontIgnoreFalse)
            {
                serialization.DontIgnoreFalse();
            }

            if (includeDefault)
            {
                serialization.AddExtraSettings(_ => _.DefaultValueHandling = DefaultValueHandling.Include);
            }
        });

        return Verify(target, settings);
    }

    public static IEnumerable<object?[]> GetBoolData()
    {
        foreach (var boolean in new[] {true, false})
        foreach (var nullableBoolean in new bool?[] {true, false, null})
        foreach (var dontIgnoreFalse in new[] {true, false})
        foreach (var includeDefault in new[] {true, false})
        {
            yield return new object?[]
            {
                boolean,
                nullableBoolean,
                dontIgnoreFalse,
                includeDefault
            };
        }
    }

    class BoolModel
    {
        public bool BoolMember;
        public bool? NullableBoolMember;
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

        return Verify(person)
            .AddExtraSettings(_ => _.TypeNameHandling = TypeNameHandling.All);
    }

#if NET6_0_OR_GREATER
    [Fact]
    public Task TimeOnlyNested() =>
        Verify(new {value = new TimeOnly(10, 10)});

    [Fact]
    public Task ShouldIgnoreDatetimeDefaults()
    {
        var target = new DateTimeTarget();

        return Verify(target);
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
            DateTimeOffsetString = dateTimeOffset.ToString("F")
        };

        await Verify(target);

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
            DateTimeOffsetString = dateTimeOffset.ToString("F")
        };

        await Verify(target);
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
            DateTimeOffsetString = dateTimeOffset.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK")
        };

        await Verify(target);
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

        return Verify(target);
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
    public Task VerifyBytes() =>
        Verify(File.ReadAllBytes("sample.jpg"))
            .UseExtension("jpg");

    [Fact]
    public Task ShouldNotScrubInlineGuidsByDefault()
    {
        var id = new Guid("ebced679-45d3-4653-8791-3d969c4a986c");
        var product = new
        {
            Title = $"item {id} - (ID={{{id}}})",
            Variant = new
            {
                Id = "variant id: " + id
            }
        };

        return Verify(product);
    }

    [Fact]
    public Task ShouldBeAbleToExcludeInlineGuidsInString()
    {
        var id = Guid.NewGuid();
        return Verify($"The string {id} ")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldBeAbleToExcludeInlineGuidsWrappedInSymbols()
    {
        var id = Guid.NewGuid();
        return Verify($"({id})")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldNotExcludeInlineGuidsWrappedInDash() =>
        Verify("-087ea433-d83b-40b6-9e37-465211d9508-")
            .ScrubInlineGuids();

    [Fact]
    public Task ShouldNotExcludeInlineGuidsWrappedInLetters() =>
        Verify("before087ea433-d83b-40b6-9e37-465211d9508cafter")
            .ScrubInlineGuids();

    [Fact]
    public Task ShouldNotExcludeInlineGuidsWrappedInNumber() =>
        Verify("1087ea433-d83b-40b6-9e37-465211d95081")
            .ScrubInlineGuids();

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

        return Verify(product)
            .ScrubInlineGuids();
    }

    void DontIgnoreEmptyCollections()
    {
        #region DontIgnoreEmptyCollections

        VerifierSettings.DontIgnoreEmptyCollections();

        #endregion
    }

    void DontScrubGuids()
    {
        #region DontScrubGuidsGlobal

        VerifierSettings.DontScrubGuids();

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

    [Fact]
    public Task ThrowForDateTimeZoneHandling() =>
        ThrowsTask(
                () => Verify("foo")
                    .AddExtraSettings(_ => _.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified))
            .IgnoreStackTrack();

    [Fact]
    public Task ThrowForDateFormatString() =>
        ThrowsTask(
                () => Verify("foo")
                    .AddExtraSettings(_ => _.DateFormatString = "DateFormatHandling.MicrosoftDateFormat"))
            .IgnoreStackTrack();

    Task DontScrubDateTimes()
    {
        #region DontScrubDateTimes

        var target = new
        {
            Date = DateTime.Now
        };

        var settings = new VerifySettings();
        settings.DontScrubDateTimes();

        return Verify(target, settings);

        #endregion
    }

    Task DontScrubDateTimesFluent()
    {
        #region DontScrubDateTimesFluent

        var target = new
        {
            Date = DateTime.Now
        };

        return Verify(target)
            .DontScrubDateTimes();

        #endregion
    }

    void DontScrubDateTimesGlobal()
    {
        #region DontScrubDateTimesGlobal

        VerifierSettings.DontScrubDateTimes();

        #endregion
    }

    void DontIgnoreFalseGlobal()
    {
        #region DontIgnoreFalse

        VerifierSettings.DontIgnoreFalse();

        #endregion
    }

    [Fact]
    public Task NewLineNotEscapedInProperty() =>
        Verify(new {Property = "a\r\nb\\nc"});

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
        return Verify(new
        {
            tempPath,
            altTempPath,
            tempPathTrailing = tempPath + Path.DirectorySeparatorChar,
            altTempPathTrailing = altTempPath + Path.AltDirectorySeparatorChar
        });
    }

    [Fact]
    public Task ScrubCurrentDirectory()
    {
        var currentDirectory = Environment.CurrentDirectory.TrimEnd('/', '\\');
        var altCurrentDirectory = currentDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verify(new
        {
            currentDirectory,
            altCurrentDirectory,
            currentDirectoryTrailing = currentDirectory + Path.DirectorySeparatorChar,
            altCurrentDirectoryTrailing = altCurrentDirectory + Path.AltDirectorySeparatorChar
        });
    }

    [Fact]
    public Task MoreSpecificScrubberShouldOverride()
    {
        var currentDirectory = Environment.CurrentDirectory.TrimEnd('/', '\\') + "Foo";
        return Verify(new
            {
                currentDirectory
            })
            .AddScrubber(builder => builder.Replace(currentDirectory, "Bar"));
    }

    [Fact]
    public Task ScrubUserProfile()
    {
        var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SomePath").Replace('\\', '/');
        return Verify(target).UniqueForOSPlatform();
    }

#if !NET5_0_OR_GREATER
    [Fact]
    public Task ScrubCodeBaseLocation()
    {
        var codeBaseLocation = CodeBaseLocation.CurrentDirectory!.TrimEnd('/', '\\');
        var altCodeBaseLocation = codeBaseLocation.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verify(new {codeBaseLocation, altCodeBaseLocation});
    }
#endif

    [Fact]
    public Task ScrubBaseDirectory()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory!.TrimEnd('/', '\\');
        var altBaseDirectory = baseDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verify(new {baseDirectory, altBaseDirectory});
    }

    class TypeTarget
    {
        public Type Type;
        public Type Dynamic;
    }

    [Fact]
    public Task QuoteEscaping() =>
        Verify(
            new
            {
                singleQuote = "'",
                doubleQuote = "\"",
                mixed = "\"'"
            });

    [Fact]
    public async Task ShouldUseShortTypeName()
    {
        #region type

        var foo = new {x = 1};
        var target = new TypeTarget
        {
            Type = GetType(),
            Dynamic = foo.GetType()
        };

        await Verify(target);

        #endregion
    }

#if (!NETSTANDARD2_0 && !NET461)
    [Fact]
    public async Task NamedTuple()
    {
        #region VerifyTuple

        await VerifyTuple(() => MethodWithNamedTuple());

        #endregion
    }

    #region MethodWithNamedTuple

    static (bool Member1, string Member2, string Member3) MethodWithNamedTuple() =>
        (true, "A", "B");

    #endregion

#if !NET461
    [Fact]
    public async Task PartialNamedTuple()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => VerifyTuple(() => MethodWithPartialNamedTuple()));
        PrefixUnique.Clear();
        await Verify(exception.Message);
    }
#endif

    static (bool, string Member2, string Member3) MethodWithPartialNamedTuple() =>
        (true, "A", "B");

    [Fact]
    public Task NamedTupleWithNull() =>
        VerifyTuple(() => MethodWithNamedTupleWithNull());

    [Fact]
    public Task Claim() =>
        Verify(new Claim("TheType", "TheValue"));

    [Fact]
    public Task ClaimWithClaimType() =>
        Verify(new Claim(ClaimTypes.Email, "TheValue"));

    [Fact]
    public Task ClaimsPrincipal()
    {
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new("TheType", "TheValue"));
        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(claimsIdentity);
        return Verify(claimsPrincipal);
    }

    [Fact]
    public Task ClaimsIdentity()
    {
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new("TheType", "TheValue"));
        return Verify(claimsIdentity);
    }

    static (string Member1, string? Member2) MethodWithNamedTupleWithNull() =>
        ("A", null);

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
            OtherGuid = Guid.NewGuid()
        };

        await Verify(target);

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
            OtherGuid = Guid.NewGuid()
        };

        await Verify(target);
    }

    [Fact]
    public Task ShouldIgnoreGuidDefaults()
    {
        var target = new GuidTarget();
        return Verify(target);
    }

    [Fact]
    public Task ShouldScrubProjectDirectory()
    {
        var projectDirectory = FileEx.GetProjectDirectory();
        var path = Path.GetFullPath(Path.Combine(projectDirectory, "Foo"));
        var altPath = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verify(
            new
            {
                path,
                trimmed = path.TrimEnd('/', '\\'),
                altPath,
                altPathTrimmed = altPath.TrimEnd('/', '\\')
            });
    }

    [Fact]
    public Task ShouldScrubSolutionDirectory()
    {
        var solutionDirectory = FileEx.GetSolutionDirectory();
        var path = Path.GetFullPath(Path.Combine(solutionDirectory, "Foo"));
        var altPath = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verify(
            new
            {
                path,
                trimmed = path.TrimEnd('/', '\\'),
                altPath,
                altPathTrimmed = altPath.TrimEnd('/', '\\')
            });
    }


    [Fact]
    public Task ShouldScrubGuid()
    {
        var target = new GuidTarget
        {
            Guid = Guid.NewGuid(),
            GuidNullable = Guid.NewGuid(),
            GuidString = Guid.NewGuid().ToString()
        };
        return Verify(target);
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
        return Verify(target);
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
        return Verify(target);
    }

    class NotDatesTarget
    {
        public string NotDate;
    }

    [Fact]
    public Task ShouldScrubDictionaryKey() =>
        Verify(
            new
            {
                guid = new Dictionary<Guid, string>
                {
                    {Guid.NewGuid(), "value"}
                },
                dateTime = new Dictionary<DateTime, string>
                {
                    {DateTime.Now, "value"}
                },
                dateTimeOffset = new Dictionary<DateTimeOffset, string>
                {
                    {DateTimeOffset.Now, "value"}
                },
                type = new Dictionary<Type, string>
                {
                    {typeof(SerializationTests), "value"}
                }
            });

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
        return Verify(target);
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

        VerifierSettings.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore");

        #endregion
    }

    #region IgnoreMembersThatThrowExpression

    [Fact]
    public Task ExceptionMessageProp()
    {
        var target = new WithExceptionIgnoreMessage();

        var settings = new VerifySettings();
        settings.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore");
        return Verify(target, settings);
    }

    [Fact]
    public Task ExceptionMessagePropFluent()
    {
        var target = new WithExceptionIgnoreMessage();

        return Verify(target)
            .IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore");
    }

    #endregion

    [Fact]
    public Task ExpressionString()
    {
        var parameter = Expression.Parameter(typeof(Exception), "source");
        var property = Expression.Property(parameter, "Message");
        var convert = Expression.Convert(property, typeof(object));
        var expression = Expression.Lambda<Func<Exception, object>>(convert, parameter);
        return Verify(expression)
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
        return Verify(target);
    }

    class WithNotImplementedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotImplementedException();
    }

    [Fact]
    public Task TargetInvocationException()
    {
        var member = GetType().GetMethod("MethodThatThrows")!;
        return Throws(() =>
        {
            member.Invoke(null, Array.Empty<object>());
        });
    }

    [Fact]
    public Task NestedTargetInvocationException()
    {
        var member = GetType().GetMethod("MethodThatThrows")!;
        TargetInvocationException? exception = null;
        try
        {
            member.Invoke(null, Array.Empty<object>());
        }
        catch (TargetInvocationException e)
        {
            exception = e;
        }

        return Verify(new {exception});
    }

    public static void MethodThatThrows() =>
        throw new ("the message");

    void AddIgnoreInstanceGlobal()
    {
        #region AddIgnoreInstanceGlobal

        VerifierSettings.IgnoreInstance<Instance>(x => x.Property == "Ignore");

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
        settings.IgnoreInstance<Instance>(x => x.Property == "Ignore");
        return Verify(target, settings);
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
        return Verify(target)
            .IgnoreInstance<Instance>(x => x.Property == "Ignore");
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

        VerifierSettings.IgnoreMembersWithType<ToIgnore>();

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
            ToIgnoreNullable = new()
            {
                Property = "Value"
            },
            ToIgnoreByInterface = new()
            {
                Property = "Value"
            },
            ToIgnoreByBase = new()
            {
                Property = "Value"
            },
            ToIgnoreByBaseGeneric = new()
            {
                Property = "Value"
            },
            ToIgnoreByType = new()
            {
                Property = "Value"
            },
            ToInclude = new()
            {
                Property = "Value"
            },
            ToIncludeNullable = new()
            {
                Property = "Value"
            },
            ToIgnoreStruct = new("Value"),
            ToIgnoreStructNullable = new("Value"),
            ToIncludeStruct = new("Value"),
            ToIncludeStructNullable = new("Value")
        };
        var settings = new VerifySettings();
        settings.ModifySerialization(_ =>
        {
            _.IgnoreMembersWithType<ToIgnore>();
            _.IgnoreMembersWithType<ToIgnoreByType>();
            _.IgnoreMembersWithType<InterfaceToIgnore>();
            _.IgnoreMembersWithType<BaseToIgnore>();
            _.IgnoreMembersWithType(typeof(BaseToIgnoreGeneric<>));
            _.IgnoreMembersWithType<ToIgnoreStruct>();
        });
        return Verify(target, settings);
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
            ToIgnoreNullable = new()
            {
                Property = "Value"
            },
            ToIgnoreByInterface = new()
            {
                Property = "Value"
            },
            ToIgnoreByBase = new()
            {
                Property = "Value"
            },
            ToIgnoreByBaseGeneric = new()
            {
                Property = "Value"
            },
            ToIgnoreByType = new()
            {
                Property = "Value"
            },
            ToInclude = new()
            {
                Property = "Value"
            },
            ToIncludeNullable = new()
            {
                Property = "Value"
            },
            ToIgnoreStruct = new("Value"),
            ToIgnoreStructNullable = new("Value"),
            ToIncludeStruct = new("Value"),
            ToIncludeStructNullable = new("Value")
        };
        return Verify(target)
            .ModifySerialization(_ =>
            {
                _.IgnoreMembersWithType<ToIgnore>();
                _.IgnoreMembersWithType<ToIgnoreByType>();
                _.IgnoreMembersWithType<InterfaceToIgnore>();
                _.IgnoreMembersWithType<BaseToIgnore>();
                _.IgnoreMembersWithType(typeof(BaseToIgnoreGeneric<>));
                _.IgnoreMembersWithType<ToIgnoreStruct>();
            });
    }

    #endregion

    [Fact]
    public Task IgnoreMembersNullable()
    {
        ToIgnoreStruct? toIgnoreStruct = new ToIgnoreStruct("Value");

        return Verify(toIgnoreStruct)
            .IgnoreMembers<ToIgnoreStruct>(_ => _.Property);
    }

    [Fact]
    public Task IgnoreMembersNullableNested()
    {
        var target = new IgnoreMembersNullableNestedTarget
        {
            ToIgnoreStruct = new ToIgnoreStruct("Value")
        };

        return Verify(target)
            .IgnoreMembers<IgnoreMembersNullableNestedTarget>(_ => _.ToIgnoreStruct);
    }

    class IgnoreMembersNullableNestedTarget
    {
        public ToIgnoreStruct? ToIgnoreStruct { get; set; }
    }

    [Fact]
    public Task Type() =>
        Verify(GetType());

    [Fact]
    public Task Field()
    {
        var target = Info.OfField<SerializationTests>("MyField");
        return Verify(target);
    }

    public string MyField;

    [Fact]
    public Task GetProperty()
    {
        var target = Info.OfPropertyGet<SerializationTests>("MyProperty");
        return Verify(target);
    }

    [Fact]
    public Task SetProperty()
    {
        var target = Info.OfPropertySet<SerializationTests>("MyProperty");
        return Verify(target);
    }

    [Fact]
    public Task Property()
    {
        var target = typeof(SerializationTests).GetProperty("MyProperty");
        return Verify(target);
    }

    public string MyProperty { get; set; }

    [Fact]
    public Task Method() =>
        Verify(Info.OfMethod<SerializationTests>("Method"));

    [Fact]
    public Task Constructor() =>
        Verify(Info.OfConstructor<SerializationTests>());

    [Fact]
    public Task Parameter()
    {
        var method = Info.OfMethod<SerializationTests>("MyMethodWithParameters");
        return Verify(method.GetParameters().First());
    }

    [Fact]
    public Task MethodWithParameters() =>
        Verify(Info.OfMethod<SerializationTests>("MyMethodWithParameters"));

    // ReSharper disable UnusedParameter.Local
    void MyMethodWithParameters(int x, string y)
        // ReSharper restore UnusedParameter.Local
    {
    }

    class IgnoreTypeTarget
    {
        public ToIgnore ToIgnore;
        public ToIgnoreByType ToIgnoreByType;
        public ToIgnoreByInterface ToIgnoreByInterface;
        public ToIgnoreByBase ToIgnoreByBase;
        public ToIgnoreByBaseGeneric ToIgnoreByBaseGeneric;
        public ToIgnore ToIgnoreNullable;
        public ToIgnoreStruct ToIgnoreStruct;
        public ToIgnoreStruct? ToIgnoreStructNullable;
        public ToInclude ToInclude;
        public ToInclude ToIncludeNullable;
        public ToIncludeStruct ToIncludeStruct;
        public ToIncludeStruct? ToIncludeStructNullable;
    }

    struct ToIncludeStruct
    {
        public ToIncludeStruct(string property) =>
            Property = property;

        public string Property { get; }
    }

    struct ToIgnoreStruct
    {
        public ToIgnoreStruct(string property) =>
            Property = property;

        public string Property { get; }
    }

    class ToInclude
    {
        public string Property;
    }

    class ToIgnore
    {
        public string Property;
    }

    class ToIgnoreByType
    {
        public string Property;
    }

    class ToIgnoreByInterface :
        InterfaceToIgnore
    {
        public string Property;
    }

    class InterfaceToIgnore
    {
    }

    class ToIgnoreByBase :
        BaseToIgnore
    {
        public string Property;
    }

    class BaseToIgnore
    {
    }
    class ToIgnoreByBaseGeneric :
        BaseToIgnoreGeneric<int>
    {
        public string Property;
    }

// ReSharper disable once UnusedTypeParameter
    class BaseToIgnoreGeneric<T>
    {
    }

    void IgnoreMemberByExpressionGlobal()
    {
        #region IgnoreMemberByExpressionGlobal

        VerifierSettings.IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
        VerifierSettings.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyWithPropertyName);
        VerifierSettings.IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
        VerifierSettings.IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
        VerifierSettings.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);

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
        return Verify(target, settings);
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
        return Verify(target)
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
        var input = new MemberTarget
        {
            Field = "FieldValue",
            Property = "PropertyValue"
        };

        // using only the member
        VerifierSettings.MemberConverter<MemberTarget, string>(
            expression: x => x.Field,
            converter: member => $"{member}_Suffix");

        // using target and member
        VerifierSettings.MemberConverter<MemberTarget, string>(
            expression: x => x.Property,
            converter: (target, member) => $"{target}_{member}_Suffix");

        return Verify(input);
    }

    #endregion

    class MemberTarget
    {
        public string Property { get; set; }
        public string Field;
    }

    void IgnoreMemberByNameGlobal()
    {
        #region IgnoreMemberByNameGlobal

        // For all types
        VerifierSettings.IgnoreMember("PropertyByName");

        // For a specific type
        VerifierSettings.IgnoreMember(typeof(IgnoreExplicitTarget), "Property");

        // For a specific type generic
        VerifierSettings.IgnoreMember<IgnoreExplicitTarget>("Field");

        // For a specific type with expression
        VerifierSettings.IgnoreMember<IgnoreExplicitTarget>(_ => _.PropertyThatThrows);

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
            // For all types
            _.IgnoreMember("PropertyByName");

            // For a specific type
            _.IgnoreMember(typeof(IgnoreExplicitTarget), "Property");

            // For a specific type generic
            _.IgnoreMember<IgnoreExplicitTarget>("Field");

            // For a specific type with expression
            _.IgnoreMember<IgnoreExplicitTarget>(_ => _.PropertyThatThrows);
        });
        return Verify(target, settings);
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
        return Verify(target)
            .ModifySerialization(_ =>
            {
                // For all types
                _.IgnoreMember("PropertyByName");

                // For a specific type
                _.IgnoreMember(typeof(IgnoreExplicitTarget), "Property");

                // For a specific type generic
                _.IgnoreMember<IgnoreExplicitTarget>("Field");

                // For a specific type with expression
                _.IgnoreMember<IgnoreExplicitTarget>(_ => _.PropertyThatThrows);
            });
    }

    #endregion

    public class IgnoreTargetBase
    {
        public string Property { get; set; }
    }

    public class IgnoreTargetSub :
        IgnoreTargetBase
    {
    }

    [Fact]
    public Task IgnoreMemberSubClass() =>
        Throws(() => VerifierSettings.IgnoreMember<IgnoreTargetSub>(_ => _.Property))
            .IgnoreStackTrack();

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
        return Verify(target).IgnoreMember("Ignore1");
    }

    [Fact]
    public Task VerifyJsonGuid()
    {
        var json = "{'key': 'c572ff75-e1a2-49bd-99b9-4550697946c3'}";
        return VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonDateTime()
    {
        var json = $"{{'key': '{DateTime.Now:yyyy-MM-ddTHH:mm:ss}'}}";
        return VerifyJson(json);
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
        return VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonWithArrayAtRoot()
    {
        var json = @"[
      {
        id: '9585dadf-551a-43eb-960c-18b935993cc3',
        title: 'Commitment1'
      }
    ]";
        return VerifyJson(json);
    }

    #region VerifyJson

    [Fact]
    public Task VerifyJsonString()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        return VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonStream()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return VerifyJson(stream);
    }

    [Fact]
    public Task StreamMember()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("value"));
        return Verify(new{stream});
    }

    [Fact]
    public Task VerifyJsonJToken()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        var target = JToken.Parse(json);
        return VerifyJson(target);
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
                    {"Ignore", "Value1"},
                    {"Key1", "Value2"}
                }
            },
            {"Ignore", "Value3"},
            {"Key2", "Value4"}
        };
        return Verify(target)
            .IgnoreMember("Ignore");
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

        VerifierSettings.IgnoreMembersThatThrow<CustomException>();

        #endregion
    }

    #region IgnoreMembersThatThrow

    [Fact]
    public Task CustomExceptionProp()
    {
        var target = new WithCustomException();
        var settings = new VerifySettings();
        settings.IgnoreMembersThatThrow<CustomException>();
        return Verify(target, settings);
    }

    [Fact]
    public Task CustomExceptionPropFluent()
    {
        var target = new WithCustomException();
        return Verify(target)
            .IgnoreMembersThatThrow<CustomException>();
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
            return Verify(exception);
        }
    }

    [Fact]
    public Task ExceptionProp()
    {
        var settings = new VerifySettings();
        settings.IgnoreMembersThatThrow<CustomException>();

        var target = new WithException();

        return Assert.ThrowsAsync<JsonSerializationException>(() => Verify(target, settings));
    }

    class WithException
    {
        public Guid ExceptionProperty => throw new();
    }

    [Fact]
    public Task ExceptionNotIgnoreMessageProp()
    {
        var settings = new VerifySettings();
        settings.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore");
        var target = new WithExceptionNotIgnoreMessage();

        return Assert.ThrowsAsync<JsonSerializationException>(() => Verify(target, settings));
    }

    class WithExceptionNotIgnoreMessage
    {
        public Guid ExceptionMessageProperty => throw new("NotIgnore");
    }

    [Fact]
    public Task DelegateProp()
    {
        var target = new WithDelegate();
        return Verify(target);
    }

    class WithDelegate
    {
        public Action DelegateProperty => () =>
        {
        };
    }

    class CustomException :
        Exception
    {
    }

    [Fact]
    public Task NotSupportedExceptionProp()
    {
        var target = new WithNotSupportedException();
        return Verify(target);
    }

    class WithNotSupportedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotSupportedException();
    }


    void WithObsoletePropIncludedGlobally()
    {
        #region WithObsoletePropIncludedGlobally

        VerifierSettings.IncludeObsoletes();

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
        settings.IncludeObsoletes();
        return Verify(target, settings);
    }

    [Fact]
    public Task WithObsoletePropIncludedFluent()
    {
        var target = new WithObsolete
        {
            ObsoleteProperty = "value1",
            OtherProperty = "value2"
        };
        return Verify(target)
            .IncludeObsoletes();
    }

    #endregion

    #region WithObsoleteProp

    class WithObsolete
    {
        [Obsolete] public string ObsoleteProperty { get; set; }

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
        return Verify(target);
    }

    #endregion

#pragma warning restore 612
    [Fact]
    public async Task Tuple()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => VerifyTuple(() => MethodWithTuple()));
        PrefixUnique.Clear();
        await Verify(exception.Message);
    }

    static (bool, string, string) MethodWithTuple() =>
        (true, "A", "B");

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
        settings.AddExtraSettings(_ => _.TypeNameHandling = TypeNameHandling.All);
        return Verify(person, settings);
    }

    [Fact]
    public Task ScopedSerializerFluent()
    {
        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith"
        };
        return Verify(person)
            .AddExtraSettings(_ => _.TypeNameHandling = TypeNameHandling.All);
    }

    #endregion

    [Fact]
    public Task WithConverter() =>
        Verify(new ConverterTarget {Name = "The name"})
            .AddExtraSettings(_ => _.Converters.Add(new Converter()));

    [Fact]
    public Task WithConverterAndNewline() =>
        Verify(new ConverterTarget {Name = "A\rB\nC\r\nD"})
            .AddExtraSettings(_ => _.Converters.Add(new Converter()));

    [Fact]
    public Task WithConverterAndIgnore() =>
        Verify(new ConverterTarget {Name = "The name"})
            .IgnoreMember("Name")
            .AddExtraSettings(_ => _.Converters.Add(new Converter()));

    class Converter :
        WriteOnlyJsonConverter<ConverterTarget>
    {
        public override void Write(VerifyJsonWriter writer, ConverterTarget target)
        {
            writer.WriteStartObject();
            writer.WriteProperty(target, target.Name, "Name");
            writer.WritePropertyName("Custom");
            writer.WriteValue("CustomValue");
            writer.WriteEnd();
        }
    }

    class ConverterTarget
    {
        public string Name { get; set; } = null!;
    }

    [Fact]
    public Task WithConverterAndMemberConverter()
    {
        VerifierSettings.MemberConverter<StaticConverterTarget, string>(
            target => target.Name,
            (target, value) => "New Value");
        return Verify(new StaticConverterTarget {Name = "The name"})
            .AddExtraSettings(_ => _.Converters.Add(new StaticConverter()));
    }

    class StaticConverter :
        WriteOnlyJsonConverter<StaticConverterTarget>
    {
        public override void Write(VerifyJsonWriter writer, StaticConverterTarget target)
        {
            writer.WriteStartObject();
            writer.WriteProperty(target, target.Name, "Name");
            writer.WritePropertyName("Custom");
            writer.WriteValue("CustomValue");
            writer.WriteEnd();
        }
    }

    class StaticConverterTarget
    {
        public string Name { get; set; } = null!;
    }
}