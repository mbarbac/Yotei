#nullable enable

using System;

namespace Yotei.Tools.CloneGenerator
{
    /// <summary>
    /// Used to decorate types for which a 'Clone' method will be generated.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
        Inherited = false,
        AllowMultiple = false
    )]
    public class CloneableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="tag"></param>
        public CloneableAttribute(string? tag = null)
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
