﻿namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic method invocation operation.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeMethod : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    public LambdaNodeMethod(
        LambdaNode host,
        string name,
        IEnumerable<LambdaNode> arguments) : base(host)
    {
        LambdaName = LambdaHelpers.ValidateName(name);
        LambdaArguments = LambdaHelpers.ValidateLambdaArguments(arguments, canBeEmpty: true);
        LambdaGenericArguments = ImmutableList<Type>.Empty;

        LambdaHelpers.PrintNode(this);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    public LambdaNodeMethod(
        LambdaNode host,
        string name,
        IEnumerable<Type> types,
        IEnumerable<LambdaNode> arguments) : base(host)
    {
        LambdaName = LambdaHelpers.ValidateName(name);
        LambdaArguments = LambdaHelpers.ValidateLambdaArguments(arguments, canBeEmpty: true);
        LambdaGenericArguments = LambdaHelpers.ValidateLambdaTypes(types);

        LambdaHelpers.Print(LambdaHelpers.NewNodeColor, $"- New: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"{LambdaHost}.{LambdaName}");

        if (LambdaGenericArguments.Count != 0)
        {
            sb.Append('<');
            sb.Append(string.Join(", ", LambdaGenericArguments.Select(x => x.EasyName())));
            sb.Append('>');
        }
        sb.Append('(');
        sb.Append(string.Join(", ", LambdaArguments.Select(x => x.ToString())));
        sb.Append(')');

        return sb.ToString();
    }

    /// <inheritdoc/>
    public override LambdaNodeMethod Clone()
    {
        if (LambdaGenericArguments.Count == 0)
        {
            return new(
                LambdaHost.Clone(),
                LambdaName,
                LambdaArguments.Select(x => x.Clone()).ToImmutableList());
        }
        else
        {
            return new(
                LambdaHost.Clone(),
                LambdaName,
                LambdaGenericArguments,
                LambdaArguments.Select(x => x.Clone()).ToImmutableList());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic method.
    /// </summary>
    public string LambdaName { get; }

    /// <summary>
    /// The collection of generic arguments of the given method, or an empty list if any.
    /// </summary>
    public IImmutableList<Type> LambdaGenericArguments { get; }

    /// <summary>
    /// The collection of arguments to use with the given method, or an empty list if any.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaArguments { get; }
}