namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Used to decorate non-record types for which the inherited '<c>With[Name](value)</c>' methods
/// of the <see cref="WithAttribute"/>-decorated members will be redeclated (for interfaces) or
/// reimplemented (for regular classes and structs).
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritWithsAttribute : Attribute { }

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritWithsAttribute<T> : Attribute { }