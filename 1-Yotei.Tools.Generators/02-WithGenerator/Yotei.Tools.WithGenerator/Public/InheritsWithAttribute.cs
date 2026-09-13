#nullable enable

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Decorates host types for which their inherited 'With[Name](value)' methods, associated with
/// base properties or fields decorated with a <see cref="WithAttribute"/>-alike attribute, will
/// be redeclared or reimplemented.
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