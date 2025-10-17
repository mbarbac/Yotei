namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Emulates the '<c>with</c>' keyword for the decorated properties and fields of non-record types
/// for which their respective '<c>With[Name](value)</c>' methods will be declared (interfaces) or
/// implemented (regular classes and structs).
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute : Attribute { }

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute<T> : Attribute { }