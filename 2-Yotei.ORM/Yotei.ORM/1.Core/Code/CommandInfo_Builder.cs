namespace Yotei.ORM.Code;

partial class CommandInfo
{
    // ====================================================
    /// <inheritdoc cref="ICommandInfo.IBuilder"/>
    [Cloneable<ICommandInfo.IBuilder>]
    public partial class Builder : ICommandInfo.IBuilder
    {
        readonly StringBuilder _Text;
        readonly ParameterList.Builder _Parameters;

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine)
        {
            _Text = new();
            _Parameters = new(engine);
        }

        /// <summary>
        /// Initializes a new instance with the contents of the given source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iterable"></param>
        public Builder(ICommand source, bool iterable)
            : this(source.ThrowWhenNull().GetCommandInfo(iterable)) { }

        /// <summary>
        /// Initializes a new instance with the contents of the given source
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo source)
        {
            source.ThrowWhenNull();

            _Text = new(source.Text);
            _Parameters = new(source.Engine, source.Parameters);
        }

        /// <summary>
        /// Initializes a new instance with the contents of the given source
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo.IBuilder source)
        {
            source.ThrowWhenNull();

            _Text = new(source.Text);
            _Parameters = new(source.Engine, source.Parameters);
        }

        /// <summary>
        /// Initializes a new instance with the given text and with the collection of parameters
        /// obtained from the given range of values.
        /// <br/> If values are used, then they must be encoded in the given text using either a '{n}'
        /// positional specification or a '{name}' named one. In the later case, 'name' may or may not
        /// start with the engine's prefix. Unused values or dangling specifications are not allowed.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="text"></param>
        /// <param name="range"></param>
        public Builder(IEngine engine, string text, params object?[]? range)
            : this(engine)
            => Add(text, range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source)
        {
            source.ThrowWhenNull();

            _Text = new(source.Text);
            _Parameters = new(source.Engine, source.Parameters);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (_Parameters.Count == 0) return _Text.ToString();

            var pars = $"[{string.Join(", ", _Parameters)}]";
            return _Text.Length == 0 ? pars : $"{_Text} : {pars}";
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public IEngine Engine => _Parameters.Engine;
        string Prefix => Engine.ParameterPrefix;
        StringComparison Comparison => Engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        /// <inheritdoc/>
        public string Text => _Text.ToString();

        /// <inheritdoc/>
        public IParameterList Parameters => _Parameters.CreateInstance();

        /// <inheritdoc/>
        public bool IsEmpty => _Text.Length == 0 && _Parameters.Count == 0;

        /// <inheritdoc/>
        public virtual ICommandInfo CreateInstance() => new CommandInfo(this);

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Add(ICommand source, bool iterable)
        {
            source.ThrowWhenNull();
            return Add(source.GetCommandInfo(iterable));
        }

        /// <inheritdoc/>
        public virtual bool Add(ICommandInfo source) => throw null;

        /// <inheritdoc/>
        public virtual bool Add(ICommandInfo.IBuilder source) => throw null;

        /// <inheritdoc/>
        public virtual bool Add(string text, params object?[]? range) => throw null;

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Clear()
        {
            if (IsEmpty) return false;

            _Text.Clear();
            _Parameters.Clear();
            return true;
        }

        // ------------------------------------------------

        /// <summary>
        /// Determines if the given text has any dangling '{...}' bracket specification, or not.
        /// </summary>
        static bool AreRemainingBrackets(string text)
        {
            var ini = text.IndexOf('{'); if (ini < 0) return false;
            var end = text.IndexOf('}', ini); if (end < 0) return false;
            return true;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of an ordinal '{value}' bracket in the given
        /// text, starting at the given position, or -1 if none is found. The out bracket argument
        /// is set to the found one, or null.
        /// </summary>
        static int FindOrdinalBracket(string text, int value, int ini, out string? bracket)
        {
            if (ini < text.Length)
            {
                bracket = $"{{{value}}}";

                var pos = text.IndexOf(bracket, ini);
                if (pos >= 0) return pos;
            }

            bracket = null;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a named '{name}' bracket in the given text,
        /// starting at the given position, or -1 if none is found. The out bracket argument is set
        /// to the found one, or null. The 'name' literal is tested with and without the engine's
        /// prefix as needed.
        /// </summary>
        [SuppressMessage("", "IDE0057")]
        int FindNamedBracket(string text, string name, int ini, out string? bracket)
        {
            if (ini < text.Length)
            {
                bracket = $"{{{name}}}";
                var pos = text.IndexOf(bracket, ini, Comparison);
                if (pos >= 0) return pos;

                if (!name.StartsWith(Prefix, Comparison))
                {
                    bracket = $"{{{Prefix + name}}}";
                    pos = text.IndexOf(bracket, ini, Comparison);
                    if (pos >= 0) return pos;
                }
                else
                {
                    name = name.Remove(0, Prefix.Length);
                    if (name.Length > 0)
                    {
                        bracket = $"{{{name}}}";
                        pos = text.IndexOf(bracket, ini, Comparison);
                        if (pos >= 0) return pos;
                    }
                }
            }

            bracket = null;
            return -1;
        }

        /// <summary>
        /// Validates that the name of the given parameter starts with the engine's prefix and,
        /// if not, returns a new instance with a compliant name.
        /// </summary>
        IParameter ValidateName(IParameter par)
        {
            return par.Name.StartsWith(Prefix, Comparison)
                ? par
                : par.WithName(Prefix + par.Name);
        }

        /// <summary>
        /// Captures the given parameter into both this instance and into the additional extra
        /// collection, intended to be a temporary repository of captured ones used to prevent
        /// duplicated names in that collection.
        /// <br/> Returns either the given parameter, or a new one created to prevent collision
        /// of its name with any existing one.
        /// </summary>
        IParameter Capture(IParameter par, ParameterList.Builder captured)
        {
            par = ValidateName(par);

            if (captured.Contains(par.Name)) throw new DuplicateException(
                "Duplicated name element detected in captured elements.")
                .WithData(par.Name)
                .WithData(captured);

            if (_Parameters.Contains(par.Name)) _Parameters.AddNew(par.Value, out par);
            else _Parameters.Add(par);

            captured.Add(par);
            return par;
        }

        /// <summary>
        /// Represents an arbitrary element in the given range of values.
        /// </summary>
        class RangeElement(object? value, bool used = false)
        {
            public object? Value = value;
            public bool Used = used;
            public override string ToString()
            {
                var sb = new StringBuilder(); switch (Value)
                {
                    case IParameter item: sb.Append($"Parameter: {item}"); break;
                    case AnonymousElement item: sb.Append($"Anonymous: {item}"); break;
                    default: sb.Append($"Value: '{Value.Sketch()}'"); break;
                }
                if (Used) sb.Append(" (Used)");
                return sb.ToString();
            }

            /// <summary>
            /// Captures the elements of the given range of value into normalized elements.
            /// </summary>
            public static RangeElement[] Capture(object?[]? range)
            {
                range ??= [null];
                if (range.Length == 0) return [];

                var items = new RangeElement[range.Length];
                for (int i = 0; i < range.Length; i++)
                {
                    var temp = AnonymousElement.TryCapture(range[i]);
                    items[i] = new(temp);
                }
                return items;
            }
        }

        /// <summary>
        /// Represents an anonymous element in the range of values, as in 'new {Name = Value}'.
        /// </summary>
        class AnonymousElement(string name, object? value)
        {
            public string Name = name;
            public object? Value = value;
            public override string ToString() => $"{Name}:'{Value}'";

            /// <summary>
            /// Tries to capture the given value as an anonymous item, if possible, or otherwise
            /// returns the original object itself.
            /// </summary>
            public static object? TryCapture(object? value)
            {
                if (value is not null)
                {
                    var type = value.GetType();
                    if (type.IsAnonymous())
                    {
                        var members = type.GetProperties();
                        if (members.Length == 0) throw new ArgumentException("No properties found in anonymous argument.").WithData(value);
                        if (members.Length > 1) throw new ArgumentException("Too many properties found in anonymous argument.").WithData(value);

                        var member = members[0];
                        var name = member.Name;
                        var temp = member.GetValue(value);

                        value = new AnonymousElement(name, temp);
                    }
                }
                return value;
            }
        }
    }
}