namespace Yotei.Tools;

// ========================================================
public static class StringBuilderPoolExtensions
{
    readonly static StringBuilderPool DefaultPool = new();

    extension(StringBuilder)
    {
        /// <summary>
        /// Gets the default string builder pool.
        /// </summary>
        public static StringBuilderPool Pool => DefaultPool;
    }
}

// ========================================================
/// <summary>
/// Represents a pool of string builders for reuse.
/// </summary>
public class StringBuilderPool
{
    private readonly Stack<StringBuilder> Items = [];

    /// <summary>
    /// The period between pruning runs.
    /// </summary>
    public TimeSpan PruneDelay
    {
        get;
        set => field = value > TimeSpan.Zero ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Must be greater than zero.");
    }
    = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Used to limit the number of builders in the pool.
    /// </summary>
    public int MaxPoolSize
    {
        get;
        set => field = value > 0 ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Must be greater than zero.");
    }
    = 31;

    // ----------------------------------------------------

    DateTime LastPrune = DateTime.UtcNow;

    /// <summary>
    /// Tries to prune the pool if the prune delay has elapsed.
    /// This method assumes it runs under a lock.
    /// </summary>
    void TryPrune()
    {
        if (Items.Count == 0) return;

        var ini = LastPrune;
        var now = LastPrune = DateTime.UtcNow;
        if ((now - ini) < PruneDelay) return;

        while (Items.Count > 0) Items.Pop();
    }

    /// <summary>
    /// Returns a valid builder instance.
    /// </summary>
    /// <returns></returns>
    public StringBuilder Rent()
    {
        lock (Items)
        {
            TryPrune();

            if (Items.Count == 0) return new StringBuilder();
            else
            {
                var sb = Items.Pop(); sb.Clear();
                return sb;
            }
        }
    }

    /// <summary>
    /// Returns a valid builder instance initialized with the given string.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public StringBuilder Rent(string value)
    {
        var sb = Rent(); if (value is not null && value.Length > 0) sb.Append(value);
        return sb;
    }

    /// <summary>
    /// Returns the builder to the pool. The caller shall not use the builder any longer.
    /// <br/> By default, unless <paramref name="create"/> is false, the string value of the
    /// builder is returned. If it is false, then an empty string is returned.
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="create"></param>
    public string Return(StringBuilder sb, bool create = true)
    {
        lock (Items)
        {
            var str = create ? sb.ToString() : string.Empty;

            TryPrune();
            if (Items.Count < MaxPoolSize) Items.Push(sb);

            return str;
        }
    }
}