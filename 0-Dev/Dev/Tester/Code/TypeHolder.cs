namespace Dev.Tester;

// ========================================================
/// <summary>
/// A holder for a given test class.
/// </summary>
public class TypeHolder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    public TypeHolder(Type type)
    {
        Type = type.ThrowIfNull();
        Enforced = type.IsEnforced();

        if (!Type.IsValid()) throw new ArgumentException(
            $"Method '{Type.Name}' is not a valid test.");
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
    /// Determines if this type is enforced, or not.
    /// </summary>
    public bool Enforced { get; }

    /// <summary>
    /// The collection of method holders in this instance.
    /// </summary>
    public MethodHolderList MethodHolders { get; } = new();

    /// <summary>
    /// Populates the collection of method holders.
    /// </summary>
    public void Populate()
    {
        var methods = Type.GetMethods(Tester.Flags);
        foreach (var method in methods)
        {
            if (!method.IsValid()) continue;

            var holder = new MethodHolder(method);
            MethodHolders.Add(holder);
        }
    }

    /// <summary>
    /// Ensures that only the decorated elements are taken into consideration, if any is
    /// decorated.
    /// </summary>
    public void EnsureEnforced()
    {
        if (Enforced)
            foreach (var holder in MethodHolders) holder.Enforced = true;
    }

    /// <summary>
    /// Purges not enforced elements.
    /// </summary>
    public void PurgeNotEnforced()
    {
        var items = MethodHolders.Where(x => !x.Enforced).ToList();
        foreach (var item in items) MethodHolders.Remove(item);
    }
}

// ========================================================
internal static class TypeHolderExtensions
{
    /// <summary>
    /// Determines if this type is enforced, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsEnforced(this Type type)
    {
        type = type.ThrowIfNull();

        var ats = type.GetAttributes(Tester.EnforcedAttribute, true);
        return ats.Length != 0;
    }

    /// <summary>
    /// Determines if this type is a valid test, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsValid(this Type type)
    {
        type = type.ThrowIfNull();

        if (!type.IsClass) return false;
        return true;
    }
}
