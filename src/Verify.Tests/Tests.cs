using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

// Non-nullable field is uninitialized.
#pragma warning disable CS8618

public class Tests :
    VerifyBase
{
    static Tests()
    {
        SharedVerifySettings.AddExtraDatetimeFormat("F");
        SharedVerifySettings.AddExtraDatetimeOffsetFormat("F");
    }

    [Fact]
    public Task ScrubCurrentDirectory()
    {
        return Verify(Environment.CurrentDirectory.TrimEnd('/', '\\'));
    }

    [Fact]
    public Task ScrubCodeBaseLocation()
    {
        return Verify(CodeBaseLocation.CurrentDirectory.TrimEnd('/', '\\'));
    }

    [Fact]
    public Task ScrubBaseDirectory()
    {
        return Verify(AppDomain.CurrentDomain.BaseDirectory!.TrimEnd('/', '\\'));
    }

    [Fact]
    public async Task Stream()
    {
        await Verify(new MemoryStream(new byte[] {1}));
        Assert.False(File.Exists(Path.Combine(SourceDirectory, "Tests.Stream.received.bin")));
    }

    [Fact]
    public async Task StreamNegative()
    {
        if (BuildServerDetector.Detected)
        {
            return;
        }

        var binFile = Path.Combine(SourceDirectory, "Tests.StreamNegative.verified.bin");
        File.Delete(binFile);
        DiffRunner.Enabled = false;
        var exception = await Assert.ThrowsAsync<XunitException>(() => Verify(new MemoryStream(new byte[] {1})));
        DiffRunner.Enabled = true;
        File.Delete(binFile);
        await Verify(exception.Message);
    }

    [Fact]
    public async Task StreamMultiple()
    {
        var stream1 = new MemoryStream(new byte[] {1});
        var stream2 = new MemoryStream(new byte[] {1});
        await Verify(new Stream[] {stream1, stream2});

        Assert.Empty(Directory.EnumerateFiles(SourceDirectory, "Tests.StreamMultiple.*.received.bin"));
    }

    [Fact]
    public async Task StreamMultipleDangling()
    {
        var stream1 = new MemoryStream(new byte[] {1});

        var path0 = Path.Combine(SourceDirectory, "Tests.StreamMultipleDangling.00.verified.bin");
        File.WriteAllBytes(path0, new byte[] {1});

        var path1 = Path.Combine(SourceDirectory, "Tests.StreamMultipleDangling.01.verified.bin");
        File.WriteAllBytes(path1, new byte[] {1});

        var exception = await Assert.ThrowsAsync<XunitException>(
            () => { return Verify(new Stream[] {stream1}); });

        var settings = new VerifySettings();
        settings.ScrubLinesContaining("clipboard");
        await Verify(exception.Message, settings);
        File.Delete(path0);
        File.Delete(path1);
    }

    [Fact]
    public async Task StreamMultipleNegative()
    {
        if (BuildServerDetector.Detected)
        {
            return;
        }

        void DeleteTempFiles()
        {
            foreach (var binFile in Directory.EnumerateFiles(SourceDirectory, "Tests.StreamMultipleNegative*.verified.bin"))
            {
                File.Delete(binFile);
            }
        }

        DeleteTempFiles();
        DiffRunner.Enabled = false;
        var exception = await Assert.ThrowsAsync<XunitException>(
            () =>
            {
                var stream1 = new MemoryStream(new byte[] {1});
                var stream2 = new MemoryStream(new byte[] {1});
                return Verify(new Stream[] {stream1, stream2});
            });
        DiffRunner.Enabled = true;
        DeleteTempFiles();
        var settings = new VerifySettings();
        settings.ScrubMachineName();
        await Verify(exception.Message, settings);
    }

    [Fact]
    public Task Text()
    {
        return Verify("someText");
    }

    [Fact]
    public async Task TextNegative()
    {
        if (BuildServerDetector.Detected)
        {
            return;
        }

        var txtFile = Path.Combine(SourceDirectory, "Tests.TextNegative.verified.tmp");
        File.Delete(txtFile);
        DiffRunner.Enabled = false;
        var exception = await Assert.ThrowsAsync<XunitException>(() =>
        {
            var settings = new VerifySettings();
            settings.UseExtension("tmp");
            return Verify("someText", settings);
        });
        DiffRunner.Enabled = true;
        File.Delete(txtFile);
        await Verify(exception.Message);
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

        await Verify(target);

        #endregion
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

        await Verify(target);

        #endregion
    }
#if(!NETSTANDARD2_0)
    [Fact]
    public async Task NamedTuple()
    {
        #region VerifyTuple

        await Verify(() => MethodWithNamedTuple());

        #endregion
    }

    #region MethodWithNamedTuple

    static (bool Member1, string Member2, string Member3) MethodWithNamedTuple()
    {
        return (true, "A", "B");
    }

    #endregion

    [Fact]
    public async Task Tuple()
    {
        var exception = await Assert.ThrowsAsync<Exception>(async () => await Verify(() => MethodWithTuple()));
        await Verify(exception.Message);
    }

    static (bool, string, string) MethodWithTuple()
    {
        return (true, "A", "B");
    }

#endif
    [Fact]
    public async Task AddIgnoreInstance()
    {
        #region AddIgnoreInstance
        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.IgnoreInstance<Instance>(x => x.Property == "Ignore"));

        var target = new IgnoreInstanceTarget
        {
            ToIgnore = new Instance
            {
                Property = "Ignore"
            },
            ToInclude = new Instance
            {
                Property = "Include"
            },
        };
        await Verify(target, settings);

        #endregion
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

    [Fact]
    public async Task IgnoreType()
    {
        #region AddIgnoreType
        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.IgnoreMembersWithType<ToIgnore>());

        var target = new IgnoreTypeTarget
        {
            ToIgnore = new ToIgnore
            {
                Property = "Value"
            },
            ToInclude = new ToInclude
            {
                Property = "Value"
            }
        };
        await Verify(target, settings);

        #endregion
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

    [Fact]
    public async Task IgnoreMemberByExpression()
    {
        #region IgnoreMemberByExpression
        var settings = new VerifySettings();
        settings.ModifySerialization(_ =>
        {
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
            _.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);
        });

        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        await Verify(target, settings);

        #endregion
    }

    [Fact]
    public async Task IgnoreMemberByName()
    {
        #region IgnoreMemberByName
        var settings = new VerifySettings();
        settings.ModifySerialization(_ =>
        {
            var type = typeof(IgnoreExplicitTarget);
            _.IgnoreMember(type, "Property");
            _.IgnoreMember(type, "Field");
            _.IgnoreMember(type, "GetOnlyProperty");
            _.IgnoreMember(type, "PropertyThatThrows");
        });

        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        await Verify(target, settings);

        #endregion
    }

    class IgnoreExplicitTarget
    {
        public string Include;
        public string Property { get; set; }
        public string GetOnlyProperty => "asd";
        public string PropertyThatThrows => throw new Exception();
        public string Field;
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
    public async Task CustomExceptionProp()
    {
        #region IgnoreMembersThatThrow
        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());

        var target = new WithCustomException();
        await Verify(target, settings);

        #endregion
    }

    class WithCustomException
    {
        public Guid CustomExceptionProperty => throw new CustomException();
    }

    [Fact]
    public Task NewlinesText()
    {
        return Verify("a\r\nb\nc");
    }

    [Fact]
    public Task Newlines()
    {
        return Verify("a\r\nb\nc");
    }

    [Fact]
    public Task ExceptionProps()
    {
        try
        {
            throw new Exception();
        }
        catch (Exception exception)
        {
            return Verify(exception);
        }
    }

    [Fact]
    public void ExceptionProp()
    {
        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());

        var target = new WithException();

        Assert.ThrowsAsync<JsonSerializationException>(async () => await Verify(target, settings));
    }

    class WithException
    {
        public Guid ExceptionProperty => throw new Exception();
    }

    internal class CustomException : Exception
    {
    }

    [Fact]
    public async Task ExceptionMessageProp()
    {
        #region IgnoreMembersThatThrowExpression
        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));

        var target = new WithExceptionIgnoreMessage();
        await Verify(target, settings);

        #endregion
    }

    class WithExceptionIgnoreMessage
    {
        public Guid ExceptionMessageProperty => throw new Exception("Ignore");
    }

    [Fact]
    public void ExceptionNotIgnoreMessageProp()
    {
        var settings = new VerifySettings();
        settings.ModifySerialization(_ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));
        var target = new WithExceptionNotIgnoreMessage();

        Assert.ThrowsAsync<JsonSerializationException>(async () => await Verify(target, settings));
    }

    class WithExceptionNotIgnoreMessage
    {
        public Guid ExceptionMessageProperty => throw new Exception("NotIgnore");
    }

    [Fact]
    public Task DelegateProp()
    {
        var target = new WithDelegate();
        return Verify(target);
    }

    class WithDelegate
    {
        public Action DelegateProperty => () => { };
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

    [Fact]
    public Task WithObsoleteProp()
    {
        var target = new WithObsolete();
        return Verify(target);
    }

    class WithObsolete
    {
        Guid obsoleteProperty;

        [Obsolete]
        public Guid ObsoleteProperty
        {
            get { throw new NotImplementedException(); }
            set => obsoleteProperty = value;
        }
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

    public class EscapeTarget
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

    public class NotDatesTarget
    {
        public string NotDate;
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
        return Verify(target);
    }

    [Fact]
    public Task ShouldIgnoreEmptyList()
    {
        var target = new CollectionTarget
        {
            DictionaryProperty = new Dictionary<int, string>(),
            IReadOnlyDictionary = new Dictionary<int, string>(),
            ReadOnlyList = new List<string>(),
            ListProperty = new List<string>(),
            ReadOnlyCollection = new List<string>(),
            Array = new string[] { }
        };
        return Verify(target);
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

    [Fact]
    public Task TaskResult()
    {
        var target = Task.FromResult("value");
        return Verify(target);
    }

#if NETCOREAPP3_1
    [Fact]
    public async Task TaskResultAsyncDisposable()
    {
        var disposableTarget = new AsyncDisposableTarget();
        var target = Task.FromResult(disposableTarget);
        await Verify(target);
        Assert.False(disposableTarget.Disposed);
        Assert.True(disposableTarget.AsyncDisposed);
    }

    class AsyncDisposableTarget :
        IAsyncDisposable,
        IDisposable
    {
#pragma warning disable 414
        public string Property = "Value";
#pragma warning restore 414
        public bool AsyncDisposed;

        public ValueTask DisposeAsync()
        {
            AsyncDisposed = true;
            return new ValueTask();
        }

        public bool Disposed;

        public void Dispose()
        {
            Disposed = true;
        }
    }
#endif

    [Fact]
    public async Task TaskResultDisposable()
    {
        var disposableTarget = new DisposableTarget();
        var target = Task.FromResult(disposableTarget);
        await Verify(target);
        Assert.True(disposableTarget.Disposed);
    }

    class DisposableTarget :
        IDisposable
    {
#pragma warning disable 414
        public string Property = "Value";
#pragma warning restore 414
        public bool Disposed;

        public void Dispose()
        {
            Disposed = true;
        }
    }

    [Fact]
    public Task ShouldIgnoreGuidDefaults()
    {
        var target = new GuidTarget();
        return Verify(target);
    }

    public class GuidTarget
    {
        public Guid Guid;
        public Guid? GuidNullable;
        public string GuidString;
        public Guid OtherGuid;
    }

    public class TypeTarget
    {
        public Type Type;
        public Type Dynamic;
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
            DateTimeNullable = dateTime,
            DateTimeString = dateTime.ToString("F"),
            DateTimeOffset = dateTimeOffset,
            DateTimeOffsetNullable = dateTimeOffset,
            DateTimeOffsetString = dateTimeOffset.ToString("F"),
        };

        await Verify(target);

        #endregion
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
        };

        return Verify(target);
    }

    [Fact]
    public Task ShouldIgnoreDatetimeDefaults()
    {
        var target = new DateTimeTarget();

        return Verify(target);
    }

    [Fact]
    public async Task VerifyFilePath()
    {
        await VerifyFile("sample.txt");
        Assert.False(FileEx.IsFileLocked("sample.txt"));
    }

    [Fact]
    public async Task VerifyFilePathSplit()
    {
        SharedVerifySettings.RegisterFileConverter("split", "txt", DoSplit);
        await VerifyFile("sample.split");
        Assert.False(FileEx.IsFileLocked("sample.split"));
    }

    static IEnumerable<Stream> DoSplit(Stream stream)
    {
        var reader = new StreamReader(stream);
        var line1 = reader.ReadLine()!;
        yield return new MemoryStream(Encoding.UTF8.GetBytes(line1));
        var line2 = reader.ReadLine()!;
        yield return new MemoryStream(Encoding.UTF8.GetBytes(line2));
    }

    public class DateTimeTarget
    {
        public DateTime DateTime;
        public DateTime? DateTimeNullable;
        public DateTimeOffset DateTimeOffset;
        public DateTimeOffset? DateTimeOffsetNullable;
        public string DateTimeString;
        public string DateTimeOffsetString;
    }

    [Fact]
    public Task ExampleNonDefaults()
    {
        var person = new Person
        {
            Id = new Guid("ebced679-45d3-4653-8791-3d969c4a986c"),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new List<string> {"Sam", "Mary"},
            Address = new Address
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
            Children = new List<string> {"Sam", "Mary"},
            Address = new Address
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        var settings = new VerifySettings();
        settings.AddExtraSettings(_ => { _.TypeNameHandling = TypeNameHandling.All; });
        return Verify(person, settings);
    }

    //[Fact(Skip = "explicit")]
    //public async Task ShouldUseExtraSettings()
    //{
    //    ApplyExtraSettings(settings => { settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat; });

    //    var person = new Person
    //    {
    //        Dob = new DateTime(1980, 5, 5, 1, 1, 1)
    //    };
    //    DontScrubDateTimes();
    //    await Verify(person);
    //}

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}