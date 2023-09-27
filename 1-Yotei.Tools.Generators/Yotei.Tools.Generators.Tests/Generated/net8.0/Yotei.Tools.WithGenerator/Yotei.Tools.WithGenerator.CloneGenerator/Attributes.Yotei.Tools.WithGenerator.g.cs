#nullable enable

using System;

namespace Yotei.Tools.WithGenerator
{
    /// <summary>
    /// When used to decorate type members, generates a 'With[name](value)' method for
    /// that member that returns a new instance of the host type where the value of the
    /// decorated member has been replaced by the new given one.
    /// <para>
    /// When used to decorate host types, generates 'With' methods for the decorated
    /// members inherited from their base types and interfaces.
    /// </para>
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
        /// <param name="tag"></param>
        public WithGeneratorAttribute(string? tag = null)
        {
            Tag = tag == null || (tag = tag.Trim()).Length == 0 ? null : tag;
        }
        
        /// <summary>
        /// If not null describes the method to use to generate a new instance of the
        /// host type. If null, then only the available constructors will be taken into
        /// consideration.
        /// </summary>
        public string? Tag { get; }
        
        /// <summary>
        /// If true instructs the generator not to use virtual-alike methods, but rather
        /// regular or new ones. The default value of this setting is false.
        /// </summary>
        public bool PreventVirtual { get; set; }
    }
}
