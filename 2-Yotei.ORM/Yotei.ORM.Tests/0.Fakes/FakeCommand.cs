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
    CommandInfo _FakeInfo = default!;

    // ----------------------------------------------------

    public object?[][] FakeArrays { get; set; } = [];
    public ISchema? FakeSchema { get; set; }
    public int FakeDelayMs { get; set; }

    public int FakeExeResult { get; set; }

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

    public override FakeCommand Clear()
    {
        FakeInfo.Clear();
        FakeArrays = [];
        FakeSchema = FakeSchema?.Clear();

        return this;
    }
}