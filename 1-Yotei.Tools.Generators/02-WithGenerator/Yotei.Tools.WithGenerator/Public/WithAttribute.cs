namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Decorates properties and fields for which the <see langword="with"/> keyword will be emulated
/// by generating 'With[Name](value)' methods that return new instances of the host type where
/// the value of the decorated member is set to the new given one.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// <br/> Record types are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute : Attribute
{
    /// <summary>
    /// If not <see langword="null"/>, then specifies the return type of the generated method.
    /// Otherwise, the type of the host of the decorated member will be used by default.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public Type? ReturnType { get; set; } = null;

    /// <summary>
    /// Determines if the generated method will be a virtual-alike one, or not. If not used, then
    /// the default value of this property is considered to be <see langword="true"/>.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}