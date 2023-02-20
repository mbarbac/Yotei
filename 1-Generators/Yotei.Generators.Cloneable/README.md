# Yotei Cloneable Source Generator

Yotei Source Code Generator for Cloneable attributes.

The aim of this library is to decorate types with the `[CloneableType]` attribute
so that an `Clone()` method declaration (for interfaces) or implementation (for
classes and structs) is generated.

## Code Generation

If there was an existing (inherited) `Clone()` method, the generated one will
have the `new` modifier.

For implementation purposes on classes and structs, the generator tries to find a
suitable constructor it can use.

It first tries to find a **_copy_** constructor that either takes an instance of
the class or struct being generates, or an instance of a base class or interface.
This is an explicit design decision to permit using copy constructors in the
following very common scenario

      public interface IFoo { ... }

      public partial class Foo : IFoo
      {
         public Foo(IFoo source) { ... }
         ...
      }

If no copy constructor is available, it then tries to find a **_constructor with
matching arguments_**, where these argument must match with the _candidate_ members by
both name (case insensitive) and type. By default, all public and protected members
are considered as candidate ones, although this can be changed by using the attributes
explained below.

If a constructor is found with at least one match, but not matching all the candidate
members, then that constructor is used injecting the value of the remaining members
as part of the initialization of the new instance.

Finally, it tries to find an **_empty_** (parameterless) constructor, and the
generator will use it injecting the candidate members, as in:

      new Foo() { ..., ... };

If no suitable constructor is found, then an error is reported.

## Attributes

This package permits customizing the code generated both at type and member levels.

### CloneableType attribute

By default, `[CloneableType]` adds the `ICloneable` interface to the type being
decorated. You can prevent this using its `PreventAddICloneable` property, as in:

      [CloneableType(PreventAddICloneable = true)]
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

