namespace Experimental.Monads;

// ========================================================
/// <summary>
/// Helpers for the <see cref="Maybe{T}"/> monad.
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
    /// Represents an invalid monad of an arbitrary type.
    /// </summary>
    public static Invalid None { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Represents an arbitrary invalid monad.
    /// </summary>
    [SuppressMessage("", "IDE0060")]
    public readonly struct Invalid : IComparable<Invalid>, IEquatable<Invalid>
    {
        public int CompareTo(Invalid _) => 0;
        public bool Equals(Invalid _) => true;
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is Invalid;
        public override int GetHashCode() => int.MinValue;

        public static bool operator ==(Invalid x, Invalid y) => true;
        public static bool operator !=(Invalid x, Invalid y) => false;
        public static bool operator <(Invalid x, Invalid y) => false;
        public static bool operator >(Invalid x, Invalid y) => false;
        public static bool operator >=(Invalid x, Invalid y) => true;
        public static bool operator <=(Invalid x, Invalid y) => true;
    }
}