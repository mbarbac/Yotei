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
        public Builder(IEngine engine, string text, params object?[]? values)
            : this(engine)
            => Add(text, values);

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
            _Parameters = new(info.Parameters.Engine, info.Parameters);
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
            _Parameters = new(info.Parameters.Engine, info.Parameters);
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo source)
        {
            ArgumentNullException.ThrowIfNull(source);

            _Text = new(source.Text);
            _Parameters = new(source.Parameters.Engine, source.Parameters);
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo.IBuilder source)
        {
            ArgumentNullException.ThrowIfNull(source);

            _Text = new(source.Text);
            _Parameters = new(source.Parameters.Engine, source.Parameters);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            ArgumentNullException.ThrowIfNull(other);

            _Text = new(other._Text.ToString());
            _Parameters = new(other._Parameters.Engine, other._Parameters);
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
                    index = FindNamedSequence(text, 0, par.Name, out _);
                    if (index < 0) return false;
                }

                // No remaining '{...}' brackets...
                index = FindBracket(text, 0, out _);
                if (index >= 0) return false;

                // No invalid '#...' sequences...
                pos = 0;
                while (FindNamedSequence(text, pos, out var str) >= 0)
                {
                    index = _Parameters.IndexOf(str!);
                    if (index < 0) return false;

                    pos += str!.Length;
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
            var done = Add(info.Text, info.Parameters);
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
            var done = Add(info.Text, info.Parameters);
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

            var done = Add(source.Text, source.Parameters);
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

            var done = Add(source.Text, source.Parameters);
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

            var done = Add(string.Empty, values);
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
            var done = Add(string.Empty, values);

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
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="values"></param>
        public virtual bool Add(string text, params object?[]? values)
        {
            ArgumentNullException.ThrowIfNull(text);
            values ??= [null];

            // Capturing the range of values...
            var items = RangeItem.CaptureValues(values);
            var done = false;
            int pos, index, ordinal;
            string? str, name;

            // Validations...
            PreventInvalidValues(items);
            PreventDuplicatedNames(items);
            PreventInvalidOrdinals(text, items);

            // Intercepting ranges with just one special element...
            if (items.Length == 1)
            {
                switch (items[0].Payload)
                {
                    case IParameterList pars:
                        values = [.. pars];
                        items = RangeItem.CaptureValues(values);
                        break;

                    case IParameterList.IBuilder pars:
                        values = [.. pars];
                        items = RangeItem.CaptureValues(values);
                        break;
                }
            }

            // Intercepting collisions...
            for (int i = 0; i < items.Length; i++)
            {
                var need = items[i].Payload switch
                {
                    IParameter temp => IsCollision(i, temp.Name),
                    AnonymousElement temp => IsCollision(i, temp.Name),
                    _ => false
                };
                if (need)
                {
                    name = _Parameters.NextName();
                    string oldname = "";
                    
                    switch (items[i].Payload)
                    {
                        case IParameter temp:
                            oldname = temp.Name;
                            items[i] = new(new Parameter(name, temp.Value));
                            break;

                        case AnonymousElement temp:
                            oldname = temp.Name;
                            items[i] = new(new Parameter(name, temp.Value));
                            break;
                    }

                    pos = 0;
                    while ((index = FindNamedBracket(text, pos, oldname, out str)) >= 0)
                    {
                        text = text.Remove(index, str!.Length);
                        text = text.Insert(index, name);
                        pos = index + name.Length;
                    }

                    pos = 0;
                    while ((index = FindNamedSequence(text, pos, oldname, out str)) >= 0)
                    {
                        text = text.Remove(index, str!.Length);
                        text = text.Insert(index, name);
                        pos = index + name.Length;
                    }
                }
            }

            // Iterating through captured items...
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                IParameter par;

                // Capturing into a parameter...
                switch (item.Payload)
                {
                    case IParameter temp:
                        name = temp.Name;
                        par = Capture(temp);
                        break;

                    case AnonymousElement temp:
                        name = temp.Name;
                        par = new Parameter(temp.Name, temp.Value);
                        par = Capture(par);
                        break;

                    default:
                        name = string.Empty;
                        _Parameters.AddNew(item.Payload, out par);
                        break;
                }

                pos = 0;
                while ((index = FindNamedBracket(text, pos, name, out str)) >= 0)
                {
                    text = text.Remove(index, str!.Length);
                    text = text.Insert(index, par.Name);
                    pos = index + par.Name.Length;
                }

                pos = 0;
                while ((index = FindNamedSequence(text, pos, name, out str)) >= 0)
                {
                    text = text.Remove(index, str!.Length);
                    text = text.Insert(index, par.Name);
                    pos = index + par.Name.Length;
                }

                pos = 0;
                while ((index = FindOrdinalBracket(text, pos, out str, out ordinal)) >= 0)
                {
                    if (ordinal == i)
                    {
                        text = text.Remove(index, str!.Length);
                        text = text.Insert(index, par.Name);
                    }
                    pos = index + par.Name.Length;
                }

                pos = 0;
                while ((index = FindOrdinalSequence(text, pos, out str, out ordinal)) >= 0)
                {
                    if (ordinal == i)
                    {
                        text = text.Remove(index, str!.Length);
                        text = text.Insert(index, par.Name);
                    }
                    pos = index + par.Name.Length;
                }
            }

            // Finishing...
            if (items.Length > 0) done = true;
            if (text.Length > 0) { _Text.Append(text); done = true; }
            return done;
        }

        /// <summary>
        /// Prevents invalid initial values.
        /// </summary>
        static void PreventInvalidValues(RangeItem[] items)
        {
            foreach (var item in items)
            {
                switch (item.Payload)
                {
                    case ICommand:
                    case ICommandInfo:
                    case ICommandInfo.IBuilder:
                        throw new ArgumentException("Element cannot be a command-alike one.")
                        .WithData(item.Payload);
                }
            }
        }

        /// <summary>
        /// Prevents initial ranges with duplicated names.
        /// </summary>
        /// <param name="items"></param>
        void PreventDuplicatedNames(RangeItem[] items)
        {
            List<string> names = [];

            foreach (var item in items)
            {
                string? name = item.Payload switch
                {
                    IParameter temp => temp.Name,
                    AnonymousElement temp => temp.Name,
                    _ => null
                };
                if (name is not null)
                {
                    if (!name.StartsWith(Prefix, Comparison)) name = Prefix + name;

                    var temp = names.Find(x => string.Compare(x, name, Comparison) == 0);
                    if (temp != null) throw new DuplicateException(
                        "Range of values contains duplicated names.")
                        .WithData(name, "name")
                        .WithData(items);

                    names.Add(name);
                }
            }
        }

        /// <summary>
        /// Prevents invalid initial ordinals.
        /// </summary>
        void PreventInvalidOrdinals(string text, RangeItem[] items)
        {
            int index, pos = 0;

            while ((index = FindOrdinalBracket(text, pos, out var str, out var value)) >= 0)
            {
                if (value >= items.Length) throw new ArgumentException(
                    "Ordinal bracket value bigger than the number of values.")
                    .WithData(value)
                    .WithData(items);

                pos = index + str!.Length;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Guarantees that the name of the given parameter starts with the engine's prefix, or
        /// creates and returns a new one that does it.
        /// </summary>
        IParameter WithPrefixName(IParameter par)
        {
            return par.Name.StartsWith(Prefix, Comparison)
                ? par
                : par.WithName(Prefix + par.Name);
        }

        /// <summary>
        /// Captures the given parameter into this instance. If its name already exists, then it
        /// is changed to prevent name collisions.
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        IParameter Capture(IParameter par)
        {
            par = WithPrefixName(par);

            if (_Parameters.Contains(par.Name)) _Parameters.AddNew(par.Value, out par);
            else _Parameters.Add(par);

            return par;
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to returns the ordinal value carried by the given source, looking into '{...}'
        /// brackets and '#...' named sequences if needed.
        /// </summary>
        bool ToOrdinal(string str, out int value)
        {
            if (str.StartsWith('{') && str.EndsWith('}'))
            {
                str = str.Unwrap('{', '}', trim: true)!;
                return TryParse(str, out value);
            }

            if (str.StartsWith(Prefix, Comparison)) str = str[Prefix.Length..];
            return TryParse(str, out value);

            static bool TryParse(string str, out int value)
            {
                if (str.Length == 0) { value = 0; return false; }
                foreach (var c in str) if (!char.IsAsciiDigit(c)) { value = 0; return false; }
                return int.TryParse(str, out value);
            }
        }

        /// <summary>
        /// Determines if either the given ordinal or the given name represent a collision with
        /// the contents already captured. If the ordinal if less than cero or the name is null
        /// then they are ignored.
        /// </summary>
        bool IsCollision(int ordinal, string name)
        {
            var text = Text;
            var xname = $"{Prefix}{ordinal}";

            if (ordinal >= 0)
            {
                if (FindOrdinalBracket(text, 0, ordinal, out _) >= 0) return true;
                if (FindOrdinalSequence(text, 0, ordinal, out _) >= 0) return true;
                foreach (var par in _Parameters)
                    if (string.Compare(par.Name, xname, Comparison) == 0) return true;
            }
            if (name is not null)
            {
                if (FindNamedBracket(text, 0, name, out _) >= 0) return true;
                if (FindNamedSequence(text, 0, name, out _) >= 0) return true;
                foreach (var par in _Parameters)
                    if (string.Compare(par.Name, name, Comparison) == 0) return true;
            }

            return false; // No collisions detected...
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index of the first ocurrence of a bracket (as in '{...}') starting at the
        /// given initial index, or -1 if any is found. When found, the bracket is returned in the
        /// out argument.
        /// <br/>- This method accepts empty '{}' brackets to signal potential errors.
        /// </summary>
        static int FindBracket(string text, int ini, out string? str)
        {
            str = null;
            if (ini >= text.Length) return -1;

            var pos = text.IndexOf('{', ini); if (pos < 0) return -1;
            var end = text.IndexOf('}', pos); if (end < 0) return -1;

            str = text.Substring(pos, end - pos + 1);
            return pos;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a '{value}' bracket starting at the given
        /// initial index, or -1 if any is found. When found, the bracket is returned in the out
        /// argument.
        /// </summary>
        int FindNamedBracket(string text, int ini, string name, out string? str)
        {
            str = null;
            if (ini >= text.Length) return -1;

            var xtra = false;
            var xname = name;
            if (!name.StartsWith(Prefix, Comparison)) { xname = Prefix + name; xtra = true; }

            int index;
            while ((index = FindBracket(text, ini, out str)) >= 0)
            {
                str = str.Unwrap('{', '}', trim: false);
                if (string.Compare(name, str, Comparison) == 0) return index;

                if (xtra &&
                    string.Compare(xname, str, Comparison) == 0) return index;

                ini = index + str!.Length;
            }

            str = null;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of an ordinal sequence (as in '{n}'), starting
        /// at the given initial index, or -1 if any is found. If found, the sequence and its value
        /// are returned in the out arguments.
        /// </summary>
        int FindOrdinalBracket(string text, int ini, out string? str, out int value)
        {
            str = null;
            value = 0;
            if (ini >= text.Length) return -1;

            int index;
            while ((index = FindBracket(text, ini, out str)) >= 0)
            {
                if (ToOrdinal(str!, out value)) return index;
                ini = index + str!.Length;
            }

            str = null;
            value = 0;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of the ordinal '{n}' bracket starting at the
        /// given initial index, or -1 if any is found. If found, the sequence is returned in the
        /// out argument.
        /// </summary>
        int FindOrdinalBracket(string text, int ini, int value, out string? str)
        {
            str = null;
            if (ini >= text.Length) return -1;

            int index;
            while ((index = FindBracket(text, ini, out str)) >= 0)
            {
                if (ToOrdinal(str!, out var temp) && temp == value) return index;
                ini = index + str!.Length;
            }

            str = null;
            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index of the first ocurrence of a named sequence (as in '#...', where '#'
        /// is the engine's prefix), starting at the given initial index, or -1 if any is found.
        /// When found, that sequence is returned in the out argument.
        /// <br/>- This method accepts empty '#' sequences to signal potential errors.
        /// </summary>
        int FindNamedSequence(string text, int ini, out string? str)
        {
            str = null;
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
                str = text.Substring(pos, end + Prefix.Length);
                return pos;
            }
            else // Sequence spans to the end of the text...
            {
                str = text[pos..];
                return pos;
            }
        }

        /// <summary>
        /// Returns the index of the first ocurrence of the named '#name' sequence starting at the
        /// given initial index, or -1 if any is found. If found, that sequence is returned in the
        /// out argument.
        /// </summary>
        int FindNamedSequence(string text, int ini, string name, out string? str)
        {
            str = null;
            if (ini >= text.Length) return -1;

            if (!name.StartsWith(Prefix, Comparison)) name = name[Prefix.Length..];
            if (name.Length == 0) { str = null; return -1; }

            int index;
            while ((index = FindNamedSequence(text, ini, out str)) >= 0)
            {
                if (string.Compare(name, str, Comparison) == 0) return index;
                ini = index + str!.Length;
            }

            str = null;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of an ordinal sequence (as in '#n'), starting
        /// at the given initial index, or -1 if any is found. If found, the sequence and its value
        /// are returned in the out arguments.
        /// </summary>
        int FindOrdinalSequence(string text, int ini, out string? str, out int value)
        {
            str = null;
            value = 0;
            if (ini >= text.Length) return -1;

            int index;
            while ((index = FindNamedSequence(text, ini, out str)) >= 0)
            {
                if (ToOrdinal(str!, out value)) return index;
                ini = index + str!.Length;
            }

            str = null;
            value = 0;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of the ordinal '#n' sequence starting at the
        /// given initial index, or -1 if any is found. If found, the sequence is returned in the
        /// out argument.
        /// </summary>/returns>
        int FindOrdinalSequence(string text, int ini, int value, out string? str)
        {
            str = null;
            if (ini >= text.Length) return -1;

            int index;
            while ((index = FindNamedSequence(text, ini, out str)) >= 0)
            {
                if (ToOrdinal(str!, out var temp) && temp == value) return index;
                ini = index + str!.Length;
            }

            str = null;
            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// Represents an item in the range of captured arguments.
        /// </summary>
        class RangeItem(object? payload, bool used = false)
        {
            public object? Payload = payload;
            public bool Used = used;

            // The string representation of this instance.
            public override string ToString()
            {
                var str = Payload switch
                {
                    IParameter payload => $"Parameter: {payload}",
                    AnonymousElement payload => $"Anonymous: {payload}",
                    _ => $"Payload: {Payload.Sketch()}"
                };
                if (Used) str += " (Used)";
                return str;
            }

            /// <summary>
            /// Captures the given collection of values into a collection of range items.
            /// </summary>
            public static RangeItem[] CaptureValues(object?[]? values)
            {
                values ??= [null];
                if (values.Length == 0) return [];

                var items = new RangeItem[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    var value = values[i];

                    items[i] = AnonymousElement.TryCapture(value, out var element)
                        ? new RangeItem(element)
                        : new RangeItem(value);
                }
                return items;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Represent a payload in the form of an anonymous element, as in '{ Name = Value }'
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        class AnonymousElement(string name, object? value)
        {
            public string Name = name;
            public object? Value = value;

            // The string representation of this instance.
            public override string ToString() => $"{Name}:'{Value}'";

            /// <summary>
            /// Tries to capture the given source as an anonymous element. Returns true and the
            /// captured element in the out argument if such was possible, or false otherwise.
            /// </summary>
            public static bool TryCapture(object? source, out AnonymousElement? element)
            {
                if (source is not null)
                {
                    var type = source.GetType();
                    if (type.IsAnonymous)
                    {
                        var members = type.GetProperties();
                        if (members.Length == 0) throw new ArgumentException("Anonymous element with no properties.").WithData(source);
                        if (members.Length > 1) throw new ArgumentException("Anonymous element with too many properties.").WithData(source);

                        var member = members[0];
                        var name = member.Name;
                        var value = member.GetValue(source);

                        element = new AnonymousElement(name, value);
                        return true;
                    }
                }

                element = null;
                return false;
            }
        }
    }
}