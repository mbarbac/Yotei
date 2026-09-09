namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandEnumerator{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class CommandEnumerator<T> : DisposableClass, ICommandEnumerator<T>
{
    ICommandEnumerator _Enumerator;
    bool _Cached;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    /// <param name="token"></param>
    public CommandEnumerator(
        IEnumerableCommand command, Func<dynamic, T> converter,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(converter);

        _Enumerator = command.Connection.Records.CreateEnumerator(command, token);
        _Cached = false;
        Current = default;
        Converter = converter;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"><inheritdoc/></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || disposing) return;

        _Enumerator?.Dispose();
        _Enumerator = null!;
        _Cached = false;
        Converter = null!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || disposing) return;

        if (_Enumerator != null) await _Enumerator.DisposeAsync().ConfigureAwait(false);
        _Enumerator = null!;
        _Cached = false;
        Converter = null!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerableCommand Command => _Enumerator.Command;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CancellationToken CancellationToken => _Enumerator.CancellationToken;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<dynamic, T> Converter { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public T? Current
    {
        get
        {
            if (!_Cached)
            {
                var record = _Enumerator.Current;
                field = record is null ? default : Converter(record);
                _Cached = true;
            }
            return field;
        }
    }
    object IEnumerator.Current => Current!;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ICommandEnumerator<T> WithSchema(bool value)
    {
        _Enumerator.WithSchema(value);
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public void WithSchema(out bool value) => _Enumerator.WithSchema(out value);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Reset()
    {
        _Cached = false;
        _Enumerator.Reset();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public bool MoveNext()
    {
        _Cached = false;
        return _Enumerator.MoveNext();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask<bool> MoveNextAsync()
    {
        _Cached = false;
        return await _Enumerator.MoveNextAsync().ConfigureAwait(false);
    }
}