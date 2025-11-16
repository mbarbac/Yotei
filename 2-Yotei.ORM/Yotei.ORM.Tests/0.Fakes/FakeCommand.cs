namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable<ICommand>]
[InheritWiths<ICommand>]
public partial class FakeCommand : Command
{
    public CommandInfo FakeInfo { get; set => field = value.ThrowWhenNull(); }

    // ----------------------------------------------------

    public FakeCommand(IConnection connection)
        : base(connection)
        => FakeInfo = new(connection);

    public FakeCommand(IConnection connection, string? text, params object?[]? range)
        : base(connection)
        => FakeInfo = new(connection, text, range);

    protected FakeCommand(FakeCommand source)
        : base(source)
        => FakeInfo = new(source, iterable: false);

    // ----------------------------------------------------

    public override bool IsValid => !FakeInfo.IsEmpty && FakeInfo.IsConsistent;

    public override ICommandInfo GetCommandInfo() => FakeInfo;

    public override ICommandInfo GetCommandInfo(bool _) => FakeInfo;

    public override ICommand Clear()
    {
        FakeInfo.Clear();
        return this;
    }
}