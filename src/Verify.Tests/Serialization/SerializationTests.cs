using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VerifyTests;
using VerifyXunit;
using Xunit;
using Xunit.Sdk;
// ReSharper disable UnusedParameter.Local

// Non-nullable field is uninitialized.
#pragma warning disable CS8618

[UsesVerify]
public class SerializationTests
{
    static SerializationTests()
    {
        VerifierSettings.AddExtraDatetimeFormat("F");
        VerifierSettings.AddExtraDatetimeOffsetFormat("F");
    }

    [Fact]
    public async Task ShouldReUseDatetime()
    {
        #region Date

        var dateTime = DateTime.Now;
        var dateTimeOffset = DateTimeOffset.Now;
        DateTimeTarget target = new()
        {
            DateTime = dateTime,
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
    public Task PathInfos()
    {
        return Verifier.Verify(
            new
            {
                file = new FileInfo(@"c:/foo\bar.txt"),
                directory = new DirectoryInfo(@"c:/foo\bar/")
            });
    }

    [Fact]
    public Task ShouldScrubDatetime()
    {
        var dateTime = DateTime.Now;
        var dateTimeOffset = DateTimeOffset.Now;
        DateTimeTarget target = new()
        {
            DateTime = dateTime,
            DateTimeNullable = dateTime.AddDays(1),
            DateTimeString = dateTime.AddDays(2).ToString("F"),
            DateTimeOffset = dateTimeOffset,
            DateTimeOffsetNullable = dateTimeOffset.AddDays(1),
            DateTimeOffsetString = dateTimeOffset.AddDays(2).ToString("F"),
        };

        return Verifier.Verify(target);
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

    [Fact]
    public Task GuidScrubbingDisabled()
    {
        return Verifier.Verify(
                new
                {
                    value = Guid.Parse("b6993f86-c1b9-44db-bfc5-33ed9e5c048e")
                })
            .ModifySerialization(settings => settings.DontScrubGuids());
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
    public Task Uri()
    {
        return Verifier.Verify(
            new
            {
                uri = new Uri("http://foo")
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
        Person person = new()
        {
            Id = new Guid("ebced679-45d3-4653-8791-3d969c4a986c"),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new(){"Sam", "Mary"},
            Address = new()
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        VerifySettings settings = new();
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
        Person person = new()
        {
            Id = Guid.NewGuid(),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new(){"Sam", "Mary"},
            Address = new()
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        return Verifier.Verify(person)
            .AddExtraSettings(_ => { _.TypeNameHandling = TypeNameHandling.All; });
    }

    class DateTimeTarget
    {
        public DateTime DateTime;
        public DateTime? DateTimeNullable;
        public DateTimeOffset DateTimeOffset;
        public DateTimeOffset? DateTimeOffsetNullable;
        public string DateTimeString;
        public string DateTimeOffsetString;
    }

    [Fact]
    public Task ShouldIgnoreDatetimeDefaults()
    {
        DateTimeTarget target = new();

        return Verifier.Verify(target);
    }

    [Fact]
    public async Task VerifyBytes()
    {
        await Verifier.Verify(File.ReadAllBytes("sample.jpg"))
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
        #region DontScrubGuids

        VerifierSettings.ModifySerialization(_ => _.DontScrubGuids());

        #endregion
    }

    void ScrubInlineGuids()
    {
        #region ScrubInlineGuids

        VerifierSettings.ScrubInlineGuids();

        #endregion
    }

    void DontScrubDateTimes()
    {
        #region DontScrubDateTimes

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
        VerifySettings verifySettings = new();

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
        return Verifier.Verify(Path.GetTempPath().TrimEnd('/', '\\'));
    }

    [Fact]
    public Task ScrubCurrentDirectory()
    {
        return Verifier.Verify(Environment.CurrentDirectory.TrimEnd('/', '\\'));
    }

#if !NET5_0
    [Fact]
    public Task ScrubCodeBaseLocation()
    {
        return Verifier.Verify(CodeBaseLocation.CurrentDirectory!.TrimEnd('/', '\\'));
    }
#endif

    [Fact]
    public Task ScrubBaseDirectory()
    {
        return Verifier.Verify(AppDomain.CurrentDomain.BaseDirectory!.TrimEnd('/', '\\'));
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
        TypeTarget target = new()
        {
            Type = GetType(),
            Dynamic = foo.GetType(),
        };

        await Verifier.Verify(target);

        #endregion
    }

#if(!NETSTANDARD2_0)
    [Fact]
    public async Task NamedTuple()
    {
        #region VerifyTuple

        await Verifier.Verify(() => MethodWithNamedTuple());

        #endregion
    }

    #region MethodWithNamedTuple

    static (bool Member1, string Member2, string Member3) MethodWithNamedTuple()
    {
        return (true, "A", "B");
    }

    #endregion

    [Fact]
    public async Task PartialNamedTuple()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verifier.Verify(() => MethodWithPartialNamedTuple()));
        await Verifier.Verify(exception.Message);
    }

    static (bool, string Member2, string Member3) MethodWithPartialNamedTuple()
    {
        return (true, "A", "B");
    }

    [Fact]
    public async Task NamedTupleWithNull()
    {
        await Verifier.Verify(() => MethodWithNamedTupleWithNull());
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
        GuidTarget target = new()
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
    public Task ShouldIgnoreGuidDefaults()
    {
        GuidTarget target = new();
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

    string GetProjectDirectory([CallerFilePath] string file = "")
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

    string GetSolutionDirectory([CallerFilePath] string file = "")
    {
        return new FileInfo(file).Directory!.Parent!.Parent!.FullName;
    }

    [Fact]
    public Task ShouldScrubGuid()
    {
        GuidTarget target = new()
        {
            Guid = Guid.NewGuid(),
            GuidNullable = Guid.NewGuid(),
            GuidString = Guid.NewGuid().ToString(),
        };
        return Verifier.Verify(target);
    }

    public class GuidTarget
    {
        public Guid Guid;
        public Guid? GuidNullable;
        public string GuidString;
        public Guid OtherGuid;
    }

    [Fact]
    public Task Escaping()
    {
        EscapeTarget target = new()
        {
            Property = @"\"
        };
        return Verifier.Verify(target);
    }

    public class EscapeTarget
    {
        public string Property;
    }

    [Fact]
    public Task OnlySpecificDates()
    {
        NotDatesTarget target = new()
        {
            NotDate = "1.2.3"
        };
        return Verifier.Verify(target);
    }

    public class NotDatesTarget
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
    }

    [Fact]
    public Task ShouldIgnoreEmptyList()
    {
        CollectionTarget target = new()
        {
            DictionaryProperty = new(),
            IReadOnlyDictionary = new Dictionary<int, string>(),
            ReadOnlyList = new List<string>(),
            ListProperty = new(),
            ReadOnlyCollection = new List<string>(),
            Array = new string[] { }
        };
        return Verifier.Verify(target);
    }

    public class CollectionTarget
    {
        public Dictionary<int, string> DictionaryProperty;
        public List<string> ListProperty;
        public string[] Array;
        public IReadOnlyList<string> ReadOnlyList;
        public IReadOnlyCollection<string> ReadOnlyCollection;
        public IReadOnlyDictionary<int, string> IReadOnlyDictionary;
    }
#pragma warning disable 612

    #region IgnoreMembersThatThrowExpression

    [Fact]
    public async Task ExceptionMessageProp()
    {
        WithExceptionIgnoreMessage target = new();

        VerifySettings settings = new();
        settings.ModifySerialization(
            _ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));
        await Verifier.Verify(target, settings);

    }

    [Fact]
    public async Task ExceptionMessagePropFluent()
    {
        WithExceptionIgnoreMessage target = new();

        await Verifier.Verify(target)
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
        WithNotImplementedException target = new();
        return Verifier.Verify(target);
    }

    class WithNotImplementedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotImplementedException();
    }

    #region AddIgnoreInstance

    [Fact]
    public async Task AddIgnoreInstance()
    {
        IgnoreInstanceTarget target = new()
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
        VerifySettings settings = new();
        settings.ModifySerialization(
            _ => { _.IgnoreInstance<Instance>(x => x.Property == "Ignore"); });
        await Verifier.Verify(target, settings);
    }

    [Fact]
    public async Task AddIgnoreInstanceFluent()
    {
        IgnoreInstanceTarget target = new()
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
        await Verifier.Verify(target)
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

    #region AddIgnoreType

    [Fact]
    public async Task IgnoreType()
    {
        IgnoreTypeTarget target = new()
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
        VerifySettings settings = new();
        settings.ModifySerialization(_ => _.IgnoreMembersWithType<ToIgnore>());
        await Verifier.Verify(target, settings);
    }

    [Fact]
    public async Task IgnoreTypeFluent()
    {
        IgnoreTypeTarget target = new()
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
        await Verifier.Verify(target)
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


    #region IgnoreMemberByExpression

    [Fact]
    public async Task IgnoreMemberByExpression()
    {
        IgnoreExplicitTarget target = new()
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        VerifySettings settings = new();
        settings.ModifySerialization(_ =>
        {
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);
        });
        await Verifier.Verify(target, settings);
    }

    [Fact]
    public async Task IgnoreMemberByExpressionFluent()
    {
        IgnoreExplicitTarget target = new()
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        await Verifier.Verify(target)
            .ModifySerialization(_ =>
            {
                _.IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
                _.IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
                _.IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
                _.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);
            });
    }

    #endregion


    #region MemberConverterByExpression

    [Fact]
    public async Task MemberConverterByExpression()
    {
        MemberConverterTarget target = new()
        {
            Field = "Value",
            Property = "Value"
        };
        VerifySettings settings = new();
        settings.ModifySerialization(_ =>
        {
            _.MemberConverter<MemberConverterTarget, string>(x => x.Property, (target, value) => value + "Suffix");
            _.MemberConverter<MemberConverterTarget, string>(x => x.Field, (target, value) => value + "Suffix");
        });
        await Verifier.Verify(target, settings);
    }

    [Fact]
    public async Task MemberConverterByExpressionFluent()
    {
        MemberConverterTarget target = new()
        {
            Field = "Value",
            Property = "Value"
        };
        await Verifier.Verify(target)
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
    class MemberConverterTarget
    {
        public string Property { get; set; }
        public string Field;
    }

    #region IgnoreMemberByName

    [Fact]
    public async Task IgnoreMemberByName()
    {
        IgnoreExplicitTarget target = new()
        {
            Include = "Value",
            Field = "Value",
            Property = "Value",
            PropertyByName = "Value"
        };
        VerifySettings settings = new();
        settings.ModifySerialization(_ =>
        {
            _.IgnoreMember("PropertyByName");
            var type = typeof(IgnoreExplicitTarget);
            _.IgnoreMember(type, "Property");
            _.IgnoreMember(type, "Field");
            _.IgnoreMember(type, "GetOnlyProperty");
            _.IgnoreMember(type, "PropertyThatThrows");
        });
        await Verifier.Verify(target, settings);
    }

    [Fact]
    public async Task IgnoreMemberByNameFluent()
    {
        IgnoreExplicitTarget target = new()
        {
            Include = "Value",
            Field = "Value",
            Property = "Value",
            PropertyByName = "Value"
        };
        await Verifier.Verify(target)
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
    public async Task IgnoreDictionaryKeyByName()
    {
        Dictionary<string, string> target = new()
        {
            {"Include", "Value1"},
            {"Ignore", "Value2"},
        };
        await Verifier.Verify(target)
            .ModifySerialization(_ =>
            {
                _.IgnoreMember("Ignore");
                _.AddExtraSettings(json => json.TypeNameHandling = TypeNameHandling.All);
            });
    }

    class IgnoreExplicitTarget
    {
        public string Include;
        public string Property { get; set; }
        public string PropertyByName { get; set; }
        public string GetOnlyProperty => "asd";
        public string PropertyThatThrows => throw new();
        public string Field;
    }

    #region IgnoreMembersThatThrow

    [Fact]
    public async Task CustomExceptionProp()
    {
        WithCustomException target = new();
        VerifySettings settings = new();
        settings.ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());
        await Verifier.Verify(target, settings);
    }

    [Fact]
    public async Task CustomExceptionPropFluent()
    {
        WithCustomException target = new();
        await Verifier.Verify(target)
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
        VerifySettings settings = new();
        settings.ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());

        WithException target = new();

        return Assert.ThrowsAsync<JsonSerializationException>(() => Verifier.Verify(target, settings));
    }

    class WithException
    {
        public Guid ExceptionProperty => throw new();
    }

    [Fact]
    public Task ExceptionNotIgnoreMessageProp()
    {
        VerifySettings settings = new();
        settings.ModifySerialization(
            _ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));
        WithExceptionNotIgnoreMessage target = new();

        return Assert.ThrowsAsync<JsonSerializationException>(() => Verifier.Verify(target, settings));
    }

    class WithExceptionNotIgnoreMessage
    {
        public Guid ExceptionMessageProperty => throw new("NotIgnore");
    }

    [Fact]
    public Task DelegateProp()
    {
        WithDelegate target = new();
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
        WithNotSupportedException target = new();
        return Verifier.Verify(target);
    }

    class WithNotSupportedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotSupportedException();
    }

    #region WithObsoletePropIncluded

    [Fact]
    public Task WithObsoletePropIncluded()
    {
        WithObsolete target = new()
        {
            ObsoleteProperty = "value1",
            OtherProperty = "value2"
        };
        VerifySettings settings = new();
        settings.ModifySerialization(_ => { _.IncludeObsoletes(); });
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task WithObsoletePropIncludedFluent()
    {
        WithObsolete target = new()
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
        [Obsolete] public string ObsoleteProperty { get; set; }
        public string OtherProperty { get; set; }
    }

    [Fact]
    public Task WithObsoleteProp()
    {
        WithObsolete target = new()
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
        var exception = await Assert.ThrowsAsync<XunitException>(() => Verifier.Verify(() => MethodWithTuple()));
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
        Person person = new()
        {
            GivenNames = "John",
            FamilyName = "Smith"
        };
        VerifySettings settings = new();
        settings.AddExtraSettings(
            _ => { _.TypeNameHandling = TypeNameHandling.All; });
        return Verifier.Verify(person, settings);
    }

    [Fact]
    public Task ScopedSerializerFluent()
    {
        Person person = new()
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