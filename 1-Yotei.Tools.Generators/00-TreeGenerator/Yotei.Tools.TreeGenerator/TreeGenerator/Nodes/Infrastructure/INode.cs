namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents the result of transforming a potential syntax node candidate into either a tree
/// oriented source code generator node, or into an error condition to be reported.
/// </summary>
/// NOTE: Types that implement this interface must implement their equatable capabilities in
/// a incremental generator cache friendly manner.
public interface INode : IEquatable<INode> { }