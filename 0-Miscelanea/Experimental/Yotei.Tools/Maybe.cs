namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Helpers for <see cref="Maybe{T}"/> monads.
/// </summary>
public static class Maybe
{
    /// <summary>
    /// Returns a new instance that carries the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Maybe<T> Some<T>(T value) => new(value);

    /// <summary>
    /// A common shared invalid monad.
    /// </summary>
    public static Invalid None { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Represents an invalid monad.
    /// </summary>
    public readonly struct Invalid : IComparable<Invalid>, IEquatable<Invalid>
    {
        public int CompareTo(Invalid _) => 0;
        public bool Equals(Invalid _) => true;
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is Invalid;
        override public int GetHashCode() => 0;

        public static bool operator ==(Invalid _, Invalid _1) => true;
        public static bool operator !=(Invalid _, Invalid _1) => false;
        public static bool operator <(Invalid _, Invalid _1) => false;
        public static bool operator >(Invalid _, Invalid _1) => false;
        public static bool operator >=(Invalid _, Invalid _1) => true;
        public static bool operator <=(Invalid _, Invalid _1) => true;
    }

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
    /// Gets an enumerator for the valid values of the monads in the range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="range"></param>
    /// <returns></returns>
    public static IEnumerator<T> GetEnumerator<T>(this IEnumerable<Maybe<T>> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        foreach (var item in range)
            if (item.TryGetValue(out var value)) yield return value;
    }
}