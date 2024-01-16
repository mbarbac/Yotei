namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a node in a chain of dynamic operations.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class LambdaNode : DynamicObject, ICloneable
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public LambdaNode()
    {
        LambdaId = Interlocked.Increment(ref _LastLambdaId);
        LambdaVersion = Interlocked.Increment(ref _LastLambdaVersion);
    }

    /// <inheritdoc cref="ICloneable.Clone"/>
    public abstract LambdaNode Clone();
    object ICloneable.Clone() => Clone();

    /// <inheritdoc/>
    public override string ToString() => nameof(LambdaNode);

    /// <summary>
    /// Returns a DEBUG representation of this instance.
    /// </summary>
    /// <returns></returns>
    internal string ToDebugString()
    {
        var type = GetType().EasyName();
        return $"{type}(Id:{LambdaId}, V:{LambdaVersion}, {ToString()})";
    }

    // ----------------------------------------------------

    /// <summary>
    /// The unique Id of this instance.
    /// </summary>
    public long LambdaId { get; }
    static long _LastLambdaId = 0;

    /// <summary>
    /// The current version of this instance.
    /// </summary>
    public long LambdaVersion { get; internal set; }
    static long _LastLambdaVersion = 0;

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
            throw new ArgumentException("Collection of arguments cannot be empty.");

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

        if (list.Count == 0) throw new ArgumentException("Collection of types cannot be empty.");

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

    /// <summary>
    /// Flags to find the method info of the version-related methods.
    /// </summary>
    static readonly BindingFlags LAMBDA_FLAGS
        = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Determines if the version of this instance is the same as the latest one, or not.
    /// </summary>
    internal bool ValidateLambdaVersion()
    {
        var result = LambdaVersion == _LastLambdaVersion;

        LambdaParser.Print(this, $"- Version: {(result ? "Valid" : "Invalid")}, {ToDebugString()}");
        return result;
    }

    static MethodInfo ValidateLambdaVersionInfo
        => typeof(LambdaNode).GetMethod(nameof(ValidateLambdaVersion), LAMBDA_FLAGS)!;

    /// <summary>
    /// Updates the version of this instance to the lates one, which is also incremented.
    /// </summary>
    internal void UpdateLambdaVersion()
    {
        var old = ToDebugString();
        LambdaVersion = Interlocked.Increment(ref _LastLambdaVersion);

        LambdaParser.Print(this, $"- Updating: {old} ==> {ToDebugString()}");
    }

    static MethodInfo UpdateLambdaVersionInfo
        => typeof(LambdaNode).GetMethod(nameof(UpdateLambdaVersion), LAMBDA_FLAGS)!;

    /// <summary>
    /// Obtains binding restrictions that validates that the version of this instance is equal
    /// to the latest one, and if not, updates it.
    /// <para>
    /// The DLR caches the results it obtains using the type of the call site and its arguments.
    /// For the purposes of <see cref="LambdaParser"/>, this mechanism will render the same node
    /// over and over again, instead of a new binded one. This method intercepts this mechanism
    /// using a custom binding restriction that forces the cache to discard previous values and
    /// to use the new binded one, when neccesary.
    /// </para>
    /// </summary>
    internal BindingRestrictions GetBindingRestrictions(Expression updateExpr)
    {
        var nodeExpr = Expression.Constant(this);
        var argExpr = Expression.Parameter(typeof(object));

        var condition = Expression.Block(
            new[] { argExpr },
            Expression.Assign(argExpr, nodeExpr),
            Expression.Condition(
                Expression.IsFalse(
                    Expression.Call(
                        Expression.Convert(argExpr, typeof(LambdaNode)),
                        ValidateLambdaVersionInfo)),
                Expression.Block(
                    Expression.Call(
                        Expression.Convert(argExpr, typeof(LambdaNode)),
                        UpdateLambdaVersionInfo),
                    updateExpr),
                Expression.Constant(true)));

        var rest = BindingRestrictions.GetExpressionRestriction(condition);
        return rest;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        var parser = GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(this, $"* GetMember:");
        LambdaParser.Print(this, $"- This: {ToDebugString()}");
        LambdaParser.Print(this, $"- Name: {binder.Name}");

        var node = new LambdaNodeMember(this, binder.Name);
        result = node;

        // Finishing...
        LambdaParser.Print(this, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <inheritdoc/>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        var parser = GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(this, $"* GetIndex:");
        LambdaParser.Print(this, $"- This: {ToDebugString()}");
        var list = LambdaParser.ToLambdaNodes(indexes, parser);
        foreach (var temp in list) LambdaParser.Print(this, $"- Index: {temp.ToDebugString()}");

        var node = new LambdaNodeIndexed(this, list);
        result = node;

        // Finishing...
        LambdaParser.Print(this, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <inheritdoc/>
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        var parser = GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(this, $"* Invoke:");
        LambdaParser.Print(this, $"- This: {ToDebugString()}");
        var list = LambdaParser.ToLambdaNodes(args, parser);
        foreach (var temp in list) LambdaParser.Print(this, $"- Argument: {temp.ToDebugString()}");

        var node = new LambdaNodeInvoke(this, list);
        result = node;

        // Finishing...
        LambdaParser.Print(this, $"- Result: {node.ToDebugString()}");
        return true;
    }

    /// <inheritdoc/>
    public override bool TryInvokeMember(
        InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        var parser = GetArgument()?.LambdaParser;
        if (parser != null) parser.LastNode = null;

        LambdaParser.Print(this, $"* Method:");
        LambdaParser.Print(this, $"- This: {ToDebugString()}");
        LambdaParser.Print(this, $"- Name: {binder.Name}");
        var list = LambdaParser.ToLambdaNodes(args, parser);
        foreach (var temp in list) LambdaParser.Print(this, $"- Argument: {temp.ToDebugString()}");

        LambdaNode node;

        // Intercepting 'Coalesce' methods...
        if (this is LambdaNodeArgument && binder.Name == "Coalesce" && list.Length == 2)
        {
            LambdaParser.Print(this, $"* Intercepting 'Coalesce' method...");

            node = new LambdaNodeCoalesce(list[0], list[1]);
            result = node;
        }

        // Intercepting 'Ternary' methods...
        else if (this is LambdaNodeArgument && binder.Name == "Ternary" && list.Length == 3)
        {
            LambdaParser.Print(this, $"* Intercepting 'Ternary' method...");

            node = new LambdaNodeTernary(list[0], list[1], list[2]);
            result = node;
        }

        // Regular methods...
        else
        {
            var types = Array.Empty<Type>();

            if (binder.GetType().Name == "CSharpInvokeMemberBinder") // Not public!
            {
                var flags = BindingFlags.Instance | BindingFlags.Public;
                var info = binder.GetType().GetProperty("TypeArguments", flags);
                types = (Type[])info!.GetValue(binder)!;
            }

            node = types.Length == 0
                ? new LambdaNodeMethod(this, binder.Name, list)
                : new LambdaNodeMethod(this, binder.Name, types, list);

            result = node;
        }

        // Finishing
        LambdaParser.Print(this, $"- Result: {node.ToDebugString()}");
        return true;
    }

    // ----------------------------------------------------

    /* bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result) */
    /* bool TryUnaryOperation(UnaryOperationBinder binder, out object? result) */
    /* bool TrySetMember(SetMemberBinder binder, object? value) */
    /* bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) */
    /* bool TryConvert(ConvertBinder binder, out object? result) */

    /* bool TryCreateInstance(CreateInstanceBinder binder, object?[]? args, [NotNullWhen(true)] out object? result) */
    /* bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes) */
    /* bool TryDeleteMember(DeleteMemberBinder binder) */
}