namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Used to decorate types whose previously decorated members (properties and fields) shall be
/// redeclared or reimplemented.
/// <br/> Records are not supported.
/// <br/> The '<see cref="PreventVirtual"/>' setting is used to prevent the generation of virtual
/// alike methods.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class InheritWithsAttribute : Attribute
{
    /// <summary>
    /// If <c>true</c>, instructs the generator not to add a 'virtual', 'abstract' or 'override'
    /// modifier to the generated 'With()' methods. Otherwise, such modifiers are used unless the
    /// the host type is sealed.
    /// <br/> Note that this setting has not effects when the decorated host is an interface.
    /// <br/> The default value of this property is <c>false</c>.
    /// </summary>
    public bool PreventVirtual { get; set; }
}