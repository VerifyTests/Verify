using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

// Non-nullable field is uninitialized.
#pragma warning disable CS8618

public class Tests :
    VerifyBase
{
    static Tests()
    {
        StringScrubber.AddExtraDatetimeFormat("F");
        StringScrubber.AddExtraDatetimeOffsetFormat("F");
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
#if(NETFRAMEWORK)
    [Fact]
    public async Task NamedTuple()
    {
        #region VerifyTuple

        await VerifyTuple(() => MethodWithNamedTuple());

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
        var exception = await Assert.ThrowsAsync<Exception>(async () => await VerifyTuple(() => MethodWithTuple()));
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

        IgnoreInstance<Instance>(x => x.Property == "Ignore");

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

        IgnoreMembersWithType<ToIgnore>();

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

        IgnoreMember<IgnoreExplicitTarget>(x => x.Property);
        IgnoreMember<IgnoreExplicitTarget>(x => x.Field);
        IgnoreMember<IgnoreExplicitTarget>(x => x.GetOnlyProperty);
        IgnoreMember<IgnoreExplicitTarget>(x => x.PropertyThatThrows);

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

        var type = typeof(IgnoreExplicitTarget);
        IgnoreMember(type, "Property");
        IgnoreMember(type, "Field");
        IgnoreMember(type, "GetOnlyProperty");
        IgnoreMember(type, "PropertyThatThrows");

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

        IgnoreMembersThatThrow<CustomException>();

        var target = new WithCustomException();
        await Verify(target);

        #endregion
    }

    class WithCustomException
    {
        public Guid CustomExceptionProperty => throw new CustomException();
    }

    [Fact]
    public async Task ExceptionProps()
    {
        try
        {
            throw new Exception();
        }
        catch (Exception exception)
        {
            await Verify(exception);
        }
    }

    [Fact]
    public void ExceptionProp()
    {
        IgnoreMembersThatThrow<CustomException>();

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

        IgnoreMembersThatThrow<Exception>(
            x => x.Message == "Ignore");

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
        IgnoreMembersThatThrow<Exception>(x => x.Message == "Ignore");
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
            Dob = DateTime.MaxValue,
            Spouse = "Jill",
            Children = new List<string> {"Sam", "Mary"},
            Dead = false,
            UnDead = null,
            Address = new Address
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        DontScrubDateTimes();
        DontIgnoreFalse();
        DontScrubGuids();
        DontIgnoreEmptyCollections();
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
            Dob = DateTime.Now,
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

    [Fact]
    public async Task Example()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Dob = DateTime.Now,
            Spouse = "Jill",
            Children = new List<string> {"Sam", "Mary"},
            Address = new Address
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        await Verify(person);
    }

    class Person
    {
        public string GivenNames;
        public string FamilyName;
        public string Spouse;
        public Address Address;
        public List<string> Children;
        public Title Title;
        public DateTime Dob;
        public Guid Id;
        public bool Dead;
        public bool? UnDead;
    }

    class Address
    {
        public string Street;
        public string Suburb;
        public string Country;
    }

    enum Title
    {
        Mr
    }

    [Fact(Skip = "explicit")]
    public async Task ShouldUseExtraSettings()
    {
        ApplyExtraSettings(settings => { settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat; });

        var person = new Person
        {
            Dob = new DateTime(1980, 5, 5, 1, 1, 1)
        };
        DontScrubDateTimes();
        await Verify(person);
    }

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}