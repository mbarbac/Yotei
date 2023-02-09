namespace Yotei.Generator.Deprecated
{
    // ====================================================
    internal static class NamespaceSyntaxExtensions
    {
        /// <summary>
        /// Returns the syntax declaration of the type whose name is given, on the given namespace,
        /// or null if it cannot be found.
        /// </summary>
        /// <param name="nsSyntax"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static TypeDeclarationSyntax? GetTypeDeclarationSyntax(
            this BaseNamespaceDeclarationSyntax nsSyntax,
            string typeName)
        {
            nsSyntax = nsSyntax.ThrowIfNull(nameof(nsSyntax));
            typeName = typeName.NotNullNotEmpty(nameof(typeName));

            return nsSyntax.Members
                .OfType<TypeDeclarationSyntax>()
                .SingleOrDefault(x => typeName == x.Identifier.Text);
        }
    }
}