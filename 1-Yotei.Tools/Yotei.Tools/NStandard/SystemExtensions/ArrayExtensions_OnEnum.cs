#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static partial class ArrayExtension
{
    /// <summary>
    /// Returns the index of the first ocurrence of any of the given values, or -1 if none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexOfAny(values, comparer);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of any of the given values, or -1 if none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            foreach (var value in values) if (comparer.Equals(item, value)) return i;
        }
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last ocurrence of any of the given values, or -1 if none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int LastIndexOfAny<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.LastIndexOfAny(values, comparer);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of any of the given values, or -1 if none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOfAny<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            foreach (var value in values) if (comparer.Equals(item, value)) return i;
        }
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the ocurrences of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static List<int> IndexesOfAny<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexesOfAny(values, comparer);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOfAny<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        List<int> nums = []; for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            foreach (var value in values) if (comparer.Equals(value, item))
            {
                nums.Add(i);
                break;
            }
        }
        return nums;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.ContainsAny(values, comparer);
    }

    /// <summary>
    /// Determines if the array contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAny(values, comparer) >= 0;
}