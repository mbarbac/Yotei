namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// Decorates types for which a <see cref="ICloneable.Clone"/> method will be generated.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// <br/> Record types are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute : Attribute
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