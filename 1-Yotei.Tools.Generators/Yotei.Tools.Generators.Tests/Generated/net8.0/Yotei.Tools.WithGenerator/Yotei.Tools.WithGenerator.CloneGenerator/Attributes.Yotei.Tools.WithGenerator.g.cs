#nullable enable

using System;

namespace Yotei.Tools.WithGenerator
{
    /// <summary>
    /// When used to decorate type members, generates a 'With[name](value)' method for
    /// that member that returns a new instance of the host type where the value of the
    /// decorated member has been replaced by the new given one.
    /// <br/> When used to decorate host types, generates 'With' methods for the decorated
    /// members inherited from their base types and interfaces.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface |
        AttributeTargets.Property | AttributeTargets.Field,
        Inherited = false,
        AllowMultiple = false
    )]
    public class WithGeneratorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="specs">
        /// If not null, the specifications that describe how to obtain a new instance of
        /// the host type, using the '[builder][(arguments)][optionals]' format, where:
        /// <para>
        /// - [builder]: null to only take into consideration the type constructors, or
        /// the name of the builder method(s) to take into consideration. If so, they must
        /// return an object whose type must be compatible with the host one.
        /// </para>
        /// <para>
        /// - [(arguments)]: the comma-separated list of arguments to use by the builders.
        /// If not used, or if it is '(*)', then all builders will be tried. If it is '()',
        /// then only the parameterless ones will be tried. Otherwise, each element shall
        /// follow the '[name][=@|member][!]' format, where:
        /// <br/>- [name]: the actual name of the builder argument, or '*' to indicate that
        /// all arguments shall be taken into consideration.
        /// <br/>- [=@|member]: the source from which to obtain the value of that argument.
        /// If it is not used, the name of a matching member will be used. If '=@', then
        /// the name of the variable for the the enforced member (if any) will be used
        /// instead. Otherwise, the actual name of the member that becomes the source of
        /// that value.
        /// <br/>>- [!]: If used, then a clone of the value will be used instead.
        /// </para>
        /// <para>
        /// - [optionals]: the optional chain of comma-separated specs of the remaining
        /// init/set elements not yet used by the builder. If not used, no optional element
        /// is injected. Otherwise, each follows the '[+|-][*|member][=@][!]' format
        /// where:
        /// <br/>>- [+|-]: determines if it is an include or exclude specification.
        /// <br/>>- [*|member]: if '*', the specification affects to all remaining members,
        /// and any previous ones are erased. Otherwise, the name of the member to use from
        /// the set of remaining ones.
        /// <br/>>- [=@]: if used then the name of the value for the enforced member, if
        /// any, will be used.
        /// <br/>>- [!]: If used, then a clone of the value will be used instead.
        /// </para>
        /// <para>
        /// Some generators permit the use of 'enforced' members to modify their value while
        /// building the new instance of the type to return. If so, the name of the external
        /// variable representing that value is used as needed.
        /// </para>
        /// </param>
        public WithGeneratorAttribute(string? specs = null)
        {
            Specs = specs == null || (specs = specs.Trim()).Length == 0 ? null : specs;
        }
        
        /// <summary>
        /// If not null contains the specifications of the  method to use to generate a
        /// new instance of the host type. If null, then only the available constructors
        /// will be taken into consideration.
        /// </summary>
        public string? Specs { get; }
        
        /// <summary>
        /// If true instructs the generator not to use virtual-alike methods, but rather
        /// regular or new ones. The default value of this setting is false.
        /// </summary>
        public bool PreventVirtual { get; set; }
    }
}
