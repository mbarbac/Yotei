namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Decorates members of non-record types for which the '<see langword="with"/>' keyword is to be
/// emulated by, in their host type, either declaring (interfaces) or emitting (classes and structs)
/// appropriate '<see langword="With[Name](value)"/>' methods. Only properties and fields are
/// acceptable as members.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute : Attribute
{
    /// <summary>
    /// If not <see langword="null"/>, then the return type of the generated method. Otherwise,
    /// the type of the host of the decorated member is used by default.
    /// </summary>
    public Type? ReturnType { get; set; } = null;

    /// <summary>
    /// Whether the generated method is, by default, a virtual one, or not. If not used, then the
    /// default value of this property is <see langword="true"/>.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}