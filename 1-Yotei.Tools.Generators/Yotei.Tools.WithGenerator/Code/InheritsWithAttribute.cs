namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Use to decorate types where the methods of their previously with-decorated members (properties
/// and fields) are to be redeclared or reimplemented, using their original settings.
/// <br/> If there is the need to modify that original settings, reimplement the member.
/// <br/> Not-interface types must implement a private or protected copy constructor.
/// <br/> C# record types are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritWithsAttribute : Attribute { }