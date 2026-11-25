#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static partial class ArrayExtension
{
    extension<T>(T[] source)
    {
        /// <summary>
        /// Gets a value indicating whether the source sequence is empty.
        /// </summary>
        public bool IsEmpty => source.Length == 0;

        // ------------------------------------------------

        /// <summary>
        /// Obtains a typed enumerator for the contents of the array.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetTypedEnumerator() => ((IEnumerable<T>)source).GetEnumerator();

        // ------------------------------------------------

        /// <summary>
        /// Returns a clone of this instance where its elements are either the original ones, or
        /// their deep clones, as requested.
        /// </summary>
        /// <param name="deep"></param>
        /// <returns></returns>
        public T[] Clone(bool deep)
        {
            if (deep) // Deep cloning requested...
            {
                var len = source.Length;
                var result = new T[len];

                for (int i = 0; i < len; i++) result[i] = source[i].TryClone()!;
                return result;
            }
            else // Standard case...
            {
                return (T[])source.Clone();
            }
        }
    }
}