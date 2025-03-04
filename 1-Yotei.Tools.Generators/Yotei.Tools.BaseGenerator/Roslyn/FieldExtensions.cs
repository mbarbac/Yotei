namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class FieldExtensions
{
    /// <summary>
    /// Determines if the given field is a writtable one.
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public static bool IsWrittable(this IFieldSymbol field)
    {
        if (!field.IsConst && !field.IsReadOnly && !field.HasConstantValue) return true;
        return false;
    }
}