namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a node in a chain of dynamic operations.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class DynamicNode : DynamicObject
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    protected DynamicNode()
    {
        DynamicId = GetNextDynamicId();
        DynamicVersion = GetNextDynamicVersion();
    }

    /// <summary>
    /// Returns the dynamic argument node this instance is ultimately associated with, or null
    /// if any.
    /// </summary>
    /// <returns></returns>
    public abstract DynamicNodeArgument? GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public long DynamicId { get; }
    static long _LastDynamicId = 0;

    /// <summary>
    /// Returns the next available node id.
    /// </summary>
    /// <returns></returns>
    internal static long GetNextDynamicId() => Interlocked.Increment(ref _LastDynamicId);

    /// <summary>
    /// The current version of this instance.
    /// </summary>
    internal long DynamicVersion { get; set; }
    static long _LastDynamicVersion = 0;

    /// <summary>
    /// Returns the next available version.
    /// </summary>
    /// <returns></returns>
    internal static long GetNextDynamicVersion() => Interlocked.Increment(ref _LastDynamicVersion);

    // ----------------------------------------------------

    /// <summary>
    /// Returns the debug string representation of this instance.
    /// </summary>
    /// <returns></returns>
    internal string ToDebugString()
        => $"Node#{DynamicId}(V:{DynamicVersion}/{_LastDynamicVersion}, {ToString()})";

    /// <summary>
    /// Used to print this node when it was created.
    /// </summary>
    [Conditional("DEBUG_PARSER")]
    internal void DebugPrintNew()
        => DynamicParser.DebugPrint($"- New: {ToDebugString()}");

    // ----------------------------------------------------

    /// <summary>
    /// Invalid name characters.
    /// </summary>
    internal readonly static char[] INVALID_NAME_CHARS = @" .+-/*[]{}()^=?%&!\".ToCharArray();

    /// <summary>
    /// Validates the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    internal static string ValidateDynamicName(string name)
    {
        name = name.NotNullNotEmpty();

        if (name.ContainsAny(INVALID_NAME_CHARS)) throw new ArgumentException(
            "Name contains invalid characters.")
            .WithData(name);

        return name;
    }

    /// <summary>
    /// Validates the given array of arguments, and returns an immutable collection for them.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="canbeEmpty"></param>
    /// <returns></returns>
    internal static IImmutableList<DynamicNode> ValidateDynamicArguments(
        IEnumerable<DynamicNode> args,
        bool canbeEmpty)
    {
        args = args.ThrowIfNull();

        var items = args.ToList();

        if (!canbeEmpty && items.Count == 0) throw new ArgumentException(
            "Array of arguments cannot be empty.")
            .WithData(items);

        if (items.Any(x => x == null)) throw new ArgumentException(
            "Array of arguments contains null elements.")
            .WithData(items);

        return items.ToImmutableList();
    }

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    public override DynamicMetaObject GetMetaObject(Expression expression)
    {
        var master = base.GetMetaObject(expression);
        var rest = BindingRestrictions.GetInstanceRestriction(expression, this);
        var meta = new DynamicMetaNode(master, expression, rest, this);

        return meta;
    }

    /// <summary>
    /// Gets binding restrictions that validates that the version of this instance is equal to
    /// the latest one, updating it if not.
    /// </summary>
    /// <param name="updateExpr"></param>
    /// <returns></returns>
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
                        Expression.Convert(argExpr, typeof(DynamicNode)),
                        ValidateDynamicVersionInfo)),
                Expression.Block(
                    Expression.Call(
                        Expression.Convert(argExpr, typeof(DynamicNode)),
                        UpdateDynamicVersionInfo),
                    updateExpr),
                Expression.Constant(true)));

        var rest = BindingRestrictions.GetExpressionRestriction(condition);
        return rest;
    }

    /// <summary>
    /// Flags to find the method info of the version-related methods.
    /// </summary>
    static readonly BindingFlags DYNAMIC_FLAGS
        = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Returns if the version of this instance equals the latest one, or not.
    /// </summary>
    /// <returns></returns>
    internal bool ValidateDynamicVersion()
    {
        var result = DynamicVersion == _LastDynamicVersion;

        DynamicParser.DebugPrint($"- {(result ? "Valid" : "Invalid ")}: {ToDebugString()}");
        return result;
    }

    static MethodInfo ValidateDynamicVersionInfo
        => typeof(DynamicNode).GetMethod(nameof(ValidateDynamicVersion), DYNAMIC_FLAGS)!;

    /// <summary>
    /// Updates the version of this instance to the latest one, which is incremented previously.
    /// </summary>
    internal void UpdateDynamicVersion()
    {
        var old = ToDebugString();
        DynamicVersion = GetNextDynamicVersion();
        DynamicParser.DebugPrint($"- Updating: {old} ==> {ToDebugString()}");
    }

    static MethodInfo UpdateDynamicVersionInfo
        => typeof(DynamicNode).GetMethod(nameof(UpdateDynamicVersion), DYNAMIC_FLAGS)!;

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        DynamicParser.DebugPrint($"- GetMember:");
        DynamicParser.DebugPrint($"- This: {ToDebugString()}");
        DynamicParser.DebugPrint($"- Name: {binder.Name}");

        var node = new DynamicNodeMember(this, binder.Name);
        result = node;
        DynamicParser.DebugPrint($"-- Result: {node.ToDebugString()}");
        DynamicParser.DebugPrint();
        return true;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        DynamicParser.DebugPrint($"- GetIndex:");
        DynamicParser.DebugPrint($"- This: {ToDebugString()}");

        var list = DynamicParser.ValuesToDynamicNodes(indexes);
        DynamicParser.DebugPrint(list.Select(x => $"- Index: {x.ToDebugString()}"));

        var node = new DynamicNodeIndexed(this, list);
        result = node;
        DynamicParser.DebugPrint($"-- Result: {node.ToDebugString()}");
        DynamicParser.DebugPrint();
        return true;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        DynamicParser.DebugPrint($"- Invoke:");
        DynamicParser.DebugPrint($"- This: {ToDebugString()}");

        var list = DynamicParser.ValuesToDynamicNodes(args!);
        DynamicParser.DebugPrint(list.Select(x => $"- Argument: {x.ToDebugString()}"));

        var node = new DynamicNodeInvoked(this, list);
        result = node;
        DynamicParser.DebugPrint($"-- Result: {node.ToDebugString()}");
        DynamicParser.DebugPrint();
        return true;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        DynamicParser.DebugPrint($"- Method:");
        DynamicParser.DebugPrint($"- This: {ToDebugString()}");
        DynamicParser.DebugPrint($"- Name: {binder.Name}");

        var list = DynamicParser.ValuesToDynamicNodes(args!);
        DynamicParser.DebugPrint(list.Select(x => $"- Argument: {x.ToDebugString()}"));

        var types = Array.Empty<Type>();

        if (binder.GetType().Name == "CSharpInvokeMemberBinder")
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            var info = binder.GetType().GetProperty("TypeArguments", flags);
            types = (Type[])info!.GetValue(binder)!;
        }

        DynamicNode node = null!;

        if (node == null)
        {
            node = types.Length == 0
                ? new DynamicNodeMethod(this, binder.Name, list)
                : new DynamicNodeMethod(this, binder.Name, types, list);
        }

        result = node;
        DynamicParser.DebugPrint($"-- Result: {node.ToDebugString()}");
        DynamicParser.DebugPrint();
        return true;
    }
}