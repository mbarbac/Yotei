namespace Yotei.ORM.Records.Code;

partial class MetadataTag
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IMetadataTag.IBuilder"/>
    /// </summary>
    [Cloneable]
    public partial class Builder : IMetadataTag.IBuilder
    {
        readonly List<string> Items;

        /// <summary>
        /// Initializes a new instance with the given default tag name.
        /// </summary>
        /// <param name="sensitive"></param>
        /// <param name="name"></param>
        public Builder(bool sensitive, string name)
        {
            CaseSensitiveTags = sensitive;
            Items = [Validate(name)];
        }

        /// <summary>
        /// Initializes a new instance with the names from the given range.
        /// </summary>
        /// <param name="sensitive"></param>
        /// <param name="range"></param>
        public Builder(bool sensitive, IEnumerable<string> range)
        {
            CaseSensitiveTags = sensitive;
            Items = [];

            AddRange(range);
            if (Items.Count == 0) throw new EmptyException("Range of tag names is empty.");
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source)
        {
            CaseSensitiveTags = source.ThrowWhenNull().CaseSensitiveTags;
            Items = [.. source];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"[{string.Join(", ", Items)}]";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IMetadataTag CreateInstance() => new MetadataTag(CaseSensitiveTags, this);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool CaseSensitiveTags { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Default => Items[0];

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
        public bool Contains(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            foreach (var name in range) if (Contains(name)) return true;
            return false;
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Trim() => Items.TrimExcess();

        // ----------------------------------------------------

        /// <summary>
        /// Invoked to validate the given tag name.
        /// </summary>
        static string Validate(string name) => name.NotNullNotEmpty(true);

        /// <summary>
        /// Determines if the two given tag names can be considered equal, or not.
        /// </summary>
        bool AreEqual(string x, string y) => string.Compare(x, y, !CaseSensitiveTags) == 0;

        /// <summary>
        /// Returns the internal index associated with the given name, or -1 if any.
        /// </summary>
        int IndexOf(string name)
        {
            for (int i = 0; i < Items.Count; i++) if (AreEqual(name, Items[i])) return i;
            return -1;
        }

        // ----------------------------------------------------

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

            // Replacing source element, if any...
            var index = IndexOf(oldname);
            if (index >= 0)
            {
                Items.RemoveAt(index);

                var temp = IndexOf(newname);
                if (temp >= 0) throw new DuplicateException(
                    "Metadata name is already in this collection.")
                    .WithData(newname)
                    .WithData(this);

                Items.Insert(index, newname);
                return true;
            }

            // Finishing...
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool Add(string name)
        {
            var index = IndexOf(name = Validate(name));
            if (index >= 0) throw new DuplicateException(
                "Metadata name is already in this collection.")
                .WithData(name)
                .WithData(this);

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
            range.ThrowWhenNull();

            var done = false;
            foreach (var name in range) if (Add(name)) done = true;
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
                    "Cannot remove the unique remaining tag name.")
                    .WithData(name)
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
            if (Count <= 1) return false; // Always keep at least one name!

            var name = Default;
            Items.Clear();
            Items.Add(name);
            return true;
        }
    }
}