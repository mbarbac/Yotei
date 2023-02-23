# Yotei MemberWith Source Generator

Source code generator for the MemberWith attribute.

`Records` in C# provide very useful things. Among them, the ability to use the `with`
keyword to obtain a clone of an instance modifying the value of the members used in
that expression. But it cannot be used with classes nor with structs, which is very
unfortunate as we often have hierarchies we cannot modify.

When a member of a type is decorated with the `[MemberWith]` attribute, the generator
will generate either a `WithXXX()` method declaration (for interfaces), or a method
implementation(for classes or structs), where the `XXX` part of the method name is
just the name of the member (it being either a property or a field).

### Example

Let's suppose we have the following original decorated interface:

      public partial interface IFoo
      {
         [MemberWith]
         string Name { get; }
      }

and its concrete class:

      public partial class Foo : IFoo
      {
         [MemberWith]
         public string Name { get; init; } = default!;
      }

then, we can write code as the following:

      var source = new Foo() { Name = "Anthony" };
      var target = source.WithName("Berta");

### Caveats

It is not, of course, a complete emulation of the `with` keyword for classes and
structs, but it will suffice in many scenarios.