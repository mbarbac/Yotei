namespace Experiments.Designs.UpCastGenerator;

// UpCast generator is designed to facilitate inheritance of base interfaces and classes when
// the real need is just to upcast, or reimplement, the properties and methods whose original
// types are the base type, and upcast them to the implementing one.

// The base types can be interfaces, classes and records. They can be written the way you want
// and tested they way you need. They require no modification, no attribute - indeed it may
// happen you have not access to the source code.

public interface IFoo<T>
{
    int Count { get; }
    IFoo<T> Legend { get; }
    IFoo<T> Add(T item);
}

public class Foo<T> : IFoo<T>
{
    readonly List<T> items = [];

    public int Count => items.Count;
    public virtual Foo<T> Legend
    {
        get => _Legend;
        init => _Legend = value.ThrowWhenNull();
    }
    Foo<T> _Legend = new();
    IFoo<T> IFoo<T>.Legend => Legend;

    public virtual Foo<T> Add(T item)
    {
        items.Add(item);
        return this;
    }
    IFoo<T> IFoo<T>.Add(T item) => Add(item);
}

// Now, when you want to reimplement these types upcasting their methods and properties to the
// derived type, those derived types need to be decorated with the UpCast attribute.

// Problem: if we want to use it with a generic type the compiler will complain

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = true // To inherit from several...
)]
public class UpCastAttribute : Attribute
{
    public UpCastAttribute(Type type) => Type = type;
    public Type Type { get; }
}

[UpCast(typeof(IFoo<>))]
public class Any<T> { }


