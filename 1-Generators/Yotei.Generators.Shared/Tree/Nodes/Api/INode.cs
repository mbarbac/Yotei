namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface INode
{
    /// <summary>
    /// The generator this instance refers to.
    /// </summary>
    IGenerator Generator { get; }

    /// <summary>
    /// The parent node of this instance, or null if it is the top-most one.
    /// </summary>
    INode? Parent { get; }

    /// <summary>
    /// The name of this instance.
    /// </summary>
    string Name { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate this instance and its contents.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked to append to the given code the source code of this instance.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Print(SourceProductionContext context, CodeBuilder cb);
}