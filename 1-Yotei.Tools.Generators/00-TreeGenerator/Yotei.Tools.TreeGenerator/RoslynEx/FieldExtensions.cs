namespace Yotei.Tools.TreeGenerator;

// ========================================================
internal static class FieldExtensions
{
    extension(IFieldSymbol symbol)
    {
        /// <summary>
        /// Determines if the given field is writtable or not.
        /// </summary>
        public bool IsWrittable
            => !symbol.IsConst && !symbol.IsReadOnly && !symbol.HasConstantValue;
    }
}