namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a node in a chain of dynamic operations.
/// </summary>
public abstract class LambdaNode : DynamicObject, ICloneable
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    public LambdaNode(LambdaParser parser) => LambdaParser = parser.ThrowWhenNull();

    /// <inheritdoc cref="ICloneable.Clone"/>
    public abstract LambdaNode Clone();
    object ICloneable.Clone() => Clone();

    /// <inheritdoc/>
    public override string ToString() => nameof(LambdaNode);

    /// <summary>
    /// Returns a string representation of this instance suitable for debug purposes.
    /// </summary>
    /// <returns></returns>
    public string ToDebugString()
    {
        var type = GetType().EasyName();
        return $"[{type}](Id:{LambdaId}, {ToString()})";
    }

    // ----------------------------------------------------

    /// <summary>
    /// The parser this instance is associated with.
    /// </summary>
    public LambdaParser LambdaParser { get; }

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public long LambdaId { get; }
    static long LastLambdaId = 0;

    /// <summary>
    /// Gets the next available id.
    /// </summary>
    /// <returns></returns>
    public static long NextLambdaId() => Interlocked.Increment(ref LastLambdaId);

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override DynamicMetaObject GetMetaObject(Expression expression)
    {
        var master = base.GetMetaObject(expression);
        var rest = BindingRestrictions.GetInstanceRestriction(expression, this);
        var meta = new LambdaMetaNode(master, expression, rest, this);

        return meta;
    }
}