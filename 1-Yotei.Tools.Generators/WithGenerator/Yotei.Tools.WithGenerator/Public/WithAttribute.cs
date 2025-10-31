namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Emulates the '<c>with</c>' keyword for the decorated properties and fields of non-record types
/// for which their respective '<c>With[Name](value)</c>' methods will be declared (interfaces) or
/// implemented (regular classes and structs).
/// <br/> Regular types must implement a copy constructor.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute : Attribute
{
    /// <summary>
    /// The return type of the generated methods.
    /// <br/> The default value of this setting is the type of the host of the decorated member.
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
/// <inheritdoc cref="WithAttribute"/>
/// <br/> The generic type argument provides the return type of the generated methods.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute<T> : Attribute
{
    /// <summary>
    /// Whether the generated methods are virtual or not.
    /// <br/> The default value of this setting is considered to be '<c>true</c>'.
    /// </summary>
    public bool UseVirtual { get; set; }
}