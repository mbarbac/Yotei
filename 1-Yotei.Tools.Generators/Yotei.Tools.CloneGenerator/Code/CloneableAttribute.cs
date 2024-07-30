namespace Yotei.Tools.CloneGenerator;

// =========================================================
/// <summary>
/// Decorates host types for which a 'Clone()' method will be declared or implemented.
/// <br/> Non-interface host types must implement a copy constructor.
/// <br/> Records are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute : Attribute
{
    /// <summary>
    /// If <c>true</c> instructs the generator to not produce virtual-alike methods.
    /// </summary>
    public bool PreventVirtual { get; set; }
}