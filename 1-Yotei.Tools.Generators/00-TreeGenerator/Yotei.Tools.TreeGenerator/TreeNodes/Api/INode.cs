namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents the result of transforming a potential syntax node candidate into either a tree
/// oriented source code generator node, or into an error condition to be reported.
/// </summary>
/// NOTE: Types that implement this interface must implement their equatable capabilities in an
/// incremental generator friendly manner, which typically means to only cache the static info
/// that mey later be needed (or values with custom equality capabilities).
internal interface INode : IEquatable<INode> { }