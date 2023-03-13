namespace Dev.Tester;

// ========================================================
/// <summary>
/// A holder for a given test method.
/// </summary>
public class MethodHolder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="method"></param>
    public MethodHolder(MethodInfo method)
    {
        Method = method.ThrowIfNull();
        Enforced = method.IsEnforced();

        if (!method.IsValid()) throw new ArgumentException(
            $"Method '{Method.Name}' is not a valid test.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
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
    /// Determines if this method is enforced, or not.
    /// </summary>
    public bool Enforced { get; set; }
}

// ========================================================
internal static class MethodHolderExtensions
{
    /// <summary>
    /// Determines if this Method is enforced, or not.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsEnforced(this MethodInfo method)
    {
        method = method.ThrowIfNull();

        var ats = method.GetAttributes(Tester.EnforcedAttribute, true);
        return ats.Length != 0;
    }

    /// <summary>
    /// Determines if this method is a valid test, or not.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsValid(this MethodInfo method)
    {
        method = method.ThrowIfNull();

        var pars = method.GetParameters();
        if (pars.Length != 0) return false;

        var ats = method.GetAttributes(Tester.FactAttribute, true);
        if (ats.Length == 0) return false;

        return true;
    }
}
