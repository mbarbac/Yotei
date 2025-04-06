namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// When used to decorate members of a type, emulates the 'with' keyword for non-record types,
/// such as regular classes and interfaces, by generating a 'With[name](bvalue)' method for each
/// member (properties or fields) decorated with this attribute. For obvious reasons, records are
/// not supported.
/// <br/><br/>
/// When used to decorate types, the '<see cref="InheritMembers"/>' property is used to determine
/// if the decorated type shall redeclare or reimplement the inherited members that were decorated
/// in its base types or interfaces. If not, then this type-level attribute is just ignored.
/// Otherwise, the value its '<see cref="PreventVirtual"/>' setting is used if the attribute
/// applied to a member has not that value set.
/// </summary>
public class WithAttribute : Attribute
{
    /// <summary>
    /// Used to determine if the decorated type shall redeclare or reimplement the inherited that
    /// were decorated in its base types or interfaces. If the decorated element is not a type,
    /// then this setting is ignored.
    /// </summary>
    public bool InheritMembers { get; set; }

    /// <summary>
    /// If <c>true</c>, instructs the generator not to add a 'virtual', 'abstract' or 'override'
    /// modifier to the generated 'With()' methods. Otherwise, such modifiers are used unless the
    /// the host type is sealed.
    /// <br/> Note that this setting has not effects when the decorated host is an interface.
    /// <br/> The default value of this property is <c>false</c>.
    /// </summary>
    public bool PreventVirtual { get; set; }
}