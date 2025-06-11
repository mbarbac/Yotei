namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions returning the last database-alike
/// token in the chain that contains the dynamic operations in that expression.
/// </summary>
public class DbLambdaParser
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public DbLambdaParser(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given dynamic lambda expression and returns the last database-alike token in
    /// the chain that contains the dynamic operations in that expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public IDbToken Parse<T>(Func<dynamic, T> expression)
    {
        expression.ThrowWhenNull();

        var method = expression.GetMethodInfo();

        throw null;
    }
}