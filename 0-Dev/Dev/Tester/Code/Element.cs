namespace Dev.Tester;

// ========================================================
/// <summary>
/// Represents an explicit inclusion or exclusion specification.
/// </summary>
public class Element
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    public Element(string? assemblyName, string? typeName, string? methodName)
    {
        AssemblyName = assemblyName?.NotNullNotEmpty();
        TypeName = typeName?.NotNullNotEmpty();
        MethodName = methodName?.NotNullNotEmpty();

        if (AssemblyName == null &&
            TypeName == null &&
            MethodName == null)
            throw new ArgumentException(
                "Cannot initialize an empty instance.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
        => $"{AssemblyName ?? "-"}, {TypeName ?? "-"}, {MethodName ?? "-"}";

    /// <summary>
    /// The assembly name, including its extension, or null if this filter is not used.
    /// </summary>
    public string? AssemblyName { get; }

    /// <summary>
    /// The type name, or null if this filter is not used. If used, this filter will match
    /// any type whose name ends with the value of this property.
    /// </summary>
    public string? TypeName { get; }

    /// <summary>
    /// The method name, or null if this filter is not used.
    /// </summary>
    public string? MethodName { get; }

    /// <summary>
    /// Determines if this instance matches the given specifications, or not.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public bool Match(Element specs)
    {
        specs = specs.ThrowIfNull();

        var done = false;

        if (specs.AssemblyName != null &&
            AssemblyName != null &&
            string.Compare(AssemblyName, specs.AssemblyName, StringComparison.OrdinalIgnoreCase) == 0)
            done = true;

        if (specs.TypeName != null &&
            TypeName != null &&
            TypeName.EndsWith(specs.TypeName))
            done = true;

        if (specs.MethodName != null &&
            MethodName != null &&
            string.Compare(MethodName, specs.MethodName) == 0)
            done = true;

        return done;
    }

    /// <summary>
    /// Determines if this instance matches the given specifications, or not.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public bool Match(string? assemblyName, string? typeName, string? methodName)
    {
        var specs = new Element(assemblyName, typeName, methodName);
        return Match(specs);
    }
}