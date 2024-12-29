namespace Runner;

// ========================================================
/// <summary>
/// Represents a explicit inclusion or exclusion request.
/// </summary>
public class Request
{
    /// <summary>
    /// Initializes a new instance.
    /// <br/> The three arguments, used as filters, must not be null simultaneously.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    public Request(string? assemblyName, string? typeName, string? methodName)
    {
        AssemblyName = assemblyName?.NotNullNotEmpty();
        TypeName = typeName?.NotNullNotEmpty();
        MethodName = methodName?.NotNullNotEmpty();

        if (assemblyName is null &&
            typeName is null &&
            methodName is null)
            throw new ArgumentException("Cannot initialize an empty instance.");
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"({AssemblyName ?? "-"}, {TypeName ?? "-"}, {MethodName ?? "-"})";

    /// <summary>
    /// The assembly name, or <c>null</c> if this filter is not used.
    /// </summary>
    public string? AssemblyName { get; }

    /// <summary>
    /// The type name, or <c>null</c> if this filter is not used.
    /// <br/> Type names that ends with the value of this property are considered a match.
    /// </summary>
    public string? TypeName { get; }

    /// <summary>
    /// The method name, or <c>null</c> if this filter is not used.
    /// </summary>
    public string? MethodName { get; }
}