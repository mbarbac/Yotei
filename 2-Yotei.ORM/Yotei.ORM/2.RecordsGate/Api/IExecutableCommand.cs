namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented database command that, when executed using a given connection,
/// produces an integer as the result of that execution.
/// </summary>
public partial interface IExecutableCommand : ICommand
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    new IExecutableCommand Clone();

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    new IExecutableCommand WithConnection(IConnection value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    new IExecutableCommand WithLocale(Locale value);

    // ----------------------------------------------------

    /// <summary>
    /// Returns an object that can execute this command and produce the integer that results from
    /// that execution.
    /// </summary>
    /// <returns></returns>
    ICommandExecutor GetExecutor();

    // ----------------------------------------------------

    /// <inheritdoc cref="ICommand.Clear"/>
    new IExecutableCommand Clear();
}