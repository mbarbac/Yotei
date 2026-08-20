namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an object that can execute its associated command, enumerating the records produced
/// by that execution.
/// </summary>
public interface ICommandEnumerator
    : IEnumerator<IRecord?>, IAsyncEnumerator<IRecord?>
    , IDisposableEx
{
    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    IEnumerableCommand Command { get; }

    /// <summary>
    /// The cancellation token passed to this instance.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// The element at the current position of this enumerator, or <see langword="null"/> if it
    /// has not been yet executed, or if there are no more records available.
    /// </summary>
    new IRecord? Current { get; }

    /// <summary>
    /// The schema that describes the records produced by this instance, or <see langword="null"/>
    /// if the associated command has not been executed yet, or if the schema is not available.
    /// </summary>
    ISchema? Schema { get; }

    /// <summary>
    /// Determines if this instance must capture the schema of the records produced by the
    /// execution of the associated command. The default value is <see langword="false"/>.
    /// <br/> Returns a reference to itself to support a fluent syntax usage.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ICommandEnumerator CaptureSchema(bool value);

    /// <summary>
    /// Obtains the current value of this setting.
    /// </summary>
    /// <param name="value"></param>
    void CaptureSchema(out bool value);
}