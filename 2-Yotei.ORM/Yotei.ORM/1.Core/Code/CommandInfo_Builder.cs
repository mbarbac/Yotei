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
        /// start with the engine's prefix.
        /// <br/> Unused values or dangling specifications are not allowed.
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
        public bool IsValid()
        {
            throw null;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Add(ICommand source, bool iterable)
        {
            source.ThrowWhenNull();
            return Add(source.GetCommandInfo(iterable));
        }

        /// <inheritdoc/>
        public virtual bool Add(ICommandInfo source)
        {
            source.ThrowWhenNull();

            var text = source.Text;
            var pars = source.Parameters;

            // 'source' may be in an inconsistent state, so we cannot enforce specs and values...
            var noRemainingSpecs = false;
            var noUnusedValues = false;
            return Append(noRemainingSpecs, noUnusedValues, text, pars);
        }

        /// <inheritdoc/>
        public virtual bool Add(ICommandInfo.IBuilder source)
        {
            source.ThrowWhenNull();

            var text = source.Text;
            var pars = source.Parameters;

            // 'source' may be in an inconsistent state, so we cannot enforce specs and values...
            var noRemainingSpecs = false;
            var noUnusedValues = false;
            return Append(noRemainingSpecs, noUnusedValues, text, pars);
        }

        /// <inheritdoc/>
        public virtual bool Add(string text, params object?[]? range)
        {
            // Here we enforce adding a consistent state...
            var noRemainingSpecs = true;
            var noUnusedValues = true;
            return Append(noRemainingSpecs, noUnusedValues, text, range);
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool ReplaceText(string text)
        {
            text.ThrowWhenNull();

            if (_Text.Length == 0 && text.Length == 0) return false;
            if (string.Compare(_Text.ToString(), text) == 0) return false;

            _Text.Clear();
            _Text.Append(text);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool ReplaceValues(params object?[]? range)
        {
            range ??= [null];

            if (range.Length == 0)
            {
                if (_Parameters.Count == 0) return false;

                _Parameters.Clear();
                return true;
            }

            var old = _Parameters.ToImmutableArray();
            _Parameters.Clear();

            var noRemainingSpecs = false;
            var noUnusedValues = false;
            var changed = Append(noRemainingSpecs, noUnusedValues, string.Empty, range);

            if (!changed)
            {
                _Parameters.Clear();
                _Parameters.AddRange(old);
            }
            return changed;
        }

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
        /// Appends to this instance the given text and the collection of parameters obtained from
        /// the given collection of values, if any. Values must be encoded in the text using either
        /// a named '{name}' or an ordinal '{n}' bracket specification.
        /// <br/> If a given 'name' already exist in this collection, it is changed in both the
        /// given text and values so that it can be added without name collisions.
        /// </summary>
        bool Append(
            bool noRemainingSpecs,
            bool noUnusedValues,
            string text, params object?[]? range)
        {
            text.ThrowWhenNull();
            range ??= [null];

            var changed = false;

            // Capturing values, intercepting ranges with just one special element...
            var items = RangeElement.Capture(range);
            if (items.Length == 1)
            {
                if (items[0].Value is IParameterList parsList)
                {
                    text = NamesToOrdinals(text, parsList, Comparison);
                    range = parsList.ToArray();
                    return Append(noRemainingSpecs, noUnusedValues, text, range);
                }
                if (items[0].Value is IParameterList.IBuilder parsBuilder)
                {
                    text = NamesToOrdinals(text, parsBuilder, Comparison);
                    range = parsBuilder.ToArray();
                    return Append(noRemainingSpecs, noUnusedValues, text, range);
                }
            }

            // Command-alike parameters not allowed...
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                if (item.Value is ICommand) throw new ArgumentException("Element cannot be a command.").WithData(item);
                if (item.Value is ICommandInfo) throw new ArgumentException("Element cannot be a command info.").WithData(item);
                if (item.Value is ICommandInfo.IBuilder) throw new ArgumentException("Element cannot be a command info builder.").WithData(item);
            }

            // Iterating through the range of given values...
            var captured = new ParameterList.Builder(Engine);
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                string name = default!;
                IParameter par;

                // Capturing a suitable parameter...
                switch (item.Value)
                {
                    case IParameter temp:
                        par = Capture(temp, captured);
                        name = temp.Name;
                        changed = true;
                        break;

                    case AnonymousElement temp:
                        par = new Parameter(temp.Name, temp.Value);
                        par = Capture(par, captured);
                        name = temp.Name;
                        changed = true;
                        break;

                    default:
                        _Parameters.AddNew(item.Value, out par);
                        captured.Add(par);
                        name = par.Name;
                        changed = true;
                        break;
                }

                // Finding the element's 'name' in the text, may not be the same as 'par.Name'...
                var pos = 0;
                while ((pos = FindNamedBracket(text, name, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);

                    pos += par.Name.Length;
                    item.Used = true;
                }

                // Finding by ordinal braket...
                pos = 0;
                while ((pos = FindOrdinalBracket(text, i, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);

                    pos += par.Name.Length;
                    item.Used = true;
                }
            }

            // Adding the text if needed...
            if (text.Length > 0)
            {
                _Text.Append(text);
                changed = true;
            }

            // No remaining specifications...
            if (noRemainingSpecs && AreRemainingBrackets(text))
            {
                throw new ArgumentException(
                    "There are unused brackets in the given text.")
                    .WithData(text);
            }

            // No unused values...
            if (noUnusedValues && items.Length > 0 && items.Any(x => !x.Used))
            {
                throw new ArgumentException(
                    "There are unused brackets in the given text.")
                    .WithData(text);
            }

            // Finishing...
            return changed;
        }

        /// <summary>
        /// Replaces the named specifications of the given collection of parameters in the given
        /// text with ordinal specifications.
        /// </summary>
        static string NamesToOrdinals(
            string text, IEnumerable<IParameter> pars, StringComparison comparison)
        {
            var finder = new StrFindIsolated();
            var i = 0;
            foreach (var par in pars)
            {
                var bracket = $"{{{i}}}"; i++;
                var name = par.Name;
                var pos = 0;

                while ((pos = finder.Find(text, name, pos, comparison)) >= 0)
                {
                    text = text.Remove(pos, par.Name.Length);
                    text = text.Insert(pos, bracket);
                    pos += bracket.Length;
                }
            }

            return text;
        }

        /// <summary>
        /// Determines if the given text has any dangling '{...}' bracket specification, or not.
        /// </summary>
        static bool AreRemainingBrackets(string text)
        {
            if (text.Length == 0) return false;

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