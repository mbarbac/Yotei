#pragma warning disable IDE0079
#pragma warning disable CA1816

namespace Yotei.ORM;

// ========================================================
/// <inheritdoc cref="ICommandEnumerator{T}"/>
public class CommandEnumerator<T> : DisposableClass, ICommandEnumerator<T>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="enumerator"></param>
    /// <param name="converter"></param>
    public CommandEnumerator(ICommandEnumerator enumerator, Func<IRecord, T> converter)
    {
        Enumerator = enumerator.ThrowWhenNull();
        Converter = converter.ThrowWhenNull();
    }

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (Enumerator is null) return;

        Enumerator.Dispose();
        Enumerator = null!;
        Converter = null!;

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected async override ValueTask OnDisposeAsync(bool disposing)
    {
        if (Enumerator is null) return;

        await Enumerator.DisposeAsync();
        Enumerator = null!;
        Converter = null!;

        GC.SuppressFinalize(this);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEnumerator<T?> GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;

    /// <inheritdoc/>
    public IAsyncEnumerator<T?> GetAsyncEnumerator(CancellationToken cancellationToken = default) => this;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandEnumerator Enumerator { get; private set; }

    /// <inheritdoc/>
    public IEnumerableCommand Command => Enumerator.Command;

    /// <inheritdoc/>
    public CancellationToken CancellationToken
    {
        get => Enumerator.CancellationToken;
        set => Enumerator.CancellationToken = value;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public T? Current { get; private set; }
    object? IEnumerator.Current => Current;

    /// <inheritdoc/>
    public Func<IRecord, T> Converter { get; private set; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Reset()
    {
        ThrowIfDisposed();

        Enumerator.Reset();
        Current = default;
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        if (Enumerator.MoveNext())
        {
            var record = Enumerator.Current;
            if (record is not null)
            {
                Current = Converter(record);
                if (Current is not null) return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> MoveNextAsync()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        if (await Enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var record = Enumerator.Current;
            if (record is not null)
            {
                Current = Converter(record);
                if (Current is not null) return true;
            }
        }

        return false;
    }
}