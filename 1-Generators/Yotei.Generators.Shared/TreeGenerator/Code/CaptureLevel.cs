namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// The level at which a generator captures elements for code generation purposes.
/// </summary>
internal enum CaptureLevel
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