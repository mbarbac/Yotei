namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable<ICommand>]
[InheritWiths<ICommand>]
public partial class FakeCommand : Command
{
    public CommandInfo FakeInfo
    {
        get => _FakeInfo;
        set => _FakeInfo = value.ThrowWhenNull();
    }
    CommandInfo _FakeInfo = default!;

    // ----------------------------------------------------

    public FakeCommand(IConnection connection)
        : base(connection)
        => FakeInfo = new CommandInfo(connection.Engine);

    public FakeCommand(IConnection connection, string text, params object?[]? range)
        : base(connection)
        => FakeInfo = new CommandInfo(connection.Engine, text, range);

    protected FakeCommand(FakeCommand source)
        : base(source)
        => FakeInfo = (CommandInfo)source.FakeInfo.Clone();

    // ----------------------------------------------------

    public override bool IsValid => !FakeInfo.IsEmpty && FakeInfo.IsConsistent();

    public override ICommandInfo GetCommandInfo() => FakeInfo;

    public override ICommandInfo GetCommandInfo(bool _) => FakeInfo;

    public override ICommand Clear() { FakeInfo.Clear(); return this; }
}