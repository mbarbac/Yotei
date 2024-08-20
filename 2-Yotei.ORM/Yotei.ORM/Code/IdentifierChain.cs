namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierChain"/>
[FrozenList<string, IIdentifierPart>]
public partial class IdentifierChain : IIdentifierChain
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    public virtual Builder ToBuilder() => Items.Clone();

    protected override Builder Items { get; }

    // -----------------------------------------------------

    /// <inheritdoc/>
    public IdentifierChain(IEngine engine) : base() => Items = new(engine);

    /// <inheritdoc/>
    public IdentifierChain(IEngine engine, IIdentifierPart item) : this(engine) => Items.Add(item);

    /// <inheritdoc/>
    public IdentifierChain(
        IEngine engine, IEnumerable<IIdentifierPart> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierChain(IEngine engine, string? value) : this(engine) => Items.Add(value);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierChain(
        IEngine engine, IEnumerable<string?> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierChain(IdentifierChain source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string? Value => _Value ??= Items.Value;
    string? _Value = null;

    /// <inheritdoc/>
    public bool Match(string? specs) => Identifier.Match(this, specs);

    // -----------------------------------------------------

    /// <inheritdoc/>
    public new bool Contains(string? identifier) => base.Contains(identifier!);

    /// <inheritdoc/>
    public new int IndexOf(string? identifier) => base.IndexOf(identifier!);

    /// <inheritdoc/>
    public new int LastIndexOf(string? identifier) => base.LastIndexOf(identifier!);

    /// <inheritdoc/>
    public new List<int> IndexesOf(string? identifier) => base.IndexesOf(identifier!);

    // -----------------------------------------------------

    /// <inheritdoc cref="IIdentifierChain.Replace(int, string?)"/>
    public IdentifierChain Replace(int index, string? value)
    {
        var builder = Items.Clone();
        var done = builder.Replace(index, value);
        return done > 0 ? new(Engine, builder) : this;
    }
    IIdentifierChain IIdentifierChain.Replace(int index, string? value) => Replace(index, value);

    /// <inheritdoc cref="IIdentifierChain.Add(string?)"/>
    public IdentifierChain Add(string? value)
    {
        var builder = Items.Clone();
        var done = builder.Add(value);
        return done > 0 ? new(Engine, builder) : this;
    }
    IIdentifierChain IIdentifierChain.Add(string? value) => Add(value);

    /// <inheritdoc cref="IIdentifierChain.AddRange(IEnumerable{string?})"/>
    public IdentifierChain AddRange(IEnumerable<string?> range)
    {
        var builder = Items.Clone();
        var done = builder.AddRange(range);
        return done > 0 ? new(Engine, builder) : this;
    }
    IIdentifierChain IIdentifierChain.AddRange(IEnumerable<string?> range) => AddRange(range);

    /// <inheritdoc cref="IIdentifierChain.Insert(int, string?)"/>
    public IdentifierChain Insert(int index, string? value)
    {
        var builder = Items.Clone();
        var done = builder.Insert(index, value);
        return done > 0 ? new(Engine, builder) : this;
    }
    IIdentifierChain IIdentifierChain.Insert(int index, string? value) => Insert(index, value);

    /// <inheritdoc cref="IIdentifierChain.InsertRange(int, IEnumerable{string?})"/>
    public IdentifierChain InsertRange(int index, IEnumerable<string?> range)
    {
        var builder = Items.Clone();
        var done = builder.InsertRange(index, range);
        return done > 0 ? new(Engine, builder) : this;
    }
    IIdentifierChain IIdentifierChain.InsertRange(int index, IEnumerable<string?> range) => InsertRange(index, range);

    // ====================================================

    /// <summary>
    /// Represents a builder of <see cref="IIdentifierChain"/> instances.
    /// </summary>
    [Cloneable]
    public partial class Builder : CoreList<string, IIdentifierPart>
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) : base()
        {
            Engine = engine.ThrowWhenNull();

            ValidateItem = (item) =>
            {
                item.ThrowWhenNull();

                if (!EngineComparer.Instance.Equals(Engine, item.Engine))
                    throw new ArgumentException(
                        "The engine of the given element is not equivalent to the one of this instance.")
                        .WithData(item)
                        .WithData(this);

                return item;
            };
            GetKey = (x) => x.ThrowWhenNull().Value!;
            ValidateKey = (key) => new IdentifierPart(Engine, key).Value!;
            CompareKeys = (x, y) =>
            {
                x = (x == null ? null : new IdentifierPart(Engine, x).Value)!;
                y = (y == null ? null : new IdentifierPart(Engine, y).Value)!;
                return string.Compare(x, y, !Engine.CaseSensitiveNames) == 0;
            };
            GetDuplicates = (key) => [];
            CanInclude = (item, other) => true;
            ExpandItems = false;
        }

        /// <summary>
        /// Initializes a new instance with the given element.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="item"></param>
        public Builder(IEngine engine, IIdentifierPart item) : this(engine) => Add(item);
        
        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IIdentifierPart> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Initializes a new instance with the elements obtained from the given value.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="value"></param>
        public Builder(IEngine engine, string? value) : this(engine) => Add(value);

        /// <summary>
        /// Initializes a new instance with the elements obtained from the given range of values.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(IEngine engine, IEnumerable<string?> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.Engine) => AddRange(source);

        /// <inheritdoc/>
        public override string ToString() => Value ?? string.Empty;

        // -------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the captured contents.
        /// </summary>
        /// <returns></returns>
        public virtual IIdentifierChain ToInstance() => new IdentifierChain(Engine, this);

        /// <inheritdoc cref="IIdentifier.Engine"/>
        public IEngine Engine { get; }

        /// <inheritdoc cref="IIdentifier.Value"/>
        public string? Value
        {
            get
            {
                if (Changed)
                {
                    _Value = Count == 0 || this.All(x => x.Value == null)
                        ? null
                        : string.Join('.', this.Select(x => x.Value));

                    Changed = false;
                }
                return _Value;
            }
        }
        string? _Value = null;
        bool Changed = true;

        // -------------------------------------------------

        /// <summary>
        /// Reduces this instance to a simpler form.
        /// </summary>
        void Reduce()
        {
            while (Count > 0)
            {
                if (this[0].Value == null) { RemoveAt(0); Changed = true; }
                else break;
            }
        }

        // ------------------------------------------------
        // TO-DO: use reduce to prevent redundant reduce operations

        /// <inheritdoc/>
        public override int GetRange(int index, int count) => GetRange(index, count, true);
        int GetRange(int index, int count, bool reduce)
        {
            var r = base.GetRange(index, count); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int Replace(int index, IIdentifierPart item) => Replace(index, item, true);
        int Replace(int index, IIdentifierPart item, bool reduce)
        {
            var r = base.Replace(index, item); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int Add(IIdentifierPart item) => Add(item, true);
        int Add(IIdentifierPart item, bool reduce)
        {
            item.ThrowWhenNull();
            if (Count == 0 && item.Value == null) return 0;

            var r = base.Add(item); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int AddRange(IEnumerable<IIdentifierPart> range) => AddRange(range, true);
        int AddRange(IEnumerable<IIdentifierPart> range, bool reduce)
        {
            var r = base.AddRange(range); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int Insert(int index, IIdentifierPart item) => Insert(index, item, true);
        int Insert(int index, IIdentifierPart item, bool reduce)
        {
            var r = base.Insert(index, item); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int InsertRange(int index, IEnumerable<IIdentifierPart> range) => InsertRange(index, range, true);
        int InsertRange(int index, IEnumerable<IIdentifierPart> range, bool reduce)
        {
            var r = base.InsertRange(index, range); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int RemoveAt(int index) => RemoveAt(index, true);
        int RemoveAt(int index, bool reduce)
        {
            var r = base.RemoveAt(index); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int RemoveRange(int index, int count) => RemoveRange(index, count, true);
        int RemoveRange(int index, int count, bool reduce)
        {
            var r = base.RemoveRange(index, count); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int Remove(string? key) => Remove(key, true);
        int Remove(string? key, bool reduce)
        {
            if (Count == 0) return 0;

            var r = base.Remove(key!); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int RemoveLast(string? key) => RemoveLast(key, true);
        int RemoveLast(string? key, bool reduce)
        {
            if (Count == 0) return 0;

            var r = base.RemoveLast(key!); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int RemoveAll(string? key) => RemoveAll(key, true);
        int RemoveAll(string? key, bool reduce)
        {
            if (Count == 0) return 0;

            var r = base.RemoveAll(key!); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int Remove(Predicate<IIdentifierPart> predicate) => Remove(predicate, true);
        int Remove(Predicate<IIdentifierPart> predicate, bool reduce)
        {
            var r = base.Remove(predicate); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int RemoveLast(Predicate<IIdentifierPart> predicate) => RemoveLast(predicate, true);
        int RemoveLast(Predicate<IIdentifierPart> predicate, bool reduce)
        {
            var r = base.RemoveLast(predicate); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int RemoveAll(Predicate<IIdentifierPart> predicate) => RemoveAll(predicate, true);
        int RemoveAll(Predicate<IIdentifierPart> predicate, bool reduce)
        {
            var r = base.RemoveAll(predicate); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc/>
        public override int Clear() => Clear(true);
        int Clear(bool reduce)
        {
            if (Count == 0) return 0;

            var r = base.Clear(); if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        // ------------------------------------------------

        /// <summary>
        /// Obtains a list of parts from the given value.
        /// <br/> Note that the produced list is never null or empty, even null source values
        /// produce at least one entry.
        /// </summary>
        List<IdentifierPart> GetParts(string? value)
        {
            var items = Identifier.GetParts(Engine, value, false);
            var parts = items.Select(x => new IdentifierPart(Engine, PartToString(x))).ToList();

            if (parts.Count == 0) parts.Add(new IdentifierPart(Engine));
            return parts;

            string? PartToString(string? value)
            {
                if (value != null && Engine.UseTerminators)
                    value = $"{Engine.LeftTerminator}{value}{Engine.RightTerminator}";

                return value;
            }
        }

        // ------------------------------------------------

        /// <inheritdoc cref="IIdentifierChain.Replace(int, string?)"/>
        public int Replace(int index, string? value) => Replace(index, value, true);
        int Replace(int index, string? value, bool reduce)
        {
            if (value != null && !value.Contains('.'))
            {
                var source = this[index].Value;
                var target = new IdentifierPart(Engine, value).Value;
                var same = string.Compare(source, target) == 0;
                if (same) return 0;
            }

            var parts = GetParts(value);

            var removed = RemoveAt(index, reduce: false);
            var inserted = 0;
            foreach (var part in parts)
            {
                var temp = Insert(index, part, reduce: false);
                inserted += temp;
                index += temp;
            }
            var r = inserted == 0 ? removed : inserted;
            if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc cref="IIdentifierChain.Add(string?)"/>
        public int Add(string? value) => Add(value, true);
        int Add(string? value, bool reduce)
        {
            var parts = GetParts(value);

            var r = 0; foreach (var part in parts)
            {
                var temp = Add(part, false);
                r += temp;
            }
            if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc cref="IIdentifierChain.AddRange(IEnumerable{string?})"/>
        public int AddRange(IEnumerable<string?> range) => AddRange(range, true);
        int AddRange(IEnumerable<string?> range, bool reduce)
        {
            range.ThrowWhenNull();

            var r = 0; foreach (var item in range)
            {
                var temp = Add(item, false);
                r += temp;
            }
            if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc cref="IIdentifierChain.Insert(int, string?)"/>
        public int Insert(int index, string? value) => Insert(index, value, true);
        int Insert(int index, string? value, bool reduce)
        {
            var parts = GetParts(value);

            var r = 0; foreach (var part in parts)
            {
                if (index == 0 && part.Value == null) continue;

                var temp = Insert(index, part, false);
                r += temp;
                index += temp;
            }
            if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }

        /// <inheritdoc cref="IIdentifierChain.InsertRange(int, IEnumerable{string?})"/>
        public int InsertRange(int index, IEnumerable<string?> range) => InsertRange(index, range, true);
        int InsertRange(int index, IEnumerable<string?> range, bool reduce)
        {
            range.ThrowWhenNull();

            var r = 0; foreach (var item in range)
            {
                var temp = Insert(index, item, false);
                r += temp;
                index += temp;
            }
            if (r > 0)
            {
                if (reduce) Reduce();
                Changed = true;
            }
            return r;
        }
    }
}