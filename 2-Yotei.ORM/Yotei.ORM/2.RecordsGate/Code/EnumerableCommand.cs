namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IEnumerableCommand"/>
public abstract class EnumerableCommand : Command, IEnumerableCommand
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
        Skip = source.Skip;
        Take = source.Take;
    }

    /// <inheritdoc/>
    public abstract override IEnumerableCommand Clone();

    /// <inheritdoc/>
    public abstract override IEnumerableCommand WithConnection(IConnection value);

    /// <inheritdoc/>
    public abstract override IEnumerableCommand WithLocale(Locale value);

    /// <inheritdoc/>
    public abstract IEnumerableCommand WithSkip(int value);

    /// <inheritdoc/>
    public abstract IEnumerableCommand WithTake(int value);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual ICommandEnumerator GetEnumerator()
        => Connection.Records.CreateCommandEnumerator(this);

    IEnumerator<IRecord> IEnumerable<IRecord>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public virtual ICommandEnumerator GetAsyncEnumerator(
        CancellationToken token = default)
        => Connection.Records.CreateCommandEnumerator(this, token);

    IAsyncEnumerator<IRecord> IAsyncEnumerable<IRecord>.GetAsyncEnumerator(
        CancellationToken token)
        => GetAsyncEnumerator(token);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract bool SupportsNativePaging { get; }

    /// <inheritdoc/>
    public int Skip
    {
        get => _Skip;
        set => _Skip = value >= 0 ? value
            : throw new ArgumentOutOfRangeException(nameof(value)).WithData(value);
    }
    int _Skip = 0;

    /// <inheritdoc/>
    public int Take
    {
        get => _Take;
        set => _Take = value >= 0 ? value
            : throw new ArgumentOutOfRangeException(nameof(value)).WithData(value);
    }
    int _Take = 0;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IEnumerableCommand Clear()
    {
        Take = 0;
        Skip = 0;
        return this;
    }
}