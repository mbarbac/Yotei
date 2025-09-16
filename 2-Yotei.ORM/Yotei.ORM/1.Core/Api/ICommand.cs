namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a database command that can be executed on its associated connection.
/// <br/> Instances of this type typically are mutables ones.
/// </summary>
public interface ICommand
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    ICommand Clone();

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    ICommand WithConnection(IConnection value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    ICommand WithLocale(Locale value);

    // ----------------------------------------------------

    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// The locale to use with culture-sensitive objects in the underlying database.
    /// </summary>
    Locale Locale { get; }

    /// <summary>
    /// Determines if this instance is in an execution-ready state, or not.
    /// </summary>
    bool IsValid { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the information needed to execute this command, using its default iterable mode.
    /// </summary>
    /// <returns></returns>
    ICommandInfo GetCommandInfo();

    /// <summary>
    /// Obtains the information needed to execute this command, using the given iterable mode
    /// (or its default one if different modes are not supported).
    /// </summary>
    /// <param name="iterable"></param>
    /// <returns></returns>
    ICommandInfo GetCommandInfo(bool iterable);

    // ----------------------------------------------------

    /// <summary>
    /// Clears all the contents captured by this instance.
    /// <br/> Returns a reference to itself to support fluent syntax usage.
    /// </summary>
    /// <returns></returns>
    ICommand Clear();
}