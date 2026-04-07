namespace Yotei.Tools.Generators;

// ========================================================
public static class SyntaxNodeExtensions
{
    extension(SyntaxNode source)
    {
        /// <summary>
        /// Obtains a suitable identifier name for the given syntax node.
        /// </summary>
        /// <returns></returns>
        public string ToNodeName()
        {
            switch (source)
            {
                case BaseTypeDeclarationSyntax item: return item.Identifier.Text;
                case MethodDeclarationSyntax item: return item.Identifier.Text;
                case PropertyDeclarationSyntax item: return item.Identifier.Text;
                case BaseFieldDeclarationSyntax item
                    when item.Declaration.Variables.Count > 0:
                    return item.Declaration.Variables[0].Identifier.Text;
            }

            var name = source.ChildTokens()
                .FirstOrDefault(x => x.IsKind(SyntaxKind.IdentifierToken))
                .Text;

            return name;
        }
    }
}