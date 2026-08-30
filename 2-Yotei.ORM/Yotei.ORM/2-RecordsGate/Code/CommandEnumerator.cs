namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandEnumerator"/>
/// </summary>
public abstract class CommandEnumerator : DisposableClass, ICommandEnumerator
{
    bool _CaptureSchema = false;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    [SuppressMessage("", "IDE0290")]
    public CommandEnumerator(IEnumerableCommand command, CancellationToken token = default)
    {
        Command = command.ThrowWhenNull();
        CancellationToken = token;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"><inheritdoc/></param>
    protected override void OnDispose(bool disposing) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    protected override ValueTask OnDisposeAsync(bool disposing) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerableCommand Command { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IRecord? Current { get; private set; }
    object IEnumerator.Current => Current!;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ISchema? Schema { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual ICommandEnumerator CaptureSchema(bool value)
    {
        _CaptureSchema = value;
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    public void CaptureSchema(out bool value) { value = _CaptureSchema; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract void Reset();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract bool MoveNext();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public abstract ValueTask<bool> MoveNextAsync();
}