namespace Experiments.Mixins;

// INTRODUCTION.
// This is a design document for mixins and traits, understood as follows:
// - Mixins: refer to contexts where code is imported using inheritance-alike mechanisms.
// - Traits: refer to contexts where code is imported using composition-alike mechanisms.

// SCENARIO.
// The base scenario is the core list-alike collection, and the immutable (frozen) collection
// built using that core one. Both are implemented through interfaces and base classes derived
// from them, as follows:

public interface ICoreList<K, T>
{
    int Add(T item);
}

public class CoreList<K, T> : ICoreList<K, T>
{
    readonly List<T> Items = [];
    public int Add(T item) { Items.Add(item); return 1; }
}

public interface IFrozenList<K, T>
{
    IFrozenList<K, T> Add(T item);
}

public class FrozenList<K, T> : IFrozenList<K, T>
{
    protected virtual CoreList<K, T> Items { get; } = new();
    public virtual FrozenList<K, T> Add(T item) { Items.Add(item); return this; }
    IFrozenList<K, T> IFrozenList<K, T>.Add(T item) => Add(item);
}

// The IPerson interface as out T-type, with keys of int-type. 

public interface IPerson
{
    int Id { get; }
    string? Name { get; }
}

public class Person : IPerson
{
    public int Id { get; init; }
    public string? Name { get; set; }
}

// USING MIXINS.
// Now we want to implement a collection builder (based on the CoreList one, and a frozen
// collection (based on the FrozenList one) for IPerson elements, but such a way that by using
// code generation, the new classes inherit from the given ones.


[AttributeUsage(AttributeTargets.All)]
public class MixinAttribute<IFace> : Attribute { }

[AttributeUsage(AttributeTargets.All)]
public class MixinAttribute<IFace, TType> : Attribute { }


// CASE: Applied to an interface.
// Only interfaces can be used as the generic, otherwise an error.

[Mixin<IFrozenList<int, IPerson>>]
public partial interface IMyList
{ }
partial interface IMyList : IFrozenList<int, IPerson> // Added if needed...
{
    new IMyList Add(IPerson item); // Using 'new'...
}

// CASE: Applied to class.
// Only classes can be used as the generic, otherwise an error.
// Firstly, we implement the builder class:

public class MyListBuilder : CoreList<int, IPerson> { }

// And now the class itself:

[Mixin<IMyList, FrozenList<int, IPerson>>]
public partial class MyList
{
    protected override MyListBuilder Items { get; } = new();
}
partial class MyList : FrozenList<int, IPerson>, IMyList // Added if needed...
{
    public override MyList Add(IPerson item) => (MyList)base.Add(item);
    IMyList IMyList.Add(IPerson item) => Add(item);
}