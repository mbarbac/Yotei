namespace Experiments.Designs.InheritGenerator;

// Inherit generator is designed to facilitate inheritance of base interfaces and classes when
// the real need is just to upcast (or reimplement) elements whose original types are the base
// one, and upcast them to the implementing one.

// The base types can be interfaces, classes and records. They can be written the way you want
// and tested they way you need. They require no modifications and no attributes applied to them.
// Indeed, it may even happen you have no access to the source code.

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

// Now, when you want to reimplement these types upcasting their elements to a given derived
// type, you typically have to write a lot of boiler plate code just for upcasting purposes.
// Instead, the idea is to apply to those derived types the Inherit attribute, and let the code
// generator take care of all of that repetitive code in our behalf.

// Since C# 11 we can use generic attributes, ie: Inherit<T>, where T, in our case, can specify
// the base type to inherit from. But there is a problem: suppose that T itself is a generic
// type (as IFoo<T> in the previous example). If this happens, the compiler will complain
// because it expects, for generic attributes, that their types are fully constructed. We will
// get a CS8968 error:

//[Inherit<IFoo<B>] // CS8968: an attribute type argument cannot use type parameters.
//public class Any<A, B> { }

// A possible solution is to use a marker type that the compiler considers fully constructed.
// For instance, we can envision using an interface whose only purpose in live is to serve as
// a placeholder. But then we have the problem of substituting that placeholder with the right
// name of the generic type it refers to (or names in the case where we have several ones). So,
// we add to the attribute a property that precissely holds those names.

// And with all of these we would have ended with something like the following:

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = true // To inherit from several...
)]
public class InheritAttribute<T> : Attribute
{
    public Type Type => typeof(T);
    public string[] GenericNames { get; set; } = [];
}

public interface IGeneric { }

[Inherit<IFoo<IGeneric>>(GenericNames = ["B"])]
public class Other<A, B, C> { }

// Now, this requires introducing another type, the marker one, which opens the door of naming
// problems (potential colisions of IGeneric with any user-defined element) and, in any case,
// the syntax feels not natural. Indeed, it is an ugly one, and even more if 'IFoo' itself uses
// several generic ones.

// So we will discard the generic attribute option. Instead, we are going to specify the type to
// inherit from (the one to upcast) as the first mandatory argument of the attribute:

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = true // To inherit from several...
)]
public class InheritAttribute(Type type) : Attribute
{
    public Type Type { get; } = type;
    public string[] GenericNames { get; set; } = [];
    public bool ChangeProperties { get; set; }
}

// So that we can apply the attribute in a more natural way:

[Inherit(typeof(IFoo<>), GenericNames = ["B"])]
public class Any<A, B, C> { }

public interface IFoo<T, K> { }

[Inherit(typeof(IFoo<,>), GenericNames = ["B", "A"])]
public class Another<A, B, C> { }

// We still have the problem of matching the generic attributes from the inherited type to the
// corresponding ones in the derived type, for which we will use the optional string of names,
// as before. In the example we can see that we have reversed the derived order when matching
// them with the base ones.

// We have two main cases to consider. Let's start by using it with an interface:

[Inherit(typeof(IFoo<>), GenericNames = ["T"], ChangeProperties = true)]
public partial interface IBar<T> { }

// then the following code is generated:

partial interface IBar<T> : IFoo<T>
{
    new IBar<T> Legend { get; }
    new IBar<T> Add(T item);
}

// Inherit will validate that IFoo<T> is not specified in the interface list implemented by the
// derived type, and if not, adds it in the code generated. If it was already specified, then
// this step is skipped.

// The 'ChangeProperties' argument governs if the code generator will upcast properties or not.
// Reasons are better understood when discussing the next case (applyin the attribute to classes
// or records), so we postpone the discussion for a while.

// Now we have the case of using the Inherit attribute with a class (or a record). Inherit can be
// applied several times to a given derived class, each specifying a different base type to
// upcast from (at most one base class, but as many interfaces as needed).

[Inherit(typeof(IBar<>), GenericNames = ["T"], ChangeProperties = true)]
[Inherit(typeof(Foo<>), GenericNames = ["T"], ChangeProperties = true)]
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

// Inherit will also prevent re-implementing something the user has coded manually. It the
// receiving type already has a method (with the same name and compatible parameters), or it has
// a property with the same name, then Inherit will skip them.

// In addition, if a method or property has been generated by another Yotei generator, Inherit
// will not mess with it. It is assumed the original generator provides the mechanisms to use
// in inheritance or implementing scenarios.

// BACKLOG:
//
// - Include an exclusion mechanism for methods. Ie: a string list of those to exclude. If
//   any element contains rounded brackets, then what is inside the brackets govern the possible
//   method override to exclude: name() only parameterless method, name(int) only the method
//   with one parameter that is an int, name(int, string) ..., and so forth.
//
// - Include an exclusion mechanism for properties. Ie: a string list of those to exclude. If
//   any element contains square brackets, then what is inside the brackets govern the possible
//   property override to exclude: name[] is an error as there are not empty indexers, name[int]
//   only the indexer with one parameter that is an int, name[int, string] ..., and so forth.