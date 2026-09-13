namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents an implementation of the visitor pattern for lambda node chains.
/// </summary>
public class LambdaNodeVisitor
{
    /// <summary>
    /// Visits the given lambda node.
    /// <br/> It is assumed that the node to visit is the result of a parsing operation, so it
    /// being the last one in a node chain.
    /// </summary>
    /// <param name="node"></param>
    public virtual void Visit(LambdaNode node)
    {
        switch (node)
        {
            case LambdaNodeArgument item: VisitArgument(item); break;
            case LambdaNodeBinary item: VisitBinary(item); break;
            case LambdaNodeConvert item: VisitConvert(item); break;
            case LambdaNodeIndexed item: VisitIndexed(item); break;
            case LambdaNodeInvoke item: VisitInvoke(item); break;
            case LambdaNodeMember item: VisitMember(item); break;
            case LambdaNodeMethod item: VisitMethod(item); break;
            case LambdaNodeSetter item: VisitSetter(item); break;
            case LambdaNodeUnary item: VisitUnary(item); break;
            case LambdaNodeValue item: VisitValue(item); break;
            case LambdaNodeCoalesce item: VisitCoalesce(item); break;
            case LambdaNodeTernary item: VisitTernary(item); break;
            default: VisitUnknown(node); break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitArgument(LambdaNodeArgument node) { }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitBinary(LambdaNodeBinary node)
    {
        Visit(node.LambdaLeft);
        Visit(node.LambdaRight);
    }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitConvert(LambdaNodeConvert node)
    {
        Visit(node.LambdaTarget);
    }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitIndexed(LambdaNodeIndexed node)
    {
        foreach (var item in node.LambdaIndexes) Visit(item);
        Visit(node.LambdaHost);
    }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitInvoke(LambdaNodeInvoke node)
    {
        foreach (var item in node.LambdaArguments) Visit(item);
        Visit(node.LambdaHost);
    }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitMember(LambdaNodeMember node)
    {
        Visit(node.LambdaHost);
    }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitMethod(LambdaNodeMethod node)
    {
        foreach (var item in node.LambdaArguments) Visit(item);
        Visit(node.LambdaHost);
    }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitSetter(LambdaNodeSetter node)
    {
        Visit(node.LambdaTarget);
        Visit(node.LambdaValue);
    }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitUnary(LambdaNodeUnary node)
    {
        Visit(node.LambdaTarget);
    }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitValue(LambdaNodeValue node) { }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitCoalesce(LambdaNodeCoalesce node)
    {
        Visit(node.LambdaLeft);
        Visit(node.LambdaRight);
    }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitTernary(LambdaNodeTernary node)
    {
        Visit(node.LambdaLeft);
        Visit(node.LambdaMiddle);
        Visit(node.LambdaRight);
    }

    /// <summary>
    /// Visits the given node.
    /// </summary>
    /// <param name="node"></param>
    protected virtual void VisitUnknown(LambdaNode node) { }
}