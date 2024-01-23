namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents the source code hierarchy associated to a given run of a tree generator.
/// </summary>
internal class Hierarchy : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="generator"></param>
    public Hierarchy(TreeGenerator generator)
    {
        Generator = generator.ThrowWhenNull();
        FileChildren = new(this);
    }

    /// <summary>
    /// The generator this instance is associated with.
    /// </summary>
    public TreeGenerator Generator { get; }

    /// <summary>
    /// The collection of file nodes registered into this instance.
    /// </summary>
    public FileChildrenList FileChildren { get; }
    public class FileChildrenList : ChildrenList<FileNode>
    {
        public FileChildrenList(Hierarchy master) : base(master)
        {
            ItemToDebug = (item) => item.FileName.Split('.')[0];
            Comparer = (x, y) => x.FileName == y.FileName;
        }
    }

    // ----------------------------------------------------

    Hierarchy INode.Hierarchy => this;

    INode? INode.ParentNode => null;

    bool INode.Validate(
        SourceProductionContext context)
        => throw new InvalidOperationException("Not supposed to be invoked.");

    void INode.Print(
        SourceProductionContext context, CodeBuilder cb)
        => throw new InvalidOperationException("Not supposed to be invoked.");
}