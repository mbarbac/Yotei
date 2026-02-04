namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class SyntaxListExtensions
{
    extension(SyntaxList<AttributeListSyntax> sources)
    {
        /// <summary>
        /// Returns a flattened enumeration of the attributes that decorates each of the syntax
        /// elements. No attempts are made to prevent duplicate attributes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AttributeSyntax> GetAttributes()
        {
            return sources.SelectMany(static x => x.Attributes);
        }
    }
}