namespace Yotei.ORM.Records.Code;

partial class CommandInfo
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="ICommandInfo.IBuilder"/>
    /// </summary>
    [Cloneable(ReturnType = typeof(ICommandInfo.IBuilder))]
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
        /// Initializes a new instance using the the given text and the parameters obtained from
        /// the given range of values. If used, the parameters should be encoded in the given text
        /// using either a positional '{n}' specification, or a '{name}' named one.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="text"></param>
        /// <param name="values"></param>
        public Builder(IEngine engine, string text, params object?[]? values) : this(engine)
        {
            // Being a builder we accept by exception to initialize in an inconsistent state...
            var strict = false;
            Append(strict, text, values);
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source, using its default
        /// iterable mode.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommand source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var info = source.GetCommandInfo();
            _Text = new(info.Text);
            _Parameters = new(Engine, info.Parameters);
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source, using the requested
        /// iterable mode.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommand source, bool iterable)
        {
            ArgumentNullException.ThrowIfNull(source);

            var info = source.GetCommandInfo(iterable);
            _Text = new(info.Text);
            _Parameters = new(Engine, info.Parameters);
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo source)
        {
            ArgumentNullException.ThrowIfNull(source);

            _Text = new(source.Text);
            _Parameters = new(Engine, source.Parameters);
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo.IBuilder source)
        {
            ArgumentNullException.ThrowIfNull(source);

            _Text = new(source.Text);
            _Parameters = new(Engine, source.Parameters);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            ArgumentNullException.ThrowIfNull(other);

            _Text = new(other._Text.ToString());
            _Parameters = new(Engine, other._Parameters);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var str = Text; if (_Parameters.Count > 0)
            {
                var pars = $"[{string.Join(", ", _Parameters)}]";
                str = str.Length == 0 ? pars : $"{str} -- {pars}";
            }
            return str;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo ToInstance()
        {
            if (!IsConsistent) throw new InvalidOperationException(
                "This builder is in an inconsistent state.")
                .WithData(this);

            return IsEmpty ? new CommandInfo(Engine) : new CommandInfo(this);

        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine => _Parameters.Engine;

        string Prefix => Engine.ParameterPrefix;

        StringComparison Comparison => Engine.IgnoreCase
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Text => _Text.ToString();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IParameterList Parameters => _Parameters.ToInstance();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsEmpty => _Text.Length == 0 && _Parameters.Count == 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsConsistent
        {
            get
            {
                var text = Text;
                int pos, index;

                // Empty instances are consistent by definition...
                if (IsEmpty) return true;

                // No unused parameters...
                foreach (var par in _Parameters)
                {
                    index = FindNameSequence(text, 0, par.Name, false, out _);
                    if (index < 0) return false;
                }

                // No remaining '{...}' brackets...
                index = FindBracket(text, 0, out _);
                if (index >= 0) return false;

                // No invalid '#...' sequences...
                pos = 0;
                while (FindNameSequence(text, pos, out var sequence) >= 0)
                {
                    index = _Parameters.IndexOf(sequence!);
                    if (index < 0) return false;

                    pos += sequence!.Length;
                }

                // All tests passed, instance in a consistent state...
                return true;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommand source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var info = source.GetCommandInfo();
            var strict = true;
            var done = Append(strict, info.Text, info.Parameters);
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iterable"></param>
        /// <returns></returns>
        public virtual bool Add(ICommand source, bool iterable)
        {
            ArgumentNullException.ThrowIfNull(source);

            var info = source.GetCommandInfo(iterable);
            var strict = true;
            var done = Append(strict, info.Text, info.Parameters);
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommandInfo source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var strict = true;
            var done = Append(strict, source.Text, source.Parameters);
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommandInfo.IBuilder source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var strict = false;
            var done = Append(strict, source.Text, source.Parameters);
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool Add(string text, params object?[]? values)
        {
            ArgumentNullException.ThrowIfNull(text);
            values ??= [null];

            var strict = true;
            var done = Append(strict, text, values);
            return done;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool AddText(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            if (text.Length == 0) return false;

            _Text.Append(text);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool AddValues(params object?[]? values)
        {
            values ??= [null];

            if (values.Length == 0) return false;

            var strict = false;
            var done = Append(strict, string.Empty, values);
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool ReplaceText(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            _Text.Clear();
            _Text.Append(text);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReplaceValues(params object?[]? values)
        {
            values ??= [null];

            // Shortcut when value is an empty range...
            if (values.Length == 0)
            {
                if (_Parameters.Count == 0) return false; // Trivial case...

                _Parameters.Clear();
                return true;
            }

            // Standard case...
            var old = _Parameters.Clone();
            var strict = false;
            var done = Append(strict, string.Empty, values);

            if (done) // We need to remove the old ones as they have been replaced...
            {
                for (int i = 0; i < old.Count; i++) _Parameters.RemoveAt(0);
            }
            else // Something happened, let's play in the safe zone...
            {
                _Parameters.Clear();
                _Parameters.AddRange(old);
            }

            return done;
        }

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

        // ------------------------------------------------

        /// <summary>
        /// Appends to this instance the given text, if not null, and the collection of parameters
        /// obtained from the given range of values. If a parameter name already exists, it is
        /// changed to prevent name collisions.
        /// <br/> Strict mode is used when a consistent source text and is enforced. Otherwise, it
        /// is assumed they may be in an inconsistent state.
        /// </summary>
        bool Append(bool strict, string text, params object?[]? range)
        {
            ArgumentNullException.ThrowIfNull(text);
            range ??= [null];

            // Capturing range of values...
            var items = RangeElement.Capture(range);
            var changed = false;
            int pos, index, value;
            string? bracket;

            // Validations...
            PreventInvalidRangeValues();
            PreventInitialDuplicatedNames();
            PreventInvalidOrdinals();

            // Intercepting ranges with just one special element...
            if (items.Length == 1)
            {
                switch (items[0].Value)
                {
                    case IParameterList pars:
                        if (pars.Count == 1) { items[0] = new RangeElement(pars[0]); break; }
                        //text = NamesToOrdinals(text, pars, Comparison);
                        range = [.. pars];
                        return Append(strict, text, range);

                    case IParameterList.IBuilder pars:
                        if (pars.Count == 1) { items[0] = new RangeElement(pars[0]); break; }
                        //text = NamesToOrdinals(text, pars, Comparison);
                        range = [.. pars];
                        return Append(strict, text, range);
                }
            }

            // Iterating to capture parameters, and to change brackets into sequences...
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                IParameter par;
                string name, xname;

                // Capturing a parameter from the value...
                switch (item.Value)
                {
                    case IParameter temp:
                        par = Capture(temp, changeDuplicatedName: true);
                        name = temp.Name;
                        break;

                    case AnonymousElement temp:
                        xname = temp.Name;
                        if (!xname.StartsWith(Prefix, Comparison)) xname = Prefix + xname;
                        par = new Parameter(xname, temp.Value);
                        par = Capture(par, changeDuplicatedName: true);
                        name = temp.Name;
                        break;

                    default:
                        _Parameters.AddNew(item.Value, out par);
                        name = par.Name;
                        break;
                }

                // Not strict mode...
                if (!strict) item.Used = true;

                // Finding by named bracket ('name' may not be 'par.Name')...
                pos = 0;
                while ((index = FindNamedBracket(text, pos, name, out bracket, strict: false)) >= 0)
                {
                    text = text.Remove(index, bracket!.Length);
                    text = text.Insert(index, par.Name);
                    item.Used = true;

                    pos = index + par.Name.Length;
                }

                // Finding by ordinal bracket...
                pos = 0;
                while ((index = FindOrdinalBracket(text, pos, out bracket, out value)) >= 0)
                {
                    if (value == i)
                    {
                        text = text.Remove(index, bracket!.Length);
                        text = text.Insert(index, par.Name);
                        item.Used = true;
                    }
                    pos = index + par.Name.Length;
                }
            }

            // Finishing...
            if (text.Length > 0) { _Text.Append(text); changed = true; }
            if (items.Any(x => x.Used)) changed = true;
            return changed;

            // --------------------------------------------

            /// <summary>
            /// Prevents receiving a range with invalid values.
            /// </summary>
            void PreventInvalidRangeValues()
            {
                foreach (var item in items)
                {
                    switch (item.Value)
                    {
                        case ICommand:
                        case ICommandInfo:
                        case ICommandInfo.IBuilder:
                            throw new ArgumentException("Element cannot be a command-alike one.")
                            .WithData(item.Value);
                    }
                }
            }

            /// <summary>
            /// Prevents receiving a range with duplicated names.
            /// </summary>
            void PreventInitialDuplicatedNames()
            {
                List<string> names = [];

                foreach (var item in items)
                {
                    string? name = item.Value switch
                    {
                        IParameter temp => temp.Name,
                        AnonymousElement temp => temp.Name,
                        _ => null
                    };
                    if (name is not null)
                    {
                        // Normalizing...
                        if (!name.StartsWith(Prefix, Comparison)) name = Prefix + name;

                        // Trying to find...
                        var temp = names.Find(x => string.Compare(x, name, Comparison) == 0);
                        if (temp != null) throw new DuplicateException(
                            "Range of values contains duplicated names.")
                            .WithData(name, "name")
                            .WithData(items);

                        // Iterating...
                        names.Add(name);
                    }
                }
            }

            /// <summary>
            /// Prevents invalid ordinals to appear in the given text.
            /// </summary>
            void PreventInvalidOrdinals()
            {
                pos = 0;
                while ((index = FindOrdinalBracket(text, pos, out bracket, out value)) >= 0)
                {
                    if (value >= items.Length) throw new ArgumentException(
                        "Ordinal bracket value bigger than the number of values.")
                        .WithData(value)
                        .WithData(items);

                    pos = index + bracket!.Length;
                }
            }

            /// <summary>
            /// Validates that there are no remaining unused values.
            /// </summary>
            /*void ValidateNoUnusedValues()
            {
                if (items.Any(x => !x.Used)) throw new ArgumentException(
                    "There are unused values in the given range.")
                    .WithData(text)
                    .WithData(range);
            }*/

            /// <summary>
            /// Validates that there are no remaining dangling '{...}' brackets.
            /// </summary>
            /*void ValidateNoDanglingBrackets()
            {
                if (FindBracket(text, 0, out _) >= 0) throw new ArgumentException(
                    "There are unused '{...}' brackets in the given text.")
                    .WithData(text)
                    .WithData(range);
            }*/
        }

        // ------------------------------------------------

        /// <summary>
        /// Replaces the named '{name}' specifications in the given text with their corresponding
        /// ordinal '{n}' ones, where 'n' is the ordinal of the named specification in the given
        /// collection of parameters.
        /// </summary>
        /*static string NamesToOrdinals(
            string text, IEnumerable<IParameter> pars, StringComparison comparison)
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
        }*/

        // ------------------------------------------------

        /// <summary>
        /// Validates that the name of the given parameter starts with the engine's prefix, and
        /// if not, return a new instance that starts with it.
        /// </summary>
        IParameter WithValidatedName(IParameter par)
        {
            return par.Name.StartsWith(Prefix, Comparison)
                ? par
                : par.WithName(Prefix + par.Name);
        }

        /// <summary>
        /// Captures the given parameter into this instance. If its name already exists in this
        /// instance, either it is changed to a new one to prevent collisions, if requested, or
        /// an exception is thrown.
        /// </summary>
        IParameter Capture(IParameter par, bool changeDuplicatedName)
        {
            par = WithValidatedName(par);

            if (_Parameters.Contains(par.Name))
            {
                if (changeDuplicatedName) _Parameters.AddNew(par.Value, out par);
                else throw new DuplicateException(
                    "Name of parameter is a duplicated.")
                    .WithData(par)
                    .WithData(this);
            }
            else
            {
                _Parameters.Add(par);
            }

            return par;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index of the first '{...}' bracket in the given source text, starting
        /// from the given initial index, or -1 if any is found. If found, returns the bracket
        /// in the out argument, even if it is an empty one (only the '{}' bracket).
        /// </summary>
        static int FindBracket(string text, int ini, out string? bracket)
        {
            bracket = null;
            if (ini >= text.Length) return -1;

            var pos = text.IndexOf('{', ini); if (pos < 0) return -1;
            var end = text.IndexOf('}', pos); if (end < 0) return -1;

            bracket = text.Substring(pos, end - pos + 1);
            return pos;
        }

        /// <summary>
        /// Returns the index of the next ordinal bracket in the given source text, starting
        /// at the given initial index, or -1 if any is found. If found, then the bracket and
        /// its ordinal value are returned in the out arguments.
        /// </summary>
        static int FindOrdinalBracket(string text, int ini, out string? bracket, out int value)
        {
            bracket = null;
            value = 0;
            if (ini >= text.Length) return -1;

            int pos = ini;
            while ((pos = FindBracket(text, pos, out bracket)) >= 0)
            {
                var span = bracket.AsSpan(1, bracket!.Length - 2);
                var valid = true;
                foreach (var c in span) if (!char.IsAsciiDigit(c)) { valid = false; break; }

                if (valid && int.TryParse(span, out value)) return pos;
                pos += bracket.Length;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the next ocurrence of an ordinal '{value} bracket in the given
        /// source text, starting at the given initial index, or -1 if any is found.
        /// </summary>
        /*int FindOrdinalBracket(string text, int ini, int value)
        {
            if (ini < text.Length)
            {
                var bracket = $"{{{value}}}";

                var pos = text.IndexOf(bracket, ini);
                if (pos >= 0) return pos;
            }
            return -1;
        }*/

        /// <summary>
        /// Returns the index of the next named bracket in the given source text, starting at
        /// the given initial index, or -1 if any is found. If found, the bracket is returned
        /// in the out argument.
        /// </summary>
        /*int FindNamedBracket(string text, int ini, out string? bracket)
        {
            bracket = null;
            if (ini >= text.Length) return -1;

            int pos = ini;
            while ((pos = FindBracket(text, pos, out bracket)) >= 0)
            {
                var span = bracket.AsSpan(1, bracket!.Length - 2);
                var valid = false;
                foreach (var c in span) if (!char.IsAsciiDigit(c)) { valid = true; break; }

                if (valid) return pos;
                pos += bracket.Length;
            }

            return -1;
        }*/

        /// <summary>
        /// Returns the index of the next ocurrence of a named '{name} bracket in the given source
        /// text, starting at the given initial index, or -1 if any is found. If found, the bracket
        /// is returned in the out argument.
        /// <br/> If strict mode is requested, then the name must match the bracket's contents.
        /// Otherwise, such contents are tested with and without the engine's prefix.
        /// </summary>
        int FindNamedBracket(string text, int ini, string name, out string? bracket, bool strict)
        {
            if (ini < text.Length && name.Length > 0)
            {
                bracket = $"{{{name}}}";
                var pos = text.IndexOf(bracket, ini, Comparison);
                if (pos >= 0) return pos;

                if (!strict)
                {
                    if (!name.StartsWith(Prefix, Comparison)) // Add the engine's prefix
                    {
                        bracket = $"{{{Prefix + name}}}";
                        
                        pos = text.IndexOf(bracket, ini, Comparison);
                        if (pos >= 0) return pos;
                    }
                    else // Remove the engine's prefix...
                    {
                        name = name.Remove(0, Prefix.Length);

                        if (name.Length > 0 &&
                            !name.All(x => char.IsAsciiDigit(x)))
                        {
                            bracket = $"{{{name}}}";
                            pos = text.IndexOf(bracket, ini, Comparison);
                            if (pos >= 0) return pos;
                        }
                    }
                }
            }

            bracket = null;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a named sequence, as in '#...' (where '#'
        /// is the engine prefix), in the given source text starting from the given initial index,
        /// or -1 if any. If found, returns that sequence in the out argument, even if it is an
        /// empty one (only the engine's prefix).
        /// </summary>
        int FindNameSequence(string text, int ini, out string? sequence)
        {
            sequence = null;
            if (ini >= text.Length) return -1;

            var comparer = char.CharComparer(Comparison);
            var pos = text.IndexOf(Prefix, ini, Comparison);
            if (pos < 0) return -1;
            if (pos > 0)
            {
                var c = text[pos - 1];
                if (!IsolatedFinder.SEPARATORS.Contains(c, comparer)) return -1;
            }

            var span = text.AsSpan(pos + Prefix.Length);
            var end = span.IndexOfAny(IsolatedFinder.SEPARATORS, Comparison);

            if (end >= 0) // Found an embedded sequence...
            {
                sequence = text.Substring(pos, end + Prefix.Length);
                return pos;
            }
            else // Sequence spans to the end of the text...
            {
                sequence = text[pos..];
                return pos;
            }
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a name sequence '#...' using the given
        /// parameter name and the engine's prefix, starting from the given initial index, or
        /// -1 if any is found. If relax mode is requeste, the name is also tested adding at
        /// its head the engine's prefix. If found, the sequence is returned in the out argument.
        /// </summary>
        int FindNameSequence(string text, int ini, string name, bool relax, out string? sequence)
        {
            sequence = null;
            if (ini >= text.Length) return -1;

            var finder = new IsolatedFinder();
            int pos;

            if (name.StartsWith(Prefix, Comparison))
            {
                pos = finder.Find(text, ini, name);
                if (pos >= 0) { sequence = name; return pos; }
            }
            else if (relax)
            {
                name = Prefix + name;
                pos = finder.Find(text, ini, name);
                if (pos >= 0) { sequence = name; return pos; }
            }
            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// Represents an arbitrary element in the range of values.
        /// </summary>
        class RangeElement(object? value, bool used = false)
        {
            public object? Value = value;
            public bool Used = used;
            public override string ToString()
            {
                var str = Value switch
                {
                    IParameter item => $"Parameter: {item}",
                    AnonymousElement item => $"Anonymous: {item}",
                    _ => $"Value: {Value.Sketch()}",
                };
                if (Used) str += " (Used)";
                return str;
            }

            /// <summary>
            /// Captures the given range of values into a collection of normalized elements.
            /// </summary>
            public static RangeElement[] Capture(object?[]? range)
            {
                range ??= [null];
                if (range.Length == 0) return [];

                var items = new RangeElement[range.Length];
                for (int i = 0; i < range.Length; i++)
                {
                    var temp = AnonymousElement.TryCapture(range[i]);
                    items[i] = new(temp, false);
                }
                return items;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Represents an anonymous element in the range of values, as in '{ Name = Value }'
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