namespace Dev.Tester;

// ========================================================
/// <summary>
/// Represents a holder for a given type.
/// </summary>
internal class TypeHolder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    public TypeHolder(Type type)
    {
        Type = type.ThrowIfNull();
        IsEnforced = type.IsEnforced();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// The type this instance refers to.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The name of the type this instance refers to.
    /// </summary>
    public string Name => Type.Name;

    /// <summary>
    /// Whether this instance is enforced, or not.
    /// </summary>
    public bool IsEnforced { get; set; } = false;

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child holders maintained by this instance.
    /// </summary>
    public MethodHolderList MethodHolders { get; } = new();

    /// <summary>
    /// Populates the child holders of this instance using the given specifications.
    /// </summary>
    /// <param name="methodName"></param>
    public void Populate(string? methodName = null)
    {
        methodName = methodName?.NotNullNotEmpty();

        // All methods...
        if (methodName == null)
        {
            var methods = Type.GetMethods(RunTester.Flags);
            foreach (var method in methods)
            {
                if (!method.IsValidTest(out _)) continue;
                MethodHolders.Add(method);
            }
        }

        // Requested one...
        else
        {
            var method = Type.GetMethods(RunTester.Flags)
                .Where(x => x.Name == methodName)
                .SingleOrDefault();

            if (method == null) throw new NotFoundException(
                "Requested method not found.")
                .WithData(methodName);

            if (!method.IsValidTest(out var ex)) throw ex;
            MethodHolders.Add(method);
        }
    }

    /// <summary>
    /// Purges the child holders of this instance using the given specifications.
    /// </summary>
    /// <param name="methodName"></param>
    public void Purge(string? methodName = null)
    {
        methodName = methodName?.NotNullNotEmpty();

        // All methods...
        if (methodName == null)
        {
            MethodHolders.Clear();
        }

        // Requested one...
        else
        {
            var holder = MethodHolders.Find(methodName);
            if (holder != null) MethodHolders.Remove(holder);
        }
    }
}

// ========================================================
internal static class TypeExtensions
{
    /// <summary>
    /// Determines if the given type is a valid test one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static bool IsValidTest(this Type type, [NotNullWhen(false)] out Exception? ex)
    {
        type = type.ThrowIfNull();

        if (type.IsClass)
        {
            var methods = type.GetMethods(RunTester.Flags);
            var any = methods.Any(x => x.IsValidTest(out _));

            ex = null;
            return any;
        }
        else
        {
            ex = new ArgumentException("Type is not a class.").WithData(type);
            return false;
        }
    }

    /// <summary>
    /// Determines if the given type is decorated with the <see cref="EnforcedAttribute"/>
    /// attribute, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsEnforced(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var ats = type.GetAttributes(RunTester.EnforcedAttribute, true);
        return ats.Length != 0;
    }
}