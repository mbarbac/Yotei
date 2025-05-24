namespace Yotei.Tools;

// ========================================================
public static class IDisposableExtensions
{
    /// <summary>
    /// Throws an exception if this instance has been already disposed. Otherwise this method
    /// is ignored.
    /// </summary>
    /// <param name="instance"></param>
    public static void ThrowIfDisposed(this IDisposableEx instance)
    {
        instance.ThrowWhenNull();
        ObjectDisposedException.ThrowIf(instance.IsDisposed, instance);
    }

    /// <summary>
    /// Throws an exception if this instance is being disposed when this method is invoked.
    /// Otherwise this method is ignored.
    /// </summary>
    /// <param name="instance"></param>
    public static void ThrowOnDisposing(this IDisposableEx instance)
    {
        instance.ThrowWhenNull();
        if (instance.OnDisposing) throw new InvalidOperationException(
            "This instance is being disposed.")
            .WithData(instance);
    }
}