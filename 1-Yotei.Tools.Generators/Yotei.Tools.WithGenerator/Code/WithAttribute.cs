﻿namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// When used to decorate members of a type, properties and fields, to emulate the 'with' keyword
/// for non-record types, by generating a 'With[name](bvalue)' method for each member decorated
/// with this attribute.
/// <br/> Records are not supported.
/// <br/> The '<see cref="PreventVirtual"/>' setting is used to prevent the generation of virtual
/// alike methods.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute : Attribute
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