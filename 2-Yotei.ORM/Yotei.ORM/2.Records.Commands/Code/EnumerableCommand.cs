namespace Yotei.ORM;

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
    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(CancellationToken token) => GetAsyncEnumerator(token);

    /// <inheritdoc/>
    public ICommandEnumerator<T> SelectItems<T>(Func<IRecord, T> converter)
    {
        var iter = GetEnumerator();
        var temp = new CommandEnumerator<T>(iter, converter);
        return temp;
    }

    /// <inheritdoc/>
    public ICommandEnumerator<T> SelectItemsAsync<T>(Func<IRecord, T> converter, CancellationToken token = default)
    {
        var iter = GetAsyncEnumerator(token);
        var temp = new CommandEnumerator<T>(iter, converter);
        return temp;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// We cannot determine up-front if the contents of this abstract command, along with the
    /// capabilities of the underlying database engine, will support native paging or not. So,
    /// this property is made virtual to be overriden as needed, and in addition, it can be
    /// init-set to true/false.
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
}