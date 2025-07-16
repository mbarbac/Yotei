namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IEnumerableCommand"/>
[Cloneable]
[InheritWiths]
public abstract partial class EnumerableCommand : Command, IEnumerableCommand
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public EnumerableCommand(IConnection connection) : base(connection) { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected EnumerableCommand(EnumerableCommand source) : base(source)
    {
        SupportsNativePaging = source.SupportsNativePaging;
        Skip = source.Skip;
        Take = source.Take;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandEnumerator GetEnumerator()
    {
        var temp = Connection.Records.CreateCommandEnumerator(this);
        return temp;
    }
    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public virtual ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default)
    {
        var temp = Connection.Records.CreateCommandEnumerator(this, token);
        return temp;
    }
    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(
        CancellationToken token) => GetAsyncEnumerator(token);

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// We cannot determine up-front if the contents of this abstract command, along with the
    /// capabilities of the underlying database engine, will support native paging or not. So,
    /// this property is made virtual to be overriden as needed.
    /// <br/> Its default value is <c>false</c>, so even if the engine supports native paging,
    /// it will be emulated by the framework. If init-set to <c>true</c>, then the engine's value
    /// determines if it will be emulated or not.
    /// </remarks>
    public virtual bool SupportsNativePaging
    {
        get => _SupportsNativePaging;
        init => _SupportsNativePaging = value;
    }
    bool _SupportsNativePaging = false;

    /// <inheritdoc/>
    public int Skip
    {
        get => _Skip;
        set => _Skip = value >= 0 ? value : -1;
    }
    int _Skip = -1;

    /// <inheritdoc/>
    public int Take
    {
        get => _Take;
        set => _Take = value >= 0 ? value : -1;
    }
    int _Take = -1;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override EnumerableCommand Clear()
    {
        Skip = -1;
        Take = -1;

        return this;
    }
    IEnumerableCommand IEnumerableCommand.Clear() => Clear();
}