namespace Yotei.Tools.Generators;

// ========================================================
public static class IFieldSymbolExtensions
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