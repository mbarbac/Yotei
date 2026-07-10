namespace Yotei.ORM.Records.Code;

partial class Record
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IRecord.IElement"/>
    /// </summary>
    public readonly struct Element : IRecord.IElement
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        [SuppressMessage("", "IDE0290")]
        public Element(object? value, ISchemaEntry entry)
        {
            Value = value;
            Entry = entry.ThrowWhenNull();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Entry?.Identifier?.Value ?? "-"}={Value.Sketch()}";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ISchemaEntry Entry { get; }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IRecord.IElement? other)
        {
            if (other is null) return false;
            if (!Entry.EqualsEx(other.Entry)) return false;
            if (!Value.EqualsEx(other.Value)) return false;
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => Equals(obj as IRecord.IElement);

        public static bool operator ==(Element? host, IRecord.IElement? item)
        {
            if (host is null && item is null) return true;
            if (host is null || item is null) return false;

            return host.Equals(item);
        }

        public static bool operator !=(Element? host, IRecord.IElement? item) => !(host == item);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var code = 0;
            code = HashCode.Combine(code, Value);
            code = HashCode.Combine(code, Entry);
            return code;
        }
    }
}