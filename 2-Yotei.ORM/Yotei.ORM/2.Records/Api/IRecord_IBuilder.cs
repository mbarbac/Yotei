namespace Yotei.ORM.Records;

partial interface IRecord
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IRecord"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<object?>
    {
        /// <summary>
        /// Returns a new host instance based upon the contents of this builder.
        /// </summary>
        /// <param name="withSchema"></param>
        /// <returns></returns>
        IRecord ToInstance(bool withSchema = true);
    }
}