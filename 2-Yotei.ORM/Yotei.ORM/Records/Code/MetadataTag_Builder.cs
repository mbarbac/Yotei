namespace Yotei.ORM.Records.Code;
partial class MetadataTag
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IMetadataTag.IBuilder"/>
    /// </summary>
    [DebuggerDisplay("{ToDebugString(3)}")]
    [Cloneable(ReturnType = typeof(IMetadataTag.IBuilder))]
    public partial class Builder : IMetadataTag.IBuilder
    {
        readonly List<string> Items;

        /// <summary>
        /// Initializes a new instance with the given name as its default one.
        /// </summary>
        /// <param name="ignoreCase"></param>
        /// <param name="name"></param>
        public Builder(bool ignoreCase, string name)
        {
            IgnoreCase = ignoreCase;
            Items = [Validate(name)];
        }

        /// <summary>
        /// Initializes a new instance with the names from the given not-empty range.
        /// </summary>
        /// <param name="ignoreCase"></param>
        /// <param name="range"></param>
        public Builder(bool ignoreCase, IEnumerable<string> range)
        {
            IgnoreCase = ignoreCase;
            Items = [];

            AddRange(range);
            if (Count == 0) throw new EmptyException("Range of names is empty.");
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            IgnoreCase = other.IgnoreCase;
            Items = [.. other];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToDebugString(3);

        /// <summary>
        /// Returns a string representation of this instance for DEBUG purposes.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual string ToDebugString(int count) => Count < count
            ? $"'{string.Join(", ", Items)}'"
            : $"'{string.Join(", ", Items.Take(count))}, ...'";

        // ------------------------------------------------

        // Invoked to validate the given name.
        static string Validate(string name) => name.NotNullNotEmpty(trim: true);

        // Determines if the two given names shall be considered equal, or not.
        bool AreEqual(string xname, string yname) => string.Compare(xname, yname, IgnoreCase) == 0;

        // Returns the index of the given name, or -1 if it is not in this collection.
        int IndexOf(string name) => Items.FindIndex(x => AreEqual(x, name));

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IgnoreCase { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Default
        {
            get => Items[0];
            set
            {
                var index = IndexOf(value = Validate(value));
                if (index < 0) throw new NotFoundException("Tag name not found.").WithData(value);
                if (index != 0)
                {
                    Items.RemoveAt(index);
                    Items.Insert(0, value);
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count => Items.Count;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name) => IndexOf(Validate(name)) >= 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool ContainsAny(IEnumerable<string> range)
        {
            ArgumentNullException.ThrowIfNull(range);
            return range.Any(Contains);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public string[] ToArray() => [.. Items];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<string> ToList() => [.. Items];

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IMetadataTag ToInstance() => Count == 1
            ? new MetadataTag(IgnoreCase, Default)
            : new MetadataTag(IgnoreCase, this);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        /// <returns></returns>
        public virtual bool Replace(string oldname, string newname)
        {
            oldname = Validate(oldname);
            newname = Validate(newname);
            if (AreEqual(oldname, newname)) return false;

            var index = IndexOf(oldname);
            if (index < 0) return false;

            Items.RemoveAt(index);
            var temp = IndexOf(newname);
            if (temp >= 0) throw new DuplicateException("Tag name is duplicated.").WithData(newname);

            Items.Insert(index, newname);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool Add(string name)
        {
            var index = IndexOf(name = Validate(name));
            if (index >= 0) throw new DuplicateException("Tag name is duplicated.").WithData(name);

            Items.Add(name);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<string> range)
        {
            ArgumentNullException.ThrowIfNull(range);

            var done = false; foreach (var value in range) if (Add(value)) done = true;
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool Remove(string name)
        {
            var index = IndexOf(Validate(name));
            if (index >= 0)
            {
                if (Count == 1) throw new InvalidOperationException(
                    "Cannot remove the unique tag name.")
                    .WithData(this);

                Items.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear()
        {
            if (Count <= 1) return false;

            Items.RemoveRange(1, Count - 1);
            return true;
        }
    }
}