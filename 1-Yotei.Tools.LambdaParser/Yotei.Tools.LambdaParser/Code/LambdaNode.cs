namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represent a node in a chain of dynamic operations.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class LambdaNode
{
    /// <summary>
    /// Obtains a debug representation of this instance.
    /// </summary>
    /// <returns></returns>
    public string ToDebugString() => throw null;

    /// <summary>
    /// Returns the dynamic argument this instance is ultimately associated with, or null if any.
    /// </summary>
    /// <returns></returns>
    public abstract LambdaNodeArgument? GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// Validates that the given name can be used to name dynamic elements.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ValidateName(string name)
    {
        name = name.NotNullNotEmpty(true);

        if (!VALID_FIRST.Contains(name[0])) throw new ArgumentException(
            "Name first character is invalid.")
            .WithData(name);

        for (int i = 1; i < name.Length; i++)
            if (!VALID_OTHER.Contains(name[i])) throw new ArgumentException(
            "Name contains an invalid character.")
            .WithData(name);

        return name;
    }

    readonly static string VALID_FIRST =
        "_$@"
        + "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        + "abcdefghijklmnopqrstuvwxyz";

    readonly static string VALID_OTHER = VALID_FIRST
        + "#"
        + "0123456789";

    /// <summary>
    /// Validates that the given collection of lambda nodes can be used as the arguments of a
    /// method or indexer. The collection can be empty or not, as requested, but if not, cannot
    /// carry null elements.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="canBeEmpty"></param>
    /// <returns></returns>
    public static ImmutableArray<LambdaNode> ValidateArguments(
        IEnumerable<LambdaNode> args,
        bool canBeEmpty)
    {
        args.ThrowWhenNull();

        var list = args is ImmutableArray<LambdaNode> temp ? temp : [.. args];

        if (list.Length == 0 && !canBeEmpty) throw new ArgumentException(
            "Collection of arguments cannot be empty.");

        if (list.Length != 0 && list.Any(x => x is null)) throw new ArgumentException(
            "Collection of arguments carries null elements.")
            .WithData(args);

        return list;
    }

    /// <summary>
    /// Validates that the given collection of types can be used as the type arguments of a
    /// method. The collection cannot be empty and cannot carry null elements.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static ImmutableArray<Type> ValidateTypeArguments(IEnumerable<Type> args)
    {
        args.ThrowWhenNull();

        var list = args is ImmutableArray<Type> temp ? temp : [.. args];

        if (list.Length == 0) throw new EmptyException(
            "Collection of type arguments cannot be empty.");

        if (list.Any(x => x is null)) throw new ArgumentException(
            "Collection of type arguments carries null elements.")
            .WithData(args);

        return list;
    }
}