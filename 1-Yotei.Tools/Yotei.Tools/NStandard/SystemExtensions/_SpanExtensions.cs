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
static partial class SpanExtensions
{
    extension<T>(Span<T> source)
    {
    }
}
/*  IsEmpty
 *  Clone
 *  
 *  IndexOf
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