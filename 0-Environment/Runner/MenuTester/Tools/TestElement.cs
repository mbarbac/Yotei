namespace Runner;

// ========================================================
/// <summary>
/// Represents a fully qualified test method.
/// </summary>
public record TestElement : IEquatable<TestElement>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="method"></param>
    public TestElement(MethodInfo method) => Method = method.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{AssemblyName}: {TypeFullName}.{MethodName}";

    // ----------------------------------------------------

    /// <summary>
    /// The assembly the test method belongs to.
    /// </summary>
    public Assembly Assembly => Type.Assembly;

    /// <summary>
    /// The type the test method belongs to.
    /// </summary>
    public Type Type => Method.DeclaringType!;

    /// <summary>
    /// The actual test method this instance refers to.
    /// </summary>
    public MethodInfo Method { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The name of the assembly the test method belongs to.
    /// </summary>
    public string AssemblyName => Assembly.GetName().Name
        ?? throw new InvalidOperationException("Assembly name is null.").WithData(Assembly);

    /// <summary>
    /// The name of the type the test method belongs to.
    /// </summary>
    public string TypeName => Type.Name;

    /// <summary>
    /// The fully qualified name of the type the test method belongs to, including its namespace
    /// but not its assembly.
    /// </summary>
    public string TypeFullName => Type.FullName
        ?? throw new InvalidOperationException("Declaring ype name is null.").WithData(Assembly);

    /// <summary>
    /// The name of the test method.
    /// </summary>
    public string MethodName => Method.Name;

    /// <summary>
    /// The full name of the test method.
    /// </summary>
    public string MethodFullName => $"{TypeFullName}.{MethodName}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(TestElement? other) => Method.Equals(other);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Method.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given method is a valid test one, or not.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsValid(MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);

        // Must be a parameter-less method...
        var pars = method.GetParameters();
        if (pars.Length != 0) return false;

        // Must be a [Fact] decorated method...
        var ats = method.GetCustomAttributes(false).Where(static x =>
            x is not null &&
            x.GetType().Name == "FactAttribute");

        if (!ats.Any()) return false;

        // Its declaring type must exist and be a class...
        if (method.DeclaringType is null) return false;
        if (!method.DeclaringType.IsClass) return false;

        // Valid method...
        return true;
    }
}