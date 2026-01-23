namespace Runner;

// ========================================================
/// <summary>
/// Represents an explicit inclusion or exclusion request.
/// </summary>
public class Request
{
    /// <summary>
    /// Initializes a new instance with the given filters, which can be null to be ignored. If a
    /// filter ends with '*' then any element that starts with the given specification will be
    /// considered a match. Otherwise, only exact matches are then considered.
    /// <br/> Type and method names are case sensitive. Assemblies are case insensitive.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    public Request(string? assemblyName, string? typeName, string? methodName)
    {
        AssemblyName = assemblyName?.NotNullNotEmpty(true);
        TypeName = typeName?.NotNullNotEmpty(true);
        MethodName = methodName?.NotNullNotEmpty(true);

        if (assemblyName is null &&
            typeName is null &&
            methodName is null)
            throw new ArgumentException("Cannot initialize an empty instance.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
        => $"({AssemblyName ?? "-"}.{TypeName ?? "-"}.{MethodName ?? "-"})";

    // ----------------------------------------------------

    /// <summary>
    /// The assembly name, or '<c>null</c>' if this filter is not used.
    /// </summary>
    public string? AssemblyName { get; }

    /// <summary>
    /// The type name, or '<c>null</c>' if this filter is not used.
    /// </summary>
    public string? TypeName { get; }

    /// <summary>
    /// The method name, or '<c>null</c>' if this filter is not used.
    /// </summary>
    public string? MethodName { get; }
}