using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerifyTests;
using VerifyXunit;
using Xunit;
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
    public void SettingsIsCloned()
    {
        SerializationSettings settings = new();

        List<string> ignoredMemberList = new();
        settings.ignoredMembers.Add(GetType(), ignoredMemberList);

        List<Func<object, bool>> ignoredInstances = new();
        settings.ignoredInstances.Add(GetType(), ignoredInstances);

        Dictionary<string, ConvertMember> memberConverterList = new();
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
    public Task ExtensionAwareScrubbers()
    {
        var settings = new VerifySettings();
        settings.UseExtension("html");
        settings.AddScrubber("html", builder => builder.Replace("a", "b"));
        return Verifier.Verify("a", settings);
    }

    [Fact]
    public Task Uri()
    {
        return Verifier.Verify(
            new
            {
                uri1 = new Uri("http://127.0.0.1:57754/admin/databases"),
                uri2 = new Uri("http://127.0.0.1:57754/admin/databases?name=HttpRecordingTest&replicationFactor=1&raft-request-id=1331f44c-02de-4d00-a645-28bc1b639483"),
                uri3 = new Uri("http://127.0.0.1/admin/databases?name=HttpRecordingTest&replicationFactor=1&raft-request-id=1331f44c-02de-4d00-a645-28bc1b639483"),
                uri4 = new Uri("http://127.0.0.1/?name"),
                uri5 = new Uri("http://127.0.0.1/?name=")
            });
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
    public Task HttpRequestHeaders()
    {
        var constructor = typeof(HttpRequestHeaders).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null)!;
        var instance = (HttpRequestHeaders) constructor.Invoke(null);
        instance.Add("key", "value");
        return Verifier.Verify(instance);
    }

    [Fact]
    public Task HttpRequestHeadersWithIgnored()
    {
        var constructor = typeof(HttpRequestHeaders).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null)!;
        var instance = (HttpRequestHeaders) constructor.Invoke(null);
        instance.Add("key1", "value");
        instance.Add("key2", "value");
        return Verifier.Verify(instance)
            .ModifySerialization(settings => settings.IgnoreMember("key1"));
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
            Children = new() {"Sam", "Mary"},
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

#if !NET5_0
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
        FileNameBuilder.ClearPrefixList();
        await Verifier.Verify(exception.Message);
    }

    static (bool, string Member2, string Member3) MethodWithPartialNamedTuple()
    {
        return (true, "A", "B");
    }

    [Fact]
    public Task NamedTupleWithNull()
    {
        return Verifier.Verify(() => MethodWithNamedTupleWithNull());
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
        ClaimsIdentity claimsIdentity = new();
        claimsIdentity.AddClaim(new Claim("TheType", "TheValue"));
        ClaimsPrincipal claimsPrincipal = new();
        claimsPrincipal.AddIdentity(claimsIdentity);
        return Verifier.Verify(claimsPrincipal);
    }

    [Fact]
    public Task ClaimsIdentity()
    {
        ClaimsIdentity claimsIdentity = new();
        claimsIdentity.AddClaim(new Claim("TheType", "TheValue"));
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
        GuidTarget target = new()
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
        EscapeTarget target = new()
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
        NotDatesTarget target = new()
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

    #region IgnoreMembersThatThrowExpression

    [Fact]
    public Task ExceptionMessageProp()
    {
        WithExceptionIgnoreMessage target = new();

        VerifySettings settings = new();
        settings.ModifySerialization(
            _ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));
        return Verifier.Verify(target, settings);

    }

    [Fact]
    public Task ExceptionMessagePropFluent()
    {
        WithExceptionIgnoreMessage target = new();

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
        WithNotImplementedException target = new();
        return Verifier.Verify(target);
    }

    class WithNotImplementedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotImplementedException();
    }

    #region AddIgnoreInstance

    [Fact]
    public Task AddIgnoreInstance()
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
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task AddIgnoreInstanceFluent()
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

    #region AddIgnoreType

    [Fact]
    public Task IgnoreType()
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
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task IgnoreTypeFluent()
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


    #region IgnoreMemberByExpression

    [Fact]
    public Task IgnoreMemberByExpression()
    {
        IgnoreExplicitTarget target = new()
        {
            Include = "Value",
            Field = "Value",
            Property = "Value",
            PropertyWithPropertyName = "Value"
        };
        VerifySettings settings = new();
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
        IgnoreExplicitTarget target = new()
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


    #region MemberConverterByExpression

    [Fact]
    public Task MemberConverterByExpression()
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
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task MemberConverterByExpressionFluent()
    {
        MemberConverterTarget target = new()
        {
            Field = "Value",
            Property = "Value"
        };
        return Verifier.Verify(target)
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
    public Task IgnoreMemberByName()
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
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task IgnoreMemberByNameFluent()
    {
        IgnoreExplicitTarget target = new()
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
            .ModifySerialization(_ =>
            {
                _.IgnoreMember("Ignore1");
            });
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
        Dictionary<string, object> target = new()
        {
            {
                "Include", new Dictionary<string, string>
                {
                    {"Ignore", "Value1"},
                    {"Key1", "Value2"}
                }
            },
            {"Ignore", "Value3"},
            {"Key2", "Value4"},
        };
        return Verifier.Verify(target)
            .ModifySerialization(_ =>
            {
                _.IgnoreMember("Ignore");
            });
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

    #region IgnoreMembersThatThrow

    [Fact]
    public Task CustomExceptionProp()
    {
        WithCustomException target = new();
        VerifySettings settings = new();
        settings.ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task CustomExceptionPropFluent()
    {
        WithCustomException target = new();
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
        [Obsolete]
        public string ObsoleteProperty { get; set; }

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
        var exception = await Assert.ThrowsAsync<Exception>(() => Verifier.Verify(() => MethodWithTuple()));
        FileNameBuilder.ClearPrefixList();
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