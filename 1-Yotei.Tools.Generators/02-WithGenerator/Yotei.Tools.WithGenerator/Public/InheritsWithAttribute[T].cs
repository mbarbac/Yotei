namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc cref="InheritsWithAttribute"/>
/// <br/> The type of the generic argument of the attribute is the return type of the generated
/// methods.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class InheritsWithAttribute<T> : Attribute
{
    /// <summary>
    /// Whether the generated methods are, by default, virtual ones or not. If not used, then the
    /// default value of this property is <see langword="true"/>.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}