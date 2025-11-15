namespace Yotei.ORM.Code;

partial class CommandInfo
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="ICommandInfo.IBuilder"/>
    /// </summary>
    [Cloneable<ICommandInfo.IBuilder>]
    public partial class Builder : ICommandInfo.IBuilder
    {
        readonly StringBuilder _Text;
        readonly ParameterList.Builder _Parameters;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="connection"></param>
        public Builder(IConnection connection)
        {
            Connection = connection.ThrowWhenNull();
            _Text = new();
            _Parameters = new(Engine);
        }

        /// <summary>
        /// Initializes a new instance using the contents from the given source.
        /// <br/> This method accepts sources in inconsistent states.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iterable"></param>
        public Builder(ICommand source, bool iterable)
            : this(source.ThrowWhenNull().GetCommandInfo(iterable)) { }

        /// <summary>
        /// Initializes a new instance using the contents from the given source.
        /// <br/> This method accepts sources in inconsistent states.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo source)
        {
            source.ThrowWhenNull();

            Connection = source.Connection;
            _Text = new(source.Text);
            _Parameters = new(Engine, source.Parameters);
        }

        /// <summary>
        /// Initializes a new instance using the contents from the given source.
        /// <br/> This method accepts sources in inconsistent states.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo.IBuilder source)
        {
            source.ThrowWhenNull();

            Connection = source.Connection;
            _Text = new(source.Text);
            _Parameters = new(Engine, source.Parameters);
        }

        /// <summary>
        /// Initializes a new instance with the given text and the collection of parameters
        /// obtained from the given range of values, if any.
        /// <br/> If values are used, then they must be encoded in the given text using either a
        /// '{n}' positional specification, or a '{name}' named one (where 'name' may or may not
        /// start with the engine parameter prefix).
        /// <br/> Unused values or dangling specifications are not allowed.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="text"></param>
        /// <param name="range"></param>
        public Builder(IConnection connection, string? text, params object?[]? range)
            : this(connection)
            => Add(text, range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source)
        {
            Connection = source.ThrowWhenNull().Connection;
            _Text = new(source.Text);
            _Parameters = new(Engine, source.Parameters);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var str = Text; if (Count > 0)
            {
                var pars = $"[{string.Join(", ", _Parameters)}]";
                str = str.Length == 0 ? pars : $"{str} -- {pars}";
            }
            return str;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo CreateInstance() => new CommandInfo(this);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IConnection Connection { get; }
        IEngine Engine => Connection.Engine;
        string Prefix => Engine.ParameterPrefix;
        StringComparison Comparison => Engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Text => _Text.ToString();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int TextLen => _Text.Length;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IParameterList Parameters => _Parameters.CreateInstance();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count => _Parameters.Count;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual bool IsEmpty => TextLen == 0 && Count == 0;

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual bool IsConsistent
        {
            get
            {
                var text = Text;
                var finder = new IsolatedFinder();
                int count, pos;
                bool found;

                // Shortcut for empty instances...
                if (IsEmpty) return true;

                // No dangling '{...}' brackets...
                if (AreRemainingBrackets(text)) return false;

                // No unused parameters, removing names of existing for simplicity...
                count = 0;
                foreach (var par in _Parameters)
                {
                    found = false;
                    pos = 0;
                    while ((pos = finder.Find(text, pos, par.Name, Comparison)) >= 0)
                    {
                        text = text.Remove(pos, par.Name.Length);
                        found = true;
                    }
                    if (found) count++;
                }
                if (count != _Parameters.Count) return false;

                // No dangling '#...' sequences, '#' being the parameter prefix...
                pos = 0;
                while ((pos = text.IndexOf(Prefix, pos, Comparison)) >= 0)
                {
                    if (pos == 0) return false;
                    if (IsolatedFinder.SEPARATORS.Contains(text[pos - 1])) return false;
                }

                // Finishing without impediments...
                return true;
            }
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iterable"></param>
        /// <returns></returns>
        public virtual bool Add(ICommand source, bool iterable)
        {
            source.ThrowWhenNull();
            return Add(source.GetCommandInfo(iterable));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommandInfo source)
        {
            source.ThrowWhenNull();

            var noRemainingSpecs = false;
            var noUnusedValues = false;
            return Append(noRemainingSpecs, noUnusedValues, source.Text, source.Parameters);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommandInfo.IBuilder source)
        {
            source.ThrowWhenNull();

            var noRemainingSpecs = false;
            var noUnusedValues = false;
            return Append(noRemainingSpecs, noUnusedValues, source.Text, source.Parameters);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool Add(string? text, params object?[]? range)
        {
            range ??= [null];

            var noRemainingSpecs = true;
            var noUnusedValues = true;
            return Append(noRemainingSpecs, noUnusedValues, text, range);
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool ReplaceText(string text)
        {
            text.ThrowWhenNull();

            if (TextLen == 0 && text.Length == 0) return false;
            if (string.Compare(Text, text) == 0) return false;

            _Text.Clear();
            _Text.Append(text);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool ReplaceParameters(params object?[]? range)
        {
            range ??= [null];

            // Shortcuts when given range is empty...
            if (range.Length == 0)
            {
                if (_Parameters.Count == 0) return false;

                _Parameters.Clear();
                return true;
            }

            // Standard case...
            var old = _Parameters.Clone();
            var noRemainingSpecs = false;
            var noUnusedValues = false;
            var changed = Append(noRemainingSpecs, noUnusedValues, null, range);

            if (changed) // Removing the original parameters...
            {
                for (int i = 0; i < old.Count; i++) _Parameters.RemoveAt(0);
            }
            else // Recovering the original collection...
            {
                _Parameters.Clear();
                _Parameters.AddRange(old);
            }
            return changed;
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear()
        {
            if (IsEmpty) return false;

            _Text.Clear();
            _Parameters.Clear();
            return true;
        }

        // ----------------------------------------------------

        /// <summary>
        /// Appends to this instance the given text and the collection of parameters obtained from
        /// the given range of values, if any.
        /// <br/> If text is null, then the range of values is captured without validanting that
        /// their names are encoded in the text. Otherwise, if both text and values are used, then
        /// they must be encoded in the text using either a positional '{n}' or a named '{name}'
        /// specification, where 'name' may or may not start with the engine prefix.
        /// <br/> If a 'name' is already captured, then it is changed in both the text and parameter
        /// names to prevent colisions (so enabling using positional '{0}' in the input text).
        /// <br/> Unused values or dangling specifications are allowed only if explicitly requested.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="noRemainingSpecs"></param>
        /// <param name="noUnusedValues"></param>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Append(
            bool noRemainingSpecs,
            bool noUnusedValues,
            string? text, params object?[]? range)
        {
            var textnull = text is null;

            text ??= string.Empty;
            range ??= [null];
            var items = RangeElement.Capture(range);
            var changed = false;

            // Intercepting ranges with just one special element...
            if (items.Length == 1)
            {
                switch (items[0].Value)
                {
                    case IParameterList parsList:
                        text = NamesToOrdinals(text, parsList, Comparison);
                        range = parsList.ToArray();
                        return Append(noRemainingSpecs, noUnusedValues, text, range);

                    case IParameterList.IBuilder parsBuilder:
                        text = NamesToOrdinals(text, parsBuilder, Comparison);
                        range = parsBuilder.ToArray();
                        return Append(noRemainingSpecs, noUnusedValues, text, range);
                }
            }

            // Command-alike parameters are not allowed...
            for (int i = 0; i < items.Length; i++)
            {
                switch (items[i].Value)
                {
                    case ICommand:
                    case ICommandInfo:
                    case ICommandInfo.IBuilder:
                        throw new ArgumentException("Element cannot be a command-alike one.")
                        .WithData(items[i].Value);
                }
            }

            // Intercepting duplicated names in the given range...
            List<string> names = [];
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var name = item.Value switch
                {
                    IParameter temp => temp.Name,
                    AnonymousElement temp => temp.Name,
                    _ => null
                };

                if (name is not null)
                {
                    // Normalizing name if needed...
                    if (!name.StartsWith(Prefix, Comparison))
                    {
                        name = Prefix + name;
                        switch (item.Value)
                        {
                            case IParameter temp: item.Value = new Parameter(name, temp.Value); break;
                            case AnonymousElement temp: item.Value = new Parameter(name, temp.Value); break;
                        }
                    }

                    // Intercepting duplicates...
                    if (names.Contains(name)) throw new DuplicateException(
                        "Duplicated name detected in the given range of values.")
                        .WithData(name)
                        .WithData(names);

                    names.Add(name);
                }
            }

            // Iterating through the given values...
            if (items.Length > 0) changed = true;

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                string name;
                IParameter par;

                // Capturing a suitable parameter...
                switch (item.Value)
                {
                    case IParameter temp:
                        par = Capture(temp);
                        name = temp.Name;
                        break;

                    case AnonymousElement temp:
                        par = new Parameter(temp.Name, temp.Value);
                        par = Capture(par);
                        name = temp.Name;
                        break;

                    default:
                        _Parameters.AddNew(item.Value, out par);
                        name = par.Name;
                        break;
                }

                // If text was null, then we only need to capture values...
                if (textnull)
                {
                    item.Used = true;
                    continue;
                }

                // Finding by named bracket, 'name' may not be the same as 'par.Name'...
                var pos = 0;
                while ((pos = FindNamedBracket(text, name, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);

                    pos += par.Name.Length;
                    item.Used = true;
                }

                // Finding by ordinal bracket...
                pos = 0;
                while ((pos = FindOrdinalBracket(text, i, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);

                    pos += par.Name.Length;
                    item.Used = true;
                }
            }

            // No remaining specifications...
            if (noRemainingSpecs && text.Length > 0 && AreRemainingBrackets(text))
            {
                throw new ArgumentException(
                    "There are unused brackets in the given text.")
                    .WithData(text);
            }

            // No unused values...
            if (noUnusedValues && !textnull &&
                items.Length > 0 && items.Any(static x => !x.Used))
            {
                throw new ArgumentException(
                    "There are unused values in the given range.")
                    .WithData(text);
            }

            // Finishing...
            if (text.Length > 0) { _Text.Append(text); changed = true; }
            return changed;
        }

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the named '{name}' specifications of the given parameters in the given text
        /// with their ordinal '{n}' ones.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pars"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        /// This method needs to be public as it is used in other places.
        public static string NamesToOrdinals(
            string text,
            IEnumerable<IParameter> pars,
            StringComparison comparison)
        {
            text.ThrowWhenNull();
            pars.ThrowWhenNull();

            var finder = new IsolatedFinder();
            var i = 0;
            foreach (var par in pars)
            {
                var bracket = $"{{{i}}}"; i++;
                var pos = 0;

                while ((pos = finder.Find(text, pos, par.Name, comparison)) >= 0)
                {
                    text = text.Remove(pos, par.Name.Length);
                    text = text.Insert(pos, bracket);
                    pos += bracket.Length;
                }
            }
            return text;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of an ordinal '{n}' bracket in the given
        /// text, starting at the given position, or -1 if the bracket was not found. The out
        /// argument contains the found bracket, if any.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="ini"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
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
        /// Returns the index of the first ocurrence of an ordinal '{name}' bracket in the given
        /// text, starting at the given position, or -1 if the bracket was not found. The 'name'
        /// literal is tested with and without the engine prefix, if needed. The out argument
        /// contains the found bracket, if any.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <param name="ini"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
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
        /// Determines if the given text has any dangling '{...}' braket specification.
        /// </summary>
        static bool AreRemainingBrackets(string text)
        {
            if (text.Length == 0) return false;

            var ini = text.IndexOf('{'); if (ini < 0) return false;
            var end = text.IndexOf('}', ini); if (end < 0) return false;
            return true;
        }

        /// <summary>
        /// Validates that the name of the given parameter starts with the engine's prefix, and
        /// if not, creates a new instance that complies.
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        IParameter ValidateName(IParameter par)
        {
            return par.Name.StartsWith(Prefix, Comparison)
                ? par
                : par.WithName(Prefix + par.Name);
        }

        /// <summary>
        /// Captures the given parameter into both this instance. Returns the captured parameter,
        /// which can either be the original one, or a new one created to prevent name duplicates
        /// with the *existing* ones.
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        IParameter Capture(IParameter par)
        {
            par = ValidateName(par);

            if (_Parameters.Contains(par.Name)) _Parameters.AddNew(par.Value, out par);
            else _Parameters.Add(par);

            return par;
        }

        // ----------------------------------------------------

        /// <summary>
        /// Represents an arbitrary element in a given range of values.
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
            /// Captures the given range of values into a collection of normalized elements.
            /// </summary>
            /// <param name="range"></param>
            /// <returns></returns>
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

        // ----------------------------------------------------

        /// <summary>
        /// Represents an anonymous element in a range of values, as in 'new {Name = Value}'.
        /// </summary>
        class AnonymousElement(string name, object? value)
        {
            public string Name = name;
            public object? Value = value;
            public override string ToString() => $"{Name}:'{Value}'";

            /// <summary>
            /// Tries to capture the given value into an anonymous item. If not possible, returns
            /// the original value itself.
            /// </summary>
            public static object? TryCapture(object? value)
            {
                if (value is not null)
                {
                    var type = value.GetType();
                    if (type.IsAnonymous)
                    {
                        var members = type.GetProperties();
                        if (members.Length == 0) throw new ArgumentException("Anonymous element with no properties.").WithData(value);
                        if (members.Length > 1) throw new ArgumentException("Anonymous element with too many properties.").WithData(value);

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