namespace Yotei.Tools.CloneGenerator;

// =========================================================
/// <summary>
/// Decorates host types for which a 'Clone()' method will be declared or implemented.
/// <br/> Types that are not interfaces must implement a copy constructor.
/// <br/> Records are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public CloneableAttribute() { }
}