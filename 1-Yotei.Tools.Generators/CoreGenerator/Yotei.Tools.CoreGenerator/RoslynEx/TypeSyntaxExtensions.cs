namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class TypeSyntaxExtensions
{
    extension(BaseTypeDeclarationSyntax syntax)
    {
        /// <summary>
        /// Determines if the type is a partial one, or not.
        /// </summary>
        public bool IsPartial => syntax.Modifiers.Any(SyntaxKind.PartialKeyword);

        /// <summary>
        /// Determines if the type is a record, or not.
        /// </summary>
        public bool IsRecord => syntax.Kind() is
            SyntaxKind.RecordDeclaration or
            SyntaxKind.RecordStructDeclaration;
    }
}