#pragma warning disable CS8618

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Used to decorate types for which their inherit 'With[Name](value)' methods will be redeclared
/// or reimplemented.
/// <br/> Not-interface types must implement a protected or private copy constructor.
/// <br/> C# record types are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritWithsAttribute : Attribute
{
    /// <summary>
    /// If used then its value specifies the return type of the generated methods. If not used,
    /// then the return type is the host type of the decorated member.
    /// </summary>
    public Type ReturnType { get; set; }

    /// <summary>
    /// If used specifies whether the generated methods are virtual-alike ones, or not. If not
    /// used, then the generator tries to generate virtual-alike ones.
    /// </summary>
    public bool VirtualMethod { get; set; }
}