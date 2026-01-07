namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class RoslynNameExtensions
{
    /// <summary>
    /// Obtains the C#-alike easy name of the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this TypedConstant source) => EasyNameTypedConstant.Default.EasyName(source);

    /// <summary>
    /// Obtains the C#-alike easy name of the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this TypedConstant source, EasyNameTypedConstant options)
    {
        options.ThrowWhenNull();
        return options.EasyName(source);
    }
}

// ========================================================
/// <summary>
/// Provides 'EasyName' capabilities for 'typed constant' instances.
/// </summary>
internal record EasyNameTypedConstant
{
    /// <summary>
    /// A shared read-only instance that represents empty options.
    /// </summary>
    public static EasyNameTypedConstant Empty { get; } = new(Mode.Empty);

    /// <summary>
    /// A shared read-only instance that represents default options.
    /// </summary>
    public static EasyNameTypedConstant Default { get; } = new(Mode.Default);

    /// <summary>
    /// A shared read-only instance that represents full options.
    /// </summary>
    public static EasyNameTypedConstant Full { get; } = new(Mode.Full);

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyNameTypedConstant() : this(Mode.Default) { }

    // ----------------------------------------------------

    /// <summary>
    /// If not null, the options to use to print the type of the constant. If null, then that
    /// type is not printed.
    /// </summary>
    public EasyNameTypeSymbol? TypeOptions { get; init; }

    // ----------------------------------------------------

    enum Mode { Empty, Default, Full };
    private EasyNameTypedConstant(Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                break;

            case Mode.Default:
                break;

            case Mode.Full:
                break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the C#-alike easy name of the given element.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public string EasyName(TypedConstant source, string? name = null)
    {
        source.ThrowWhenNull();

        var sb = new StringBuilder();

        source.ToCSharpString();

        // Name requested...
        if (name != null)
        {
            sb.Append(name);
            sb.Append(": ");
        }

        // Type...
        if (TypeOptions is not null && source.Type is not null)
        {
            var options = TypeOptions.HideName
                ? TypeOptions with { HideName = false }
                : TypeOptions;

            var str = options.EasyName(source.Type);
            if (source.Kind == TypedConstantKind.Array) str += "[]";
            sb.Append($"({str}) ");
        }

        // Value...
        if (source.IsNull) sb.Append("null");
        else
        {
            if (source.Kind == TypedConstantKind.Array)
            {
                //sb.Append("{ ");
                //var first = true;
                //foreach (var item in source.Values)
                //{
                //    if (!first) sb.Append(", ");
                //    var itemStr = EasyName(item);
                //    sb.Append(itemStr);
                //    first = false;
                //}
                //sb.Append(" }");
            }
            else
            {
                //var valueStr = source.Value switch
                //{
                //    string s => $"\"{s}\"",
                //    char c => $"'{c}'",
                //    bool b => b ? "true" : "false",
                //    null => "null",
                //    _ => source.Value.ToString() ?? string.Empty
                //};
                //sb.Append(valueStr);
            }
        }

        // Finishing...
        return sb.ToString();
    }

    // ----------------------------------------------------
}