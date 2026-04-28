namespace Experimental;

// ========================================================
public static class StringBuilderPool
{
    private const int MAXPOOLSIZE = 4; // Max number of objects in the pool
    private const int MAXITEMSIZE = 4096; // Prevent keeping big builders

    static readonly StringBuilder[] Items = new StringBuilder[MAXPOOLSIZE];

    /// <summary>
    /// Clears this pool by removing all kept instances.
    /// </summary>
    public static void Clear() => Array.Clear(Items);

    /// <summary>
    /// Obtains a string builder either from the pool or a new fresh one.
    /// </summary>
    /// <returns></returns>
    public static StringBuilder Rent()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            var sb = Items[i];
            if (sb != null && Interlocked.CompareExchange(ref Items[i]!, null, sb) == sb)
            {
                Debug.WriteLine($"Instance found in pool at position: {i}.");
                return sb;
            }
        }

        Debug.WriteLine($"New instance created.");
        return new StringBuilder();
    }

    /// <summary>
    /// Clears the given string builder instance and returns it to the pool.
    /// </summary>
    /// <param name="sb"></param>
    public static void Return(StringBuilder sb)
    {
        ArgumentNullException.ThrowIfNull(sb);

        sb.Clear();
        if (sb.Capacity > MAXITEMSIZE) return;

        for (int i = 0; i < Items.Length; i++)
        {
            if (Interlocked.CompareExchange(ref Items[i], sb, null) == null)
                return;
        }
    }

    /// <summary>
    /// Clears the given string builder instance and returns it to the pool, returning the string
    /// value it carried before been cleaned.
    /// </summary>
    /// <param name="sb"></param>
    /// <returns></returns>
    public static string ToStringAndReturn(StringBuilder sb)
    {
        Debug.WriteLine($"Returning instance: {sb}.");

        var str = sb.ToString(); Return(sb);
        return str;
    }
}