namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents the specifications to obtain a new instance of the associated type, including the
/// name(s) of the method to use, its arguments. amd the set of optional init/set members whose
/// values need to be set.
/// </summary>
internal class BuilderSpecs
{
    /// <summary>
    /// <inheritdoc cref="BuilderSpecs"/>
    /// </summary>
    /// <param name="specs"></param>
    public BuilderSpecs(string? specs = null)
    {
        if ((specs = specs.NullWhenEmpty()) == null) return;
        int n, m;
        string s;

        // Arguments section found...
        n = specs.IndexOf('(');
        if (n >= 0)
        {
            Name = specs.Substring(0, n).NullWhenEmpty();
            Arguments.Clear();

            m = specs.IndexOf(')', n);
            if (m < 0) throw new ArgumentException(
                "No closing ')' character found.")
                .WithData(specs, nameof(specs));

            s = specs.Substring(n + 1, m - n - 1).Trim();
            if (s.Length > 0)
            {
                var parts = s.Split(',');
                foreach (var part in parts)
                {
                    var temp = new BuilderArgument(part);
                    Arguments.Add(temp);
                }
            }
            specs = specs.Substring(m + 1);
        }

        // No arguments section...
        else
        {
            m = specs.IndexOf(')');
            if (m >= 0) throw new ArgumentException(
                "Closing ')' character found without an opening '(' one.")
                .WithData(specs, nameof(specs));

            n = specs.IndexOfAny(['+', '-' ]);
            if (n < 0)
            {
                Name = specs;
                return;
            }

            Name = specs.Substring(0, n).NullWhenEmpty();
            specs = specs.Substring(n);
        }

        // Parsing optionals, if any...
        var first = true;
        n = 0;
        while (n < specs.Length)
        {
            n = specs.IndexOfAny(['+', '-'], n);
            if (n < 0) break;

            m = specs.IndexOfAny(['+', '-'], n + 1);
            if (m < 0)
            {
                if ((s = specs.Substring(n)).Length <= 1) throw new ArgumentException(
                    "Empty optional found.")
                    .WithData(specs, nameof(specs));

                if (first) { Optionals.Clear(); first = false; }
                Optionals.Add(new(s));
                return;
            }
            else
            {
                if ((s = specs.Substring(n, m - n)).Length <= 1) throw new ArgumentException(
                    "Empty optional chain found.")
                    .WithData(specs, nameof(specs));

                if (first) { Optionals.Clear(); first = false; }
                Optionals.Add(new(s));
                n = m;
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder(Name);
        sb.Append("(");
        if (Arguments.Count > 0) sb.Append(string.Join(",", Arguments));
        sb.Append(")");
        sb.Append(string.Concat(Optionals));
        return sb.ToString();
    }

    /// <summary>
    /// The name of the method(s) to use. If it is null, then only type constructors are taken
    /// into consideration. If not, the actual name of the method(s) to consider. If several
    /// matches are found, then they are tried in descending order by their number of arguments.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// The collection of method arguments to consider. If empty, then only parameterless methods
    /// will be considered. If its unique element is '*', all methods will be considered despite
    /// their actual number of parameters. Otherwise, only the methods whose parameters match the
    /// given ones will be considered.
    /// </summary>
    public ArgumentsList Arguments { get; } = new(new("*"));

    /// <summary>
    /// Determines if the arguments collection is an 'consider all' one, or not.
    /// </summary>
    public bool AllArguments => Arguments.Count == 1 && Arguments[0].IsNameAsterisk;

    /// <summary>
    /// The collection of init/set members to consider, if any. This collection is processed as
    /// a chain of specifications, where '-*' deletes all previous ones, '+*' adds all remaining
    /// init/set members not yet used by the method under consideration, and so forth.
    /// </summary>
    public OptionalsList Optionals { get; } = new(new("+*"));

    /// <summary>
    /// Determines if the optionals collection is an 'include all' one, or not.
    /// </summary>
    public bool IncludeAll => Optionals.Count == 1 && Optionals[0].IsIncludeAll;

    // ----------------------------------------------------
    /// <summary>
    /// Represents a list of method arguments..
    /// </summary>
    internal class ArgumentsList : SimpleList<BuilderArgument>
    {
        public ArgumentsList() { }
        public ArgumentsList(BuilderArgument item) => Add(item);
        public override void Add(BuilderArgument item)
        {
            item.ThrowWhenNull(nameof(item));

            if (item.IsNameAsterisk && Items.Count > 0)
                throw new InvalidOperationException
                    ("The '*' specification must be unique.")
                    .WithData(item, nameof(item))
                    .WithData(this, "this");

            if (Count == 1 && Items[0].IsNameAsterisk)
                throw new InvalidOperationException(
                    "The '*' specification must be unique.")
                    .WithData(item, nameof(item))
                    .WithData(this, "this");

            if (Items.Any(x => x.Name == item.Name))
                throw new ArgumentException(
                    "Argument name is already used.")
                    .WithData(item, nameof(item))
                    .WithData(this, "this");

            Items.Add(item);
        }
    }

    // ----------------------------------------------------
    /// <summary>
    /// Represents a list of optional init/set members.
    /// </summary>
    internal class OptionalsList : SimpleList<BuilderOptional>
    {
        public OptionalsList() { }
        public OptionalsList(BuilderOptional item) => Add(item);
        public override void Add(BuilderOptional item)
        {
            item.ThrowWhenNull(nameof(item));

            if (item.IsExcludeAll) { Items.Clear(); return; }
            
            Items.Add(item);
        }
    }

    // ----------------------------------------------------
    internal class SimpleList<T> : IEnumerable<T>
    {
        protected List<T> Items { get; } = new();
        protected SimpleList() { }
        public override string ToString() => $"Count: {Count}";
        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        public int Count => Items.Count;
        public T this[int index] => Items[index];
        public virtual void Add(T item) => Items.Add(item);
        public void Clear() => Items.Clear();
    }
}