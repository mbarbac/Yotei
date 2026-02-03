namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc cref="WithAttribute"/>
/// <br/> The type of the generic argument of the attribute is the return type of the generated
/// method.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute<T> : Attribute
{
    /// <summary>
    /// Whether the generated method is, by default, a virtual one or not. If not used, then the
    /// default value of this property is <see langword="true"/>.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}