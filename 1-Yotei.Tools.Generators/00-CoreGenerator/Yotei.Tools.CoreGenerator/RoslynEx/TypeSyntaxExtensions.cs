namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class TypeDeclarationSyntaxExtensions
{
    extension(BaseTypeDeclarationSyntax source)
    {
        /// <summary>
        /// Determines if this type is a partial one, or not.
        /// </summary>
        public bool IsPartial => source.Modifiers.Any(SyntaxKind.PartialKeyword);

        /// <summary>
        /// Determines if this type is a record, or a record struct, or not.
        /// </summary>
        public bool IsRecord => source.Kind() is
            SyntaxKind.RecordDeclaration or
            SyntaxKind.RecordStructDeclaration;
    }
}