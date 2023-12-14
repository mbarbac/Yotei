namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a forward-only reader of the data produced by an enumerable command.
/// </summary>
public interface IReader : IEnumerable, IBaseDisposable { }