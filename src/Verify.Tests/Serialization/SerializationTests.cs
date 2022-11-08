using System.Collections.ObjectModel;
using System.Security.Claims;

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
    public Task Tasks()
    {
        var withResult = Task.FromResult("Value");
        var withException = Task.FromException(new("the exception"));
        var withExceptionAndResult = Task.FromException<string>(new("the exception"));
        var genericCompletionSource = new TaskCompletionSource<int>();
        genericCompletionSource.TrySetCanceled();
        var canceledAndResult = genericCompletionSource.Task;
        var finished = Task.Delay(0);
        var running = Task.Delay(10000);
        return Verify(
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

    #region AddExtraDatetimeFormat

    [ModuleInitializer]
    public static void UseAddExtraDatetimeFormat() =>
        VerifierSettings.AddExtraDatetimeFormat("yyyy-MM-dd");

    [Fact]
    public Task WithExtraDatetimeFormat() =>
        Verify(
            new
            {
                date = "2022-11-08"
            });

    #endregion

#if NET5_0_OR_GREATER || net48
    [Fact]
    public Task ValueTasks()
    {
        var withResult = ValueTask.FromResult("Value");
        var withException = ValueTask.FromException(new("the exception"));
        var withExceptionAndResult = ValueTask.FromException<string>(new("the exception"));
        var genericCompletionSource = new TaskCompletionSource<int>();
        genericCompletionSource.TrySetCanceled();
        var canceledAndResult = genericCompletionSource.Task;
        return Verify(
                new
                {
                    withResult,
                    withException,
                    withExceptionAndResult,
                    canceledAndResult
                })
            .UniqueForRuntime();
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
            {
                1, "1234"
            },
            {
                2, "5678"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task SortedDictionaryOrder()
    {
        var dictionary = new SortedDictionary<string, string>(new DescendingComparer<string>())
        {
            {
                "Entry_1", "1234"
            },
            {
                "ignored", "1234"
            },
            {
                "Entry_2", "5678"
            }
        };

        return Verify(dictionary)
            .IgnoreMember("ignored");
    }

    #region DontSortDictionaries

    [Fact]
    public Task DontSortDictionaries()
    {
        var dictionary = new Dictionary<string, string>
        {
            {
                "Entry_1", "1234"
            },
            {
                "Entry_3", "1234"
            },
            {
                "Entry_2", "5678"
            }
        };

        return Verify(dictionary)
            .DontSortDictionaries();
    }

    #endregion

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
        var dictionary = new Dictionary<string, string>();

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

        return Verify(dictionary);
    }

    [Fact]
    public Task DictionaryOrderOrdinal()
    {
        var dictionary = new Dictionary<string, string>();

        if (DateTime.UtcNow.Ticks % 2 == 0)
        {
            dictionary.Add("+", "plus");
            dictionary.Add("-", "minus");
        }
        else
        {
            dictionary.Add("-", "minus");
            dictionary.Add("+", "plus");
        }

        return Verify(dictionary);
    }

    [Fact]
    public Task DictionaryOrderStringAndIgnore()
    {
        var dictionary = new Dictionary<string, string>
        {
            {"ignored", "1234"},
            {"Entry_2", "5678"},
            {"Entry_1", "1234"}
        };

        return Verify(dictionary)
            .IgnoreMember("ignored");
    }

    [Fact]
    public Task DatetimeOffsetScrubbingDisabled() =>
        Verify(
                new
                {
                    noTime = new DateTimeOffset(new DateTime(2000, 1, 1), TimeSpan.FromHours(1.5)),
                    withTime = new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 1), TimeSpan.FromHours(1)),
                    withTimeZeroSeconds = new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1,0), TimeSpan.FromHours(1)),
                    withTimeMilliSeconds = new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 1, 999), TimeSpan.FromHours(1)),
                })
            .DontScrubDateTimes();

    [Fact]
    public Task OnlyScrubInlineProperties() =>
        Verify(
                new
                {
                    property = @"
line1
line2
line3"
                })
            .ScrubLinesContaining("property")
            .ScrubLinesContaining("line2");

    [Fact]
    public Task DatetimeScrubbingDisabled_ExplicitScrubber() =>
        Verify(
                new
                {
                    dateTimeNoTime = new DateTime(2000, 1, 1),
                    dateTimeWithTimeTime = new DateTime(2000, 1, 1, 1, 1, 1),
                    dateTimeOffsetNoTime = new DateTimeOffset(new DateTime(2000, 1, 1), TimeSpan.Zero),
                    dateTimeOffsetWithTime = new DateTimeOffset(new DateTime(2000, 1, 1), TimeSpan.FromHours(1))
                })
            .DontScrubDateTimes()
            .ScrubLinesWithReplace(_ => "replaced");

#if NET6_0_OR_GREATER

    [Fact]
    public Task DateScrubbingDisabled_ExplicitScrubber() =>
        Verify(
                new
                {
                    time = new TimeOnly(10, 1, 1),
                    value = new DateOnly(2000, 1, 1),
                })
            .DontScrubDateTimes()
            .ScrubLinesWithReplace(_ => "replaced");

#endif

    #region AddExtraSettings

    [Fact]
    public Task AddExtraSettings()
    {
        var settings = new VerifySettings();
        settings
            .AddExtraSettings(
                _ => _.Error += (sender, args)
                    => Console.WriteLine(args.ErrorContext.Member));
        return Verify("Value", settings);
    }

    #endregion

    #region AddExtraSettingsFluent

    [Fact]
    public Task AddExtraSettingsFluent() =>
        Verify("Value")
            .AddExtraSettings(
                _ => _.Error += (sender, args)
                    => Console.WriteLine(args.ErrorContext.Member));

    #endregion


    void AddExtraSettingsGlobal()
    {
        #region AddExtraSettingsGlobal

        VerifierSettings
            .AddExtraSettings(_ =>
                _.TypeNameHandling = TypeNameHandling.All);

        #endregion
    }

    [Theory]
    [InlineData(TypeNameHandling.All)]
    [InlineData(TypeNameHandling.Arrays)]
    [InlineData(TypeNameHandling.Auto)]
    [InlineData(TypeNameHandling.None)]
    [InlineData(TypeNameHandling.Objects)]
    public Task TypeNameHandlingInArray(TypeNameHandling typeHandling)
    {
        var target = new TypeNameHandlingAutoInArrayTarget
        {
            Item = new[]
            {
                new TypeNameHandlingAutoInArrayItemChild()
            }
        };

        return Verify(target)
            .AddExtraSettings(_ =>
            {
                _.TypeNameHandling = typeHandling;
            })
            .UseParameters(typeHandling);
    }

    class TypeNameHandlingAutoInArrayTarget
    {
        public TypeNameHandlingAutoInArrayItem[] Item { get; set; } = Array.Empty<TypeNameHandlingAutoInArrayItem>();
    }

    abstract class TypeNameHandlingAutoInArrayItem
    {
    }

    class TypeNameHandlingAutoInArrayItemChild : TypeNameHandlingAutoInArrayItem
    {
    }

    [Fact]
    public void SettingsIsCloned()
    {
        var settings = new SerializationSettings();

        settings.IgnoreMember(GetType(), "ignored");

        settings.IgnoreInstance(GetType(), _ => _ == this);

        settings.IgnoreMember("ignored");

        var clone = new SerializationSettings(settings);

        Assert.NotSame(settings, clone);

        Assert.True(clone.GetShouldIgnoreInstance(GetType(), out var shouldIgnores));
        var shouldIgnore = shouldIgnores.Single();
        Assert.Equal(ScrubOrIgnore.Ignore, shouldIgnore(this));
        Assert.Null(shouldIgnore("notIgnored"));
        Assert.True(clone.TryGetScrubOrIgnoreByMemberOfType(GetType(), "ignored", out var scrubOrIgnore));
        Assert.Equal(ScrubOrIgnore.Ignore, scrubOrIgnore);
        Assert.False(clone.TryGetScrubOrIgnoreByMemberOfType(GetType(), "notIgnored", out scrubOrIgnore));
        Assert.True(clone.TryGetScrubOrIgnoreByName("ignored", out scrubOrIgnore));
        Assert.Equal(ScrubOrIgnore.Ignore, scrubOrIgnore);
        Assert.False(clone.TryGetScrubOrIgnoreByName("notIgnored", out scrubOrIgnore));
    }

    [Fact]
    public Task IgnoreOnInterfaceNamed() =>
        Verify(
                new IgnoreOnInterfaceChild
                {
                    Value = 10
                })
            .IgnoreMember("Value");

    [Fact]
    public Task IgnoreOnInterfaceTyped() =>
        Verify(
                new IgnoreOnInterfaceChild
                {
                    Value = 10
                })
            .IgnoreMember<IIgnoreOnInterface>("Value");

    [Fact]
    public Task IgnoreOnInterfaceChildTyped() =>
        Verify(
                new IgnoreOnInterfaceChild
                {
                    Value = 10
                })
            .IgnoreMember<IgnoreOnInterfaceChild>("Value");

    interface IIgnoreOnInterface
    {
        public int Value { get; set; }
    }

    class IgnoreOnInterfaceChild : IIgnoreOnInterface
    {
        public int Value { get; set; }
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
    public Task GuidScrubbingDisabled_ExplicitScrubber() =>
        Verify(
                new
                {
                    value = Guid.Parse("b6993f86-c1b9-44db-bfc5-33ed9e5c048e")
                })
            .DontScrubGuids()
            .ScrubLinesWithReplace(_ => "replaced");

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
            .AddScrubber(_ =>
            {
                _.AppendLine("b");
                _.AppendLine("c");
            });

    [Fact]
    public Task ExtensionAwareScrubbers()
    {
        var settings = new VerifySettings();
        settings.AddScrubber("html", _ => _.Replace("a", "b"));
        return Verify("a", "html", settings);
    }

    [Fact]
    public Task NameValueCollection() =>
        Verify(
            new
            {
                item1 = new NameValueCollection
                {
                    {
                        null, null
                    }
                },
                item2 = new NameValueCollection
                {
                    {
                        "key", null
                    }
                },
                item3 = new NameValueCollection
                {
                    {
                        null, "value"
                    }
                },
                item4 = new NameValueCollection
                {
                    {
                        "key", "value"
                    }
                },
                item5 = new NameValueCollection
                {
                    {
                        "key", "value1"
                    },
                    {
                        "key", "value2"
                    }
                },
                item6 = new NameValueCollection
                {
                    {
                        "key", null
                    },
                    {
                        "key", "value2"
                    }
                },
                item7 = new NameValueCollection
                {
                    {
                        "key", "value1"
                    },
                    {
                        "key", null
                    }
                },
                item8 = new NameValueCollection
                {
                    {
                        "key1", "value1"
                    },
                    {
                        "key2", "value2"
                    }
                }
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
    public Task ExampleNonDefaults()
    {
        var person = new Person
        {
            Id = new("ebced679-45d3-4653-8791-3d969c4a986c"),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new()
            {
                "Sam",
                "Mary"
            },
            Address = new()
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        var settings = new VerifySettings();
        settings.DontScrubDateTimes();
        settings.DontScrubGuids();
        settings.DontIgnoreEmptyCollections();
        settings.AddScrubber(_ => _.Replace("Lane", "Street"));
        return Verify(person, settings);
    }

    [Fact]
    public Task ScrubberDefaultOrder() =>
        Verify("line")
            .AddScrubber(_ => _.Append(" one"))
            .AddScrubber(_ => _.Append(" two"));

    [Fact]
    public Task ScrubberInvertOrder() =>
        Verify("line")
            .AddScrubber(_ => _.Append(" one"), ScrubberLocation.Last)
            .AddScrubber(_ => _.Append(" two"), ScrubberLocation.Last);

    public static IEnumerable<object?[]> GetBoolData()
    {
        foreach (var boolean in new[]
                 {
                     true,
                     false
                 })
        foreach (var nullableBoolean in new bool?[]
                 {
                     true,
                     false,
                     null
                 })
        foreach (var includeDefault in new[]
                 {
                     true,
                     false
                 })
        {
            yield return new object?[]
            {
                boolean,
                nullableBoolean,
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
            Children = new()
            {
                "Sam",
                "Mary"
            },
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
    public Task TimeOnly() =>
        Verify(new TimeOnly(10, 10));

    [Fact]
    public Task TimeOnlyNested() =>
        Verify(new
        {
            value = new TimeOnly(10, 10)
        });

    [Fact]
    public Task TimeOnlyNestedWithNoScrubbing() =>
        Verify(new
            {
                value = new TimeOnly(10, 10)
            })
            .DontScrubDateTimes();

    [Fact]
    public Task TimeOnlyWithNoScrubbing() =>
        Verify(new TimeOnly(10, 10))
            .DontScrubDateTimes();

    [Fact]
    public Task ShouldIgnoreDatetimeDefaults()
    {
        var target = new DateTimeTarget();

        return Verify(target);
    }

    [Fact]
    public async Task DateOnlyWithNoScrubbing()
    {
        var target = new
        {
            DateOnly = new DateOnly(2020, 10, 10)
        };

        await Verify(target)
            .DontScrubDateTimes();
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
    public Task DatetimeMin()
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

        return Verify(target);
    }

    [Fact]
    public Task DatetimeMax()
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

        return Verify(target);
    }

    [Fact]
    public Task DatetimeDifferByKind()
    {
        var dateTimeLocal = new DateTime(2000, 10, 10, 1, 1, 1, DateTimeKind.Local);
        var dateTimeUtc = new DateTime(2000, 10, 10, 1, 1, 1, DateTimeKind.Utc);
        var dateTimeUnspecified = new DateTime(2000, 10, 10, 1, 1, 1, DateTimeKind.Unspecified);
        var target = new
        {
            dateTimeLocal,
            dateTimeUtc,
            dateTimeUnspecified
        };

        return Verify(target);
    }

    [Fact]
    public Task DatetimeOffsetDifferOffset()
    {
        var dateTime1 = new DateTimeOffset(new DateTime(2000, 10, 10,1,0,0),TimeSpan.FromHours(1));
        var dateTime2 = new DateTimeOffset(new DateTime(2000, 10, 10,2,0,0),TimeSpan.FromHours(2));
        var target = new
        {
            dateTime1,
            dateTime2
        };

        return Verify(target);
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
    public Task ShouldScrubInlineGuidsInString()
    {
        var id = Guid.NewGuid();
        return Verify($"The string {id} ")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task WriteRawInConverterTest()
    {
        var target = new WriteRawInConverterTarget();
        return Verify(target)
            .AddExtraSettings(_ => _.Converters.Add(new WriteRawInConverter())).ScrubEmptyLines();
    }

    class WriteRawInConverterTarget
    {
    }

    class WriteRawInConverter :
        WriteOnlyJsonConverter<WriteRawInConverterTarget>
    {
        public override void Write(VerifyJsonWriter writer, WriteRawInConverterTarget target)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Raw");
            writer.WriteRawValueIfNoStrict("Raw \" value");
            writer.WritePropertyName("WriteValue");
            writer.WriteValue("Write \" Value");
            writer.WritePropertyName("WriteRawWithScrubbers");
            writer.WriteRawValueWithScrubbers("Write \"\r\r\rRawWithScrubbers\r\r");
            writer.WriteEndObject();
        }
    }

    [Fact]
    public Task ShouldScrubInlineGuidsWrappedInSymbols()
    {
        var id = Guid.NewGuid();
        return Verify($"({id})")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldScrubInlineGuidsStartingWithSymbol()
    {
        var id = Guid.NewGuid();
        return Verify($"/{id}")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldScrubInlineGuidsEndingWithSymbol()
    {
        var id = Guid.NewGuid();
        return Verify($"{id}/")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldScrubInlineGuidsWrappedInNewLine()
    {
        var id = Guid.NewGuid();
        return Verify($@"
{id}
")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldScrubInlineGuidsWrappedWithSymbol()
    {
        var id = Guid.NewGuid();
        return Verify($"/{id}/")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ShouldNotScrubInlineGuidsWrappedInDash() =>
        Verify("-087ea433-d83b-40b6-9e37-465211d9508-")
            .ScrubInlineGuids();

    [Fact]
    public Task ShouldNotScrubInlineGuidsWrappedInLetters() =>
        Verify("before087ea433-d83b-40b6-9e37-465211d9508cafter")
            .ScrubInlineGuids();

    [Fact]
    public Task ShouldNotScrubInlineGuidsStartingInLetters() =>
        Verify("before087ea433-d83b-40b6-9e37-465211d9508")
            .ScrubInlineGuids();

    [Fact]
    public Task ShouldScrubInlineGuidsStartingInNewline1() =>
        Verify("\n087ea433-d83b-40b6-9e37-465211d95081")
            .ScrubInlineGuids();

    [Fact]
    public Task ShouldScrubInlineGuidsStartingInNewline2() =>
        Verify("\r087ea433-d83b-40b6-9e37-465211d95081")
            .ScrubInlineGuids();

    [Fact]
    public Task ShouldScrubInlineGuidsEndingInNewline1() =>
        Verify("087ea433-d83b-40b6-9e37-465211d95081\n")
            .ScrubInlineGuids();

    [Fact]
    public Task ShouldScrubInlineGuidsEndingInNewline2() =>
        Verify("087ea433-d83b-40b6-9e37-465211d95081\r")
            .ScrubInlineGuids();

    [Fact]
    public Task ShouldNotScrubInlineGuidsEndingLetters() =>
        Verify("087ea433-d83b-40b6-9e37-465211d95081after")
            .ScrubInlineGuids();

    [Fact]
    public Task ShouldNotScrubInlineGuidsWrappedInNumber() =>
        Verify("1087ea433-d83b-40b6-9e37-465211d950811")
            .ScrubInlineGuids();

    [Fact]
    public Task ScrubInlineGuids()
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

    void ScrubInlineGuidsGlobal()
    {
        #region ScrubInlineGuids

        VerifierSettings.ScrubInlineGuids();

        #endregion
    }

#if NET6_0_OR_GREATER

    [Fact]
    public Task DatetimeScrubbingDisabled() =>
        Verify(
                new
                {
                    noTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    withTime = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                    withTimeZeroSeconds = new DateTime(2000, 1, 1, 1, 1, 0, DateTimeKind.Utc),
                    withTimeMilliSeconds = new DateTime(2000, 1, 1, 1, 1, 1, 999, DateTimeKind.Utc),
                })
            .DontScrubDateTimes();

    [Fact]
    Task DontScrubDateTimes()
    {
        #region DontScrubDateTimes

        var target = new
        {
            Date = new DateTime(2020, 10, 10, 0, 0, 0, DateTimeKind.Utc)
        };

        var settings = new VerifySettings();
        settings.DontScrubDateTimes();

        return Verify(target, settings);

        #endregion
    }

    [Fact]
    Task DontScrubDateTimesFluent()
    {
        #region DontScrubDateTimesFluent

        var target = new
        {
            Date = new DateTime(2020, 10, 10, 0, 0, 0, DateTimeKind.Utc)
        };

        return Verify(target)
            .DontScrubDateTimes();

        #endregion
    }
#endif

    void DontScrubDateTimesGlobal()
    {
        #region DontScrubDateTimesGlobal

        VerifierSettings.DontScrubDateTimes();

        #endregion
    }

    [Fact]
    public Task NewLineNotEscapedInProperty() =>
        Verify(new
        {
            Property = "a\r\nb\\nc"
        });

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

        #region ScrubUserName

        verifySettings.ScrubUserName();

        #endregion

        #region AddScrubber

        verifySettings.AddScrubber(_ => _.Remove(0, 100));

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
            .AddScrubber(_ => _.Replace(currentDirectory, "Bar"));
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
        return Verify(new
        {
            baseDirectory,
            altBaseDirectory
        });
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

        var foo = new
        {
            x = 1
        };
        var target = new TypeTarget
        {
            Type = GetType(),
            Dynamic = foo.GetType()
        };

        await Verify(target);

        #endregion
    }

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

    [Fact]
    public async Task PartialNamedTuple()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => VerifyTuple(() => MethodWithPartialNamedTuple()));
        PrefixUnique.Clear();
        await Verify(exception.Message);
    }

    static (bool, string Member2, string Member3) MethodWithPartialNamedTuple() =>
        (true, "A", "B");

    [Fact]
    public Task NamedTupleWithNull() =>
        VerifyTuple(() => MethodWithNamedTupleWithNull());

    [Fact]
    public Task Claim() =>
        Verify(new Claim("TheType", "TheValue"));

    [Fact]
    public Task EncodingValue() =>
        Verify(Encoding.UTF8);

    [Fact]
    public Task EncodingValueNested() =>
        Verify(new
        {
            encoding = Encoding.UTF8
        });

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
    public Task ShouldRespectEmptyGuid()
    {
        var guid = Guid.Empty;
        var target = new GuidTarget
        {
            Guid = guid,
            GuidNullable = guid,
            GuidString = guid.ToString(),
            OtherGuid = Guid.NewGuid()
        };

        return Verify(target);
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
        var projectDirectory = AttributeReader.GetProjectDirectory();
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
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
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
                    {
                        Guid.NewGuid(), "value"
                    }
                },
                dateTime = new Dictionary<DateTime, string>
                {
                    {
                        DateTime.Now, "value"
                    }
                },
                dateTimeOffset = new Dictionary<DateTimeOffset, string>
                {
                    {
                        DateTimeOffset.Now, "value"
                    }
                },
                type = new Dictionary<Type, string>
                {
                    {
                        typeof(SerializationTests), "value"
                    }
                }
            });

    [Fact]
    public Task ScrubBeforeNewline() =>
        Verify("Line1\nLine2")
            .ScrubLinesContaining("Line1");

    [Fact]
    public Task ScrubEmptyLinesMiddle() =>
        Verify("Line1\n\n\n\n\r\n\r\rLine2")
            .ScrubEmptyLines();

    [Fact]
    public Task ScrubEmptyLinesStartAndEnd() =>
        Verify("\n\n\n\n\r\n\r\rLine\n\n\n\n\r\n\r\r")
            .ScrubEmptyLines();

    [Fact]
    public Task ScrubEmptyLinesStartAndEndAndWhiteSpace() =>
        Verify(" \n\n\n\n\r\n\r\rLine\n\n\n\n\r\n\r\r ")
            .ScrubEmptyLines();

    [Fact]
    public Task ScrubEmptyLinesStart() =>
        Verify("\n\n\n\n\r\n\r\rLine")
            .ScrubEmptyLines();

    [Fact]
    public Task ScrubEmptyLineEnd() =>
        Verify("Line\n\n\n\n\r\n\r\r")
            .ScrubEmptyLines();

    [Fact]
    public Task ScrubEmptyLinesStartProperty() =>
        Verify(
                new
                {
                    Property = "\n\n\n\n\r\n\r\rLine"
                })
            .ScrubEmptyLines();

    [Fact]
    public Task ScrubEmptyLinesStartAndEndProperty() =>
        Verify(
                new
                {
                    Property = "\n\n\n\n\r\n\r\rLine\n\n\n\n\r\n\r\r"
                })
            .ScrubEmptyLines();

    [Fact]
    public Task ScrubEmptyLinesStartAndEndAndWhiteSpaceProperty() =>
        Verify(
                new
                {
                    Property = " \n\n\n\n\r\n\r\rLine\n\n\n\n\r\n\r\r "
                })
            .ScrubEmptyLines();

    [Fact]
    public Task ScrubEmptyLinesEndProperty() =>
        Verify(
                new
                {
                    Property = "Line\n\n\n\n\r\n\r\r"
                })
            .ScrubEmptyLines();

    [Fact]
    public Task ScrubEmptyLinesMiddleProperty() =>
        Verify(
                new
                {
                    Property = "Line1\n\n\n\n\r\n\r\rLine2"
                })
            .ScrubEmptyLines();

    [Fact]
    public Task ScrubPropertyBeforeNewline() =>
        Verify(
                new
                {
                    Property = @"Line1
Line2"
                })
            .ScrubLinesContaining("Line1");

    [Theory]
    [InlineData(10, "NoMatch")]
    [InlineData("Value", "Value")]
    [InlineData("BeforeValue", "BeforeValue")]
    [InlineData("BeforeValueAfter", "BeforeValueAfter")]
    [InlineData("ValueAfter", "ValueAfter")]
    [InlineData("Before\nValue", "BeforeValueWithNewLine")]
    [InlineData("Before\nValue\nAfter", "BeforeValueAfterNewLine")]
    [InlineData("Value\nAfter", "ValueAfterWithNewLine")]
    public Task ScrubDictionaryValue(string value, string name) =>
        Verify(new Dictionary<int, object>
            {
                {
                    1, value
                },
                {
                    2, new StringWriter(new StringBuilder(value))
                },
                {
                    3, new StringWriter(new StringBuilder(value))
                },
            })
            .ScrubLinesContaining("Value")
            .UseTextForParameters(name);

    [Fact]
    public Task NotScrubNonStringDictionaryValues() =>
        Verify(new Dictionary<int, object>
            {
                {
                    1, 1
                },
                {
                    2, true
                },
            })
            .ScrubLinesContaining("value");

    [Fact]
    public Task ShouldIgnoreEmptyList()
    {
        var target = new CollectionTarget
        {
            DictionaryProperty = new(),
            IReadOnlyDictionary = new ReadOnlyDictionary<int, string>(new Dictionary<int, string>()),
            EnumerableAsList = new List<string>(),
            EnumerableStaticEmpty = Enumerable.Empty<string>(),
            ReadOnlyList = new ReadOnlyList(),
            ListProperty = new(),
            ReadOnlyCollection = new ReadOnlyCollection<string>(new string[]{}),
            Array = Array.Empty<string>()
        };
        return Verify(target);
    }

    class ReadOnlyList: IReadOnlyList<string>
    {
        List<string> inner = new();
        public IEnumerator<string> GetEnumerator() =>
            inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            inner.GetEnumerator();

        public int Count => inner.Count;

        public string this[int index] => inner[index];
    }

    class CollectionTarget
    {
        public IReadOnlyList<string> ReadOnlyList;
        public Dictionary<int, string> DictionaryProperty;
        public List<string> ListProperty;
        public IEnumerable<string> EnumerableAsList;
        public IEnumerable<string> EnumerableStaticEmpty;
        public string[] Array;
        public IReadOnlyCollection<string> ReadOnlyCollection;
        public IReadOnlyDictionary<int, string> IReadOnlyDictionary;
    }
#pragma warning disable 612

    void ExceptionMessagePropGlobal()
    {
        #region IgnoreMembersThatThrowExpressionGlobal

        VerifierSettings.IgnoreMembersThatThrow<Exception>(_ => _.Message == "Ignore");

        #endregion
    }

    #region IgnoreMembersThatThrowExpression

    [Fact]
    public Task ExceptionMessageProp()
    {
        var target = new WithExceptionIgnoreMessage();

        var settings = new VerifySettings();
        settings.IgnoreMembersThatThrow<Exception>(_ => _.Message == "Ignore");
        return Verify(target, settings);
    }

    [Fact]
    public Task ExceptionMessagePropFluent()
    {
        var target = new WithExceptionIgnoreMessage();

        return Verify(target)
            .IgnoreMembersThatThrow<Exception>(_ => _.Message == "Ignore");
    }

    #endregion

    [Fact]
    public Task ExpressionString()
    {
        var expression = BuildExpression();
        return Verify(expression)
            .UniqueForRuntime();
    }

    [Fact]
    public Task ExpressionStringNested()
    {
        var expression = BuildExpression();
        return Verify(new
            {
                expression
            })
            .UniqueForRuntime();
    }

    static Expression<Func<Exception, object>> BuildExpression()
    {
        var parameter = Expression.Parameter(typeof(Exception), "source");
        var property = Expression.Property(parameter, "Message");
        var convert = Expression.Convert(property, typeof(object));
        return Expression.Lambda<Func<Exception, object>>(convert, parameter);
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
    public Task SelfReferencingWithArray()
    {
        var target = new SelfReferencingWithArrayTarget
        {
            Property = "Value"
        };
        target.List = BuildEnumerable(target);
        return Verify(target);
    }

    IEnumerable<SelfReferencingWithArrayTarget> BuildEnumerable(SelfReferencingWithArrayTarget target)
    {
        yield return target;
    }

    class SelfReferencingWithArrayTarget
    {
        public string Property { get; set; }
        public IEnumerable<SelfReferencingWithArrayTarget> List { get; set; }
    }

    [Fact]
    public Task TestEnumerableWithExistingConverter()
    {
        var target = new EnumerableWithExistingConverterTarget
        {
            "Value"
        };
        return Verify(target)
            .AddExtraSettings(_ => _.Converters.Add(new EnumerableWithExistingConverter()));
    }

    class EnumerableWithExistingConverterTarget : List<string>
    {
    }

    class EnumerableWithExistingConverter :
        WriteOnlyJsonConverter<EnumerableWithExistingConverterTarget>
    {
        public override void Write(VerifyJsonWriter writer, EnumerableWithExistingConverterTarget target) =>
            writer.Serialize(new
            {
                value = "Content"
            });
    }

    [Fact]
    public Task TestConverterWithBadNewline()
    {
        var target = new ConverterWithBadNewlineTarget();
        return Verify(target)
            .AddExtraSettings(_ => _.Converters.Add(new ConverterWithBadNewline()));
    }

    [Fact]
    public Task TestConverterWithBadNewlineScrubEmptyLines()
    {
        var target = new ConverterWithBadNewlineTarget();
        return Verify(target)
            .AddExtraSettings(_ => _.Converters.Add(new ConverterWithBadNewline()))
            .ScrubEmptyLines();
    }

    class ConverterWithBadNewlineTarget
    {
    }

    class ConverterWithBadNewline :
        WriteOnlyJsonConverter<ConverterWithBadNewlineTarget>
    {
        public override void Write(VerifyJsonWriter writer, ConverterWithBadNewlineTarget target)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Property1");
            writer.WriteRawValueIfNoStrict("\n\r\r\nA\n\r\r\nB\n\r\r\n");
            writer.WritePropertyName("Property2");
            writer.WriteValue("\n\r\r\nA\n\r\r\nB\n\r\r\n");
            writer.WriteEndObject();
        }
    }

    [Fact]
    public Task TestEnumerableWithExistingItemConverter()
    {
        var target = new EnumerableWithExistingItemConverterTarget
        {
            "Value"
        };
        return Verify(target)
            .AddExtraSettings(_ => _.Converters.Add(new EnumerableWithExistingItemConverter()));
    }

    class EnumerableWithExistingItemConverterTarget :
        List<string>
    {
    }

    class EnumerableWithExistingItemConverter :
        WriteOnlyJsonConverter<string>
    {
        public override void Write(VerifyJsonWriter writer, string target) =>
            writer.Serialize(10);
    }

    [Fact]
    public Task TargetInvocationException()
    {
        var member = GetType().GetMethod("MethodThatThrows")!;
        return Throws(
                () =>
                {
                    member.Invoke(null, Array.Empty<object>());
                })
            .UniqueForTargetFrameworkAndVersion()
            .ScrubLinesContaining("(Object ");
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

        return Verify(
                new
                {
                    exception
                })
            .UniqueForTargetFrameworkAndVersion();
    }

    public static void MethodThatThrows() =>
        throw new("the message");

    void AddIgnoreInstanceGlobal()
    {
        #region AddIgnoreInstanceGlobal

        VerifierSettings.IgnoreInstance<Instance>(_ => _.Property == "Ignore");

        #endregion

        #region AddScrubInstanceGlobal

        VerifierSettings.ScrubInstance<Instance>(_ => _.Property == "Ignore");

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
        settings.IgnoreInstance<Instance>(_ => _.Property == "Ignore");
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
            .IgnoreInstance<Instance>(_ => _.Property == "Ignore");
    }

    #endregion

    #region AddScrubInstance

    [Fact]
    public Task AddScrubInstance()
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
        settings.ScrubInstance<Instance>(_ => _.Property == "Ignore");
        return Verify(target, settings);
    }

    [Fact]
    public Task AddScrubInstanceFluent()
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
            .ScrubInstance<Instance>(_ => _.Property == "Ignore");
    }

    #endregion

    [Fact]
    public Task AddIgnoreInstanceInList()
    {
        var target = new[]
        {
            new Instance
            {
                Property = "Ignore"
            },
            new Instance
            {
                Property = "Include"
            }
        };
        return Verify(target)
            .IgnoreInstance<Instance>(_ => _.Property == "Ignore");
    }

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

        #region AddScrubTypeGlobal

        VerifierSettings.ScrubMembersWithType<ToIgnore>();

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
        settings.IgnoreMembersWithType<ToIgnore>();
        settings.IgnoreMembersWithType<ToIgnoreByType>();
        settings.IgnoreMembersWithType<InterfaceToIgnore>();
        settings.IgnoreMembersWithType<BaseToIgnore>();
        settings.IgnoreMembersWithType(typeof(BaseToIgnoreGeneric<>));
        settings.IgnoreMembersWithType<ToIgnoreStruct>();
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
            .IgnoreMembersWithType<ToIgnore>()
            .IgnoreMembersWithType<ToIgnoreByType>()
            .IgnoreMembersWithType<InterfaceToIgnore>()
            .IgnoreMembersWithType<BaseToIgnore>()
            .IgnoreMembersWithType(typeof(BaseToIgnoreGeneric<>))
            .IgnoreMembersWithType<ToIgnoreStruct>();
    }

    #endregion


    #region AddScrubType

    [Fact]
    public Task ScrubType()
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
        settings.ScrubMembersWithType<ToIgnore>();
        settings.ScrubMembersWithType<ToIgnoreByType>();
        settings.ScrubMembersWithType<InterfaceToIgnore>();
        settings.ScrubMembersWithType<BaseToIgnore>();
        settings.ScrubMembersWithType(typeof(BaseToIgnoreGeneric<>));
        settings.ScrubMembersWithType<ToIgnoreStruct>();
        return Verify(target, settings);
    }

    [Fact]
    public Task ScrubTypeFluent()
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
            .ScrubMembersWithType<ToIgnore>()
            .ScrubMembersWithType<ToIgnoreByType>()
            .ScrubMembersWithType<InterfaceToIgnore>()
            .ScrubMembersWithType<BaseToIgnore>()
            .ScrubMembersWithType(typeof(BaseToIgnoreGeneric<>))
            .ScrubMembersWithType<ToIgnoreStruct>();
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
    public Task ScrubMembersNullable()
    {
        ToIgnoreStruct? toIgnoreStruct = new ToIgnoreStruct("Value");

        return Verify(toIgnoreStruct)
            .ScrubMembers<ToIgnoreStruct>(_ => _.Property);
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

    [Fact]
    public Task ScrubMembersNullableNested()
    {
        var target = new IgnoreMembersNullableNestedTarget
        {
            ToIgnoreStruct = new ToIgnoreStruct("Value")
        };

        return Verify(target)
            .ScrubMembers<IgnoreMembersNullableNestedTarget>(_ => _.ToIgnoreStruct);
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

        VerifierSettings.IgnoreMembers<IgnoreExplicitTarget>(
            _ => _.Property,
            _ => _.PropertyWithPropertyName,
            _ => _.Field,
            _ => _.GetOnlyProperty,
            _ => _.PropertyThatThrows);

        #endregion

        #region ScrubMemberByExpressionGlobal

        VerifierSettings.ScrubMembers<IgnoreExplicitTarget>(
            _ => _.Property,
            _ => _.PropertyWithPropertyName,
            _ => _.Field,
            _ => _.GetOnlyProperty,
            _ => _.PropertyThatThrows);

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
        settings.IgnoreMembers<IgnoreExplicitTarget>(
            _ => _.Property,
            _ => _.PropertyWithPropertyName,
            _ => _.Field,
            _ => _.GetOnlyProperty,
            _ => _.PropertyThatThrows);
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
            .IgnoreMembers<IgnoreExplicitTarget>(
                _ => _.Property,
                _ => _.Field,
                _ => _.GetOnlyProperty,
                _ => _.PropertyThatThrows);
    }

    #endregion

    #region ScrubMemberByExpression

    [Fact]
    public Task ScrubMemberByExpression()
    {
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value",
            PropertyWithPropertyName = "Value"
        };
        var settings = new VerifySettings();
        settings.ScrubMembers<IgnoreExplicitTarget>(
            _ => _.Property,
            _ => _.PropertyWithPropertyName,
            _ => _.Field,
            _ => _.GetOnlyProperty,
            _ => _.PropertyThatThrows);
        return Verify(target, settings);
    }

    [Fact]
    public Task ScrubMemberByExpressionFluent()
    {
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        return Verify(target)
            .ScrubMembers<IgnoreExplicitTarget>(
                _ => _.Property,
                _ => _.Field,
                _ => _.GetOnlyProperty,
                _ => _.PropertyThatThrows);
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
            expression: _ => _.Field,
            converter: member => $"{member}_Suffix");

        // using target and member
        VerifierSettings.MemberConverter<MemberTarget, string>(
            expression: _ => _.Property,
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

        #region ScrubMemberByNameGlobal

        // For all types
        VerifierSettings.ScrubMember("PropertyByName");

        // For a specific type
        VerifierSettings.ScrubMember(typeof(IgnoreExplicitTarget), "Property");

        // For a specific type generic
        VerifierSettings.ScrubMember<IgnoreExplicitTarget>("Field");

        // For a specific type with expression
        VerifierSettings.ScrubMember<IgnoreExplicitTarget>(_ => _.PropertyThatThrows);

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

        // For all types
        settings.IgnoreMember("PropertyByName");

        // For a specific type
        settings.IgnoreMember(typeof(IgnoreExplicitTarget), "Property");

        // For a specific type generic
        settings.IgnoreMember<IgnoreExplicitTarget>("Field");

        // For a specific type with expression
        settings.IgnoreMember<IgnoreExplicitTarget>(_ => _.PropertyThatThrows);

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
            // For all types
            .IgnoreMember("PropertyByName")

            // For a specific type
            .IgnoreMember(typeof(IgnoreExplicitTarget), "Property")

            // For a specific type generic
            .IgnoreMember<IgnoreExplicitTarget>("Field")

            // For a specific type with expression
            .IgnoreMember<IgnoreExplicitTarget>(_ => _.PropertyThatThrows);
    }

    #endregion

    #region ScrubMemberByName

    [Fact]
    public Task ScrubMemberByName()
    {
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value",
            PropertyByName = "Value"
        };
        var settings = new VerifySettings();

        // For all types
        settings.ScrubMember("PropertyByName");

        // For a specific type
        settings.ScrubMember(typeof(IgnoreExplicitTarget), "Property");

        // For a specific type generic
        settings.ScrubMember<IgnoreExplicitTarget>("Field");

        // For a specific type with expression
        settings.ScrubMember<IgnoreExplicitTarget>(_ => _.PropertyThatThrows);

        return Verify(target, settings);
    }

    [Fact]
    public Task ScrubMemberByNameFluent()
    {
        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value",
            PropertyByName = "Value"
        };
        return Verify(target)
            // For all types
            .ScrubMember("PropertyByName")

            // For a specific type
            .ScrubMember(typeof(IgnoreExplicitTarget), "Property")

            // For a specific type generic
            .ScrubMember<IgnoreExplicitTarget>("Field")

            // For a specific type with expression
            .ScrubMember<IgnoreExplicitTarget>(_ => _.PropertyThatThrows);
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
            .IgnoreStackTrace();

    [Fact]
    public Task IgnoreDictionaryKeyByName()
    {
        var target = new Dictionary<string, object>
        {
            {
                "Include", new Dictionary<string, string>
                {
                    {
                        "Ignore", "Value1"
                    },
                    {
                        "Key1", "Value2"
                    }
                }
            },
            {
                "Ignore", "Value3"
            },
            {
                "Key2", "Value4"
            }
        };
        return Verify(target)
            .IgnoreMember("Ignore");
    }

    [Fact]
    public Task ScrubDictionaryKeyByName()
    {
        var target = new Dictionary<string, object>
        {
            {
                "Include", new Dictionary<string, string>
                {
                    {
                        "Scrub", "Value1"
                    },
                    {
                        "Key1", "Value2"
                    }
                }
            },
            {
                "Scrub", "Value3"
            },
            {
                "Key2", "Value4"
            }
        };
        return Verify(target)
            .ScrubMember("Scrub");
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
        public Guid CustomExceptionGuid => throw new CustomException();

        public int[] CustomExceptionArray => throw new CustomException();

        public List<string> CustomExceptionList => throw new CustomException();
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
        settings.IgnoreMembersThatThrow<Exception>(_ => _.Message == "Ignore");
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
        Verify(new ConverterTarget
            {
                Name = "The name"
            })
            .AddExtraSettings(_ => _.Converters.Add(new Converter()));

    [Fact]
    public Task WithConverterAndNewline() =>
        Verify(new ConverterTarget
            {
                Name = "A\rB\nC\r\nD"
            })
            .AddExtraSettings(_ => _.Converters.Add(new Converter()));

    [Fact]
    public Task WithConverterAndIgnore() =>
        Verify(new ConverterTarget
            {
                Name = "The name"
            })
            .IgnoreMember("Name")
            .AddExtraSettings(_ => _.Converters.Add(new Converter()));

    class Converter :
        WriteOnlyJsonConverter<ConverterTarget>
    {
        public override void Write(VerifyJsonWriter writer, ConverterTarget target)
        {
            writer.WriteStartObject();
            writer.WriteMember(target, target.Name, "Name");
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
        return Verify(new StaticConverterTarget
            {
                Name = "The name"
            })
            .AddExtraSettings(_ => _.Converters.Add(new StaticConverter()));
    }

    class StaticConverter :
        WriteOnlyJsonConverter<StaticConverterTarget>
    {
        public override void Write(VerifyJsonWriter writer, StaticConverterTarget target)
        {
            writer.WriteStartObject();
            writer.WriteMember(target, target.Name, "Name");
            writer.WritePropertyName("Custom");
            writer.WriteValue("CustomValue");
            writer.WriteEnd();
        }
    }

    class StaticConverterTarget
    {
        public string Name { get; set; } = null!;
    }

    Parent ListReferenceData()
    {
        var parent = new Parent();

        var children = new List<Child>
        {
            new()
            {
                Parent = parent
            },
            new()
            {
                Parent = parent
            }
        };

        parent.Children = children;
        return parent;
    }

    [Fact]
    public Task ListIgnoreLoopReference() =>
        Verify(ListReferenceData());

    public class Parent
    {
        public List<Child> Children { get; set; } = new();
    }

    public class Child
    {
        public Parent Parent { get; set; }
    }
}