namespace Yotei.Generator.Deprecated
{
    // ====================================================
    internal static class Miscelanea
    {
        /// <summary>
        /// Searches for a type across all available assemblies in the compilation, by its
        /// metadata name. This method prevents returning null if the same type is present in
        /// two or more assemblies.
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="metadataName"></param>
        /// <returns></returns>
        public static IEnumerable<INamedTypeSymbol?> GetTypesByMetadataName(
            this Compilation compilation,
            string metadataName)
        {
            return compilation.References
                .Select(compilation.GetAssemblyOrModuleSymbol)
                .OfType<IAssemblySymbol>()
                .Select(asmSymbol => asmSymbol.GetTypeByMetadataName(metadataName))
                .Where(x => x != null);
        }

        /// <summary>
        /// Examples of finding a generic type and of constructing a concrete one out of it.
        /// </summary>
        /// <param name="compilation"></param>
        /// <returns></returns>
        public static INamedTypeSymbol FindGenericTypes(Compilation compilation)
        {
            var nullableT = compilation.GetTypeByMetadataName("System.Nullable`1");
            var nullableInt = nullableT!.Construct(compilation.GetSpecialType(SpecialType.System_Int32));
            return nullableInt;
        }

        /// <summary>
        /// Examples of getting a type symbol from a type syntax node.
        /// </summary>
        /// <param name="semantic"></param>
        /// <param name="node"></param>
        public static void GetTypeOfSyntaxNode(SemanticModel semantic, SyntaxNode node)
        {
            // int? a = 10;         // Type: int,   Converted: int?
            // int? b = (int?)10;   // Type: int?,  Converted: int?

            ITypeSymbol? type = semantic.GetTypeInfo(node).Type;
            ITypeSymbol? converted = semantic.GetTypeInfo(node).ConvertedType;
        }
    }
}