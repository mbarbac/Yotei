namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions (defined as lambda expressions
/// whose sole argument is a dynamic one), and returning the last node in the chain of dynamic
/// operations that expression represents.
/// </summary>
public class DynamicParser
{
    // Prevents creation.
    private DynamicParser() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"({Argument}) => {Result}";

    /// <summary>
    /// Parses the given dynamic lambda expression, returning an instance of this class with the
    /// last node in the chain of dynamic operations binded to the dynamic arguments, as well as
    /// other relevant information.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static DynamicParser Parse(Func<dynamic, object> expression)
    {
        expression = expression.ThrowIfNull();

        var method = expression.GetMethodInfo();
        if (method == null) throw new ArgumentException(
            "Cannot obtain the method info of the given dynamic lambda expression.");

        var pars = method.GetParameters();
        if (pars.Length != 1) throw new ArgumentException(
            "The given dynamic lambda expression must have just one argument.");

        var name = pars[0].Name;
        if (name == null) throw new ArgumentException(
            "The dynamic argument of the dynamic lambda expression has no name.");

        var parser = new DynamicParser() {
            Argument = new DynamicNodeArgument(name)
        };

        var obj = expression(parser.Argument);
        parser.Result = ValueToDynamicNode(obj);
        return parser;
    }

    /// <summary>
    /// The argument used to invoke the dynamic lambda expression.
    /// </summary>
    public DynamicNodeArgument Argument { get; internal set; } = null!;

    /// <summary>
    /// The result of the parsing the dynamic lambda expression, it being the last node in the
    /// chain of dynamic operations that expression represents.
    /// </summary>
    public DynamicNode Result { get; private set; } = null!;

    // ----------------------------------------------------

    /// <summary>
    /// Used to print a debug new line, if debug is enabled.
    /// </summary>
    /// <param name="message"></param>
    [Conditional("DEBUG_PARSER")]
    internal static void DebugPrint() => DebugPrint("");

    /// <summary>
    /// Used to print the given debug message, indented and followed by a new line, if debug is
    /// enabled.
    /// </summary>
    /// <param name="message"></param>
    [Conditional("DEBUG_PARSER")]
    internal static void DebugPrint(string message)
    {
        Debug.Indent(); Debug.WriteLine(message);
        Debug.Unindent();
    }

    /// <summary>
    /// Used to print the given debug message, each indented and followed by a new line, if debug
    /// is enabled.
    /// </summary>
    /// <param name="messages"></param>
    [Conditional("DEBUG_PARSER")]
    internal static void DebugPrint(IEnumerable<string> messages)
    {
        Debug.Indent(); foreach (var item in messages) Debug.WriteLine(item);
        Debug.Unindent();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an array with the dynamic nodes associated with the given objects, creating new
    /// constant ones to carry that values, if needed.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static DynamicNode[] ValuesToDynamicNodes(object?[] values)
    {
        if (values is null) return Array.Empty<DynamicNode>();

        var items = new DynamicNode[values.Length];
        for (int i = 0; i < values.Length; i++) items[i] = ValueToDynamicNode(values[i]);

        return items;
    }

    /// <summary>
    /// Returns the dynamic node associated with the given object, or creates a new constant one
    /// to carry that value.
    /// </summary>
    public static DynamicNode ValueToDynamicNode(object? value)
    {
        return value switch {
            DynamicNode item => item,
            DynamicMetaNode item => item.DynamicNode,
            DynamicMetaObject item => ValueToDynamicNode(item.Value),

            _ => new DynamicNodeValued(value)
        };
    }
}