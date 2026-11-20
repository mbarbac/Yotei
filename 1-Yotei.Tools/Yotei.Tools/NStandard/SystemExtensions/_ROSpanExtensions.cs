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
static partial class ReadOnlySpanExtensions
{
    extension<T>(ReadOnlySpan<T> source)
    {
        /// <summary>
        /// Returns the index of the first ocurrence of the given value, or -1 if any.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int IndexOf(T value, IEqualityComparer<T> comparer)
        {
            comparer.ThrowWhenNull();
            return source.IndexOf(x => comparer.Equals(x, value));
        }

        /// <summary>
        /// Returns the index of the first value in the span that matches the given predicate,
        /// or -1 if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int IndexOf(Predicate<T> predicate)
        {
            predicate.ThrowWhenNull();

            for (int i = 0; i < source.Length; i++) if (predicate(source[i])) return i;
            return -1;
        }

        // ------------------------------------------------
    }
}
/* 
 *  LastIndexOf
 *  IndexesOf
 *  Contains
 *  StartsWith
 *  EndsWith
 *  
 *  TryFind
 *  TryFindLast
 *  TryFindAll
 *  
 *  TrimStart
 *  TrimEnd
 *  Trim
 *  
 *  IndexOfAny
 *  LastIndexOfAny
 *  IndexesOfAny
 *  ContainsAny
 *  StartsWithAny
 *  EndsWithAny
 *  
 *  IndexOfAnyExcept
 *  LastIndexOfAnyExcept
 *  IndexesOfAnyExcept
 *  ContainsAnyExcept
 *  
 *  Add(value)
 *  AddRange(span)
 *  AddRange(sequence)
 *  Insert(value)
 *  InsertRange(span)
 *  InsertRange(sequence)
 *  
 *  RemoveAt(index)
 *  RemoveRange(index, count)
 *  Remove(value)
 *  RemoveLast(value)
 *  RemoveAll(value)
 *  Remove(predicate)
 *  RemoveLast(predicate)
 *  RemoveAll(predicate)
 *  Clear()
 */