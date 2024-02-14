namespace Experiments.Designs.InheritGenerator;

// ========================================================

// The attribute...
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = true // To inherit from several
)]
// Problem is that when using the attribute, it must be fully constructed, so we cannot specify
// inheritance from a generic one to a generic one. Solution: use a placeholder type, IGeneric,
// whose only purpose in live is to mark where a generic attribute appears, and the use an array
// of names with the names of those generic attributes. It makes more complex getting names and
// comparing types, but is solvable.
public class InheritAttribute<T> : Attribute
{
    public InheritAttribute(params string[] names)
    {
        if (names == null) throw new ArgumentNullException(nameof(names));
        if (names.Length == 0) throw new ArgumentException("Generic names is empty.");
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == null) throw new ArgumentException("Generic names carries null ones.");
            if ((names[i] = names[i].Trim()) == null)
                throw new ArgumentException("Generic names carries empty ones.");
        }
        GenericNames = names;
    }
    public Type Type => typeof(T);
    public string[] GenericNames { get; }
}

// Only purpose in live is to mark generics...
public interface IGeneric { }

// ========================================================

// Base functionality...
public interface IFoo<T>
{
    // Arbitrary property that needs no inheritance...
    int Count { get; }

    // Property that returns an instance of the template interface...
    // Problem: it might not be of a derived one!
    IFoo<T> Legend { get; }

    // Method that returns an instance of the template interface...
    // Problem: it might not be of a derived one!
    IFoo<T> Add(T item);
}

// Base functionality...
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

// ========================================================

// Inheriting an interface...
[Inherit<IFoo<IGeneric>>("T")]
public partial interface IBar<T> { }
partial interface IBar<T> : IFoo<T>
{
    // Why do we have the right to assume is can be casted to IBar?
    new IBar<T> Legend { get; }

    // Well, similar question.
    new IBar<T> Add(T item);
}

// Inheriting an interface and a base class...
[Inherit<IBar<IGeneric>>("T")]
[Inherit<Foo<IGeneric>>("T")]
public partial class Bar<T> { }
partial class Bar<T> : Foo<T>, IBar<T>
{
    IBar<T> IBar<T>.Legend => (IBar<T>)base.Legend;

    public override Bar<T> Add(T item) => (Bar<T>)base.Add(item);
    IBar<T> IBar<T>.Add(T item) => Add(item);
}