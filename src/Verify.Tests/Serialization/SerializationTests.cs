using System.Collections.ObjectModel;
using System.Security.Claims;

// ReSharper disable NotAccessedField.Local

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


    [ModuleInitializer]
    public static void OrderEnumerableByGlobalInit() =>
        VerifierSettings.OrderEnumerableBy<EnumerableOrderDescendingGlobalItem>(_ => _.value);

    [Fact]
    public Task OrderEnumerableByGlobal() =>
        Verify(
            new List<EnumerableOrderGlobalItem>
            {
                new("a"),
                new("c"),
                new("b")
            });

    public record EnumerableOrderGlobalItem(string value);

    [ModuleInitializer]
    public static void OrderEnumerableByDescendingGlobalInit() =>
        VerifierSettings.OrderEnumerableByDescending<EnumerableOrderDescendingGlobalItem>(_ => _.value);

    [Fact]
    public Task OrderEnumerableByDescendingGlobal() =>
        Verify(
            new List<EnumerableOrderDescendingGlobalItem>
            {
                new("a"),
                new("c"),
                new("b")
            });

    public record EnumerableOrderDescendingGlobalItem(string value);

    [Fact]
    public Task EnumerableOrder()
    {
        var settings = new VerifySettings();
        settings.OrderEnumerableBy<string>(_ => _);
        return Verify(
            new List<string>
            {
                "a",
                "c",
                "b"
            },
            settings);
    }

    [Fact]
    public Task EnumerableOrderWithNull()
    {
        var settings = new VerifySettings();
        settings.OrderEnumerableBy<string>(_ => null);
        return Verify(
            new List<string>
            {
                "a",
                "c",
                "b"
            },
            settings);
    }

    [Fact]
    public Task OrderEnumerableByDescending()
    {
        var settings = new VerifySettings();
        settings.OrderEnumerableBy<string>(_ => _);
        return Verify(
            new List<string>
            {
                "a",
                "c",
                "b"
            },
            settings);
    }

    [Fact]
    public Task EnumerableOrderFluent() =>
        Verify(
                new List<string>
                {
                    "a",
                    "c",
                    "b"
                })
            .OrderEnumerableBy<string>(_ => _);

    [Fact]
    public Task OrderEnumerableByDescendingFluent() =>
        Verify(
                new List<string>
                {
                    "a",
                    "c",
                    "b"
                })
            .OrderEnumerableByDescending<string>(_ => _);


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

    public class NonComparableKey(string member)
    {
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
            {
                "ignored", "1234"
            },
            {
                "Entry_2", "5678"
            },
            {
                "Entry_1", "1234"
            }
        };

        return Verify(dictionary)
            .IgnoreMember("ignored");
    }

#if NET6_0_OR_GREATER
    [Fact]
    public Task DateKeys()
    {
        var dictionary = new Dictionary<Date, string>
        {
            {
                new Date(10, 1, 2), "1234"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task StringDateKeys()
    {
        var dictionary = new Dictionary<string, string>
        {
            {
                new Date(10, 1, 2).ToString(), "1234"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task DateTimeKeys()
    {
        var dictionary = new Dictionary<DateTime, string>
        {
            {
                DateTime.Now, "1234"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task StringDateTimeKeys()
    {
        var dictionary = new Dictionary<string, string>
        {
            {
                DateTime.Now.ToString("F"), "1234"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task DateTimeOffsetKeys()
    {
        var dictionary = new Dictionary<DateTimeOffset, string>
        {
            {
                DateTimeOffset.Now, "1234"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task StringDateTimeOffsetKeys()
    {
        var dictionary = new Dictionary<string, string>
        {
            {
                DateTimeOffset.Now.ToString("F"), "1234"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task GuidKeys()
    {
        var dictionary = new Dictionary<Guid, string>
        {
            {
                Guid.NewGuid(), "1234"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task StringGuidKeys()
    {
        var dictionary = new Dictionary<string, string>
        {
            {
                Guid
                    .NewGuid()
                    .ToString(),
                "1234"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task TimeKeys()
    {
        var dictionary = new Dictionary<Time, string>
        {
            {
                new Time(10, 1), "1234"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task StringTimeKeys()
    {
        var dictionary = new Dictionary<string, string>
        {
            {
                new Time(10, 1).ToString("HH:mm tt"), "1234"
            }
        };

        return Verify(dictionary);
    }

    [Fact]
    public Task BoxedKeys()
    {
        var dictionary = new Dictionary<object, string>
        {
            {
                Guid.NewGuid(), "1234"
            },
            {
                DateTime.Now, "1234"
            },
            {
                DateTimeOffset.Now, "1234"
            },
            {
                new Date(10, 1, 2), "1234"
            },
            {
                new Time(10, 1), "1234"
            },
            {
                "ignored", "5678"
            }
        };

        return Verify(dictionary)
            .IgnoreMember("ignored");
    }
#endif

    [Fact]
    public Task DatetimeOffsetScrubbingDisabled() =>
        Verify(
                new
                {
                    noTime = new DateTimeOffset(new DateTime(2000, 1, 1), TimeSpan.FromHours(1.5)),
                    withTime = new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 1), TimeSpan.FromHours(1)),
                    withTimeZeroSeconds = new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 0), TimeSpan.FromHours(1)),
                    withTimeMilliSeconds = new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 1, 999), TimeSpan.FromHours(1))
                })
            .DontScrubDateTimes();

    [Fact]
    public Task OnlyScrubInlineProperties() =>
        Verify(
                new
                {
                    property = """
                               line1
                               line2
                               line3
                               """
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
                    time = new Time(10, 1, 1),
                    value = new Date(2000, 1, 1)
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
                _ => _.Error = (currentObject, originalObject, location, exception, handled) =>
                    Console.WriteLine(location.Member));
        return Verify("Value", settings);
    }

    #endregion

    #region AddExtraSettingsFluent

    [Fact]
    public Task AddExtraSettingsFluent() =>
        Verify("Value")
            .AddExtraSettings(
                _ => _.Error = (currentObject, originalObject, location, exception, handled) =>
                    Console.WriteLine(location.Member));

    #endregion


    // ReSharper disable once UnusedMember.Local
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
            Item = [new TypeNameHandlingAutoInArrayItemChild()]
        };

        return Verify(target)
            .AddExtraSettings(_ => _.TypeNameHandling = typeHandling)
            .UseParameters(typeHandling);
    }

    class TypeNameHandlingAutoInArrayTarget
    {
        public TypeNameHandlingAutoInArrayItem[] Item { get; set; } = [];
    }

    abstract class TypeNameHandlingAutoInArrayItem;

    class TypeNameHandlingAutoInArrayItemChild :
        TypeNameHandlingAutoInArrayItem;

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
        var target = new Guid("b6993f86-c1b9-44db-bfc5-33ed9e5c048e");

        #region DontScrubGuids

        var settings = new VerifySettings();
        settings.DontScrubGuids();
        await Verify(target, settings);

        #endregion
    }

    [Fact]
    public async Task GuidScrubbingDisabledFluent()
    {
        var target = new Guid("b6993f86-c1b9-44db-bfc5-33ed9e5c048e");

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
                    value = new Guid("b6993f86-c1b9-44db-bfc5-33ed9e5c048e")
                })
            .DontScrubGuids()
            .ScrubLinesWithReplace(_ => "replaced");

    [Fact]
    public Task GuidScrubbingDisabledNested() =>
        Verify(
                new
                {
                    value = new Guid("b6993f86-c1b9-44db-bfc5-33ed9e5c048e")
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
    public Task StringDictionary() =>
        Verify(
            new
            {
                item2 = new StringDictionary
                {
                    {
                        "key", null
                    }
                },
                item4 = new StringDictionary
                {
                    {
                        "key", "value"
                    }
                },
                item8 = new StringDictionary
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
            Children = ["Sam", "Mary"],
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
            yield return
            [
                boolean,
                nullableBoolean,
                includeDefault
            ];
        }
    }

    [Fact]
    public Task BoolDefault()
    {
        var target = new BoolModel();
        return Verify(target);
    }

    [Fact]
    public Task BoolFalse()
    {
        var target = new BoolModel
        {
            BoolMember = false,
            NullableBoolMember = false
        };
        return Verify(target);
    }

    [Fact]
    public Task BoolTrue()
    {
        var target = new BoolModel
        {
            BoolMember = true,
            NullableBoolMember = true
        };
        return Verify(target);
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
            Children = ["Sam", "Mary"],
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
    public Task Time() =>
        Verify(new Time(10, 10));

    [Fact]
    public Task TimeNested() =>
        Verify(new
        {
            value = new Time(10, 10)
        });

    [Fact]
    public Task TimeNestedWithNoScrubbing() =>
        Verify(new
            {
                value = new Time(10, 10)
            })
            .DontScrubDateTimes();

    [Fact]
    public Task TimeWithNoScrubbing() =>
        Verify(new Time(10, 10))
            .DontScrubDateTimes();

    [Fact]
    public Task IgnoreDatetimeDefaults()
    {
        var target = new DateTimeTarget();

        return Verify(target);
    }

    [Fact]
    public async Task DateWithNoScrubbing()
    {
        var target = new
        {
            Date = new Date(2020, 10, 10)
        };

        await Verify(target)
            .DontScrubDateTimes();
    }

    [Fact]
    public async Task ReUseDatetime()
    {
        #region Date

        var dateTime = DateTime.Now;
        var dateTimeOffset = DateTimeOffset.Now;
        var target = new DateTimeTarget
        {
            DateTime = dateTime,
            Date = new(dateTime.Year, dateTime.Month, dateTime.Day),
            DateNullable = new(dateTime.Year, dateTime.Month, dateTime.Day),
            DateString = new Date(dateTime.Year, dateTime.Month, dateTime.Day).ToString(),
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
            Date = Date.MinValue,
            DateNullable = Date.MinValue,
            DateString = Date.MinValue.ToString(),
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
            Date = Date.MaxValue,
            DateNullable = Date.MaxValue,
            DateString = Date.MaxValue.ToString(),
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
        var dateTime1 = new DateTimeOffset(new DateTime(2000, 10, 10, 1, 0, 0), TimeSpan.FromHours(1));
        var dateTime2 = new DateTimeOffset(new DateTime(2000, 10, 10, 2, 0, 0), TimeSpan.FromHours(2));
        var target = new
        {
            dateTime1,
            dateTime2
        };

        return Verify(target);
    }

    #region NamedDatesAndTimesGlobal

    [ModuleInitializer]
    public static void NamedDatesAndTimesGlobal()
    {
        VerifierSettings.AddNamedDateTime(new(2030, 1, 1), "namedDateTime");
        VerifierSettings.AddNamedTime(new(1, 1), "namedTime");
        VerifierSettings.AddNamedDate(new(2030, 1, 1), "namedDate");
        VerifierSettings.AddNamedDateTimeOffset(new(new(2030, 1, 1)), "namedDateTimeOffset");
    }

    #endregion

    [Fact]
    public async Task ScrubDatetime()
    {
        var dateTime = DateTime.Now;
        var dateTimeOffset = DateTimeOffset.Now;
        var target = new DateTimeTarget
        {
            DateTime = dateTime,
            NamedDateTime = new(2030, 1, 1),
            InstanceNamedDateTime = new(2030, 1, 2),
            NamedDateTimeOffset = new DateTime(2030, 1, 1),
            InstanceNamedDateTimeOffset = new DateTime(2030, 1, 2),
            DateTimeNullable = dateTime.AddDays(1),
            DateTimeString = dateTime
                .AddDays(2)
                .ToString("F"),
            DateTimeOffset = dateTimeOffset,
            DateTimeOffsetNullable = dateTimeOffset.AddDays(1),
            DateTimeOffsetString = dateTimeOffset
                .AddDays(2)
                .ToString("F"),
            Time = new(10, 11),
            TimeString = "10:11 AM",
            NamedTime = new(1, 1),
            InstanceNamedTime = new(1, 2),
            Date = new(2020, 10, 10),
            NamedDate = new(2020, 10, 10),
            InstanceNamedDate = new(2020, 10, 11),
            DateNullable = new(2020, 10, 12),
            DateString = new Date(2020, 10, 12).ToString()
        };

        #region NamedDatesAndTimesFluent

        await Verify(target)
            .AddNamedDate(new(2020, 10, 11), "instanceNamedDate")
            .AddNamedTime(new(1, 2), "instanceTime")
            .AddNamedDateTime(new(2030, 1, 2), "instanceNamedDateTime")
            .AddNamedDateTimeOffset(new DateTime(2030, 1, 2), "instanceNamedTimeOffset");

        #endregion
    }

    [Fact]
    public async Task ScrubDatetimeInstance()
    {
        var dateTime = DateTime.Now;
        var dateTimeOffset = DateTimeOffset.Now;
        var target = new DateTimeTarget
        {
            DateTime = dateTime,
            NamedDateTime = new(2030, 1, 1),
            InstanceNamedDateTime = new(2030, 1, 2),
            NamedDateTimeOffset = new DateTime(2030, 1, 1),
            InstanceNamedDateTimeOffset = new DateTime(2030, 1, 2),
            DateTimeNullable = dateTime.AddDays(1),
            DateTimeString = dateTime
                .AddDays(2)
                .ToString("F"),
            DateTimeOffset = dateTimeOffset,
            DateTimeOffsetNullable = dateTimeOffset.AddDays(1),
            DateTimeOffsetString = dateTimeOffset
                .AddDays(2)
                .ToString("F"),
            Time = new(10, 11),
            TimeString = "10:11 AM",
            NamedTime = new(1, 1),
            InstanceNamedTime = new(1, 2),
            Date = new(2020, 10, 10),
            NamedDate = new(2020, 10, 10),
            InstanceNamedDate = new(2020, 10, 11),
            DateNullable = new(2020, 10, 12),
            DateString = new Date(2020, 10, 12).ToString()
        };

        #region NamedDatesAndTimesInstance

        var settings = new VerifySettings();
        settings.AddNamedDate(new(2020, 10, 11), "instanceNamedDate");
        settings.AddNamedTime(new(1, 2), "instanceTime");
        settings.AddNamedDateTime(new(2030, 1, 2), "instanceNamedDateTime");
        settings.AddNamedDateTimeOffset(new DateTime(2030, 1, 2), "instanceNamedTimeOffset");
        await Verify(target, settings);

        #endregion
    }

    class DateTimeTarget
    {
        public DateTime DateTime;
        public DateTime NamedDateTime;
        public DateTime InstanceNamedDateTime;
        public DateTime? DateTimeNullable;
        public Date Date;
        public Date NamedDate;
        public Date InstanceNamedDate;
        public Time Time;
        public string TimeString;
        public Time NamedTime;
        public Time InstanceNamedTime;
        public Date? DateNullable;
        public DateTimeOffset NamedDateTimeOffset;
        public DateTimeOffset InstanceNamedDateTimeOffset;
        public DateTimeOffset DateTimeOffset;
        public DateTimeOffset? DateTimeOffsetNullable;
        public string DateTimeString;
        public string DateTimeOffsetString;
        public string DateString;
    }

#endif

    [Fact]
    public Task NotScrubInlineGuidsByDefault()
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
    public Task ScrubInlineGuidsInString()
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
            .AddExtraSettings(_ => _.Converters.Add(new WriteRawInConverter()))
            .ScrubEmptyLines();
    }

    class WriteRawInConverterTarget;

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
    public Task ScrubInlineGuidsWrappedInSymbols()
    {
        var id = Guid.NewGuid();
        return Verify($"({id})")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ScrubInlineGuidsStartingWithSymbol()
    {
        var id = Guid.NewGuid();
        return Verify($"/{id}")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ScrubInlineGuidsEndingWithSymbol()
    {
        var id = Guid.NewGuid();
        return Verify($"{id}/")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ScrubInlineGuidsWrappedInNewLine()
    {
        var id = Guid.NewGuid();
        return Verify($"""

                       {id}

                       """)
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ScrubInlineGuidsWrappedWithSymbol()
    {
        var id = Guid.NewGuid();
        return Verify($"/{id}/")
            .ScrubInlineGuids();
    }

    [Fact]
    public Task ScrubInlineGuidsWrappedInDash() =>
        Verify("-087ea433-d83b-40b6-9e37-465211d9508-")
            .ScrubInlineGuids();

    [Fact]
    public Task ScrubInlineGuidsWrappedInLetters() =>
        Verify("before087ea433-d83b-40b6-9e37-465211d9508cafter")
            .ScrubInlineGuids();

    [Fact]
    public Task ScrubInlineGuidsStartingInLetters() =>
        Verify("before087ea433-d83b-40b6-9e37-465211d9508")
            .ScrubInlineGuids();

    [Fact]
    public Task ScrubInlineGuidsStartingInNewline1() =>
        Verify("\n087ea433-d83b-40b6-9e37-465211d95081")
            .ScrubInlineGuids();

    [Fact]
    public Task ScrubInlineGuidsStartingInNewline2() =>
        Verify("\r087ea433-d83b-40b6-9e37-465211d95081")
            .ScrubInlineGuids();

    [Fact]
    public Task ScrubInlineGuidsEndingInNewline1() =>
        Verify("087ea433-d83b-40b6-9e37-465211d95081\n")
            .ScrubInlineGuids();

    [Fact]
    public Task ScrubInlineGuidsEndingInNewline2() =>
        Verify("087ea433-d83b-40b6-9e37-465211d95081\r")
            .ScrubInlineGuids();

    [Fact]
    public Task ScrubInlineGuidsEndingLetters() =>
        Verify("087ea433-d83b-40b6-9e37-465211d95081after")
            .ScrubInlineGuids();

    [Fact]
    public Task ScrubInlineGuidsWrappedInNumber() =>
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

    [Fact]
    public Task ScrubInlineDateTimesWrappedInSymbols()
    {
        var date = DateTime.Now;
        return Verify($"({date:f})")
            .ScrubInlineDateTimes("f");
    }

#if NET5_0_OR_GREATER

    [Fact]
    public async Task ScrubInlineDateTimesInValidFormat()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verify("2020 10 01")
            .ScrubInlineDates("dddd d MMM yyyy 'at' h:mm tt"));
        Assert.Equal("Format 'dddd d MMM yyyy 'at' h:mm tt' is not valid for DateOnly.ToString(format, culture).",exception.Message);
    }

#endif

    [Fact]
    public Task ScrubInlineDateTimesStartingWithSymbol()
    {
        var date = DateTime.Now;
        return Verify($"/{date:f}")
            .ScrubInlineDateTimes("f");
    }

    [Fact]
    public Task ScrubInlineDateTimesEndingWithSymbol()
    {
        var date = DateTime.Now;
        return Verify($"{date:f}/")
            .ScrubInlineDateTimes("f");
    }

    [Fact]
    public Task ScrubInlineDateTimesWrappedInNewLine()
    {
        var date = DateTime.Now;
        return Verify($"""

                       {date:f}

                       """)
            .ScrubInlineDateTimes("f");
    }

    [Fact]
    public Task ScrubInlineDateTimesWrappedWithSymbol()
    {
        var date = DateTime.Now;
        return Verify($"/{date:f}/")
            .ScrubInlineDateTimes("f");
    }

    [Fact]
    public Task ScrubInlineDateTimesWrappedInDash() =>
        Verify("-2020-12-10-")
            .ScrubInlineDateTimes("yyyy-MM-dd");

    [Fact]
    public Task ScrubInlineDateTimesWrappedInLetters() =>
        Verify("before2020-12-10after")
            .ScrubInlineDateTimes("yyyy-MM-dd");

    [Fact]
    public Task ScrubInlineDateTimesStartingInLetters() =>
        Verify("before2020-12-10")
            .ScrubInlineDateTimes("yyyy-MM-dd");

    [Fact]
    public Task ScrubInlineDateTimesStartingInNewline1() =>
        Verify("\n2020-12-10")
            .ScrubInlineDateTimes("yyyy-MM-dd");

    [Fact]
    public Task ScrubInlineDateTimesStartingInNewline2() =>
        Verify("\r2020-12-10")
            .ScrubInlineDateTimes("yyyy-MM-dd");

    [Fact]
    public Task ScrubInlineDateTimesShort() =>
        Verify(" 2020-12-10 12:10:10  2020-12-10 1:1:1 ")
            .ScrubInlineDateTimes("yyyy-MM-dd h:m:s");

    [Fact]
    public Task ScrubInlineDateTimesLong() =>
        Verify(" 2020-12-10 12:10:10  2020-12-10 01:01:01 ")
            .ScrubInlineDateTimes("yyyy-MM-dd hh:mm:ss");

    [Fact]
    public Task ScrubInlineDateTimesEndingInNewline1() =>
        Verify("2020-12-10\n")
            .ScrubInlineDateTimes("yyyy-MM-dd");

    [Fact]
    public Task ScrubInlineDateTimesEndingInNewline2() =>
        Verify("2020-12-10\r")
            .ScrubInlineDateTimes("yyyy-MM-dd");

    [Fact]
    public Task ScrubInlineDateTimesEndingLetters() =>
        Verify("2020-12-10after")
            .ScrubInlineDateTimes("yyyy-MM-dd");

    [Fact]
    public Task ScrubInlineDateTimesWrappedInNumber() =>
        Verify("12020-12-101")
            .ScrubInlineDateTimes("yyyy-MM-dd");

    [Fact]
    public Task ScrubInlineDateTimes()
    {
        var date = DateTime.Now;
        var product = new
        {
            Title = $"item {date:f} - (date={{{date:f}}})",
            Variant = new
            {
                Id = "variant date: " + date.ToString("f")
            }
        };

        return Verify(product)
            .ScrubInlineDateTimes("f");
    }

    // ReSharper disable once UnusedMember.Local
    void DontIgnoreEmptyCollections()
    {
        #region DontIgnoreEmptyCollections

        VerifierSettings.DontIgnoreEmptyCollections();

        #endregion
    }

    // ReSharper disable once UnusedMember.Local
    void DontScrubGuids()
    {
        #region DontScrubGuidsGlobal

        VerifierSettings.DontScrubGuids();

        #endregion
    }

    // ReSharper disable once UnusedMember.Local
    void DontScrubProjectDirectory()
    {
        #region DontScrubProjectDirectory

        VerifierSettings.DontScrubProjectDirectory();

        #endregion
    }

    // ReSharper disable once UnusedMember.Local
    void DontScrubSolutionDirectory()
    {
        #region DontScrubSolutionDirectory

        VerifierSettings.DontScrubSolutionDirectory();

        #endregion
    }

/*
    #region ScrubInlineGuidsGlobal

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.ScrubInlineGuids();
    }

    #endregion
*/

    #region ScrubInlineGuidsFluent

    [Fact]
    public Task ScrubInlineGuidsFluent() =>
        Verify("content 651ad409-fc30-4b12-a47e-616d3f953e4c content")
            .ScrubInlineGuids();

    #endregion

    #region ScrubInlineGuidsInstance

    [Fact]
    public Task ScrubInlineGuidsInstance()
    {
        var settings = new VerifySettings();
        settings.ScrubInlineGuids();
        return Verify(
            "content 651ad409-fc30-4b12-a47e-616d3f953e4c content",
            settings);
    }

    #endregion

/*
    #region ScrubInlineDateTimesGlobal

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.ScrubInlineDateTimes("yyyy-MM-dd");
    }

    #endregion
*/

    #region ScrubInlineDateTimesFluent

    [Fact]
    public Task ScrubInlineDateTimesFluent() =>
        Verify("content 2020-10-20 content")
            .ScrubInlineDateTimes("yyyy-MM-dd");

    #endregion

    #region ScrubInlineDateTimesInstance

    [Fact]
    public Task ScrubInlineDateTimesInstance()
    {
        var settings = new VerifySettings();
        settings.ScrubInlineDateTimes("yyyy-MM-dd");
        return Verify(
            "content 2020-10-20 content",
            settings);
    }

    #endregion

#if NET6_0_OR_GREATER
    [Fact]
    public Task DatetimeScrubbingDisabled() =>
        Verify(
                new
                {
                    noTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    withTime = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                    withTimeZeroSeconds = new DateTime(2000, 1, 1, 1, 1, 0, DateTimeKind.Utc),
                    withTimeMilliSeconds = new DateTime(2000, 1, 1, 1, 1, 1, 999, DateTimeKind.Utc)
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

/*

#region DisableDateCountingGlobal

    [ModuleInitializer]
    public static void ModuleInitializer() =>
        VerifierSettings.DisableDateCounting();

#endregion

*/

    [Fact]
    Task DisableDateCounting()
    {
        #region DisableDateCounting

        var target = new
        {
            Date = new DateTime(2020, 10, 10, 0, 0, 0, DateTimeKind.Utc)
        };

        var settings = new VerifySettings();
        settings.DisableDateCounting();

        return Verify(target, settings);

        #endregion
    }

    [Fact]
    Task DisableDateCountingFluent()
    {
        #region DisableDateCountingFluent

        var target = new
        {
            Date = new DateTime(2020, 10, 10, 0, 0, 0, DateTimeKind.Utc)
        };

        return Verify(target)
            .DisableDateCounting();

        #endregion
    }

    [Fact]
    Task DisableDateCountingInline() =>
        Verify("a 2020/10/10 b")
            .DisableDateCounting()
            .ScrubInlineDateTimes("yyyy/MM/dd");

    /*

#region DontScrubDateTimesGlobal

    [ModuleInitializer]
    public static void ModuleInitializer() =>
        VerifierSettings.DontScrubDateTimes();

#endregion

*/

    [Fact]
    public Task NewLineNotEscapedInProperty() =>
        Verify(new
        {
            Property = "a\r\nb\\nc"
        });

    // ReSharper disable once UnusedMember.Local
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
        var tempPath = Path
            .GetTempPath()
            .TrimEnd('/', '\\');
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
        var target = Path
            .Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SomePath")
            .Replace('\\', '/');
        return Verify(target)
            .UniqueForOSPlatform();
    }

#if !NET5_0_OR_GREATER
    [Fact]
    public Task ScrubCodeBaseLocation()
    {
        var codeBaseLocation = CodeBaseLocation.CurrentDirectory!.TrimEnd('/', '\\');
        var altCodeBaseLocation = codeBaseLocation.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Verify(new
        {
            codeBaseLocation,
            altCodeBaseLocation
        });
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
    public async Task UseShortTypeName()
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
    public async Task ReUseGuid()
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
    public Task RespectEmptyGuid()
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
    public Task IgnoreGuidDefaults()
    {
        var target = new GuidTarget();
        return Verify(target);
    }

    [Fact]
    public Task ScrubProjectDirectory()
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
    public Task ScrubSolutionDirectory()
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
    public Task ScrubGuid()
    {
        var target = new GuidTarget
        {
            Guid = Guid.NewGuid(),
            GuidNullable = Guid.NewGuid(),
            GuidString = Guid
                .NewGuid()
                .ToString()
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
    public Task ScrubDictionaryKey() =>
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
                    Property = """
                               Line1
                               Line2
                               """
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
                }
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
                }
            })
            .ScrubLinesContaining("value");

    [Fact]
    public Task IgnoreEmptyList()
    {
        var target = new CollectionTarget
        {
            DictionaryProperty = new(),
            IReadOnlyDictionary = new ReadOnlyDictionary<int, string>(new Dictionary<int, string>()),
            EnumerableAsList = new List<string>(),
            EnumerableStaticEmpty = Enumerable.Empty<string>(),
            ReadOnlyList = new ReadOnlyList(),
            ListProperty = [],
            ReadOnlyCollection = new ReadOnlyCollection<string>([]),
            Array = Array.Empty<string>()
        };
        return Verify(target);
    }

    class ReadOnlyList : IReadOnlyList<string>
    {
        // ReSharper disable CollectionNeverUpdated.Local
        List<string> inner = [];
        // ReSharper restore CollectionNeverUpdated.Local

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

    // ReSharper disable once UnusedMember.Local
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
        // ReSharper disable once UnusedMember.Local
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
        // ReSharper disable once UnusedMember.Local
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

    class EnumerableWithExistingConverterTarget :
        List<string>;

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

    class ConverterWithBadNewlineTarget;

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
        List<string>;

    class EnumerableWithExistingItemConverter :
        WriteOnlyJsonConverter<string>
    {
        public override void Write(VerifyJsonWriter writer, string target) =>
            writer.Serialize(10);
    }

    [Fact]
    public Task TargetInvocationException()
    {
        var member = GetType()
            .GetMethod("MethodThatThrows")!;
        return Throws(
                () =>
                {
                    member.Invoke(null, []);
                })
            .UniqueForTargetFrameworkAndVersion()
            .ScrubLinesContaining("(Object ");
    }

    [Fact]
    public Task NestedTargetInvocationException()
    {
        var member = GetType()
            .GetMethod("MethodThatThrows")!;
        TargetInvocationException? exception = null;
        try
        {
            member.Invoke(null, []);
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

    // ReSharper disable once UnusedMember.Local
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

    // ReSharper disable once UnusedMember.Local
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
        return Verify(method
            .GetParameters()
            .First());
    }

    [Fact]
    public Task MethodWithParameters() =>
        Verify(Info.OfMethod<SerializationTests>("MyMethodWithParameters"));

    // ReSharper disable once UnusedMember.Local
    void MyMethodWithParameters(int x, string y)
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

    struct ToIncludeStruct(string property)
    {
        public string Property { get; } = property;
    }

    struct ToIgnoreStruct(string property)
    {
        public string Property { get; } = property;
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

    class InterfaceToIgnore;

    class ToIgnoreByBase :
        BaseToIgnore
    {
        public string Property;
    }

    class BaseToIgnore;

    class ToIgnoreByBaseGeneric :
        BaseToIgnoreGeneric<int>
    {
        public string Property;
    }

// ReSharper disable once UnusedTypeParameter
    class BaseToIgnoreGeneric<T>;

    // ReSharper disable once UnusedMember.Local
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

    [ModuleInitializer]
    public static void MemberConverterByExpressionInit()
    {
        // using only the member
        VerifierSettings.MemberConverter<MemberTarget, string>(
            expression: _ => _.Field,
            converter: member => $"{member}_Suffix");

        // using target and member
        VerifierSettings.MemberConverter<MemberTarget, string>(
            expression: _ => _.Property,
            converter: (target, member) => $"{target}_{member}_Suffix");
    }

    [Fact]
    public Task MemberConverterByExpression()
    {
        var input = new MemberTarget
        {
            Field = "FieldValue",
            Property = "PropertyValue"
        };

        return Verify(input);
    }

    #endregion

    class MemberTarget
    {
        public string Property { get; set; }
        public string Field;
    }

    // ReSharper disable once UnusedMember.Local
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
        IgnoreTargetBase;

    static Exception ignoreMemberSubClass;

    [ModuleInitializer]
    public static void IgnoreMemberSubClassInit()
    {
        try
        {
            VerifierSettings.IgnoreMember<IgnoreTargetSub>(_ => _.Property);
        }
        catch (Exception e)
        {
            ignoreMemberSubClass = e;
        }
    }

    [Fact]
    public Task IgnoreMemberSubClass() =>
        Verify(ignoreMemberSubClass)
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

    // ReSharper disable once UnusedMember.Local
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

    // ReSharper disable UnusedMember.Local
    class WithCustomException
    {
        public Guid CustomExceptionGuid => throw new CustomException();

        public int[] CustomExceptionArray => throw new CustomException();

        public List<string> CustomExceptionList => throw new CustomException();
    }
    // ReSharper restore UnusedMember.Local

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
        // ReSharper disable once UnusedMember.Local
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
        // ReSharper disable once UnusedMember.Local
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
        // ReSharper disable once UnusedMember.Local
        public Action DelegateProperty => () =>
        {
        };
    }

    class CustomException :
        Exception;

    [Fact]
    public Task NotSupportedExceptionProp()
    {
        var target = new WithNotSupportedException();
        return Verify(target);
    }

    class WithNotSupportedException
    {
        // ReSharper disable once UnusedMember.Local
        public Guid NotImplementedExceptionProperty => throw new NotSupportedException();
    }

    // ReSharper disable once UnusedMember.Local
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
    public Task WithRecursiveConverter() =>
        Verify(new RecursiveConverterTarget())
            .AddExtraSettings(_ => _.Converters.Add(new RecursiveConverter()));

    class RecursiveConverter :
        WriteOnlyJsonConverter<RecursiveConverterTarget>
    {
        public override void Write(VerifyJsonWriter writer, RecursiveConverterTarget target)
        {
            writer.WriteStartObject();
            writer.WriteMember(target, target.Self, "SelfProperty");
            writer.WriteEnd();
        }
    }

    class RecursiveConverterTarget
    {
        public RecursiveConverterTarget Self => this;
    }

    [ModuleInitializer]
    public static void WithConverterAndMemberConverterInit() =>
        VerifierSettings.MemberConverter<StaticConverterTarget, string>(
            target => target.Name,
            (target, value) => "New Value");

    [Fact]
    public Task WithConverterAndMemberConverter() =>
        Verify(new StaticConverterTarget
            {
                Name = "The name"
            })
            .AddExtraSettings(_ => _.Converters.Add(new StaticConverter()));

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
        public List<Child> Children { get; set; } = [];
    }

    public class Child
    {
        public Parent Parent { get; set; }
    }
}