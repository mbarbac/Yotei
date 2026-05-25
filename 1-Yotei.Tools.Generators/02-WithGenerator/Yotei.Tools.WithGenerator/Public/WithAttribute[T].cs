#nullable enable

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Decorates properties and field of not-record host types for which the <see langword="with"/>
/// will be emulated by generating a 'With[Name](value)' method that returns a new instance of the
/// host type where the value of the decorated member is set to the given value.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// <br/> The generic type of the attribute becomes the return type of the generated method.
/// Derived types must maintain base compatibility.
/// <br/> Records are not supported.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute<T> : Attribute
{
    /// <summary>
    /// Determines if the generated method will be a virtual-alike one, or not. If not used, the
    /// default value of this property is true.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}