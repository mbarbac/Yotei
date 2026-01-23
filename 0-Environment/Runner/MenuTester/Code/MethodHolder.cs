namespace Runner;

// ========================================================
/// <summary>
/// Represents a test method.
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

        if (!IsValidTest(method)) throw new ArgumentException(
            $"Method '{method.Name}' is not a valid test one.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Name;

    // ----------------------------------------------------

    /// <summary>
    /// The method this instance refers to.
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    /// The name of this method.
    /// </summary>
    public string Name => Method.Name;

    /// <summary>
    /// Determines if this instance is decorated with the <see cref="EnforcedAttribute"/>.
    /// </summary>
    public bool IsEnforced => _IsEnforced ??= HasEnforcedAttribute(Method);
    bool? _IsEnforced;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given method is decorated with the <see cref="EnforcedAttribute"/>.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool HasEnforcedAttribute(MethodInfo method) => method.ThrowWhenNull()
        .GetCustomAttributes(true)
        .Any(static x => x.GetType().Name == nameof(EnforcedAttribute));

    /// <summary>
    /// Determines if the given method is a valid test one, or not.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsValidTest(MethodInfo method)
    {
        method.ThrowWhenNull();

        // Parameter-less method...
        var pars = method.GetParameters();
        if (pars.Length != 0) return false;

        // [Fact] decorated method...
        var attrs = method.GetCustomAttributes(true).Where(static x =>
            x is not null &&
            x.GetType().Name == "FactAttribute");

        return attrs.Any();
    }
}