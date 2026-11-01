#if NETSTANDARD2_0
namespace Yotei.Tools.Std20;
#pragma warning disable IDE0079
#pragma warning disable CS0436

// ========================================================
public static class IEqualityComparerExtensions
{
    extension<T>(IEqualityComparer<T>)
    {
        /// <summary>
        /// Creates an equality comparer for the given type using the given delegates.
        /// </summary>
        /// <param name="equals"></param>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        public static IEqualityComparer<T> Create(
            Func<T?, T?, bool> equals,
            Func<T, int>? hashcode = null)
        {
            equals.ThrowWhenNull();

            hashcode ??= _ => throw new NotSupportedException();
            return new InternalEqualityComparer<T>(equals, hashcode);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Used to implement the <see cref="IEqualityComparer{T}"/>'s <c>Create(...)</c> method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class InternalEqualityComparer<T> : EqualityComparer<T>
    {
        readonly Func<T?, T?, bool> _equals;
        readonly Func<T, int> _hashcode;

        [SuppressMessage("", "IDE0290")]
        public InternalEqualityComparer(Func<T?, T?, bool> equals, Func<T, int> hashcode)
        {
            _equals = equals;
            _hashcode = hashcode;
        }

        public override bool Equals(T? x, T? y) => _equals(x, y);

        public override bool Equals(object? obj) =>
            obj is InternalEqualityComparer<T> other &&
            _equals == other._equals &&
            _hashcode == other._hashcode;

        public override int GetHashCode([DisallowNull] T obj) => _hashcode(obj);

        public override int GetHashCode() =>
            HashCode.Combine(_equals.GetHashCode(), _hashcode.GetHashCode());
    }
}
#endif