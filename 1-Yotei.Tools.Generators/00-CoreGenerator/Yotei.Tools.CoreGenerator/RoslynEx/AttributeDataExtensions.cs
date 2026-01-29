namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class AttributeDataExtensions
{
    extension(AttributeData source)
    {
        /// <summary>
        /// Determines if this attribute can be considered equal to the other given one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EqualsTo(AttributeData other)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(other);

            // Compare classes...
            var comparer = SymbolEqualityComparer.Default;
            if (!comparer.Equals(source.AttributeClass, other.AttributeClass)) return false;

            // Compare constructor arguments...
            if (source.ConstructorArguments.Length != other.ConstructorArguments.Length)
                return false;

            for (int i = 0; i < source.ConstructorArguments.Length; i++)
                if (!AreEqual(source.ConstructorArguments[i], source.ConstructorArguments[i]))
                    return false;

            // Compare named arguments...
            if (source.NamedArguments.Length != source.NamedArguments.Length) return false;

            foreach (var sarg in source.NamedArguments)
            {
                var targ = other.NamedArguments.FirstOrDefault(x => x.Key == sarg.Key);
                if (targ.Key is null || !AreEqual(sarg.Value, targ.Value)) return false;
            }

            // Finishing...
            return true;

            /// <summary>
            /// Determines if the two given typed constants are equal or not.
            /// </summary>
            static bool AreEqual(TypedConstant source, TypedConstant other)
            {
                if (source.Kind != other.Kind) return false;

                if (source.Kind == TypedConstantKind.Array)
                    return source.Values.SequenceEqual(other.Values, (x, y) => AreEqual(x, y));

                return source.Value.EqualsEx(other.Value);
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to find in this attribute the named argument whose name is given and, if found,
        /// also returns in the out parameter the typed constant it carries.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool FindNamedArgument(string name, [NotNullWhen(true)] out TypedConstant? item)
        {
            ArgumentNullException.ThrowIfNull(source);
            name = name.NotNullNotEmpty(true);

            foreach (var temp in source.NamedArguments)
            {
                if (temp.Key == name)
                {
                    item = temp.Value;
                    return true;
                }
            }

            item = null;
            return false;
        }
    }
}