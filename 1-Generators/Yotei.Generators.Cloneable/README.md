# Yotei Cloneable Source Generator

Yotei Source Code Generator for Cloneable attributes.

The aim of this library is to decorate types with the `[CloneableType]` attribute
so that an `Clone()` method declaration (for interfaces) or implementation (for
classes and structs) is generated automatically.

## Code Generation

For interfaces, the generator just adds a new `Clone()` method declaration, whose
return type is the type of the interface.

For classes ans structs, the generator will add a `Clone()` method implementation
whose return type is the class or struct where it is generated. If there was an
existing (inherited) `Clone()` method, then the generated one will have the `new`
modifier.

Then, the generator strategy is to find a suitable constructor to use. It first
tries to a **_regular constructor_** with one or more arguments, and tries to
match the _candidate_ members by both name (case insensitive) and by type. By
default, all public and protected members (properties and fields) are considered
as potential candidate ones, although this can be changed using the attributes
explained below.

If a constructor is found with at least one match, but not matching all the candidate
members, then that constructor is used injecting the value of the remaining members
as part of the initialization of the new instance.

        public partial class Foo
        {
            public Foo Clone()
            {
                var item = new Foo(..., ...)
                {
                    BarMember = this.BarMember,
                    ...
                };
                return item;
            }
        }

If no regular constructor is found, then it tries to find a **_copy constructor_**
that takes an instance of the class being generated, or if not, then an instance of
an interface the type implements. This is an explicit design decision to support
the common scenario of cloning being defined in an interface and in its implementation
class:

      public interface IFoo { ... }

      public partial class Foo : IFoo
      {
         public Foo(IFoo source) { ... }
         ...
      }

Finally, it tries to find an **_empty constructor_** (a parameterless one), and the
generator will use it injecting the candidate members, as in:

      new Foo() { ..., ... };

All these constructors have to be declared by the class or struct being generated,
not taking into consideration any that would be defined in base types. Finally, if no
suitable constructor is found, then an error is reported.

## Attributes

This package permits customizing the code generated both at type and member levels.

### CloneableType attribute

By default, `[CloneableType]` adds the `ICloneable` interface to the type being
decorated. You can prevent this using its `AddICloneable` property, as in:

      [CloneableType(PreventAddICloneable = false)]
      public interface IFoo { ... }

In addition, also be default, the generator will take into consideration all public
and protected members for cloning purposes. You can prevent this by using its
`ExplicitMode` property, which indicates that only the members explicitly selected
shall be taken into consideration (note that this property is only used for
classes and structs, and ignored for interfaces):

      [CloneableType(ExplicitMode = true)]
      public class Foo { ... }

### CloneableMember attribute

This attribute is used to decorate both those properties and fields for which a
specific configuration is needed for cloning purposes.

If a member is decorated with this attribute, then it indicates that it must be
considered for cloning purposes - when the type's `ExplicitMode` is set to `true`,
otherwise is ignored. For instance, the following code indicated that the `FirstName`
property shall be used for cloning purposes, but not the `LastName` one:

      [CloneableType(ExplicitMode = true)]
      public class Foo
      {
         [CloneableMember] public string FirstName { get; set; } = default!;
         public string LastName { get; set; } = default!;
      }

In addition, its `Deep` property controls what value to obtain to inject into a
suitable constructor. If it is `true`, a deep copy is obtained. Otherwise, a
shallow copy is injected instead:

      [CloneableMember(Deep = true)] public Bar BarMember { get; set; } = default!;

