namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEnumerableCommand"/>
/// </summary>
[Cloneable(ReturnType = typeof(IEnumerableCommand))]
public abstract partial class EnumerableCommand : Command, IEnumerableCommand
{
    /// <summary>
    /// Initilizes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public EnumerableCommand(IConnection connection) : base(connection) { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected EnumerableCommand(EnumerableCommand other) : base(other)
    {
        _Skip = other._Skip;
        _Take = other._Take;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandEnumerator GetEnumerator() => Connection.Records.CreateEnumerator(this);
    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual ICommandEnumerator GetAsyncEnumerator(
        CancellationToken token = default) => Connection.Records.CreateEnumerator(this, token);

    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(
        CancellationToken cancellationToken)
        => GetAsyncEnumerator(cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public ICommandEnumerator<T> Select<T>(
        Func<dynamic, T> converter,
        CancellationToken token = default) => new CommandEnumerator<T>(this, converter, token);

    // ----------------------------------------------------

    int _Skip = -1, _Take = -1;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract bool SupportsNativePaging { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IEnumerableCommand Skip(int value)
    {
        _Skip = value < 0 ? -1 : value;
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    public void Skip(out int value) { value = _Skip; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IEnumerableCommand Take(int value)
    {
        _Take = value < 0 ? -1 : value;
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    public void Take(out int value) { value = _Take; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override IEnumerableCommand Clear()
    {
        _Skip = -1;
        _Take = -1;
        return this;
    }
}