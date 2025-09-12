namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IEnumerableCommand{T}"/>
[Cloneable(ReturnType = typeof(IEnumerableCommand<>))]
[InheritWiths(ReturnType = typeof(IEnumerableCommand<>))]
public partial class EnumerableCommand<T> : IEnumerableCommand<T>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="converter"></param>
    public EnumerableCommand(IEnumerableCommand command, Func<IRecord, ISchema?, T> converter)
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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandEnumerator<T> GetEnumerator() => new CommandEnumerator<T>(this);
    IEnumerator<T?> IEnumerable<T?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public ICommandEnumerator<T> GetAsyncEnumerator(CancellationToken token = default) => new CommandEnumerator<T>(this, token);
    IAsyncEnumerator<T?> IAsyncEnumerable<T?>.GetAsyncEnumerator(
        CancellationToken token) => GetAsyncEnumerator(token);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEnumerableCommand Command { get; }

    /// <inheritdoc/>
    public Func<IRecord, ISchema?, T> Converter { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IConnection Connection
    {
        get => Command.Connection;
        init => throw new InvalidOperationException("Use the original command.");
    }

    /// <inheritdoc/>
    public Locale Locale
    {
        get => Command.Locale;
        init => throw new InvalidOperationException("Use the original command.");
    }

    /// <inheritdoc/>
    public bool IsValid => Command.IsValid;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo GetCommandInfo() => Command.GetCommandInfo();

    /// <inheritdoc/>
    public ICommandInfo GetCommandInfo(bool iterable) => Command.GetCommandInfo(iterable);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEnumerableCommand<T> Clear()
    {
        Command.Clear();
        return this;
    }
    ICommand ICommand.Clear() => Clear();
}