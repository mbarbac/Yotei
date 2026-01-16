namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Used to decorate test classes and methods whose execution will be enforced, so that the not
/// decorated ones will not execute. It is assumed that the test execution framework understand
/// this capability.
/// </summary>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class,
    AllowMultiple = false,
    Inherited = false)]
public class EnforcedAttribute : Attribute { }