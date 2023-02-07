namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Decorates test methods and classes whose execution shall be enforced.
/// </summary>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class,
    AllowMultiple = true,
    Inherited = false)]
public class EnforcedAttribute : Attribute { }