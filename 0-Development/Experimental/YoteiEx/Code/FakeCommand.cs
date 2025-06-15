namespace Experimental.YoteiEx;

// ========================================================
[Cloneable]
[InheritWiths]
public partial class FakeCommand : Command, IEnumerableCommand
{
    public CommandInfo FakeInfo
    {
        get => _FakeInfo;
        set => _FakeInfo = value.ThrowWhenNull();
    }
    CommandInfo _FakeInfo;

    public FakeCommand(IConnection connection)
        : base(connection)
        => _FakeInfo = new CommandInfo(connection.Engine);

    public FakeCommand(IConnection connection, string? text, params object?[]? range)
        : base(connection)
        => _FakeInfo = new CommandInfo(connection.Engine, text, range);

    protected FakeCommand(FakeCommand source)
        : base(source.Connection)
        => _FakeInfo = source.FakeInfo.Clone();

    public override CommandInfo GetCommandInfo() => _FakeInfo;

    public override CommandInfo GetCommandInfo(bool _) => _FakeInfo;

    public override FakeCommand Clear() => new(Connection);

    // ----------------------------------------------------

    public virtual ICommandEnumerator GetEnumerator() => throw null;
    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public virtual ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default) => throw null;
    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(
         CancellationToken token)
         => GetAsyncEnumerator(token);

    // ----------------------------------------------------

    public bool SupportsNativePaging => throw null;

    public int Skip
    {
        get => throw null;
        set => throw null;
    }

    public int Take
    {
        get => throw null;
        set => throw null;
    }

    // ----------------------------------------------------

    //public virtual FakeCommand WithConverter<T>(Func<IRecord, T> converter) => throw null;
    //IEnumerableCommand IEnumerableCommand.WithConverter<T>(Func<IRecord, T> converter) => WithConverter(converter);

    //public virtual FakeCommand WithConverter(object spec) => throw null;
    //IEnumerableCommand IEnumerableCommand.WithConverter(object spec) => WithConverter(spec);
}