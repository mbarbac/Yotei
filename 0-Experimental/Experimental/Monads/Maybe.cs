using System.Security.Cryptography;

namespace Experimental;

// ========================================================
/// <summary>
/// Helpers for the <see cref="Maybe{T}"/> monad.
/// </summary>
public static class Maybe
{
    /// <summary>
    /// Represents an invalid <see cref="Maybe"/> monad.
    /// </summary>
    public readonly struct Invalid : IComparable<Invalid>, IEquatable<Invalid>
    {
        public int CompareTo(Invalid _) => 0;
        public bool Equals(Invalid _) => true;
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is Invalid;
        override public int GetHashCode() => 0;

        public static bool operator ==(Invalid x, Invalid y) => true;
        public static bool operator !=(Invalid x, Invalid y) => false;
        public static bool operator <(Invalid x, Invalid y) => false;
        public static bool operator >(Invalid x, Invalid y) => false;
        public static bool operator >=(Invalid x, Invalid y) => true;
        public static bool operator <=(Invalid x, Invalid y) => true;
    }

    /// <summary>
    /// Returns a new valid instance that carries the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Maybe<T> Some<T>(T value) => new(value);

    /// <summary>
    /// Represent an invalid <see cref="Maybe"/> monad of an arbitrary type.
    /// </summary>
    public static Invalid None { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Converts the <see cref="Maybe{T}"/> monad into an enumerable one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> AsEnumerable<T>(this Maybe<T> source)
    {
        if (source.TryGetValue(out var value)) yield return value;
    }

    /// <summary>
    /// Enumerates the valid values in the given range of monads.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="range"></param>
    /// <returns></returns>
    public static IEnumerator<T> GetEnumerator<T>(this IEnumerable<Maybe<T>> range)
    {
        range.ThrowWhenNull();

        foreach (var item in range)
            if (item.TryGetValue(out var value)) yield return value;
    }
}