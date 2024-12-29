namespace Runner;

// ========================================================
/// <summary>
/// Represents a holder for a test method
/// </summary>
public class MethodHolder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="method"></param>
    public MethodHolder(MethodInfo method)
    {
        Method = method.ThrowWhenNull();
        IsEnforced = HasEnforcedAttribute(method);

        if (!IsValidTestMethod(method))
            throw new ArgumentException($"Type '{Name}' is not a valid test method.");
    }

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <summary>
    /// The method this instance refers to.
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    /// The name of this method.
    /// </summary>
    public string Name => Method.Name;

    /// <summary>
    /// Whether this instance shall be considered as an enforced one, or not.
    /// </summary>
    public bool IsEnforced { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given method is decorated with the <see cref="EnforcedAttribute"/>.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool HasEnforcedAttribute(MethodInfo method)
    {
        return method.ThrowWhenNull()
            .GetCustomAttributes(true)
            .Any(x => x.GetType().Name == nameof(EnforcedAttribute));
    }

    /// <summary>
    /// Determines if the given type is a valid test method.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsValidTestMethod(MethodInfo method)
    {
        method.ThrowWhenNull();

        // Parameter-less method...
        var pars = method.GetParameters();
        if (pars.Length != 0) return false;

        // Decorated method...
        var attrs = method.GetCustomAttributes(typeof(FactAttribute), true);
        if (attrs.Length == 0) return false;

        return true;
    }
}