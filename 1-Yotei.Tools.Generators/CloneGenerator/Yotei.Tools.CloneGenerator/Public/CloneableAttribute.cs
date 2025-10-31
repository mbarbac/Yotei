namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// Used to decorate types for which a <see cref="ICloneable.Clone"/> method will be generated.
/// <br/> Regular types must implement a copy constructor.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute : Attribute
{
    /// <summary>
    /// The return type of the generated method.
    /// <br/> The default value of this setting is the decorated type itself.
    /// </summary>
    public Type ReturnType { get; set; } = null!;

    /// <summary>
    /// Whether the generated method is virtual or not.
    /// <br/> The default value of this setting is considered to be '<c>true</c>'.
    /// </summary>
    public bool UseVirtual { get; set; }
}

// ========================================================
/// <summary>
/// <inheritdoc cref="CloneableAttribute"/>
/// <br/> The generic type argument provides the return type of the generated method.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute<T> : Attribute
{
    /// <summary>
    /// Whether the generated method is virtual or not.
    /// <br/> The default value of this setting is considered to be '<c>true</c>'.
    /// </summary>
    public bool UseVirtual { get; set; }
}