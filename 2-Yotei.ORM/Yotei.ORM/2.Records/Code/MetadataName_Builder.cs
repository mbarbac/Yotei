namespace Yotei.ORM.Records.Code;

partial class MetadataName
{
    // ====================================================
    /// <inheritdoc cref="IMetadataName.IBuilder"/>
    [Cloneable]
    public partial class Builder : IMetadataName.IBuilder
    {
        readonly List<string> Items = [];

        /// <summary>
        /// Initializes a new instance with the given default name.
        /// </summary>
        /// <param name="sensitive"></param>
        /// <param name="name"></param>
        public Builder(bool sensitive, string name)
        {
            CaseSensitiveNames = sensitive;
            Add(name);
        }

        /// <summary>
        /// Initializes a new instance with the names of the given name. The first name becomes
        /// the default one. If the range is empty, then an exception is thrown.
        /// </summary>
        /// <param name="sensitive"></param>
        /// <param name="range"></param>
        public Builder(bool sensitive, IEnumerable<string> range)
        {
            CaseSensitiveNames = sensitive;
            AddRange(range);

            if (Items.Count == 0) throw new EmptyException("Range of tag names is empty.");
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source)
        {
            source.ThrowWhenNull();

            CaseSensitiveNames = source.CaseSensitiveNames;
            AddRange(source);
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{string.Join(", ", Items)}]";

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public virtual IMetadataName CreateInstance() => new MetadataName(CaseSensitiveNames, this);

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool CaseSensitiveNames { get; }

        /// <inheritdoc/>
        /// <remarks>The default name is always the first one in the internal collection.</remarks>
        public string Default
        {
            get => Items[0];
            set
            {
                var index = IndexOf(value = Validate(value));

                if (index >= 0) // Always inserting the given value, is the given one!...
                {
                    Items.RemoveAt(index);
                    Items.Insert(0, value);
                }
                else throw new NotFoundException("Metadata name not found.").WithData(value);
            }
        }

        /// <inheritdoc/>
        public int Count => Items.Count;

        /// <inheritdoc/>
        public bool Contains(string name) => IndexOf(Validate(name)) >= 0;

        /// <inheritdoc/>
        public bool Contains(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            foreach (var name in range) if (Contains(name)) return true;
            return false;
        }

        /// <inheritdoc/>
        public string[] ToArray() => Items.ToArray();

        /// <inheritdoc/>
        public List<string> ToList() => new(Items);

        /// <inheritdoc/>
        public void Trim() => Items.TrimExcess();

        // ----------------------------------------------------

        /// <summary>
        /// Invoked to validate the given name.
        /// </summary>
        static string Validate(string name) => name.NotNullNotEmpty();

        /// <summary>
        /// Determines if the two given string can be considered equal or not.
        /// </summary>
        bool AreEqual(string x, string y) => string.Compare(x, y, !CaseSensitiveNames) == 0;

        /// <summary>
        /// Returns the index associated to the given name, or -1 if it is not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int IndexOf(string name)
        {
            for (int i = 0; i < Items.Count; i++) if (AreEqual(name, Items[i])) return i;
            return -1;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Replace(string oldname, string newname)
        {
            oldname = Validate(oldname);
            newname = Validate(newname);

            if (AreEqual(oldname, newname)) return false;

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

            return false;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public virtual bool AddRange(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var name in range) if (Add(name)) done = true;

            return done;
        }

        /// <inheritdoc/>
        public virtual bool Remove(string name)
        {
            var index = IndexOf(name = Validate(name));
            if (index >= 0)
            {
                // Cannot remove the sole name...
                if (Count == 1) throw new InvalidOperationException(
                    "Cannot remove the unique metadata name.")
                    .WithData(name)
                    .WithData(this);

                Items.RemoveAt(index);
            }

            return index >= 0;
        }

        /// <inheritdoc/>
        public virtual bool Clear()
        {
            if (Count > 1) // Keeping the default name...
            {
                var name = Items[0];

                Items.Clear();
                Items.Add(name);
                return true;
            }
            else // Cannot clear the default name...
            {
                return false;
            }
        }
    }
}