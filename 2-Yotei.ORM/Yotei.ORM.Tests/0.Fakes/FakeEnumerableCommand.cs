namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable<IEnumerableCommand>]
[InheritWiths<IEnumerableCommand>]
public partial class FakeEnumerableCommand : FakeCommand, IEnumerableCommand
{
    public object?[][] FakeArrays { get; set; } = [];
    public ISchema? FakeSchema { get; set; }

    // ----------------------------------------------------

    public FakeEnumerableCommand(IConnection connection) : base(connection) { }

    public FakeEnumerableCommand(
        IConnection connection, string text, params object?[]? range)
        : base(connection, text, range) { }

    protected FakeEnumerableCommand(FakeEnumerableCommand source) : base(source) { }

    public override IEnumerableCommand Clear() => (IEnumerableCommand)base.Clear();

    // ----------------------------------------------------

    public ICommandEnumerator GetEnumerator() => Connection.Records.CreateCommandEnumerator(this);
    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default) => Connection.Records.CreateCommandEnumerator(this, token);
    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(CancellationToken token) => GetAsyncEnumerator(token);

    // ----------------------------------------------------

    public bool SupportsNativePaging { get; set; } = false;
    public int Skip { get; set; } = -1;
    public int Take { get; set; } = -1;
}