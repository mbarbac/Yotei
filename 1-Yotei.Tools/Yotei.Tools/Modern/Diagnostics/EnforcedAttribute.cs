namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Used to decorate elements whose usage is enforced.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false)]
public class EnforcedAttribute : Attribute { }