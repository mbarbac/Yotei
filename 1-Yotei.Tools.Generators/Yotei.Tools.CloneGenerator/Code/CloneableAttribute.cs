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
    /// If <c>true</c>, instructs the generator not to add a 'virtual', 'abstract' or 'override'
    /// modifier to the generated 'Clone()' method. Otherwise, such modifiers are used unless the
    /// the host type is sealed.
    /// <br/> Note that this setting has not effects when the decorated host is an interface.
    /// <br/> The default value of this property is <c>false</c>.
    /// </summary>
    public bool PreventVirtual { get; set; }

    /// <summary>
    /// If <c>true</c> instructs the generator to add a <see cref="ICloneable"/> interface to the
    /// type, in case it was not already added to it, or to any of its base elements.
    /// <br/> The default value of this property is <c>false</c>.
    /// </summary>
    public bool AddICloneable { get; set; }
}