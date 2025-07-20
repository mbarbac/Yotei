namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Use to decorate types for which the methods of their previously with-decorated members
/// (properties and fields) are to be redeclared or reimplemented, provided such methods are
/// not explicitly declared or implemented.
/// <br/> Not-interface types must implement a private or protected copy constructor.
/// <br/> C# record types are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritWithsAttribute : Attribute { }