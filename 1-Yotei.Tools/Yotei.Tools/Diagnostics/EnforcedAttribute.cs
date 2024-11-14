namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Used to decorate test classes and test methods whose execution is to be restricted to the
/// decorated ones only.
/// </summary>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class,
    AllowMultiple = false,
    Inherited = false)]
public class EnforcedAttribute : Attribute { }