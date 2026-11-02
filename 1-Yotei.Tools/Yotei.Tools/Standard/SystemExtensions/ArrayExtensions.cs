#if NETSTANDARD2_0
namespace Yotei.Tools.Std20;
#pragma warning disable IDE0079
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static class ArrayExtensions
{
    /// <summary>
    /// Returns a typed enumerator for the given array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerator<T> GetTypedEnumerator<T>(
        this T[] source)
        => ((IEnumerable<T>)source).GetEnumerator();

    /// <summary>
    /// Returns a clone of this array where its elements are either the original ones, or cloned
    /// if deep cloning was requested.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="deep"></param>
    /// <returns></returns>
    public static T[] Clone<T>(this T[] source, bool deep)
    {
        source.ThrowWhenNull();

        var len = source.Length;
        if (len > 0 && deep)
        {
            var target = new T[len];
            for (int i = 0; i < len; i++) target[i] = source[i].TryClone()!;
            return target;
        }
        else return (T[])source.Clone();
    }
}