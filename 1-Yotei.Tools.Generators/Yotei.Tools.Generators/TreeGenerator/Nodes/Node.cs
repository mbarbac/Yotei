namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal abstract class Node
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="generator"></param>
    public Node(TreeGenerator generator) => Generator = generator.ThrowWhenNull(nameof(generator));

    /// <summary>
    /// The generator this instance is associated with.
    /// </summary>
    public TreeGenerator Generator { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the contents of this instance, and its child elements, if any.
    /// Returns true if it is valid for source generation purposes, of false otherwise.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public abstract bool Validate(SourceProductionContext context);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to append to the given code builder the contents of this instance, and its
    /// child elements, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public abstract void Print(SourceProductionContext context, CodeBuilder cb);
}