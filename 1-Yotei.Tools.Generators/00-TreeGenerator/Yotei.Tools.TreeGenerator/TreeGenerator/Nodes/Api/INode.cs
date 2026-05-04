namespace Yotei.Tools.Generators;

/* NOTE: Types that implement this interface must also implement its equatable capabilities in
 * an incremental generator friendly manner. This typically means to cache only the static info
 * they may later need (or values with custom equality capabilities).
 */

// ========================================================
/// <summary>
/// Represents the result of transforming a potential syntax node candidate into either a tree
/// oriented source code generator node, or into an error condition to be reported.
/// </summary>
public interface INode : IEquatable<INode> { }