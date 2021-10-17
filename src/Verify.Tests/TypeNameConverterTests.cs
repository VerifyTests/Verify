﻿using MyNamespace;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class TypeNameConverterTests
{
    [Fact]
    public Task WithOneGeneric()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(Dictionary<string, ConcurrentDictionary<string, string>>)));
    }

    [Fact]
    public Task Simple()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(string)));
    }

    [Fact]
    public Task GenericTypeDefinition()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(IEnumerable<>)));
    }

    [Fact]
    public Task GenericArguments()
    {
        var type = typeof(IEnumerable<>)
            .GetGenericArguments()
            .First();
        return Verifier.Verify(TypeNameConverter.GetName(type));
    }

    [Fact]
    public Task Nested()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(TargetWithNested)));
    }

    [Fact]
    public Task Nullable()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(int?)));
    }

    [Fact]
    public Task Array()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(int[])));
    }

    [Fact]
    public Task ArrayMulti()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(int[,])));
    }

    [Fact]
    public Task ArrayNullable()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(int?[])));
    }

    [Fact]
    public Task List()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(List<TargetWithNamespace>)));
    }

    [Fact]
    public Task Enumerable()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(IEnumerable<TargetWithNamespace>)));
    }

    [Fact]
    public Task Dictionary()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(Dictionary<string,int>)));
    }
    [Fact]
    public Task Dictionary2()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(Dictionary<IEnumerable<TargetWithNamespace>,IEnumerable<TargetWithNamespace>>)));
    }

    [Fact]
    public Task DictionaryWrapper()
    {
        return Verifier.Verify(typeof(DictionaryWrapper<IEnumerable<TargetWithNamespace>,IEnumerable<TargetWithNamespace>>));
    }

    [Fact]
    public Task Dynamic()
    {
        return Verifier.Verify(TypeNameConverter.GetName(new {Name = "foo"}.GetType()));
    }

    [Fact]
    public Task RuntimeEnumerable()
    {
        return Verifier.Verify(TypeNameConverter.GetName(MethodWithYield().GetType()));
    }

    [Fact]
    public Task RuntimeEnumerableDynamic()
    {
        return Verifier.Verify(TypeNameConverter.GetName(MethodWithYieldDynamic().GetType()));
    }

    [Fact]
    public Task RuntimeEnumerableWithSelect()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        return Verifier.Verify(TypeNameConverter.GetName(MethodWithYield().Select(x => x != null).GetType()));
    }

    [Fact]
    public Task RuntimeEnumerableDynamicWithSelect()
    {
        return Verifier.Verify(TypeNameConverter.GetName(MethodWithYieldDynamic().Select(x => x != null).GetType()));
    }

    [Fact]
    public Task RuntimeEnumerableDynamicWithInnerSelect()
    {
        return Verifier.Verify(TypeNameConverter.GetName(MethodWithYield().Select(x => new {X = x.ToString()}).GetType()));
    }

    [Fact]
    public Task EnumerableOfArray()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(IEnumerable<TargetWithNamespace[]>)));
    }

    static IEnumerable<TargetWithNamespace> MethodWithYield()
    {
        yield return new();
    }

    static IEnumerable<dynamic> MethodWithYieldDynamic()
    {
        yield return new {X = "1"};
    }

    [Fact]
    public Task ListOfArray()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(List<TargetWithNamespace[]>)));
    }

    [Fact]
    public Task ArrayOfList()
    {
        return Verifier.Verify(TypeNameConverter.GetName(typeof(List<TargetWithNamespace>[])));
    }

    class TargetWithNested
    {
    }
}

namespace MyNamespace
{
    public class TargetWithNamespace{}
}