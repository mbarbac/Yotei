namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class PropertydExtensions
{
    extension(IPropertySymbol symbol)
    {
        /// <summary>
        /// Determines if the property has a getter, or not.
        /// </summary>
        public bool HasGetter => symbol.GetMethod != null;

        /// <summary>
        /// Determines if the property has a setter, or not.
        /// </summary>
        public bool HasSetter => symbol.SetMethod != null;

        /// <summary>
        /// Determines if the property has a setter that is not an init one, or not.
        /// </summary>
        public bool HasSetterNoInit => symbol.HasSetter && !symbol.SetMethod!.IsInitOnly;

        /// <summary>
        /// Determines if the property has a setter that is an init only one, or not.
        /// </summary>
        public bool HasSetterInitOnly => symbol.HasSetter && symbol.SetMethod!.IsInitOnly;
    }
}