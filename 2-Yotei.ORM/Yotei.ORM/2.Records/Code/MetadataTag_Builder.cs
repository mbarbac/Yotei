namespace Yotei.ORM.Records.Code;

partial class MetadataTag
{
    // ====================================================
    /// <inheritdoc cref="IMetadataName.IBuilder"/>
    [Cloneable]
    public partial class Builder : IMetadataTag.IBuilder
    {
        readonly List<string> Items = [];

        /// <summary>
        /// Initializes a new instance with the given default name.
        /// </summary>
        /// <param name="caseSensitiveTags"></param>
        /// <param name="name"></param>
        public Builder(bool caseSensitiveTags, string name)
        {
            CaseSensitiveTags = caseSensitiveTags;
            Add(name);
        }

        /// <summary>
        /// Initializes a new instance with the names of the given range.
        /// </summary>
        /// <param name="caseSensitiveTags"></param>
        /// <param name="range"></param>
        public Builder(bool caseSensitiveTags, IEnumerable<string> range)
        {
            CaseSensitiveTags = caseSensitiveTags;
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

            CaseSensitiveTags = source.CaseSensitiveTags;
            AddRange(source);
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{string.Join(", ", Items)}]";

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public virtual MetadataTag CreateInstance() => new(CaseSensitiveTags, this);
        IMetadataTag IMetadataTag.IBuilder.CreateInstance() => CreateInstance();

        // ------------------------------------------------

        /// <inheritdoc/>
        public bool CaseSensitiveTags { get; }

        /// <inheritdoc/>
        public string Default
        {
            get => Items[0];
            set
            {
                var index = IndexOf(value = Validate(value));

                if (index == 0) // Already is the default one...
                {
                    return;
                }
                else if (index > 0)
                {
                    Items.RemoveAt(index);
                    Items.Insert(0, value);
                }
                else throw new NotFoundException("Tag name not found.").WithData(value);
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

        // ------------------------------------------------

        /// <summary>
        /// Invoked to validate the given tag name.
        /// </summary>
        static string Validate(string name) => name.NotNullNotEmpty();

        /// <summary>
        /// Invoked to determine if the two given tag names shall be considered equal, or not.
        /// <br/> Names are not validated
        /// </summary>
        bool Compare(string x, string y) => string.Compare(x, y, !CaseSensitiveTags) == 0;

        /// <summary>
        /// Returns the index at which the given tag name is stored, or -1 if not found.
        /// <br/> Name is not validated.
        /// </summary>
        internal int IndexOf(string name)
        {
            for (int i = 0; i < Items.Count; i++) if (Compare(name, Items[i])) return i;
            return -1;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Replace(string oldname, string newname)
        {
            oldname = Validate(oldname);
            newname = Validate(newname);

            if (Compare(oldname, newname)) return false;

            var index = IndexOf(oldname);
            if (index >= 0)
            {
                Items.RemoveAt(index);

                var temp = IndexOf(newname);
                if (temp >= 0) throw new DuplicateException(
                    "Tag name is already in this collection.")
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
                "Tag name is already in this collection.")
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
                if (Count == 1) // Cannot remove the default name...
                    throw new InvalidOperationException(
                        "Cannot remove the default tag name.")
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