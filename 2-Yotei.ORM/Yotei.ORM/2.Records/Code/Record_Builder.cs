namespace Yotei.ORM.Records.Code;

partial class Record
{
    // ====================================================
    /// <inheritdoc cref="IBuilder"/>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : IRecord.IBuilder
    {
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) => throw null;

        /// <inheritdoc/>
        public IEnumerator<object?> GetEnumerator() => throw null;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() => throw null;

        // ------------------------------------------------

        /// <inheritdoc/>
        public IRecord ToInstance(bool withSchema = true) => throw null;
    }
}