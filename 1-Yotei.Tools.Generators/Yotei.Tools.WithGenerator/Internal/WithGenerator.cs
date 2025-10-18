namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Generates code to emulate the '<c>with</c>' keyword for the properties and fields decorated
/// with the '<see cref="WithAttribute"/>' attribute, or for the inherited members of non-record
/// types decorated with the '<see cref="InheritWithsAttribute"/>' one.
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class WithGenerator : TreeGenerator
{
#if DEBUG_WITH_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override Type[] TypeAttributes { get; } = [
        typeof(InheritWithsAttribute),
        typeof(InheritWithsAttribute<>)];

    /// <inheritdoc/>
    protected override Type[] PropertyAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>)];

    /// <inheritdoc/>
    protected override Type[] FieldAttributes { get; } = [typeof(WithAttribute)];
}