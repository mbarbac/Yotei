namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Decorates not-record types for which the 'With[Name](value)' methods inherited from members
/// in base types decorated with <see cref="WithAttribute"/> or <see cref="WithAttribute{T}"/>
/// attributes will be redeclared or reimplemented.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// <br/> Record types are not supported.
/// <br/> The type of the generic argument becomes the return type of the generated method.
/// Derived types must maintain base compatibility.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritsWithAttribute<T> : Attribute
{
    /// <summary>
    /// Determines if the generated method will be a virtual-alike one, or not. If not used, then
    /// the default value of this property is <see langword="true"/>.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}