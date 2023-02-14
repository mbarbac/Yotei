namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// The level at which syntax nodes are captured by a tree-oriented generator.
/// </summary>
public enum TreeLevel
{
    /// <summary>
    /// Capture is performed at type level.
    /// </summary>
    Type,

    /// <summary>
    /// Capture is performed at property level.
    /// </summary>
    Property,

    /// <summary>
    /// Capture is performed at field level.
    /// </summary>
    Field,

    /// <summary>
    /// Capture is performed at property of field level.
    /// </summary>
    PropertyOrField,
}