namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Maintains the collection of core diagnostics.
/// </summary>
/// <param name="Value"></param>
/// <param name="Description"></param>
internal record CoreError(int Value, string Description)
{
    public static readonly CoreError SyntaxNotSupported = new(1, "Syntax not supported");
    public static readonly CoreError SymbolNotFound = new(2, "Symbol not found");
    public static readonly CoreError SyntaxNotFound = new(3, "Syntax not found");
    public static readonly CoreError NoAttributes = new(4, "No attributes");
    public static readonly CoreError TooManyAttributes = new(5, "Too many attributes");
    public static readonly CoreError InvalidAttribute = new(6, "Invalid attribute");
    public static readonly CoreError KindNotSupported = new(7, "Kind not supported");
    public static readonly CoreError RecordsNotSupported = new(8, "Records not supported");
    public static readonly CoreError TypeNotPartial = new(9, "Type not partial");
    public static readonly CoreError NoCopyConstructor = new(10, "No copy constructor");
    public static readonly CoreError NoGetter = new(11, "No suitable getter");
    public static readonly CoreError NoSetter = new(12, "No suitable setter");
    public static readonly CoreError InvalidGetter = new(13, "Invalid getter");
    public static readonly CoreError InvalidSetter = new(14, "Invalid setter");
    public static readonly CoreError IndexerNotFound = new(15, "Indexer not found");
    public static readonly CoreError IndexerNotSupported = new(16, "Indexer not supported");
    public static readonly CoreError NotWrittable = new(17, "Not writtable");
    public static readonly CoreError InvalidMethod = new(18, "Invalid method");
    public static readonly CoreError InvalidReturnType = new(19, "Invalid return type");
}