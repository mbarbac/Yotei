# Dynamic Parser
Yotei's **Dynamic Parser** is used to extract the logic expressed in a
Dynamic Lambda Expression (DLE).

## Introduction

Regular lambda expressions do not support the use of a dynamic argument
and, if you try to do so, the compiler will complain:

      void Foo(Expression<Func<dynamic,...>> func);

This is very unfortunate because, by using `dynamic`, we can write any
arbitrary logic without any complain from the compiler. So, the idea is
to take a `Func<dynamic, object>` delegate and emulate what otherwise the
compiler would have done.

`DynamicParser` is an static class whose `Parse(Func<dynamic, object> func)`
returns an instance that has two main properties: `Argument`, that describes
the dynamic argument of the given delegate, and `Result`, that constains the
last node in the tree of dynamic operations binded against the dynamic
argument. You can walk up the tree to obtain the full set of operations the
parser has binded - aka: the complete logic written by the user.

      Func<dynamic, object> func = (x) => ...;
      var parser = DynamicParse.Parse(func);
      var argument = parser.Argument;
      var node = parser.Result;

`Result` is an instance of a class derived from `DynamicNode`, and there are
classes for the almost all possible operations, such as parsing a constant,
binary and unary operations, method invocations, setters and getters, and
so forth. Just a few operations cannot be parsed, as describes in the
'Limitations' section below.

## Examples

Let's suppose we have a method whose aim is to write the WHERE portion of
a SELECT statement, and we want the user to be able to write any filter
it may be needed, including filtering by database columns for which our
business classes don't have corresponding properties (error checking removed
for brevity):

      string GetWhere(Func<dynamic, object> func) {
         var node = DynamicParser.Parse(func).Result;
         var str = GetSql(node);
         return $"WHERE {str}";
      }

In this case, we are assuming we have a `GetSql(...)` method that can take
a tree of dynamic nodes, and translate that tree into whatever SQL dialect
we are using.

If we want to obtain the James Bond's record, we can write something like:

      var result = GetWhere(x => x.Id == "007");
      result: WHERE x.Id = '007'

You can also obtain that constant value from an 'external' method, by calling
it in the expression: `x => x.Id == MyMethod()`, but the value from that method
will be obtained _when parsing the expression_, not when it might be used.

You can also bind 'virtual' methods, as method invocations on the dynamic
argument, as in `x => x.JoinDate.Month()`. Is then the job of our SQL parser
to interpret that node and traslate it into whatever SQL expression is
appropriate.

Of course, `Id` is a fairly common property and chances are high there are for
it both a property in out business class and a database column with that name.
But let's now suppose our external CRM uses a `UserLevel` column in our
database for which we have not a corresponding property in our business
model. If we want to obtain the users for a given level and above, we can
write:

      var result = GetWhere(x => x.UserLevel >= 7);
      result: WHERE x.UserLevel >= 7

`DynamicParser` can parse much more complex dynamic lambda expressions. Some
examples are:

      x => x.Alpha.Beta 
      x => x.Id = x.Id + "_Whatever";
      x => x.Indexed[x.Index + x.Beta];
      x => x[7, "other"] = null!;
      x => x(x.Argument);
      x => x.MyMethod(x.Alpha, null, "other");
      x => x(x.Alpha = x.Beta)(x.Beta = x.Alpha);
      x => x.Alpha<int, string>(x.Beta);
      x => x.Alpha(x.Alpha = x.Alpha(x.Beta = x.Alpha)[x.Alpha = x.Beta]);
      x => x.Alpha == (x.Alpha = x.Beta);
      x => x.Alpha && x.Beta;
      x => x.Alpha += x.Beta;
      x => x.Alpha(x.Cast<string>(x.Alpha = x.Beta));

... and so forth. Please see the testing project for more examples.

## How it works

In essence, by executing the dynamic lambda expression and intercepting each
of the dynamic invocations, translating them into the appropriate
`DynamicNode` instances.

How? Each `DynamicNode` instance inherits from the DLR (Dynamic Language
Runtime) `DynamicObject` class by producing and ad-hoc `DynamicMetaObject`
instance that, when binding dynamic operations, is the one in charge of
taking note of them and, more important, _to keep the ball rolling_ in the
proper way.

The complication here is that the DLR uses, for performance reasons, quite
a sophisticated cache mechanism based on the type of the call site and the
type of its arguments. But `DynamicParser` cannot use it because it needs
to produce brand new results each time it parses a new dynamic lambda
expression.

So, to prevent this, `DynamicParser` keeps an internal version for each
result it produces, so that only the last one is considered valid. When
the parser sees that the DLR is using a cached result, it invalidates that
cache entry forcing the DLR to create a new rule. This invalidation is
achieved by creating an ad-hoc `BindingRestrictions` instance that
validates the internal version and, if needed, updates it to a new one.

## Considerations

The invalidation mechanism circumvents the DLR cache rules, so it may
DLR performance if it used for other purposes along with `DynamicParser`.
This should be a very rare case, though.

## Limitations

### Unary Post-Increment and Post-Decrement operators

Although their 'Pre' counterparts are parsed correctly, `DynamicParse` fail
when parsing post-Increment and post-Decrement operators. When parsed, they
are converted by the DLR into a setter of the element on which they
operate:

      x => x.Alpha++;
      result: (x.Alpha = (Increment x.Alpha))

but then the original `x.Alpha` node is the one used by the DLR, and not
this 'setter' one. Although I have an explanation about why this behavior
happends, I have not found (yet) a way to prevent it.

### Conversion operations

Something similar happens with conversion operations, as in
`x => (string)x.Alpha`. 

As this is quite a common operation, the workaround I'm using is a virtual
`Cast` method, that we later parse into the appropriate conversion when
needed, as in:

      x => x.Cast(typeof(int), x.Alpha);
      x => x.Cast<int>(x.Alpha);

Parsing this expressions produces a `DynamicNodeMethod` instance with the
appropriate generic and regular arguments, that I can later use as needed.