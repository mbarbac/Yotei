namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a known explicit inclusion or exclusion request.
/// </summary>
public class Request
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    public Request(string? assemblyName, string? typeName, string? methodName)
    {
        AssemblyName = assemblyName?.NotNullNotEmpty();
        TypeName = typeName?.NotNullNotEmpty();
        MethodName = methodName?.NotNullNotEmpty();

        if (assemblyName == null &&
            typeName == null &&
            methodName == null)
            throw new ArgumentException("Cannot initialize an empty instance.");
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"({AssemblyName ?? "-"}, {TypeName ?? "-"}, {MethodName ?? "-"})";

    /// <summary>
    /// The assembly name, or null if this filter is not used.
    /// </summary>
    public string? AssemblyName { get; }

    /// <summary>
    /// The type name, or null if this filter is not used.
    /// <br/> Type names that end with this value are considered a match.
    /// </summary>
    public string? TypeName { get; }

    /// <summary>
    /// The method name, or null if this filter is not used.
    /// </summary>
    public string? MethodName { get; }
}