namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ability of parsing dynamic lambda expressions, defined as lambda expressions
/// having dynamic arguments, returning the chain of dynamic operations bounded to them.
/// </summary>
public class LambdaParser
{
    // ----------------------------------------------------

    /// <summary>
    /// For the purposes of <see cref="LambdaParser"/> the DLR acts as a unique shared resource
    /// whose state must be isolated each run of the parser. The <c>Parse(...)</c> method waits
    /// until and exclusive lock can be captured on this object.
    /// </summary>
    public static object SyncRoot { get; } = new();


}