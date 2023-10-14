namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a set of specifications to obtain a new instance of an associated type, taking
/// into consideration the name of the method to use (null to only use constructors), the method
/// arguments, and the optional specifications for the remaining members not yet used by the
/// method.
/// </summary>
internal class SpecsParser
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="specs"></param>
    public SpecsParser(string? specs = null)
    {
        if ((specs = specs.NullWhenEmpty()) == null) return;
        int n, m;
        string s;

        // Having an arguments section...
        n = specs.IndexOf('(');
        if (n >= 0)
        {
            Name = specs.Substring(0, n).NullWhenEmpty();
            Arguments.Clear();

            m = specs.IndexOf(')');
            if (m < 0) throw new ArgumentException(
                "Specifications contains no closing ')' character.")
                .WithData(specs, nameof(specs));

            if (m < n) throw new ArgumentException(
                "Closing ')' character cannot appear before the opening '(' one.")
                .WithData(specs, nameof(specs));

            s = specs.Substring(n + 1, m - n - 1).Trim();
            if (s.Length > 0)
            {
                var parts = s.Split(',');
                foreach (var part in parts)
                {
                    var temp = new SpecsArgument(part);
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
                "Closing ')' character cannot appear without an opening '(' one.")
                .WithData(specs, nameof(specs));

            n = specs.IndexOfAny(new[] { '+', '-' });
            if (n < 0)
            {
                Name = specs;
                return;
            }
            
            Name = specs.Substring(0,n).NullWhenEmpty();
            specs = specs.Substring(n);
        }

        // Parsing optionals, if any...
        var first = true;
        n = 0;
        while (n < specs.Length)
        {
            n = specs.IndexOfAny(new[] { '+', '-' }, n);
            if (n < 0) break;

            m = specs.IndexOfAny(new[] { '+', '-' }, n + 1);
            if (m < 0)
            {
                if ((s = specs.Substring(n)).Length <= 1) throw new ArgumentException(
                    "Empty optionals found.")
                    .WithData(specs, nameof(specs));

                if (first) Optionals.Clear();
                Optionals.Add(new(s));
                return;
            }
            else
            {
                if ((s = specs.Substring(n, m - n)).Length <= 1) throw new ArgumentException(
                    "Empty optionals chain found.")
                    .WithData(specs, nameof(specs));

                if (first)
                {
                    Optionals.Clear();
                    first = false;
                }
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
    /// The name of the method to use, or 'null' to only consider constructors.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// The collection of arguments of the method. If it is an empty one, then only parameterless
    /// methods will be considered. If its unique element is '*', then all possible methods with
    /// the given name (or null) will be considered. Otherwise, the actual list of arguments to
    /// be matched.
    /// </summary>
    public ArgumentsList Arguments { get; } = new(new("*"));

    /// <summary>
    /// Determines if all method arguments shall be considered, or only the specified ones.
    /// </summary>
    public bool AllArguments => Arguments.Count == 1 && Arguments[0].IsNameAsterisk;

    /// <summary>
    /// The collection of optional init/set members to consider, if any.
    /// </summary>
    public OptionalsList Optionals { get; } = new(new("+*"));

    // ----------------------------------------------------
    /// <summary>
    /// Represents a list of method arguments.
    /// </summary>
    internal class ArgumentsList : SimpleList<SpecsArgument>
    {
        public ArgumentsList() { }
        public ArgumentsList(SpecsArgument item) => Add(item);
        public override void Add(SpecsArgument item)
        {
            item.ThrowWhenNull(nameof(item));

            if (item.IsNameAsterisk)
            {
                if (Items.Count > 0) throw new InvalidOperationException(
                    "The '*' specification must be unique.");
            }

            if (Count == 1 && Items[0].IsNameAsterisk) throw new InvalidOperationException(
                "The '*' specification must be unique.");

            if (Items.Any(x => x.Name == item.Name)) throw new DuplicateException(
                $"Argument name '{item.Name}' is already specified.")
                .WithData(item, nameof(item))
                .WithData(this, "this");

            Items.Add(item);
        }
    }

    // ----------------------------------------------------
    /// <summary>
    /// Represents a list of optional init/set members.
    /// </summary>
    internal class OptionalsList : SimpleList<SpecsOptional>
    {
        public OptionalsList() { }
        public OptionalsList(SpecsOptional item) => Add(item);
        public override void Add(SpecsOptional item)
        {
            item.ThrowWhenNull(nameof(item));

            if (item.IsMemberAsterisk)
            {
                Items.Clear();
                if (item.IsExclude) return;
            }
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