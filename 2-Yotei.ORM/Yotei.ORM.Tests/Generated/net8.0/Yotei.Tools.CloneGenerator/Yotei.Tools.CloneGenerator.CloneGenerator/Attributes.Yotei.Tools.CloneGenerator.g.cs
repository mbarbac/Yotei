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
        /// Initializes a new instance with the given optional specifications.
        /// </summary>
        /// <param name="specs"></param>
        public CloneableAttribute(string? specs = null)
        {
            Specs = specs == null || (specs = specs.Trim()).Length == 0 ? null : specs;
        }

        /// <summary>
        /// If not null, then contains the specifications to use to obtain a new instance
        /// of the decorated type. The default value of this property is 'null' meaning
        /// that the generator will try the type constructors.
        /// </summary>
        public string? Specs { get; }

        /// <summary>
        /// If 'true' instructs the generator not to produce virtual-alike methods. The
        /// default value of this property is 'false' meaning that if will generate new,
        /// virtual or overriden ones, as needed.
        /// </summary>
        public bool PreventVirtual { get; set; }

        /// <summary>
        /// If 'true' instructs the generator to take into consideration the type members
        /// whose names begin with an underscore. The default value of this property is
        /// 'false' meaning that these members will be ignored.
        /// </summary>
        public bool IncludeUnderscores { get; set; }
    }
}
