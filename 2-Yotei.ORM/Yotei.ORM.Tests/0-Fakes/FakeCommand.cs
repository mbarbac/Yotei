namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable]
public partial class FakeCommand : Command
{
    public FakeCommand(IConnection connection)
        : base(connection)
        => FakeInfo = new(connection.Engine);

    public FakeCommand(IConnection connection, string? text, params object?[]? values)
        : this(connection)
        => FakeInfo = new(connection.Engine, text, values);

    protected FakeCommand(FakeCommand other)
        : base(other)
        => FakeInfo = (CommandInfo)other.FakeInfo.Clone();

    // ----------------------------------------------------

    public CommandInfo FakeInfo { get; set => field = value.ThrowWhenNull(); }
    public override ICommandInfo GetCommandInfo() => FakeInfo;
    public override ICommandInfo GetCommandInfo(bool _) => FakeInfo;
}