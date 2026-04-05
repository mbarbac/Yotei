namespace Yotei.Tools.Generators;

/* To DEBUG:
 * - Install the .NET Compiler SDK (in addition to Roslyn components).
 * - Make sure the derived generator project is a Roslyn component:
 *   <IsRoslynComponent>true</IsRoslynComponent>
 * - In the derived generator project's properties, add a Roslyn debug profile, whose target is the
 *   project that when compiled will be debugged (ie: a test project).
 * - Mark the derived generator project as the startup one.
 * - In the play button, select the debug profile.
 * - Click F5 (run) to compile (F6 does nothing).
 */

// ========================================================
/// <summary>
/// Represents a tree-oriented incremental source generator that, when capturing its relevant nodes
/// organizes them by the types they refer to (which either are the types by themselves, or the
/// declaring ones for its childs). Each node is then emitted in its own file.
/// <para>
/// Derived types need to be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. In is also expected that <see cref="LanguageNames.CSharp"/> is used
/// as its argument.
/// </para>
/// </summary>
internal partial class TreeGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and the <see cref="IsNullableAttribute"/>
    /// types are emitted in the namespace of the derived generator.
    /// </summary>
    protected virtual bool EmitNullabilityHelpers => true;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to register post-initialization actions such as reading external files, generating
    /// code for marker attributes, and so on. Marker attribute classes can be decorated with the
    /// '[Microsoft.CodeAnalysis.Embedded]' attribute, as it is automatically added to the emitted
    /// source code.
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context) { }
}