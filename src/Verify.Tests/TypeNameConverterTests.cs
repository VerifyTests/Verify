using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNamespace;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class TypeNameConverterTests :
    VerifyBase
{
    [Fact]
    public Task Simple()
    {
        return Verify(TypeNameConverter.GetName(typeof(string)));
    }

    [Fact]
    public Task Nested()
    {
        return Verify(TypeNameConverter.GetName(typeof(TargetWithNested)));
    }

    [Fact]
    public Task Nullable()
    {
        return Verify(TypeNameConverter.GetName(typeof(int?)));
    }

    [Fact]
    public Task Array()
    {
        return Verify(TypeNameConverter.GetName(typeof(int[])));
    }

    [Fact]
    public Task List()
    {
        return Verify(TypeNameConverter.GetName(typeof(List<TargetWithNamespace>)));
    }

    [Fact]
    public Task Enumerable()
    {
        return Verify(TypeNameConverter.GetName(typeof(IEnumerable<TargetWithNamespace>)));
    }

    [Fact]
    public Task Dynamic()
    {
        return Verify(TypeNameConverter.GetName(new{Name="foo"}.GetType()));
    }

    [Fact]
    public Task RuntimeEnumerable()
    {
        return Verify(TypeNameConverter.GetName(MethodWithYield().GetType()));
    }

    [Fact]
    public Task RuntimeEnumerableDynamic()
    {
        return Verify(TypeNameConverter.GetName(MethodWithYieldDynamic().GetType()));
    }

    [Fact]
    public Task RuntimeEnumerableWithSelect()
    {
        return Verify(TypeNameConverter.GetName(MethodWithYield().Select(x => x!=null).GetType()));
    }

    [Fact]
    public Task RuntimeEnumerableDynamicWithSelect()
    {
        return Verify(TypeNameConverter.GetName(MethodWithYieldDynamic().Select(x => x!=null).GetType()));
    }

    [Fact]
    public Task RuntimeEnumerableDynamicWithInnerSelect()
    {
        return Verify(TypeNameConverter.GetName(MethodWithYield().Select(x => new {X=x.ToString()}).GetType()));
    }

    [Fact]
    public Task EnumerableOfArray()
    {
        return Verify(TypeNameConverter.GetName(typeof(IEnumerable<TargetWithNamespace[]>)));
    }

    static IEnumerable<TargetWithNamespace> MethodWithYield()
    {
        yield return new TargetWithNamespace();
    }

    static IEnumerable<dynamic> MethodWithYieldDynamic()
    {
        yield return new {X="1"};
    }

    [Fact]
    public Task ListOfArray()
    {
        return Verify(TypeNameConverter.GetName(typeof(List<TargetWithNamespace[]>)));
    }

    [Fact]
    public Task ArrayOfList()
    {
        return Verify(TypeNameConverter.GetName(typeof(List<TargetWithNamespace>[])));
    }

    public class TargetWithNested{}

    public TypeNameConverterTests(ITestOutputHelper output) :
        base(output)
    {
    }
}

namespace MyNamespace
{
    public class TargetWithNamespace{}
}