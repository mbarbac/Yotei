namespace Experimental.SystemEx;

// ========================================================
/// <summary>
/// Provides a thread-safe replacement for <see cref="Random"/>.
/// </summary>
public class RandomEx
{
    static readonly Random _Global = new();
    readonly ThreadLocal<Random> _Local = default!;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    [SuppressMessage("", "IDE0017")]
    public RandomEx()
    {
        _Local = new();
        _Local.Value = new(_Global.Next());
    }

    /// <inheritdoc cref="Random.Next()"/>
    public int Next() => _Local.Value!.Next();

    /// <inheritdoc cref="Random.Next(int)"/>
    public int Next(int maxValue) => _Local.Value!.Next(maxValue);

    /// <inheritdoc cref="Random.Next(int, int)"/>
    public int Next(int minValue, int maxValue) => _Local.Value!.Next(minValue, maxValue);

    /// <inheritdoc cref="Random.NextBytes(byte[])"/>
    public void NextBytes(byte[] buffer) => _Local.Value!.NextBytes(buffer);

    /// <inheritdoc cref="Random.NextBytes(Span{byte})"/>
    public void NextBytes(Span<byte> buffer) => _Local.Value!.NextBytes(buffer);

    /// <inheritdoc cref="Random.NextDouble()"/>
    public double NextDouble() => _Local.Value!.NextDouble();

    /// <inheritdoc cref="Random.NextInt64()"/>
    public long NextInt64() => _Local.Value!.NextInt64();

    /// <inheritdoc cref="Random.NextInt64(long)"/>
    public long NextInt64(long maxValue) => _Local.Value!.NextInt64(maxValue);

    /// <inheritdoc cref="Random.NextInt64(long, long)"/>
    public long NextInt64(long minValue, long maxValue) => _Local.Value!.NextInt64(minValue, maxValue);

    /// <inheritdoc cref="Random.NextSingle()"/>
    public float NextSingle() => _Local.Value!.NextSingle();
}