namespace Yotei.Tools;

// ========================================================
public static class IEnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>
        /// Determines if the enumeration is an empty one.
        /// <br/> Note that this property may have side effects as it tries to obtain the first
        /// element in the enumeration.
        /// </summary>
        public bool IsEmpty => !source.ThrowWhenNull().Any();

        /// <summary>
        /// Executes the given action for each element in the enumeration.
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(action);

            foreach (var item in source) action(item);
        }

        /// <summary>
        /// Executes the given action for each element in the enumeration that meets the given
        /// predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="action"></param>
        public void ForEach(Predicate<T> predicate, Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(action);

            foreach (var item in source) if (predicate(item)) action(item);
        }
    }
}