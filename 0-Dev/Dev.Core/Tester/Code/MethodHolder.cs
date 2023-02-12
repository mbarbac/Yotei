namespace Dev.Tester;

// ========================================================
/// <summary>
/// Represents a holder for a given method.
/// </summary>
internal class MethodHolder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="method"></param>
    public MethodHolder(MethodInfo method)
    {
        MethodInfo = method.ThrowIfNull();
        IsEnforced = method.IsEnforced();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// The method this instance refers to.
    /// </summary>
    public MethodInfo MethodInfo { get; }

    /// <summary>
    /// The name of the method this instance refers to.
    /// </summary>
    public string Name => MethodInfo.Name;

    /// <summary>
    /// Whether this instance is enforced, or not.
    /// </summary>
    public bool IsEnforced { get; set; } = false;
}

// ========================================================
internal static class MethodExtensions
{
    /// <summary>
    /// Determines if the given type is a valid test one, or not.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static bool IsValidTest(this MethodInfo method, [NotNullWhen(false)] out Exception? ex)
    {
        method = method.ThrowIfNull();

        var pars = method.GetParameters();
        if (pars.Length != 0)
        {
            ex = new ArgumentException("Method is not a parameterless one.").WithData(method);
            return false;
        }

        var ats = method.GetAttributes(MenuTester.FactAttribute, true);
        if (ats.Length == 0)
        {
            ex = new ArgumentException("Method has not the [Fact] attribute.").WithData(method);
            return false;
        }

        ex = null;
        return true;
    }

    /// <summary>
    /// Determines if the given method is decorated with the <see cref="EnforcedAttribute"/>
    /// attribute, or not.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsEnforced(this MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);

        var ats = method.GetAttributes(MenuTester.EnforcedAttribute, true);
        return ats.Length != 0;
    }
}