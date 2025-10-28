namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Used to decorate non-record types for which the inherited '<c>With[Name](value)</c>' methods
/// of the <see cref="WithAttribute"/>-decorated members will be redeclated (for interfaces) or
/// reimplemented (for regular classes and structs).
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritWithsAttribute : Attribute
{
    /// <summary>
    /// The return type of the generated methods.
    /// <br/> The default value of this setting is the type of the decorated host.
    /// </summary>
    public Type ReturnType { get; set; } = null!;

    /// <summary>
    /// Whether the generated methods are virtual or not.
    /// <br/> The default value of this setting is considered to be '<c>true</c>'.
    /// </summary>
    public bool UseVirtual { get; set; }
}

// ========================================================
/// <summary>
/// <inheritdoc cref="InheritWithsAttribute"/>
/// <br/> The generic type argument provides the return type of the generated methods.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritWithsAttribute<T> : Attribute
{
    /// <summary>
    /// Whether the generated methods are virtual or not.
    /// <br/> The default value of this setting is considered to be '<c>true</c>'.
    /// </summary>
    public bool UseVirtual { get; set; }
}