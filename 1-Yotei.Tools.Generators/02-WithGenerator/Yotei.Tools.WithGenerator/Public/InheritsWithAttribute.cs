namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Decorates not-record types for which the inherited <see cref="WithAttribute"/>-decorated members
/// will be redeclared (interfaces) or reimplemented (classes and structs).
/// <br/> This attribute has no effect for decorated members declared at the this type's level.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritsWithAttribute : Attribute
{
    /// <summary>
    /// If not <see langword="null"/>, then the return type of the generated methods. Otherwise,
    /// the type of the host of the decorated member is used by default.
    /// </summary>
    public Type? ReturnType { get; set; } = null;

    /// <summary>
    /// Whether the generated methods are, by default, a virtual one, or not. If not used, then
    /// the default value of this property is <see langword="true"/>.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}