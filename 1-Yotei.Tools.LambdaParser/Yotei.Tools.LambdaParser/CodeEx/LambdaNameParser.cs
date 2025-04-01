namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Parses into dot-separated member names the given dynamic lambda expression.
/// </summary>
public class LambdaNameParser
{
    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated names that expression
    /// resolves into. If all parts are null or empty, then returns an empty string.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static string Parse(Func<dynamic, object> expression) => Parse(expression, out _, out _);

    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated names that expression
    /// resolves into, and returns in the out arguments the collection of parts names. If all
    /// parts are null or empty, then returns an empty string, and an empty array of parts.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string Parse(
        Func<dynamic, object> expression, out string[] parts)
        => Parse(expression, out parts, out _);

    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated names that expression
    /// resolves into, and returns the  dynamic argument used for the expression. If all parts
    /// are null or empty, then returns an empty string.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string Parse(
        Func<dynamic, object> expression, out LambdaNodeArgument arg)
        => Parse(expression, out _, out arg);

    /// <summary>
    /// Parses the given dynamic lambda expression into the dot-separated names that expression
    /// resolves into, and returns in the out arguments the collection of parts names, and the
    /// dynamic argument used for the expression. If all parts are null or empty, then returns
    /// an empty string, and an empty array of parts.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="parts"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string Parse(
        Func<dynamic, object> expression, out string[] parts, out LambdaNodeArgument arg)
    {
        throw null;
    }
}