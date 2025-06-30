namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable]
[InheritWiths]
public partial class FakeCommand : Command
{
    public CommandInfo FakeInfo
    {
        get => _FakeInfo;
        set => _FakeInfo = value.ThrowWhenNull();
    }
    CommandInfo _FakeInfo;

    // ----------------------------------------------------

    public FakeCommand(IConnection connection)
        : base(connection)
        => _FakeInfo = new CommandInfo(connection.Engine);

    public FakeCommand(IConnection connection, string? text, params object?[]? range)
        : base(connection)
        => _FakeInfo = new CommandInfo(connection.Engine, text, range);

    protected FakeCommand(FakeCommand source)
        : base(source.Connection)
        => _FakeInfo = source.FakeInfo.Clone();

    public override bool IsValid => !FakeInfo.IsEmpty;

    public override CommandInfo GetCommandInfo() => _FakeInfo;

    public override CommandInfo GetCommandInfo(bool _) => _FakeInfo;

    public override FakeCommand Clear() => new(Connection);
}