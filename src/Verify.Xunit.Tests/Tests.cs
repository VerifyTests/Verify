﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        StringScrubbingConverter.AddExtraDatetimeFormat("F");
        StringScrubbingConverter.AddExtraDatetimeOffsetFormat("F");
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
    public async Task StreamMultipleNegative()
    {
        void DeleteTempFiles()
        {
            foreach (var binFile in Directory.EnumerateFiles(SourceDirectory, "Tests.StreamMultipleNegative*.verified.bin"))
            {
                File.Delete(binFile);
            }
        }

        DeleteTempFiles();
        DiffRunner.Enabled = false;
        var exception = await Assert.ThrowsAsync<XunitException>(() =>
        {
            var stream1 = new MemoryStream(new byte[] {1});
            var stream2 = new MemoryStream(new byte[] {1});
            return Verify(new Stream[] {stream1, stream2});
        });
        DiffRunner.Enabled = true;
        DeleteTempFiles();
        await Verify(exception.Message);
    }

    [Fact]
    public Task Text()
    {
        return Verify("someText");
    }

    [Fact]
    public async Task TextNegative()
    {
        var txtFile = Path.Combine(SourceDirectory, "Tests.TextNegative.verified.tmp");
        File.Delete(txtFile);
        DiffRunner.Enabled = false;
        var exception = await Assert.ThrowsAsync<XunitException>(() => Verify("someText", "tmp"));
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

        ModifySerialization(_ => _.IgnoreInstance<Instance>(x => x.Property == "Ignore"));

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
        await Verify(target);

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

        ModifySerialization(_ => _.IgnoreMembersWithType<ToIgnore>());

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
        await Verify(target);

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

        ModifySerialization(settings =>
        {
            settings.IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
            settings.IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
            settings.IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
            settings.IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);
        });

        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        await Verify(target);

        #endregion
    }

    [Fact]
    public async Task IgnoreMemberByName()
    {
        #region IgnoreMemberByName

        ModifySerialization(settings =>
        {
            var type = typeof(IgnoreExplicitTarget);
            settings.IgnoreMember(type, "Property");
            settings.IgnoreMember(type, "Field");
            settings.IgnoreMember(type, "GetOnlyProperty");
            settings.IgnoreMember(type, "PropertyThatThrows");
        });

        var target = new IgnoreExplicitTarget
        {
            Include = "Value",
            Field = "Value",
            Property = "Value"
        };
        await Verify(target);

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
    public async Task NotImplementedExceptionProp()
    {
        var target = new WithNotImplementedException();
        await Verify(target);
    }

    class WithNotImplementedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotImplementedException();
    }

    [Fact]
    public async Task CustomExceptionProp()
    {
        #region IgnoreMembersThatThrow

        ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());

        var target = new WithCustomException();
        await Verify(target);

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
        ModifySerialization(_ => _.IgnoreMembersThatThrow<CustomException>());

        var target = new WithException();

        Assert.ThrowsAsync<JsonSerializationException>(async () => await Verify(target));
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

        ModifySerialization(_ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));

        var target = new WithExceptionIgnoreMessage();
        await Verify(target);

        #endregion
    }

    class WithExceptionIgnoreMessage
    {
        public Guid ExceptionMessageProperty => throw new Exception("Ignore");
    }

    [Fact]
    public void ExceptionNotIgnoreMessageProp()
    {
        ModifySerialization(_ => _.IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore"));
        var target = new WithExceptionNotIgnoreMessage();

        Assert.ThrowsAsync<JsonSerializationException>(async () => await Verify(target));
    }

    class WithExceptionNotIgnoreMessage
    {
        public Guid ExceptionMessageProperty => throw new Exception("NotIgnore");
    }

    [Fact]
    public async Task DelegateProp()
    {
        var target = new WithDelegate();
        await Verify(target);
    }

    class WithDelegate
    {
        public Action DelegateProperty => () => { };
    }

    [Fact]
    public async Task NotSupportedExceptionProp()
    {
        var target = new WithNotSupportedException();
        await Verify(target);
    }

    class WithNotSupportedException
    {
        public Guid NotImplementedExceptionProperty => throw new NotSupportedException();
    }

    [Fact]
    public async Task WithObsoleteProp()
    {
        var target = new WithObsolete();
        await Verify(target);
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
    public async Task Escaping()
    {
        var target = new EscapeTarget
        {
            Property = @"\"
        };
        await Verify(target);
    }

    public class EscapeTarget
    {
        public string Property;
    }

    [Fact]
    public async Task OnlySpecificDates()
    {
        var target = new NotDatesTarget
        {
            NotDate = "1.2.3"
        };
        await Verify(target);
    }

    public class NotDatesTarget
    {
        public string NotDate;
    }

    [Fact]
    public async Task ShouldScrubGuid()
    {
        var target = new GuidTarget
        {
            Guid = Guid.NewGuid(),
            GuidNullable = Guid.NewGuid(),
            GuidString = Guid.NewGuid().ToString(),
        };
        await Verify(target);
    }

    [Fact]
    public async Task ShouldIgnoreEmptyList()
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
        await Verify(target);
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
    public async Task ShouldIgnoreGuidDefaults()
    {
        var target = new GuidTarget();
        await Verify(target);
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
    public async Task ShouldScrubDatetime()
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

        await Verify(target);
    }

    [Fact]
    public async Task ShouldIgnoreDatetimeDefaults()
    {
        var target = new DateTimeTarget();

        await Verify(target);
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
    public async Task ExampleNonDefaults()
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

        ModifySerialization(settings =>
        {
            settings.DontScrubDateTimes();
            settings.DontIgnoreFalse();
            settings.DontScrubGuids();
            settings.DontIgnoreEmptyCollections();
        });
        AddScrubber(s => s.Replace("Lane", "Street"));
        await Verify(person);
    }

    [Fact]
    public async Task TypeNameHandlingAll()
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

        var jsonSerializerSettings = BuildJsonSerializerSettings();
        jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;
        await Verify(person, jsonSerializerSettings);
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