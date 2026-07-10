namespace Yotei.ORM.Records;

partial interface IRecord
{
    // ====================================================
    /// <summary>
    /// Represents an element of a record, composed by its value and its associated metadata.
    /// </summary>
    public interface IElement : IEquatable<IElement>
    {
        /// <summary>
        /// The value carried by this element.
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// The metadata carried by this element.
        /// </summary>
        ISchemaEntry Entry { get; }
    }
}