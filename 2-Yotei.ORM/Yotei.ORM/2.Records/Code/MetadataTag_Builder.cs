namespace Yotei.ORM.Records.Code;

partial class MetadataTag
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IMetadataTag.IBuilder"/>
    /// </summary>
    [Cloneable<IMetadataTag.IBuilder>]
    public partial class Builder : IMetadataTag.IBuilder
    {
        readonly List<string> Items;

        /// <summary>
        /// Initializes a new instance with the given default tag name.
        /// </summary>
        /// <param name="sensitive"></param>
        /// <param name="tagname"></param>
        public Builder(bool sensitive, string tagname)
        {
            CaseSensitiveTags = sensitive;
            Items = [Validate(tagname)];
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
        /// <param name="tagname"></param>
        /// <returns></returns>
        public bool Contains(string tagname) => IndexOf(Validate(tagname)) >= 0;

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
        static string Validate(string tagname) => tagname.NotNullNotEmpty(true);

        /// <summary>
        /// Determines if the two given tag names can be considered equal, or not.
        /// </summary>
        bool AreEqual(string tagx, string tagy) => string.Compare(tagx, tagy, !CaseSensitiveTags) == 0;

        /// <summary>
        /// Returns the internal index associated with the given name, or -1 if any.
        /// </summary>
        int IndexOf(string tagname)
        {
            for (int i = 0; i < Items.Count; i++) if (AreEqual(tagname, Items[i])) return i;
            return -1;
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="oldtagname"></param>
        /// <param name="newtagname"></param>
        /// <returns></returns>
        public virtual bool Replace(string oldtagname, string newtagname)
        {
            oldtagname = Validate(oldtagname);
            newtagname = Validate(newtagname);
            if (AreEqual(oldtagname, newtagname)) return false;

            // Replacing source element, if any...
            var index = IndexOf(oldtagname);
            if (index >= 0)
            {
                Items.RemoveAt(index);

                var temp = IndexOf(newtagname);
                if (temp >= 0) throw new DuplicateException(
                    "Metadata name is already in this collection.")
                    .WithData(newtagname)
                    .WithData(this);

                Items.Insert(index, newtagname);
                return true;
            }

            // Finishing...
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public virtual bool Add(string tagname)
        {
            var index = IndexOf(tagname = Validate(tagname));
            if (index >= 0) throw new DuplicateException(
                "Metadata name is already in this collection.")
                .WithData(tagname)
                .WithData(this);

            Items.Add(tagname);
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
        /// <param name="tagname"></param>
        /// <returns></returns>
        public virtual bool Remove(string tagname)
        {
            var index = IndexOf(Validate(tagname));
            if (index >= 0)
            {
                if (Count == 1) throw new InvalidOperationException(
                    "Cannot remove the unique remaining tag name.")
                    .WithData(tagname)
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