namespace Yotei.ORM.Tools.Code.Templates;

// ========================================================
/// <summary>
/// Represents an element in a collection.
/// </summary>
public interface IElement : IEquatable<IElement>
{
    /// <summary>
    /// The name of this element.
    /// </summary>
    string Name { get; }
}