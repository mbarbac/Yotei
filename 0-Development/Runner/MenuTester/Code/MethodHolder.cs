namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a holder for a known method.
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

        if (!IsValid(method)) throw new ArgumentException(
            $"Method '{Method.Name}' is not a valid test method.");

        IsEnforced = HasEnforcedAttribute;
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

    /// <summary>
    /// Determines if the underlying type is decorated with an enforced attribute, or not.
    /// </summary>
    public bool HasEnforcedAttribute => Method
        .GetCustomAttributes(true)
        .Any(x => x.GetType().Name == nameof(EnforcedAttribute));

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given method is a valid one, or not.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsValid(MethodInfo method)
    {
        method = method.ThrowWhenNull();

        // Parameter-less method...
        var pars = method.GetParameters();
        if (pars.Length != 0) return false;

        // Decorated method...
        var attrs = method.GetCustomAttributes(typeof(FactAttribute), true);
        if (attrs.Length == 0) return false;

        return true;
    }
}