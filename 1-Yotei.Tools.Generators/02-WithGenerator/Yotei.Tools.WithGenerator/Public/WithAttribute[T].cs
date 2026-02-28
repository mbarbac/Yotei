namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Decorates properties and fields for which the <see langword="with"/> keyword will be emulated
/// by generating 'With[Name](value)' methods that return new instances of the host type where
/// the value of the decorated member is set to the new given one.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// <br/> Record types are not supported.
/// <br/> The type of the generic argument becomes the return type of the generated method.
/// Derived types must maintain base compatibility.
/// </summary>
/// <typeparam name="T"></typeparam>
public class WithAttribute<T> : Attribute
{
    /// <summary>
    /// Determines if the generated method will be a virtual-alike one, or not. If not used, then
    /// the default value of this property is considered to be <see langword="true"/>.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}