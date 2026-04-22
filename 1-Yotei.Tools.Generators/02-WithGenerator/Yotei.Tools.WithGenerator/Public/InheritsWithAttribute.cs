#nullable enable
namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Decorates not-record host types for which their inherit 'With[Name](value)' methods, associated
/// with properties or fields decorated with the <see cref="WithAttribute"/> attribute in any base
/// type or implemented interface, will be redeclared or reimplemented.
/// <br/> Note that this attribute only operates on inherited members.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// <br/> Records are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritsWithAttribute : Attribute
{
    /// <summary>
    /// If not null, specifies the return type of the generated method. Otherwise, the decorated
    /// host type will be used by default.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public Type? ReturnType { get; set; } = null;

    /// <summary>
    /// Determines if the generated method will be a virtual-alike one, or not. If not used, the
    /// default value of this property is true.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}