#pragma warning disable CS0436
namespace Yotei.Tools.Tests;

// ========================================================
public class Element<T>([AllowNull] T value) : ICloneable
{
    public override string ToString() => Value?.ToString() ?? "";
    [MaybeNull] public T Value { get; } = value!;

    public Element<T> Clone() => new(Value);
    object ICloneable.Clone() => Clone();
}

// ========================================================
public struct ElementComparer<T> : IEqualityComparer<Element<T>>
{
    public ElementComparer() { }

    public IEqualityComparer<T> InnerComparer
    {
        get;
        set => field = value.ThrowWhenNull();
    }
    = EqualityComparer<T>.Default;

    public bool Equals(Element<T>? x, Element<T>? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return InnerComparer.Equals(x.Value, y.Value);
    }

    public int GetHashCode([DisallowNull] Element<T> obj)
    {
        throw new NotImplementedException();
    }
}