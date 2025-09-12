
namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ICommandEnumerator{T}"/>
public class CommandEnumerator<T> : DisposableClass, ICommandEnumerator<T>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    public CommandEnumerator(IEnumerableCommand<T> command, CancellationToken token = default)
    {
        Command = command.ThrowWhenNull();
        CancellationToken = token;
    }

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        Enumerator?.Dispose();
        Enumerator = null;
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        if (Enumerator is not null) await Enumerator.DisposeAsync().ConfigureAwait(false);
        Enumerator = null;
    }

    /// <inheritdoc/>
    public override string ToString() => Command.ToString()!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEnumerableCommand<T> Command { get; }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <inheritdoc/>
    public T? Current
    {
        get
        {
            var record = Enumerator?.Current;
            if (record is null) return default;

            var item = Command.Converter(record, Schema);
            return item;
        }
    }
    object? IEnumerator.Current => Current;

    /// <inheritdoc/>
    public ISchema? Schema => Enumerator?.Schema;

    // ----------------------------------------------------

    ICommandEnumerator? Enumerator = null;

    /// <inheritdoc/>
    public void Reset()
    {
        Enumerator?.Reset();
        Enumerator = null;
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        Enumerator ??= Command.Command.GetEnumerator();
        return Enumerator.MoveNext();
    }

    /// <inheritdoc/>
    public async ValueTask<bool> MoveNextAsync()
    {
        Enumerator ??= Command.Command.GetAsyncEnumerator(CancellationToken);
        return await Enumerator.MoveNextAsync();
    }
}