namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
public record CloneGeneratorOptions : TreeGeneratorOptions
{
    /// <summary>
    /// Determines if the '[Microsoft.CodeAnalysis.Embedded]' attribute is added to the generated
    /// source code. This attribute is typically used to decorate marker attributes to prevent
    /// duplicates.
    /// </summary>
    public bool EmitEmbeddedAttribute { get; set; }

    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and the <see cref="IsNullableAttribute"/>
    /// nullability helpers are added to the generated code.
    /// </summary>
    public bool EmitNullabilityHelpers { get; set; }
}