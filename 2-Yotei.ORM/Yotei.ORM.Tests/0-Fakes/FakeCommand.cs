namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable(ReturnType = typeof(ICommand))]
public partial class FakeCommand : Command
{
    public FakeCommand(IConnection connection)
        : base(connection)
        => FakeInfo = new(connection.Engine);

    protected FakeCommand(FakeCommand other)
        : base(other)
        => FakeInfo = other.FakeInfo;

    // ----------------------------------------------------

    public CommandInfo FakeInfo { get; set => field = value.ThrowWhenNull(); }

    public override bool IsValid => !FakeInfo.IsEmpty;
    public override ICommandInfo GetCommandInfo() => FakeInfo;
    public override ICommandInfo GetCommandInfo(bool _) => FakeInfo;
    public override ICommand Clear() { FakeInfo.Clear(); return this; }
}