namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// ....
/// </summary>
internal class CloneGenerator : TreeGenerator
{
#if DEBUG_CLONE_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [typeof(CloneableAttribute)];
}