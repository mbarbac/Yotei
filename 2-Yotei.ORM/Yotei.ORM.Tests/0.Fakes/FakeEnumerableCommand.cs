namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable]
[InheritWiths]
public partial class FakeEnumerableCommand : EnumerableCommand
{
    public FakeEnumerableCommand(IConnection connection) : base(connection) { }

    protected FakeEnumerableCommand(FakeEnumerableCommand source) : base(source) { }

    // ----------------------------------------------------

    public override bool IsValid => true;

    public override CommandInfo GetCommandInfo() => throw new NotImplementedException();

    public override CommandInfo GetCommandInfo(bool _) => throw new NotImplementedException();
}