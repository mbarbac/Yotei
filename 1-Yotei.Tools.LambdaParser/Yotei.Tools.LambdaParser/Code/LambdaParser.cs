namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions, defined as lambda expressions
/// having dynamic arguments, returning the chain of dynamic operations bounded to them.
/// </summary>
public class LambdaParser
{
    // ----------------------------------------------------

    /// <summary>
    /// Maitains the last dynamic node binded while parsing the given dynamic lambda expression.
    /// The binders are in charge of setting the value of this property as in some circumstances
    /// the DLR does it not.
    /// </summary>
    internal LambdaNode? LastNode { get; set; }

    /// <summary>
    /// Keeps track of the dynamic lambda nodes that shall be used as surrogates for the tracked
    /// objects.
    /// </summary>
    internal Dictionary<object, LambdaNode> Surrogates { get; } = [];
}