namespace Yotei.ORM.Tests;

// ========================================================
public partial class FakeCommand : EnumerableCommand, ICommand, IExecutableCommand
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

    public override FakeCommand Clone() => new(this);
    IExecutableCommand IExecutableCommand.Clone() => Clone();

    public override FakeCommand WithConnection(IConnection value) => new(this) { Connection = value };
    IExecutableCommand IExecutableCommand.WithConnection(IConnection value) => WithConnection(value);

    public override FakeCommand WithLocale(Locale value) => new(this) { Locale = value };
    IExecutableCommand IExecutableCommand.WithLocale(Locale value) => WithLocale(value);

    public override FakeCommand WithSkip(int value) => new(this) { Skip = value };

    public override FakeCommand WithTake(int value) => new(this) { Take = value };

    // ----------------------------------------------------

    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    public override bool SupportsNativePaging => _SupportsNativePaging;

    public FakeCommand SetSupportsNativePaging(bool value) { _SupportsNativePaging = value; return this; }
    bool _SupportsNativePaging;

    // ----------------------------------------------------

    public override bool IsValid => !FakeInfo.IsEmpty && FakeInfo.IsConsistent();

    public override ICommandInfo GetCommandInfo() => FakeInfo;

    public override ICommandInfo GetCommandInfo(bool _) => FakeInfo;

    // ----------------------------------------------------

    public override FakeCommand Clear()
    {
        FakeInfo.Clear();
        FakeArrays = [];
        FakeSchema = FakeSchema?.Clear();

        return this;
    }
    IExecutableCommand IExecutableCommand.Clear() => Clear();
}