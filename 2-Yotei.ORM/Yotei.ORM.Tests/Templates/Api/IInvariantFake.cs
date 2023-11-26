namespace Yotei.ORM.Tests.Templates;

// ========================================================
/// <summary>
/// Represents a fake element in an invariant list.
/// </summary>
public interface IInvariantFake : IEquatable<IInvariantFake>
{
    /// <summary>
    /// The name by which this element is known.
    /// </summary>
    string Name { get; }
}