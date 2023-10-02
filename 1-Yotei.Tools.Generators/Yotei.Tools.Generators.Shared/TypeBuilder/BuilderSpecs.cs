namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents the specifications given to a builder.
/// </summary>
internal class BuilderSpecs
{
    /// <summary>
    /// Initializes a a new instance.
    /// </summary>
    /// <param name="specs">
    /// If not null, the specifications that describe how to obtain a new instance of the host
    /// type, using the '[builder][(arguments)][optionals]' format, where:
    /// <para>
    /// - [builder]: null to only take into consideration the type constructors, or the name of
    /// the builder method(s) to take into consideration. If so, they must return an object whose
    /// type must be compatible with the host one.
    /// </para>
    /// <para>
    /// - [(arguments)]: the comma-separated list of arguments to use by the builders. If not
    /// used, or if it is '(*)', then all builders will be tried. If it is '()', then only the
    /// parameterless ones will be tried. Otherwise, elements with the '[name][=@|member][!]'
    /// format, where:
    /// <br/>- [name]: the actual name of the builder argument, or '*' to indicate that all
    /// arguments shall be taken into consideration.
    /// <br/>- [=@|member]: the source from which to obtain the value of that argument. If it
    /// is not used, the name of a matching member will be used. If '=@', then the name of the
    /// variable for the the enforced member (if any) will be used instead. Otherwise, the
    /// actual name of the member that becomes the source of that value.
    /// <br/>>- [!]: If used, then a clone of the value will be used instead.
    /// </para>
    /// <para>
    /// - [optionals]: the optional chain of comma-separated specs of the remaining init/set
    /// elements not yet used by the builder. If not used, no optional element is injected.
    /// Otherwise, each follows the '[+|-][*|member][=@][!]' format where:
    /// <br/>>- [+|-]: determines if it is an include or exclude specification.
    /// <br/>>- [*|member]: if '*', the specification affects to all remaining members, and any
    /// previous ones are erased. Otherwise, the name of the member to use from the set of
    /// remaining ones.
    /// <br/>>- [=@]: if used then the name of the value for the enforced member, if any, will
    /// be used.
    /// <br/>>- [!]: If used, then a clone of the value will be used instead.
    /// </para>
    /// <para>
    /// Some generators permit the use of 'enforced' members to modify their value while
    /// building the new instance of the type to return. If so, the name of the external
    /// variable representing that value is used as needed.
    /// </para>
    /// </param>
    public BuilderSpecs(string? specs = null)
    {
        if ((specs = specs.NullWhenEmpty()) == null) return;
        int n, m;
        string s;

        // Case: having an arguments' section...
        n = specs.IndexOf('(');
        if (n >= 0)
        {
            Name = specs.Substring(0, n).NullWhenEmpty();
            Arguments.Clear();

            m = specs.IndexOf(')');
            if (m > 0)
            {
                s = specs.Substring(n + 1, m - n - 1).Trim();
                if (s.Length > 0)
                {
                    var parts = s.Split(',');
                    foreach (var part in parts) Arguments.Add(new(part));
                }
                specs = specs.Substring(m + 1);
            }
            else
            {
                throw new ArgumentException
                    ("Specifications contain no closing ')' character.")
                    .WithData(specs, nameof(specs));
            }
        }

        // Case: no arguments' section...
        else
        {
            n = specs.IndexOfAny(new[] { '+', '-' });
            if (n < 0)
            {
                Name = specs;
                return;
            }
            Name = specs.Substring(0, n).NullWhenEmpty();
            specs = specs.Substring(n);
        }

        // Parsing optionals, if any...
        n = 0;
        while (n < specs.Length)
        {
            n = specs.IndexOfAny(new[] { '+', '-' }, n);
            if (n < 0) break;

            m = specs.IndexOfAny(new[] { '+', '-' }, n + 1);
            if (m < 0)
            {
                if ((s = specs.Substring(n)).Length <= 1) throw new ArgumentException(
                    "Empty optionals chain found.")
                    .WithData(specs, nameof(specs));

                Optionals.Add(new(s));
                return;
            }
            else
            {
                if ((s = specs.Substring(n, m - n)).Length <= 1) throw new ArgumentException(
                    "Empty optionals chain found.")
                    .WithData(specs, nameof(specs));

                Optionals.Add(new(s));
                n = m;
            }
        }
    }

    // ----------------------------------------------------

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
        set => _Name = value?.NotNullNotEmpty(nameof(Name));
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
    /// Determines if all arguments are requested.
    /// </summary>
    public bool AllArguments => Arguments.Count == 1 && Arguments[0].Name == "*";

    /// <summary>
    /// The collection of init/set optional specifications. By default this collection is empty.
    /// </summary>
    public OptionalsList Optionals { get; } = new();

    // ====================================================
    /// <summary>
    /// Represents the list of builder arguments.
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

            // Special case: '*' must be unique...
            if (item.IsNameAsterisk)
            {
                if (Items.Count > 0)
                    throw new ArgumentException("The '*' specification must be unique.");
            }

            // Others: name must no be duplicated...
            if (Items.Any(x => x.Name == item.Name)) throw new DuplicateException(
                $"Name '{item.Name}' is already used.")
                .WithData(item.Name, nameof(item.Name))
                .WithData(this, "this");

            Items.Add(item);
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a list of optionals init/set members.
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

            if (item.IsMemberAsterisk)
            {
                Items.Clear();
                if (item.IsInclude) Items.Add(item);
            }
            else
            {
                Items.Add(item);
            }
        }
    }

    // ====================================================
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