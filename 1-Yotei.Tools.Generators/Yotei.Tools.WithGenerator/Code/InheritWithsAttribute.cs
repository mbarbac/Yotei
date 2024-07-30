namespace Yotei.Tools.WithGenerator;

// =========================================================
/// <summary>
/// Decorates non-record host types where the 'With[name](value)' methods they inherit are either
/// redeclared or reimplemented.
/// <br/> Non-interface host types must implement a copy constructor.
/// <br/> Records are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritWithsAttribute : Attribute
{
    /// <summary>
    /// If <c>true</c> instructs the generator to not produce virtual-alike methods.
    /// </summary>
    public bool PreventVirtual { get; set; }
}