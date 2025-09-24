namespace Yotei.Tools.Diagnostics;

// ========================================================
/// <summary>
/// Identifies the test classes and methods whose execution is enforced so that the tests defined
/// in other types are not executed, if the test execution framework honors this capability.
/// </summary>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class,
    AllowMultiple = false,
    Inherited = false)]
public class EnforcedAttribute : Attribute { }