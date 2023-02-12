namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a syntax node captured for code generation purposes.
/// </summary>
public interface ICaptured
{
    /// <summary>
    /// The semantic model when this instance was captured.
    /// </summary>
    SemanticModel SemanticModel { get; }

    /// <summary>
    /// The generator that captured this instance.
    /// </summary>
    IGenerator Generator { get; }

    /// <summary>
    /// The name of this instance.
    /// </summary>
    string Name { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate this instance and its contents, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked to generate the source code for this instance and its contents, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Print(SourceProductionContext context, CodeBuilder cb);
}