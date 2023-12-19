namespace Yotei.ORM.Relational.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ORM.Records.ICommandEnumerator"/>
/// </summary>
public class CommandEnumerator : ORM.Records.Code.CommandEnumerator
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    public CommandEnumerator(
        ORM.Records.ICommand command, CancellationToken token = default)
        : base(command, token) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to initialize the execution of the command.
    /// </summary>
    /// <returns></returns>
    protected override ISchema OnInitialize() => throw null;

    /// <summary>
    /// Invoked to obtain the result of the next iteration of the command.
    /// </summary>
    /// <returns></returns>
    protected override IRecord? OnNextResult() => throw null;

    /// <summary>
    /// Invoked to terminate the execution of the command.
    /// </summary>
    protected override void OnTerminate() => throw null;

    /// <summary>
    /// Invoked to abort the execution of the command.
    /// </summary>
    protected override void OnAbort() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to initialize the execution of the command.
    /// </summary>
    /// <returns></returns>
    protected override ValueTask<ISchema> OnInitializeAsync() => throw null;

    /// <summary>
    /// Invoked to obtain the result of the next iteration of the command.
    /// </summary>
    /// <returns></returns>
    protected override ValueTask<IRecord?> OnNextResultAsync() => throw null;

    /// <summary>
    /// Invoked to terminate the execution of the command.
    /// </summary>
    protected override ValueTask OnTerminateAsync() => throw null;

    /// <summary>
    /// Invoked to abort the execution of the command.
    /// </summary>
    protected override ValueTask OnAbortAsync() => throw null;
}