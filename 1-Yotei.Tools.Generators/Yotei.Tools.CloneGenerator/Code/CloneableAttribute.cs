namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// Used to decorate types for which a 'Clone()' method will be declared or implemented, provided
/// there is no explicit declaration or implementation. Note that the <see cref="ICloneable"/>
/// attribute is not added by this generator.
/// <br/> Non-interface types must immplement a copy constructor.
/// <br/> Records are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute : Attribute
{
    /// <summary>
    /// If <c>true</c>, instructs the generator to add a 'virtual', 'abstract' or 'override'
    /// modifier to the generated 'Clone()' method, unless the host type is sealed.
    /// <br/> The default value of this setting is <c>true</c>.
    /// </summary>
    public bool AddVirtual { get; set; } = true;
}