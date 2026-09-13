#nullable enable

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Decorates properties and field of not-record host types for which the <see langword="with"/>
/// will be emulated by generating a 'With[Name](value)' method that returns a new instance of the
/// host type where the value of the decorated member is set to the given value.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// <br/> Records are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute : Attribute
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