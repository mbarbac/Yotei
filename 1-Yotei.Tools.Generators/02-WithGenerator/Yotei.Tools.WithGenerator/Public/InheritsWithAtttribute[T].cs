#nullable enable

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Decorates host types for which their inherited 'With[Name](value)' methods, associated with
/// base properties or fields decorated with a <see cref="WithAttribute"/>-alike attribute, will
/// be redeclared or reimplemented.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// <br/> The generic type of the attribute becomes the return type of the generated methods.
/// Derived types must maintain base compatibility.
/// <br/> Records are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritsWithAttribute<T> : Attribute
{
    /// <summary>
    /// Determines if the generated method will be a virtual-alike one, or not. If not used, the
    /// default value of this property is true.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}