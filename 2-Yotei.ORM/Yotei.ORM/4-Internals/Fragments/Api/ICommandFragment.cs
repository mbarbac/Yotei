namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the information captured for a given command fragment.
/// </summary>
public partial interface ICommandFragment
{
    /// <summary>
    /// Returns a copy of this instance but associated with the given command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    ICommandFragment Clone(ICommand command);

    /// <summary>
    /// The command this instance is associated with.
    /// </summary>
    ICommand Command { get; }
}