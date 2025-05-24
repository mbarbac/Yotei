namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="ICommandEnumerator"/>
public class CommandEnumerator : ORM.Code.CommandEnumerator, ICommandEnumerator
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    public CommandEnumerator(IEnumerableCommand command, CancellationToken token = default)
        : base(command, token)
    {
        if (command.Connection is not IConnection)
            throw new ArgumentException(
                "Command's connection is not a relational one.")
                .WithData(command)
                .WithData(command.Connection);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Relational.Enumerator({Command})";

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override ISchema? OnInitialize() => throw null;

    /// <inheritdoc/>
    protected override IRecord? OnNextResult() => throw null;

    /// <inheritdoc/>
    protected override void OnTerminate() => throw null;

    /// <inheritdoc/>
    protected override void OnAbort() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override ValueTask<ISchema?> OnInitializeAsync() => throw null;

    /// <inheritdoc/>
    protected override ValueTask<IRecord?> OnNextResultAsync() => throw null;

    /// <inheritdoc/>
    protected override ValueTask OnTerminateAsync() => throw null;

    /// <inheritdoc/>
    protected override ValueTask OnAbortAsync() => throw null;
}