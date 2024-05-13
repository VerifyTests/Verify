using System.Collections.Immutable;

namespace VerifyMSTest.SourceGenerator;

// TODO: Is there a better place for this to live?

// Suppressing style warnings to keep code aligned with upstream version from
// https://github.com/andrewlock/StronglyTypedId/blob/e5df78d0aa72f2232f423938c0d98d9bf4517092/src/StronglyTypedIds/EquatableArray.cs
#pragma warning disable IDE1006 // Naming rule violation Prefix is not expected
#pragma warning disable IDE0021 // Use expression body for constructor
#pragma warning disable IDE0022 // Use expression body for method
#pragma warning disable IDE0024 // Use expression body for operator

/// <summary>
/// An immutable, equatable array. This is equivalent to <see cref="T:T[]"/> but with value equality support.
/// </summary>
/// <typeparam name="T">The type of values in the array.</typeparam>
public readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// The underlying <typeparamref name="T"/> array.
    /// </summary>
    private readonly T[]? _array;

    /// <summary>
    /// Creates a new <see cref="EquatableArray{T}"/> instance.
    /// </summary>
    /// <param name="array">The input <see cref="ImmutableArray{T}"/> to wrap.</param>
    public EquatableArray(T[] array)
    {
        _array = array;
    }

    /// <sinheritdoc/>
    public bool Equals(EquatableArray<T> array)
    {
        return AsSpan().SequenceEqual(array.AsSpan());
    }

    /// <sinheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is EquatableArray<T> array && Equals(this, array);
    }

    /// <sinheritdoc/>
    public override int GetHashCode()
    {
        if (_array is not T[] array)
        {
            return 0;
        }

        HashCode hashCode = default;

        foreach (var item in array)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlySpan{T}"/> wrapping the current items.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> wrapping the current items.</returns>
    public ReadOnlySpan<T> AsSpan()
    {
        return _array.AsSpan();
    }

    /// <sinheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>)(_array ?? Array.Empty<T>())).GetEnumerator();
    }

    /// <sinheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)(_array ?? Array.Empty<T>())).GetEnumerator();
    }

    public int Count => _array?.Length ?? 0;

    /// <summary>
    /// Checks whether two <see cref="EquatableArray{T}"/> values are the same.
    /// </summary>
    /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
    /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
    /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks whether two <see cref="EquatableArray{T}"/> values are not the same.
    /// </summary>
    /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
    /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
    /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
    {
        return !left.Equals(right);
    }
}