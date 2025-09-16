namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IEnumerableCommand{T}"/>
public class EnumerableCommand<T> : IEnumerableCommand<T>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    public EnumerableCommand(IEnumerableCommand command, Func<IRecord, T> converter)
    {
        Command = command.ThrowWhenNull();
        Converter = converter.ThrowWhenNull();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected EnumerableCommand(EnumerableCommand<T> source)
    {
        source.ThrowWhenNull();

        Command = source.Command.Clone();
        Converter = source.Converter;
    }

    /// <inheritdoc/>
    public override string ToString() => Command.ToString()!;

    /// <inheritdoc/>
    public virtual IEnumerableCommand<T> Clone() => new EnumerableCommand<T>(this);
    ICommand ICommand.Clone() => Clone();

    /// <inheritdoc/>
    public virtual IEnumerableCommand<T> WithConnection(
        IConnection value)
        => new EnumerableCommand<T>(this) { Command = Command.WithConnection(value) };
    ICommand ICommand.WithConnection(IConnection value) => WithConnection(value);

    /// <inheritdoc/>
    public virtual IEnumerableCommand<T> WithLocale(
        Locale value)
        => new EnumerableCommand<T>(this) { Command = Command.WithLocale(value) };
    ICommand ICommand.WithLocale(Locale value) => WithLocale(value);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandEnumerator<T> GetEnumerator() => new CommandEnumerator<T>(this);
    
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public ICommandEnumerator<T> GetAsyncEnumerator(
        CancellationToken token = default) => new CommandEnumerator<T>(this, token);
    
    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(
        CancellationToken token) => GetAsyncEnumerator(token);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IConnection Connection
    {
        get => Command.Connection;
        init => Command = Command.WithConnection(value);
    }

    /// <inheritdoc/>
    public Locale Locale
    {
        get => Command.Locale;
        init => Command = Command.WithLocale(value);
    }

    /// <inheritdoc/>
    public bool IsValid => Command.IsValid;

    /// <inheritdoc/>
    public IEnumerableCommand Command
    {
        get => _Command;
        init => _Command = value.ThrowWhenNull();
    }
    IEnumerableCommand _Command = default!;

    /// <inheritdoc/>
    public Func<IRecord, T> Converter { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo GetCommandInfo() => Command.GetCommandInfo();

    /// <inheritdoc/>
    public ICommandInfo GetCommandInfo(bool iterable) => Command.GetCommandInfo(iterable);

    // ----------------------------------------------------

    ICommand ICommand.Clear() => Command.Clear();
}