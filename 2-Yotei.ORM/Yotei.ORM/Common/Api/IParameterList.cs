namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents the ordered collection of parameters in a command.
/// </summary>
public interface IParameterList : IEnumerable<IParameter>
{
}