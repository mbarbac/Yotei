namespace Runner;

// ========================================================
/// <summary>
/// Represents a test type.
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

        if (!IsValidTest(type)) throw new ArgumentException(
            $"Type '{type.Name}' is not a valid test one.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Name;

    // ----------------------------------------------------

    /// <summary>
    /// The class this instance refers to.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The name of this Type.
    /// </summary>
    public string Name => Type.Name;

    /// <summary>
    /// The full name of this type, including its namespace.
    /// </summary>
    public string FullName => Type.FullName!;

    /// <summary>
    /// The assembly qualified full name of this type.
    /// </summary>
    public string AssemblyQualifiedName => Type.AssemblyQualifiedName!;

    /// <summary>
    /// Determines if this instance is decorated with the <see cref="EnforcedAttribute"/>.
    /// </summary>
    public bool IsEnforced => _IsEnforced ??= HasEnforcedAttribute(Type);
    bool? _IsEnforced;

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
    public static bool HasEnforcedAttribute(Type type) => type.ThrowWhenNull()
        .GetCustomAttributes(true)
        .Any(static x => x.GetType().Name == nameof(EnforcedAttribute));

    /// <summary>
    /// Determines if the given type is a valid test one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsValidTest(Type type)
    {
        // Type must be  class...
        return type.ThrowWhenNull().IsClass;
    }
}