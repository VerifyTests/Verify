public class OrderTests
{
    #region OrderEnumerableByGlobal

    [ModuleInitializer]
    public static void OrderEnumerableByInitializer() =>
        VerifierSettings.OrderEnumerableBy<TargetForGlobal>(_ => _.Value);

    #endregion

    [Fact]
    public Task OrderEnumerableByGlobal() =>
        Verify(
            new List<TargetForGlobal>
            {
                new("a"),
                new("c"),
                new("b")
            });

    public record TargetForGlobal(string Value);

    #region OrderEnumerableByDescendingGlobal

    [ModuleInitializer]
    public static void OrderEnumerableByDescendingInitializer() =>
        VerifierSettings.OrderEnumerableByDescending<TargetForGlobalDescending>(_ => _.Value);

    #endregion

    [Fact]
    public Task OrderEnumerableByDescendingGlobal() =>
        Verify(
            new List<TargetForGlobalDescending>
            {
                new("a"),
                new("c"),
                new("b")
            });

    public record TargetForGlobalDescending(string Value);

    public record Target(string Value);

    #region OrderEnumerableBy

    [Fact]
    public Task EnumerableOrder()
    {
        var settings = new VerifySettings();
        settings.OrderEnumerableBy<Target>(_ => _.Value);
        return Verify(
            new List<Target>
            {
                new("a"),
                new("c"),
                new("b")
            },
            settings);
    }

    #endregion

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

    #region OrderEnumerableByDescending

    [Fact]
    public Task OrderEnumerableByDescending()
    {
        var settings = new VerifySettings();
        settings.OrderEnumerableByDescending<Target>(_ => _.Value);
        return Verify(
            new List<Target>
            {
                new("a"),
                new("c"),
                new("b")
            },
            settings);
    }

    #endregion

    #region OrderEnumerableByFluent

    [Fact]
    public Task EnumerableOrderFluent() =>
        Verify(
                new List<Target>
                {
                    new("a"),
                    new("c"),
                    new("b")
                })
            .OrderEnumerableBy<Target>(_ => _.Value);

    #endregion

    #region OrderEnumerableByDescendingFluent

    [Fact]
    public Task OrderEnumerableByDescendingFluent() =>
        Verify(
                new List<Target>
                {
                    new("a"),
                    new("c"),
                    new("b")
                })
            .OrderEnumerableByDescending<Target>(_ => _.Value);

    #endregion

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

    class NonComparableKey(string member)
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
}