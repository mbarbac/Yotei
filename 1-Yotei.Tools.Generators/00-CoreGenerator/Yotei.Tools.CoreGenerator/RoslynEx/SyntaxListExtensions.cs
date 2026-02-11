namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class SyntaxListExtensions
{
    extension(SyntaxList<AttributeListSyntax> source)
    {
        /// <summary>
        /// Returns a flattened enumeration of the attributes that decorates each of the syntax
        /// elements. No attempts are made to prevent duplicate attributes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AttributeSyntax> GetAttributes()
        {
            return source.SelectMany(static x => x.Attributes);
        }
    }
}