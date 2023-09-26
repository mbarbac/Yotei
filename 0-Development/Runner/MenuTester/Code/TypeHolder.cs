namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a holder for a known type.
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

        if (!IsValid(type)) throw new ArgumentException(
            $"Type '{Type.Name}' is not a valid test class.");

        IsEnforced = HasEnforcedAttribute;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
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
    /// Determines if the underlying type is decorated with an enforced attribute, or not.
    /// </summary>
    public bool HasEnforcedAttribute
        => Type.GetCustomAttributes(typeof(EnforcedAttribute), true).Any();

    /// <summary>
    /// The collection of method holders in this instance.
    /// </summary>
    public MethodHolderList MethodHolders { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a valid one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsValid(Type type)
    {
        type = type.ThrowWhenNull();
        return type.IsClass;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to populate this instance with its test methods.
    /// </summary>
    public void Populate()
    {
        var flags =
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic;

        foreach (var method in Type.GetMethods(flags))
        {
            if (!MethodHolder.IsValid(method)) continue;

            if (MethodHolders.Find(method) == null)
            {
                var holder = new MethodHolder(method);
                MethodHolders.Add(holder);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to purge the methods in the given request.
    /// </summary>
    /// <param name="item"></param>
    public void PurgeExcludes(Request item)
    {
        if (item.MethodName != null)
        {
            foreach (var holder in MethodHolders.ToList())
            {
                if (string.Compare(item.MethodName, holder.Name) == 0)
                    MethodHolders.Remove(holder);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance carries any enforced methods.
    /// </summary>
    /// <returns></returns>
    public bool HasEnforcedMethods()
    {
        return MethodHolders.Any(x => x.IsEnforced);
    }

    /// <summary>
    /// Invoked to purge not-enforced methods, if any was enforced.
    /// </summary>
    public void PurgeNotEnforcedMethods()
    {
        foreach (var holder in MethodHolders.ToList())
        {
            if (!holder.IsEnforced) MethodHolders.Remove(holder);
        }
    }
}