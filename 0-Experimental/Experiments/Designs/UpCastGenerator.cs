namespace Experiments.Designs.UpCastGenerator;

// UpCast generator is designed to facilitate inheritance of base interfaces and classes when
// the real need is just to upcast, or reimplement, elements whose original types are the base
// one, and upcast them to the implementing one.

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

// Now, when you want to reimplement these types upcasting their elements to the derived type,
// those derived types need to be decorated with the UpCast attribute. We can try a generic
// attribute:

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = true // To inherit from several...
)]
public class UpCastAttribute<T> : Attribute
{
    public Type Type => typeof(T);
    public string[] GenericNames { get; set; } = [];
}

// But there is a problem: when the attribute is used, its generic argument must be fully
// constructed (aka: it cannot be itself a generic one). So we cannot use it, as such, to upcast
// from a generic base type to a generic derived one:

//[UpCast<IFoo<B>] // CS8968: an attribute type argument cannot use type parameters.
//public class Any<A, B> { }

// A possible solution is to use a market type that the compiler considers fully constructed,
// for instance an interface whose only purpose in live is to serve as a placeholder. But then
// we have the problem of identifying what is the right name to use with it (or names in the
// case where we have several one). 

public interface IGeneric { }

[UpCast<IFoo<IGeneric>>(GenericNames = ["B"])]
public class Other<A, B, C> { }

// Now, this requires introducing another type, the marker one, which opens the door of naming
// problems (potential colisions) and, in any case, the syntax feels not natural. Indeed, it is
// an ugly one, and even more if 'IFoo' itself uses several generic ones. So we discard the
// generic attribute option, and instead we are going to specify the type to inherit (upcast)
// as the first mandatory argument of the attribute:

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = true // To inherit from several...
)]
public class UpCastAttribute(Type type) : Attribute
{
    public Type Type { get; } = type;
    public string[] GenericNames { get; set; } = [];
    public bool ExplicitProperties { get; set; }
}

[UpCast(typeof(IFoo<>), GenericNames = ["B"])]
public class Any<A, B, C> { }

public interface IFoo<T, K> { }

[UpCast(typeof(IFoo<,>), GenericNames = ["B", "A"])]
public class Another<A, B, C> { }

// We still have the problem of matching the generic attributes from the inherited type to the
// corresponding ones in the derived type, for which we will use the optional string of names,
// as before. In the example we can see that we have reversed the derived order when matching
// them with the base ones.

// UpCast will then reimplement the matching elements. Let's start by using it with an interface:

[UpCast(typeof(IFoo<>), GenericNames = ["T"])]
public partial interface IBar<T> { }

// then the following code is generated:

partial interface IBar<T> : IFoo<T>
{
    new IBar<T> Legend { get; }
    new IBar<T> Add(T item);
}

// UpCast will validate that IFoo<T> is not specified in the interface list implemented by the
// derived type, and if not, adds it in the code generated. If it was already specified, then
// this step is skipped.

// Now we have the case of using the UpCast attribute with a class (or a record). UpCast can be
// applied several times to a given derived class, each specifying a different base type to
// upcast from (at most one base class, but as many interfaces as needed).

[UpCast(typeof(IBar<>), GenericNames = ["T"])]
[UpCast(typeof(Foo<>), GenericNames = ["T"], ExplicitProperties = true)]
public partial class Bar<T> { }

// and then the following code is generated:

partial class Bar<T> : Foo<T>, IBar<T>
{
    //public override Bar<T> Legend // CS1715: type must be Foo<T> to match overriden memeber
    //{
    //    get => (Bar<T>)base.Legend;
    //    set => base.Legend = (Foo<T>)value;
    //}
    IBar<T> IBar<T>.Legend => (IBar<T>)base.Legend;

    public new Bar<T> Legend
    {
        get => (Bar<T>)base.Legend;
        init => base.Legend = (Foo<T>)value;
    }

    public override Bar<T> Add(T item) => (Bar<T>)base.Add(item);
    IBar<T> IBar<T>.Add(T item) => Add(item);
}

// It happens that, starting with C# 9, covariant properties are accepted but only if they are
// readonly. So, by default, we are not going to upcast properties unless such is explicitly
// requested, and if so, we will upcast with a 'new' decorator. Note that this parameter is
// only use with base classes (or records), not with interfaces.

// UpCast will also prevent re-implementing something the user has coded manually. It the
// receiving type already has a method (with the same name and compatible parameters), or it has
// a property with the same name, then UpCast will skip them.

// In addition, if a method or property has been generated by another Yotei generator, UpCast
// will not mess with it. It is assumed the original generator provides the mechanisms to use
// in inheritance or implementing scenarios.
