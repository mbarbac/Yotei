namespace Runner;

// ========================================================
/// <summary>
/// Represents a holder for a test class.
/// </summary>
public class TypeHolder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    public TypeHolder(Type type)
    {
        Type = type.ThrowWhenNull();
        IsEnforced = HasEnforcedAttribute(type);

        if (!IsValidTestClass(type))
            throw new ArgumentException($"Type '{Type.Name}' is not a valid test class.");
    }

    /// <inheritdoc/>
    public override string ToString() => Type.Name;

    /// <summary>
    /// The class this instance refers to.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The full name of this type, including its namespace.
    /// </summary>
    public string FullName => Type.FullName!;

    /// <summary>
    /// The assembly qualified full name of this type.
    /// </summary>
    public string AssemblyQualifiedName => Type.AssemblyQualifiedName!;

    /// <summary>
    /// Whether this instance shall be considered as an enforced one, or not.
    /// </summary>
    public bool IsEnforced { get; set; }

    /// <summary>
    /// The collection of method holders in this instance.
    /// </summary>
    public MethodHolderList MethodHolders { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is decorated with the <see cref="EnforcedAttribute"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HasEnforcedAttribute(Type type)
    {
        return type.ThrowWhenNull()
            .GetCustomAttributes(true)
            .Any(x => x.GetType().Name == nameof(EnforcedAttribute));
    }

    /// <summary>
    /// Determines if the given type is a valid test class.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsValidTestClass(Type type)
    {
        type.ThrowWhenNull();
        return type.IsClass;
    }
}