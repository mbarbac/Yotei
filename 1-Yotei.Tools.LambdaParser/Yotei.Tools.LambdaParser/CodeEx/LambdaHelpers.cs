namespace Yotei.Tools;

// ========================================================
internal static class LambdaHelpers
{
    internal const ConsoleColor NewNodeColor = ConsoleColor.White;
    internal const ConsoleColor NewMetaColor = ConsoleColor.Gray;
    internal const ConsoleColor NodeBindedColor = ConsoleColor.Blue;
    internal const ConsoleColor MetaBindedColor = ConsoleColor.Yellow;

    internal const ConsoleColor ValidateLambdaColor = ConsoleColor.Cyan;
    internal const ConsoleColor UpdateLambdaColor = ConsoleColor.Magenta;

    /// <summary>
    /// Invoked to print the given debug message.
    /// </summary>
    [Conditional("DEBUG_LAMBDA_PARSER")]
    internal static void Print(string message) => DebugEx.WriteLine(message);

    /// <summary>
    /// Invoked to print the given debug message.
    /// </summary>
    [Conditional("DEBUG_LAMBDA_PARSER")]
    internal static void Print(
        ConsoleColor color, string message) => DebugEx.WriteLine(color, message);

    // ----------------------------------------------------

    /// <summary>
    /// Validates and return the given name, to be used with dynamic elements.
    /// </summary>
    internal static string ValidateName(string name)
    {
        name = name.NotNullNotEmpty();

        if (name.Any(x => !ValidChar(x))) throw new ArgumentException(
            "Name contains invalid character(s).")
            .WithData(name);

        return name;

        static bool ValidChar(char c) =>
            c is '_' or
            (>= '0' and <= '9') or
            (>= 'A' and <= 'Z') or
            (>= 'a' and <= 'z');
    }

    /// <summary>
    /// Invoked to validate the given collection of dynamic lambda nodes that can be used as the
    /// arguments of a method invocation, including an empty one if such is allowed.
    /// </summary>
    internal static IImmutableList<LambdaNode> ValidateLambdaArguments(
        IEnumerable<LambdaNode> args,
        bool canBeEmpty)
    {
        args = args.ThrowWhenNull();

        var list = args is IImmutableList<LambdaNode> temp
            ? temp
            : args.ToImmutableList();

        if (!canBeEmpty && list.Count == 0)
            throw new EmptyException("Collection of arguments cannot be empty.");

        if (list.Any(x => x is null)) throw new ArgumentException(
            "Collection of arguments carries null elements.")
            .WithData(list);

        return list;
    }

    /// <summary>
    /// Invoked to validate the given collection of types, typically used as the collection of
    /// generic arguments of a method.
    /// </summary>
    internal static IImmutableList<Type> ValidateLambdaTypes(IEnumerable<Type> types)
    {
        types = types.ThrowWhenNull();

        var list = types is IImmutableList<Type> temp
            ? temp
            : types.ToImmutableList();

        list = list.Cast<Type>().ToImmutableList();

        if (list.Count == 0) throw new EmptyException("Collection of types cannot be empty.");

        if (list.Any(x => x is null)) throw new ArgumentException(
            "Collection of types carries null elements.")
            .WithData(list);

        return list;
    }
}