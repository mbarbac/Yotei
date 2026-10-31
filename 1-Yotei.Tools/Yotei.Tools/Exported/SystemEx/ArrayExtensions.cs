#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
//#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static class ArrayExtensions
{
    extension<T>(T[] source)
    {
        /// <summary>
        /// Returns a typed enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetTypedEnumerator() => ((IEnumerable<T>)source).GetEnumerator();

        // ------------------------------------------------

        /// <summary>
        /// Returns a copy of this array where its elements have also been cloned if deep cloning
        /// is requested.
        /// </summary>
        /// <param name="deep"></param>
        /// <returns></returns>
        public T[] Clone(bool deep)
        {
            if (deep)
            {
                var len = source.Length; if (len == 0) return [];
                var target = new T[len];
                for (int i = 0; i < len; i++) target[i] = source[i].TryClone()!;
                return target;
            }
            else return (T[])source.Clone();
        }

        // ------------------------------------------------

        /// <summary>
        /// Determines if this array contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value) => source.IndexOf(value) >= 0;

        /// <summary>
        /// Returns the index of the first ocurrence of the given value, or -1 if any.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(T value)
        {
            var comparer = EqualityComparer<T>.Default; ;
            return source.IndexOf(value, comparer);
        }

        /// <summary>
        /// Returns the index of the last ocurrence of the given value, or -1 if any.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int LastIndexOf(T value)
        {
            var comparer = EqualityComparer<T>.Default; ;
            return source.LastIndexOf(value, comparer);
        }

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<int> IndexesOf(T value)
        {
            var comparer = EqualityComparer<T>.Default; ;
            return source.IndexesOf(value, comparer);
        }

        // ------------------------------------------------

        /// <summary>
        /// Determines if this array contains the given value, using the given comparer.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool Contains(
            T value, IEqualityComparer<T> comparer) => source.IndexOf(value, comparer) >= 0;

        /// <summary>
        /// Returns the index of the first ocurrence of the given value, using the given comparer,
        /// or -1 if any.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(T value, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            return source.IndexOf(x => comparer.Equals(x, value));
        }

        /// <summary>
        /// Returns the index of the last ocurrence of the given value, using the given comparer,
        /// or -1 if any.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int LastIndexOf(T value, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            return source.LastIndexOf(x => comparer.Equals(x, value));
        }

        /// <summary>
        /// Returns the indexes of the ocurrences of the given value, using the given comparer.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<int> IndexesOf(T value, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            return source.IndexesOf(x => comparer.Equals(x, value));
        }

        // ------------------------------------------------

        /// <summary>
        /// Determines if this array contains an element that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(Predicate<T> predicate) => source.IndexOf(predicate) >= 0;

        /// <summary>
        /// Returns the index of the first element that matches the given predicate, or -1 if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int IndexOf(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            for (int i = 0; i < source.Length; i++) if (predicate(source[i])) return i;
            return -1;
        }

        /// <summary>
        /// Returns the index of the last element that matches the given predicate, or -1 if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int LastIndexOf(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            for (int i = source.Length - 1; i >= 0; i--) if (predicate(source[i])) return i;
            return -1;
        }

        /// <summary>
        /// Returns the indexes of the elements that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<int> IndexesOf(Predicate<T?> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            List<int> nums = [];
            for (int i = 0; i < source.Length; i++) if (predicate(source[i])) nums.Add(i);
            return nums;
        }
    }
}