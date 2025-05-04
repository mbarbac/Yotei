namespace Yotei.ORM.Records.Code;

partial class MetadataTag
{
    // ====================================================
    /// <inheritdoc cref="IMetadataTag.IBuilder"/>
    [Cloneable]
    public partial class Builder : IMetadataTag.IBuilder
    {
        readonly List<string> Items = [];

        /// <summary>
        /// Initializes a new instance with the given default name.
        /// </summary>
        /// <param name="caseSensitive"></param>
        /// <param name="name"></param>
        public Builder(bool caseSensitive, string name)
        {
            CaseSensitiveTags = caseSensitive;
            Add(name);
        }

        /// <summary>
        /// Initializes a new instance with the names of the given range, using as the default
        /// one the first one in the range. If the range is empty, then an exception is thrown.
        /// </summary>
        /// <param name="caseSensitive"></param>
        /// <param name="range"></param>
        public Builder(bool caseSensitive, IEnumerable<string> range)
        {
            CaseSensitiveTags = caseSensitive;
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
        public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Items.Count == 1) return $"[{Items[0]}]";
            else
            {
                var sb = new StringBuilder();
                sb.Append('[');

                for (int i = 0; i < Items.Count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(Items[i]);
                }

                sb.Append(']');
                return sb.ToString();
            }
        }

        /// <inheritdoc/>
        public IMetadataTag ToInstance() => new MetadataTag(CaseSensitiveTags, this);

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
                    Items.Insert(index, value);
                }
                else throw new NotFoundException("Tag name not found.").WithData(value);
            }
        }

        /// <inheritdoc/>
        public int Count => Items.Count;

        /// <inheritdoc/>
        public bool Contains(string name) => IndexOf(Validate(name)) >= 0;

        /// <inheritdoc/>
        public bool ContainsAny(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            foreach (var name in range) if (Contains(name)) return true;
            return false;
        }

        /// <inheritdoc/>
        public string[] ToArray() => Items.ToArray();

        /// <inheritdoc/>
        public List<string> ToList() => new(Items);

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
        public bool Replace(string oldname, string newname)
        {
            var xold = Validate(oldname);
            var xnew = Validate(newname);

            if (Compare(xold, xnew)) return false;
            else
            {
                var index = IndexOf(xold);
                if (index >= 0)
                {
                    Items.RemoveAt(index);
                    Items.Insert(index, xnew);
                    return true;
                }
                else return false;
            }
        }

        /// <inheritdoc/>
        public bool Add(string name)
        {
            name = Validate(name);

            var index = IndexOf(name);
            if (index >= 0) throw new DuplicateException(
                "Tag name is already in this collection.")
                .WithData(name)
                .WithData(this);

            Items.Add(name);
            return true;
        }

        /// <inheritdoc/>
        public bool AddRange(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var name in range) if (Add(name)) done = true;

            return done;
        }

        /// <inheritdoc/>
        public bool Insert(int index, string name)
        {
            name = Validate(name);

            var temp = IndexOf(name);
            if (temp >= 0) throw new DuplicateException(
                "Tag name is already in this collection.")
                .WithData(name)
                .WithData(this);

            Items.Insert(index, name);
            return true;
        }

        /// <inheritdoc/>
        public bool InsertRange(int index, IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var name in range)
            {
                if (Insert(index, name))
                {
                    done = true;
                    index++;
                }
            }

            return done;
        }

        /// <inheritdoc/>
        public bool Remove(string name)
        {
            name = Validate(name);

            var index = IndexOf(name);
            if (index >= 0)
            {
                if (Count == 1) throw new InvalidOperationException(
                    "Cannot remove the default tag name.")
                    .WithData(this);

                Items.RemoveAt(index);
            }

            return index >= 0;
        }

        /// <inheritdoc/>
        public bool Clear()
        {
            if (Count > 1)
            {
                var name = Items[0];

                Items.Clear();
                Items.Add(name);
                return true;
            }
            else return false;
        }
    }
}