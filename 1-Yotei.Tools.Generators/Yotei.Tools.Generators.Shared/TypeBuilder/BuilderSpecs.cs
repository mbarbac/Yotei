using System.Net.Http.Headers;

namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents the specifications given to a builder.
/// </summary>
internal class BuilderSpecs
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="specs"></param>
    public BuilderSpecs(string? specs)
    {
        specs = specs.NullWhenEmpty();
        if (specs == null) return;

        int n;
        int m;
        string s;

        // Case: we have an argument section...
        n = specs.IndexOf('('); if (n >= 0)
        {
            Name = specs.Substring(0, n).NullWhenEmpty();
            Arguments.Clear();

            m = specs.IndexOf(')', n); if (m >= 0)
            {
                s = specs.Substring(n + 1, m - n - 1).Trim();
                if (s.Length > 0)
                {
                    var parts = s.Split(',');
                    foreach (var part in parts) Arguments.Add(new(part));
                }

                specs = specs.Substring(m + 1);
            }
            else throw new ArgumentException(
                "Not matching ')' specification.")
                .WithData(specs, nameof(specs));
        }

        // Case: we don't have an argument section...
        else
        {
            n = specs.IndexOfAny(new[] { '+', '-' });
            if (n < 0)
            {
                Name = specs;
                return;
            }
            Name = specs.Substring(0, n).Trim();
            specs = specs.Substring(n);
        }

        // Finally, parsing the optionals, if any...
        if (specs.Contains("+")) Optionals.Clear();

        n = 0; while (n < specs.Length)
        {
            n = specs.IndexOfAny(new[] { '+', '-' }, n);
            if (n < 0) break;

            m = specs.IndexOfAny(new[] { '+', '-' }, n + 1);
            if (m < 0)
            {
                s = specs.Substring(n);
                if (s.Length <= 1) throw new ArgumentException(
                    "Empty optional specification found.")
                    .WithData(specs, nameof(specs));

                Optionals.Add(new(s));
                return;
            }
            else
            {
                s = specs.Substring(n, m - n);
                if (s.Length <= 1) throw new ArgumentException(
                    "Empty optional specification found.")
                    .WithData(specs, nameof(specs));

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
        var sb = new StringBuilder();
        sb.Append(Name);

        sb.Append("(");
        if (Arguments.Count > 0) sb.Append(string.Join(",", Arguments));
        sb.Append(")");

        sb.Append(string.Concat(Optionals));

        return sb.ToString();
    }

    /// <summary>
    /// The name of the builder method(s) to use. If it is null then only the type constructors
    /// will be taken into consideration.
    /// </summary>
    public string? Name
    {
        get => _Name;
        set => _Name = value == null || value.Length == 0
            ? null
            : value.NotNullNotEmpty(nameof(Name));
    }
    string? _Name = null;

    /// <summary>
    /// The collection of argument specifications. If it is an empty one, then only parameterless
    /// builders are be considered. If its unique element is '*', then all filtered builders are
    /// considered despite their actual arguments. Otherwise, the actual list of arguments to be
    /// matched.
    /// </summary>
    public ArgumentsList Arguments { get; } = new(new("*"));

    /// <summary>
    /// The collection of init/set optional specifications. By default this collection contains
    /// a single '+*' element indicating that all remaining members are considered.
    /// </summary>
    public OptionalsList Optionals { get; } = new(new("+*"));

    // ====================================================
    /// <summary>
    /// Represents the list of arguments.
    /// </summary>
    internal class ArgumentsList : SimpleList<BuilderArgument>
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public ArgumentsList() { }

        /// <summary>
        /// Initializes a new instance with the given element.
        /// </summary>
        /// <param name="item"></param>
        public ArgumentsList(BuilderArgument item) => Add(item);

        /// <summary>
        /// Adds the given element to this collection. If a '*' specification is added, then it
        /// is intercepted to guarantee it is the only element in the collection.
        /// </summary>
        /// <param name="item"></param>
        public override void Add(BuilderArgument item)
        {
            item.ThrowWhenNull(nameof(item));

            if (item.Name == "*")
            {
                if (Items.Count > 0)
                    throw new ArgumentException("The '*' specification must be unique.");
            }
            else
            {
                if (Items.Any(x => x.Name == "*"))
                    throw new ArgumentException("The '*' specification must be unique.");
            }

            Items.Add(item);
        }
    }

    // ====================================================
    /// <summary>
    /// Represents the list of optionals.
    /// </summary>
    internal class OptionalsList : SimpleList<BuilderOptional>
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public OptionalsList() { }

        /// <summary>
        /// Initializes a new instance with the given element.
        /// </summary>
        /// <param name="item"></param>
        public OptionalsList(BuilderOptional item) => Add(item);

        /// <summary>
        /// Adds the given element to this collection. If a '-*' or an '+*' element is added, then
        /// all specifications in the chain are erased - and the '+*' is added as needed, but not
        /// the '-*' one.
        /// </summary>
        /// <param name="item"></param>
        public override void Add(BuilderOptional item)
        {
            item.ThrowWhenNull(nameof(item));

            if (item.Name == "*")
            {
                Items.Clear();
                if (item.IsInclude) Add(item);
            }
            else
            {
                Items.Add(item);
            }
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a simple list-alike collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SimpleList<T> : IEnumerable<T>
    {
        protected List<T> Items { get; } = new();

        /// <summary>
        /// Protected constructor.
        /// </summary>
        protected SimpleList() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Count: {Count}";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

        /// <summary>
        /// The number of elements in this collection.
        /// </summary>
        public int Count => Items.Count;

        /// <summary>
        /// Gets the item at the given position.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] => Items[index];

        /// <summary>
        /// Adds the given item into this collection.
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(T item) => Items.Add(item);

        /// <summary>
        /// Clears this collection.
        /// </summary>
        public void Clear() => Items.Clear();
    }
}