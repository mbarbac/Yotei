namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a node in a chain of dynamic operations.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract partial class LambdaNode : ICloneable
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract LambdaNode Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// Returns the dynamic argument ultimately associated with this instance, or null if any.
    /// </summary>
    /// <returns></returns>
    public abstract LambdaNodeArgument? GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a validated name that can be used with dynamic elements, or throws an exception
    /// if that name was an invalid one.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
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
    /// Invoked to return a validated collection of dynamic lambda nodes that can be used as the
    /// arguments of a method or invocation, or to throw an exception if needed.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="canBeEmpty"></param>
    /// <returns></returns>
    /// <exception cref="EmptyException"></exception>
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
    /// Invoked to return a validated collection of types, typically used as the collection of
    /// generic arguments of a method.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
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