namespace Dev.Tester;

// ========================================================
/// <summary>
/// Represents the specification of a test inclusion or exclusion.
/// </summary>
public class Element
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    public Element(
        string assemblyName,
        string? typeName = null,
        string? methodName = null)
    {
        AssemblyName = assemblyName.NotNullNotEmpty();
        TypeName = typeName?.NotNullNotEmpty();
        MethodName = methodName?.NotNullNotEmpty();

        if (MethodName != null &&
            TypeName == null)
            throw new InvalidOperationException(
                "Cannot specify a method name without a type one.")
                .WithData(assemblyName)
                .WithData(typeName)
                .WithData(methodName);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(AssemblyName);
        if (TypeName != null) sb.Append($".{TypeName}");
        if (MethodName != null) sb.Append($".{MethodName}");

        return sb.ToString();
    }

    /// <summary>
    /// The assembly name.
    /// </summary>
    public string AssemblyName { get; }

    /// <summary>
    /// The name of the test class, or null if this request refer to all suitable test classes
    /// in the assembly.
    /// </summary>
    public string? TypeName { get; }

    /// <summary>
    /// The name of the test method, or null if this request refers to all suitable methods in
    /// the test types.
    /// </summary>
    public string? MethodName { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this element matches the given specifications, or not.
    /// Null type or method names are considered an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public bool Match(Element specs)
    {
        specs = specs.ThrowIfNull();

        if (MethodName != null &&
            specs.MethodName != null &&
            string.Compare(MethodName, specs.MethodName) != 0) return false;

        if (TypeName != null &&
            specs.TypeName != null &&
            string.Compare(TypeName, specs.TypeName) != 0) return false;

        return string.Compare(AssemblyName, specs.AssemblyName, ignoreCase: true) == 0;
    }

    /// <summary>
    /// Determines if this element matches the given specifications, or not.
    /// Null type or method names are considered an implicit match.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public bool Match(string assemblyName, string? typeName = null, string? methodName = null)
    {
        var spec = new Element(assemblyName, typeName, methodName);
        return Match(spec);
    }
}