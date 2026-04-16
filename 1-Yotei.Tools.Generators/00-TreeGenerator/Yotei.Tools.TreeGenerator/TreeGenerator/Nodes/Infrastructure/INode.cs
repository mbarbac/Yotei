namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a source code generation node captured in the transform phase, including error
/// conditions to be later reported at source code generation time.
/// <para>
/// Types that implement this interface shall implement their <see cref="IEquatable{T}"/>
/// capabilities in a generator cache friendly manner.
/// </para>
/// </summary>
public interface INode : IEquatable<INode> { }