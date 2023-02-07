namespace Yotei.Tools;

// ========================================================
public static class ArrayExtensions
{
    /// <summary>
    /// Returns a new array with the requested length, adding or removing elements at its tail
    /// as needed. Value of any new added element is set to the given pad one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="len"></param>
    /// <param name="pad"></param>
    /// <returns></returns>
    public static T[] ResizeAtTail<T>(this T[] source, int len, T pad = default!)
    {
        source = source.ThrowIfNull();

        if (len < 0) throw new ArgumentException("Length cannot be less than cero.").WithData(len);

        if (len == 0) return Array.Empty<T>();
        if (len < source.Length) return source.AsSpan(0, len).ToArray();
        else
        {
            var target = GetNewArray(len, pad);
            Array.Copy(source, 0, target, 0, source.Length);
            return target;
        }
    }

    /// <summary>
    /// Returns a new array with the requested length, adding or removing elements at its head
    /// as needed. Value of any new added element is set to the given pad one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="len"></param>
    /// <param name="pad"></param>
    /// <returns></returns>
    public static T[] ResizeAtHead<T>(this T[] source, int len, T pad = default!)
    {
        source = source.ThrowIfNull();

        if (len < 0) throw new ArgumentException("Length cannot be less than cero.").WithData(len);

        if (len == 0) return Array.Empty<T>();
        if (len < source.Length) return source.AsSpan(source.Length - len).ToArray();
        else
        {
            var target = GetNewArray(len, pad);
            Array.Copy(source, 0, target, len - source.Length, source.Length);
            return target;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new array with the given length, where the value of its elements is set to
    /// the given pad one.
    /// </summary>
    static T[] GetNewArray<T>(int len, T pad)
    {
        if (len == 0) return Array.Empty<T>();
        else
        {
            var target = new T[len];
            Array.Fill(target, pad);
            return target;
        }
    }
}