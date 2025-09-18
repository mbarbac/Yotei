namespace Yotei.ORM.Internals;

// ========================================================
public static partial class Fragment
{
    /// <summary>
    /// Parses the given dynamic lambda expression and returns the top-most token of the chain
    /// that represents the dynamic operations in that expression. In addition, if it resolves
    /// into a string, and if a collection of arguments is given, then those arguments are used
    /// to build a command-info token.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="spec"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IDbToken CreateToken(
        IEngine engine,
        Func<dynamic, object> spec, params object?[]? args)
    {
        var token = new DbLambdaParser(engine).Parse(spec);

        if (args is null || args.Length > 0)
        {
            if (token is DbTokenValue value && value.Value is string str)
            {
                args ??= [null];
                var info = new CommandInfo(engine, str, args);
                token = new DbTokenCommandInfo(info);
            }
            else throw new ArgumentException(
                "Arguments are given but there is no valid string receiver.")
                .WithData(token)
                .WithData(args);
        }

        return token;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given text has any dangling '{...}' bracket specifications.
    /// </summary>
    internal static bool HasDanglingBrackets(string str)
        => CommandInfo.Builder.AreRemainingBrackets(str);
}