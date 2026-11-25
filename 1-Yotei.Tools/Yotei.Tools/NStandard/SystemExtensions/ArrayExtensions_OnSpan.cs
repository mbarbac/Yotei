#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static partial class ArrayExtension
{
#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the contents of this array are the same as ones in the given sequence.
    /// </summary>
    /// <remarks>
    /// To mimic standard behavior, no 'where T : IEquatable{T}' restriction is imposed. This
    /// is seems to be very specific for arrays, other collection do have this restriction.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool SequenceEqual<T>(this T[] source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.SequenceEqual(values, comparer);
    }
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the contents of this array are the same as ones in the given sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool SequenceEqual<T>(
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        var xlen = source.Length;
        var ylen = values.Length;
        if (xlen != ylen) return false;

        for (int i = 0; i < xlen; i++)
        {
            var xvalue = source[i];
            var yvalue = values[i];
            var same = comparer.Equals(xvalue, yvalue);
            if (!same) return false;
        }
        return true;
    }
#endif
}