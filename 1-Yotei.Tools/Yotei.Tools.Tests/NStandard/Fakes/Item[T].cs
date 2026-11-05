namespace Yotei.Tools.Tests;

// ========================================================
/// <summary>
/// A reference type for testing purposes.
/// </summary>
public class Item<T>(T value) : ICloneable
{
    public T Value = value;

    public override string ToString() => Value?.ToString() ?? "";

    public Item<T> Clone() => new(Value);
    object ICloneable.Clone() => Clone();
}