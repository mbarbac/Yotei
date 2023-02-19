# Yotei Tools

This libray contains common helpers and utilities for the Yotei framework and
related projects. It has 4 main areas:
1. Diagnostics
1. Exceptions
1. System
1. Miscelanea

## Diagnostics

### EnforcedAttribute

The enforced attribute is used to decorate the test classes and test methods
that are to be executed in isolation by the `Dev.Tester` utility,
which is part of the `Dev` project that can be found in GitHub.
If any class or any method is decorated, then only the decorated ones are
executed.

## Exceptions

The library contains the following exceptions:

1. `DuplicateException`, which is thrown when a duplicate object or value is found.
1. `EmptyException`, which is thrown when an empty object or value is found.
1. `NotFoundException`, which is thrown when an objec or value is not found.
1. `UnexpectedException`, which is thrown when the execution path reaches an unexpected situation.

## System

The library contains utilities and extensions for:

1. `Arrays`.
1. `Chars`.
1. `Exceptions`.
1. `Strings`.
1. `Types`.

In addition, it contains the following utilities:

### Clone extensions

The `TryClone()` extension method tries to clone the object on which it was invoked
so that it uses a parameterless `Clone()` method, if available, or otherwise returns
the original object.

### ConvertTo extensions

The `TryConvertTo()` extension method tries to convert the instance on which it was
invoked into an instance of a given type, with several overloads to prevent boxing.
The `ConvertTo()` extension method returns the converted instance, or throws an
exception if that conversion is not possible.

These extensions use a custom converter that, in addition of the obvious conversions, takes
into consideration any explicit or implicit conversion operator the source class may have
declared.

### EasyName extensions

The `EasyName()` family of extension methods returns the C#-alike name of the
given object, it being a type instance, a constructor, a method, a property or a field
one.

### Locale

The `Locale` class combines both a `CultureInfo` instance and a string
`CompareOptions` value in a single immutable object, so that facilitating the
work with culture-sensitive objects.

### NoDuplicatesList

The `NoDuplicatesList` class represents a list-alike collection of not null and
not-duplicated elements, where both the nullability and the comparison capabilities can be
customized on demand.

### Sketch extensions

The `Sketch()` extension method provides an alternate string representation of
the object on which it was invoked, in case its type does not override the `ToString()`
method. It also takes into consideration if the object is a dictionary, a list or an
enumerable one, and if needed, is uses the "shape" of the object (its properties and fields)
to produce that representation.

## Miscelanea

The miscelanea section contains to **classes**, `DayDate` and `ClockTime`
that were usefull when the new `TimeOnly` and `DateOnly` types were
not available.

The library keeps them for compatibility reasons, and also for testing ones because they
are both reference types but with value semantics.