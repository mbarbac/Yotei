namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Maintains the collection of core diagnostics.
/// </summary>
/// <param name="Value"></param>
/// <param name="Description"></param>
internal record TreeError(int Value, string Description)
{
    public static readonly TreeError SyntaxNotSupported = new(1, "Syntax not supported");
    public static readonly TreeError SymbolNotFound = new(2, "Symbol not found");
    public static readonly TreeError SyntaxNotFound = new(3, "Syntax not found");
    public static readonly TreeError NoAttributes = new(4, "No attributes");
    public static readonly TreeError TooManyAttributes = new(5, "Too many attributes");
    public static readonly TreeError InvalidAttribute = new(6, "Invalid attribute");
    public static readonly TreeError KindNotSupported = new(7, "Kind not supported");
    public static readonly TreeError RecordsNotSupported = new(8, "Records not supported");
    public static readonly TreeError TypeNotPartial = new(9, "Type not partial");
    public static readonly TreeError NoCopyConstructor = new(10, "No copy constructor");
    public static readonly TreeError NoGetter = new(11, "No suitable getter");
    public static readonly TreeError NoSetter = new(12, "No suitable setter");
    public static readonly TreeError InvalidGetter = new(13, "Invalid getter");
    public static readonly TreeError InvalidSetter = new(14, "Invalid setter");
    public static readonly TreeError IndexerNotFound = new(15, "Indexer not found");
    public static readonly TreeError IndexerNotSupported = new(16, "Indexer not supported");
    public static readonly TreeError NotWrittable = new(17, "Not writtable");
    public static readonly TreeError InvalidMethod = new(18, "Invalid method");
    public static readonly TreeError InvalidReturnType = new(19, "Invalid return type");
}