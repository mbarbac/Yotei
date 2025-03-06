namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// Used to decorate types for which a 'Clone()' method will be declared or implemented, provided
/// there is no explicit declaration or implementation.
/// </br> Non-interface types must immplement a copy constructor.
/// </br> Records are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute : Attribute
{
    /// <summary>
    /// If <c>true</c> instructs the generator to not produce virtual-alike methods.
    /// <br/> The default <c>false</c> value is just ignored.
    /// </summary>
    public bool PreventVirtual { get; set; }

    /// <summary>
    /// If <c>true</c> instructs the generator to add the <see cref="ICloneable"/> interface
    /// to the decorated type, if such is needed.
    /// <br/> The default <c>false</c> value is just ignored.
    /// </summary>
    public bool AddICloneable { get; set; }
}