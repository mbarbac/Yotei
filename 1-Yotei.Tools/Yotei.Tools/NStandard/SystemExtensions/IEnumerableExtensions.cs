#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static class IEnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>
        /// Determines if the collection is an empty one.
        /// <br/> CAUTION: This property may drive side effects as it tries to obtain the first
        /// element in the enumeration.
        /// </summary>
        public bool IsEmpty => !source.Any();

        // ------------------------------------------------

        /// <summary>
        /// Executes the given action with each element of the enumeration.
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<T> action)
        {
            action.ThrowWhenNull();
            foreach (var item in source) action(item);
        }

        /// <summary>
        /// Executes the given action with the elements of the enumeration that match the given
        /// predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="action"></param>
        public void ForEach(Predicate<T> predicate, Action<T> action)
        {
            predicate.ThrowWhenNull();
            action.ThrowWhenNull();

            foreach (var item in source) if (predicate(item)) action(item);
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns an array based in this enumeration, if the project was compile under a DEBUG
        /// environment, or the original enumeration otherwise.
        /// </summary>
        /// <returns></returns>
#if DEBUG
        public T[] ToDebugArray() => [.. source];
#else
        IEnumerable<T> ToDebugArray() => source;
#endif
    }
}