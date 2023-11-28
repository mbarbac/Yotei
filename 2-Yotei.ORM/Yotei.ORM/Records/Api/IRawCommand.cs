namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a records-oriented command whose contents are explicitly set.
/// </summary>
public interface IRawCommand : IEnumerableCommand<IRecord>, IExecutableCommand
{
    /// <summary>
    /// Adds to this command the given text and optional arguments. If used, the arguments must
    /// be encoded in the text using either a standard '{n}' positional format, or a '{name}' one.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    IRawCommand Add(string specs, params object?[] args);

    /// <summary>
    /// Adds to this command the contents obtained from parsing the given dynamic lambda expression.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    IRawCommand Add(Func<dynamic, object> specs);

    /// <summary>
    /// Clears this command.
    /// <br/> Returns this instance so that it can be used in a fluent-syntax chain.
    /// </summary>
    /// <returns></returns>
    new IRawCommand Clear();
}