namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// Represents a builder for the information needed to execute a database command.
/// </summary>
public class CommandInfoBuilder
{
    StringBuilder _Text;
    ParameterListBuilder _Parameters;

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine => _Parameters.Engine;

    /// <summary>
    /// The text of the command.
    /// </summary>
    public string Text => _Text.ToString();

    public IParameterList Parameters => null;
}