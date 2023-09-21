namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Used to decorate test classes and test methods that shall be enforced. This attribute is
/// intended to behave as a filter. Firstly, if it decorates any test class, then only the
/// decorated ones are taken into consideration. Secondly, if it decorates any test method
/// from the remaining classes, then only the decorated ones are executed.
/// </summary>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class,
    AllowMultiple = false,
    Inherited = false)]
public class EnforcedAttribute : Attribute { }