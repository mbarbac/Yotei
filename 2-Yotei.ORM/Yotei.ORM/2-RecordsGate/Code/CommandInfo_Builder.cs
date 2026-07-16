namespace Yotei.ORM.Records.Code;

partial class CommandInfo
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="ICommandInfo.IBuilder"/>
    /// </summary>
    [Cloneable]
    public partial class Builder : ICommandInfo.IBuilder
    {
        readonly StringBuilder _Text;
        readonly ParameterList.Builder _Parameters;

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="connection"></param>
        public Builder(IConnection connection)
        {
            ArgumentNullException.ThrowIfNull(connection);

            Connection = connection;
            _Text = new();
            _Parameters = new(connection.Engine);
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source, using its default
        /// iterable mode.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommand source)
        {
            ArgumentNullException.ThrowIfNull(source);

            Connection = source.Connection;

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

            Connection = source.Connection;

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

            Connection = source.Connection;
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

            Connection = source.Connection;
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

            Connection = other.Connection;
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
        public virtual ICommandInfo ToInstance() => throw null;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IConnection Connection { get; }

        IEngine Engine => Connection.Engine;

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
        public virtual bool IsConsistent
        {
            get
            {
                var text = Text;
                string? bracket;
                int pos, index, value;

                if (IsEmpty) return true;

                // No remaining '{...}' brackets...
                if (AreRemainingBrackets(text)) return false;

                // No dangling '#...' sequences, where '#' is the engine's prefix
                if (AreDanglingNameSequences(text)) return false;

                // No unused parameters...
                foreach (var par in _Parameters)
                {
                    index = FindNamedBracket(text, par.Name, 0, out _, strict: true);
                    if (index < 0) return false;
                }

                // No invalid ordinal specs...
                pos = 0;
                while ((index = FindNextOrdinalBracket(text, pos, out bracket, out value)) >= 0)
                {
                    if (value > _Parameters.Count) return false;
                    pos = index + bracket!.Length;
                }

                // Finishing...
                return true;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(
            ICommand source) => Add(source.ThrowWhenNull().GetCommandInfo());

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iterable"></param>
        /// <returns></returns>
        public virtual bool Add(
            ICommand source, bool iterable)
            => Add(source.ThrowWhenNull().GetCommandInfo(iterable));

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommandInfo source)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (source.IsEmpty) return false;

            var pars = source.Parameters;
            var text = NamesToOrdinals(source.Text, pars, Comparison);
            return Append(text, pars);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommandInfo.IBuilder source)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (source.IsEmpty) return false;

            var pars = source.Parameters;
            var text = NamesToOrdinals(source.Text, pars, Comparison);
            return Append(text, pars);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool Add(string? text, params object?[]? range)
        {
            return Append(text, range);
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool ReplaceText(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            if (text.Length == 0 && _Text.Length == 0) return false;

            _Text.Clear();
            _Text.Append(text);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool ReplaceValues(params object?[]? range)
        {
            range ??= [null];

            // Shortcut if range is empty...
            if (range.Length == 0)
            {
                if (_Parameters.Count == 0) return false;
                _Parameters.Clear();
                return true;
            }

            // Standard case...
            else
            {
                var old = _Parameters.Clone();
                var changed = Append(null, range);

                if (changed)
                {
                    for (int i = 0; i < old.Count; i++) _Parameters.RemoveAt(0);
                }
                else
                {
                    _Parameters.Clear();
                    _Parameters.AddRange(old);
                }
                return changed;
            }
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
        /// Appends to this instance the given text and values.
        /// <br/> If a parameter name already exist, it is changed to prevent collisions.
        /// <br/> If text is null, then only the values are captured.
        /// <br/> A 'null' value can be provided as range, it being the first and unique one.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Append(string? text, params object?[]? range)
        {
            var textnull = text is null;
            text ??= string.Empty;
            range ??= [null];

            var items = RangeElement.Capture(range);
            var changed = false;
            int pos;

            // Validations...
            PreventInitialCommandValues();
            PreventInitialDuplicatedNames();
            PreventInitialInvalidOrdinals();
            PreventInitialDanglingSequences();

            // Intercepting ranges with just one special element...
            if (items.Length == 1)
            {
                switch (items[0].Value)
                {
                    case IParameterList pars:
                        if (pars.Count == 1) { items[0] = new RangeElement(pars[0]); break; }
                        text = NamesToOrdinals(text, pars, Comparison);
                        range = [.. pars];
                        return Append(text, range);

                    case IParameterList.IBuilder pars:
                        if (pars.Count == 1) { items[0] = new RangeElement(pars[0]); break; }
                        text = NamesToOrdinals(text, pars, Comparison);
                        range = [.. pars];
                        return Append(text, range);
                }
            }

            // Iterating through the range of given values...
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                IParameter par;
                string name;

                // Capturing a parameter from the value...
                switch (item.Value)
                {
                    case IParameter temp:
                        par = Capture(temp, changeDuplicatedName: true);
                        name = temp.Name;
                        break;

                    case AnonymousElement temp:
                        par = new Parameter(temp.Name, temp.Value);
                        par = Capture(par, changeDuplicatedName: true);
                        name = temp.Name;
                        break;

                    default:
                        _Parameters.AddNew(item.Value, out par);
                        name = par.Name;
                        break;
                }

                // If text is null, then only capture values...
                if (textnull) { item.Used = true; continue; }

                // Finding by named bracket ('name' may not be 'par.Name')...
                pos = 0;
                while ((pos = FindNamedBracket(text, name, pos, out var bracket, strict: false)) >= 0)
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

            // Finishing...
            if (text.Length > 0) { _Text.Append(text); changed = true; }
            if (items.Any(x => x.Used)) changed = true;
            return changed;

            // --------------------------------------------

            /// <summary>
            /// Command-alike values are not allowed.
            /// </summary>
            void PreventInitialCommandValues()
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
            /// Intercept ranges with duplicated names.
            /// </summary>
            void PreventInitialDuplicatedNames()
            {
                List<string> names = [];

                foreach (var item in items)
                {
                    // Only parameters and anonymous carry names...
                    var name = item.Value switch
                    {
                        IParameter temp => temp.Name,
                        AnonymousElement temp => temp.Name,
                        _ => null
                    };
                    if (name is null) continue;

                    // Normalizing (and maybe re-storing, no harm in this)...
                    if (!name.StartsWith(Prefix, Comparison))
                    {
                        name = Prefix + name;
                        switch (item.Value)
                        {
                            case IParameter temp: item.Value = new Parameter(name, temp.Value); break;
                            case AnonymousElement temp: item.Value = new Parameter(name, temp.Value); break;
                        }
                    }

                    // Intercepting...
                    if (names.Any(x => string.Compare(x, name, Comparison) == 0))
                        throw new DuplicateException(
                            "Range of given values contains duplicated names.").WithData(items);

                    names.Add(name);
                }
            }

            /// <summary>
            /// Ordinal specifications in the given text must be valid ones.
            /// </summary>
            void PreventInitialInvalidOrdinals()
            {
                var pos = 0;
                int index;

                while ((index = FindNextOrdinalBracket(text, pos, out var bracket, out var value)) >= 0)
                {
                    if (value >= items.Length) throw new ArgumentException(
                        "Ordinal bracket value bigger than the number of values.")
                        .WithData(value)
                        .WithData(items);

                    pos = index + bracket!.Length;
                }
            }

            /// <summary>
            /// Prevents initial dangling '#...' sequences, where '#' is the engine's prefix.
            /// </summary>
            void PreventInitialDanglingSequences()
            {
                if (AreDanglingNameSequences(text)) throw new ArgumentException(
                    "There are dangling 'Prefix...' specifications.")
                    .WithData(text);
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Replaces the named '{name}' specifications in the given text with their corresponding
        /// ordinal '{n}' ones, where 'n' is the ordinal of the named specification in the given
        /// collection of parameters.
        /// </summary>
        static string NamesToOrdinals(
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
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index of the next '{n}' bracket, starting at the given position, or -1
        /// if any. If found, the bracket itself and its value are returned in the out arguements.
        /// </summary>
        /// Yes, yes, I know, I'm using gotos, life is hard...
        static int FindNextOrdinalBracket(string text, int pos, out string? bracket, out int value)
        {
            var ini = text.IndexOf('{', pos); if (ini < 0) goto NOTFOUND;
            var end = text.IndexOf('}', ini); if (end < 0) goto NOTFOUND;
            var len = end - ini - 1; if (len == 0) goto NOTFOUND;

            var span = text.AsSpan(ini + 1, len);
            if (int.TryParse(span, out value))
            {
                len += 2;
                bracket = text.Substring(ini, len);
                return ini;
            }

        NOTFOUND:
            bracket = null;
            value = 0;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of the given ordinal '{n}' bracket in the
        /// given source text, starting at the given index, or -1 if not found. Returns the found
        /// bracket in the out argument, or null if not found.
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
        /// Returns the index of the first ocurrence of the given '{name}' bracket in the given
        /// source text, starting at the given index, or -1 if not found. If found, returns the
        /// found bracket in the out argument.
        /// <br/> In strict mode, only the given name is used. In not-strict mode, it is tested
        /// with and without the engine's prefix, as needed.
        /// </summary>
        int FindNamedBracket(
            string text, string name, int ini, out string? bracket, bool strict)
        {
            if (ini < text.Length)
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
                        if (name.Length > 0)
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

        // ------------------------------------------------

        /// <summary>
        /// Validates that the name of the given parameter starts with the engine's prefix, and
        /// if not, return a new instance that complies.
        /// </summary>
        IParameter ValidateName(IParameter par)
        {
            return par.Name.StartsWith(Prefix, Comparison)
                ? par
                : par.WithName(Prefix + par.Name);
        }

        /// <summary>
        /// Captures into this instance the given parameter. If its name already exist in this
        /// instance, either it is substitued by a new automatic one, or a duplicated exception
        /// is thrown.
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        IParameter Capture(IParameter par, bool changeDuplicatedName)
        {
            par = ValidateName(par);

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
        /// Determines if the given text contains dangling '#...' sequences, where '#' is the
        /// engine's prefix.
        /// </summary>
        bool AreDanglingNameSequences(string text)
        {
            int index;
            var pos = 0;
            while ((index = text.IndexOf(Prefix, pos, Comparison)) >= 0)
            {
                if (index == 0) continue;
                if (text[index - 1] == ' ') return true;

                pos = index + Prefix.Length;
            }
            return false;
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