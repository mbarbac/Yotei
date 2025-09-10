namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IEnumerableCommand"/>
[Cloneable<IEnumerableCommand>]
[InheritWiths<IEnumerableCommand>]
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
        => Connection.Records.CreateCommandEnumerator(this);

    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public virtual ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default)
        => Connection.Records.CreateCommandEnumerator(this, token);

    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(
        CancellationToken token) => GetAsyncEnumerator(token);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool SupportsNativePaging { get; init; }

    /// <inheritdoc/>
    public int Skip { get; set; }

    /// <inheritdoc/>
    public int Take { get; set; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract override IEnumerableCommand Clear();
}