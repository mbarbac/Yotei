namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEnumerableCommand"/>
/// </summary>
[Cloneable(ReturnType = typeof(IEnumerableCommand))]
public abstract partial class EnumerableCommand : Command, IEnumerableCommand
{
    int _Skip = 0;
    int _Take = 0;

    /// <summary>
    /// Initializes a new instance.
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

    // HIGH: si se ha usado un Select de proyección, el enumerador tiene que devolver un enumerador
    // de proyección, NO un enumerador de records.

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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    public virtual IEnumerableCommand Select(Func<dynamic, object> spec) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spec"></param>
    /// <returns></returns>
    public virtual IEnumerableCommand Select<T>(Func<dynamic, T> spec) => throw null;

    // ----------------------------------------------------

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
        _Skip = value;
        return this;
    }

    ICommand IEnumerableCommand.Skip(int value) => Skip(value);

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
        _Take = value;
        return this;
    }

    ICommand IEnumerableCommand.Take(int value) => Take(value);

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
        _Skip = 0;
        _Take = 0;
        return this;
    }

    IEnumerableCommand IEnumerableCommand.Clear() => Clear();
}