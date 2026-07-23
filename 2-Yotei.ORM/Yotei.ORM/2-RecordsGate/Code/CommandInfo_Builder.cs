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
        /// using either a positional '{n}' specification, or a '{name}' named one (where 'name'
        /// can either begin or not with the engine's prefix).
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="text"></param>
        /// <param name="values"></param>
        public Builder(IEngine engine, string text, params object?[]? values) : this(engine)
        {
            var adjustSources = false;
            Append(adjustSources, text, values);
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
                var text = Text; // Caching...
                int pos, index;

                // Empty instances are consistent by definition...
                if (IsEmpty) return true;

                // No unused parameters...
                foreach (var par in _Parameters)
                {
                    index = FindSequence(text, 0, par.Name, out _);
                    if (index < 0) return false;
                }

                // No remaing brackets...
                index = FindBracket(text, 0, out _);
                if (index >= 0) return false;

                // No remaining sequences...
                pos = 0;
                while ((index = FindSequence(text, pos, out var str)) >= 0)
                {
                    var temp = _Parameters.IndexOf(str!);
                    if (temp < 0) return false;

                    pos = index + str!.Length;
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
        public virtual bool Add(ICommand source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var info = source.GetCommandInfo();
            var adjustSources = true;
            var done = Append(adjustSources, info.Text, info.Parameters);
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
            var adjustSources = true;
            var done = Append(adjustSources, info.Text, info.Parameters);
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

            var adjustSources = true;
            var done = Append(adjustSources, source.Text, source.Parameters);
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

            var adjustSources = true;
            var done = Append(adjustSources, source.Text, source.Parameters);
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="values"></param>
        public virtual bool Add(string text, params object?[]? values)
        {
            var adjustSources = false;
            var done = Append(adjustSources, text, values);
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

            // We can use 'Append' to capture the values without collisions...
            var adjustSources = true;
            var done = Append(adjustSources, string.Empty, values);
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

            // Standard case, we use 'Append' to capture the values without collisions...
            var old = _Parameters.Clone();
            var adjustSources = true;
            var done = Append(adjustSources, string.Empty, values);

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
        /// Appends to this instance the text and collection of parameters obtained from the given
        /// values, if any. When used, the parameters shall be encoded in the given text using a
        /// positional '{n}' specification, or a named '{name}' (where 'name' can either begin or
        /// not with the engine's prefix).
        /// <br/> When <paramref name="adjustSources"/> is requested, then the given sources are
        /// scanned to detect if there are collisions with the existing contents and, if so, to
        /// modify these sources to prevent these name collisions.
        /// </summary>
        bool Append(bool adjustSources, string text, params object?[]? values)
        {
            ArgumentNullException.ThrowIfNull(text);
            values ??= [null];

            // Capturing the range of values...
            var items = Item.CaptureValues(values);
            var done = false;

            // Validations...
            PreventInvalidValues(items);
            PreventDuplicatedRangeNames(items);
            PreventInvalidOrdinals(text, items);

            // Interceptions...
            if (items.Length == 1) items = InterceptUniqueSpecialValue(items);
            if (adjustSources) text = AdjustSources(text, items);

            // Iterating through the captured values...
            IParameter par;
            int pos, index, ordinal;
            string name;
            string? str;

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

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
                        _Parameters.AddNew(item.Payload, out par);
                        name = par.Name;
                        break;
                }

                // Processing '#...' sequences...
                pos = 0;
                while ((index = FindSequence(text, pos, name, out str)) >= 0)
                {
                    text = text.Remove(index, str!.Length);
                    text = text.Insert(index, par.Name);
                    pos = index + par.Name.Length;
                }

                // Processing '{...}' brackets...
                pos = 0;
                while ((index = FindBracket(text, pos, name, out str)) >= 0)
                {
                    text = text.Remove(index, str!.Length);
                    text = text.Insert(index, par.Name);
                    pos = index + par.Name.Length;
                }

                // Processing ordinal '#...' sequences...
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

                // Processing ordinal '{...}' brackets...
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
            }

            // Finishing...
            if (items.Length > 0) done = true;
            if (text.Length > 0) { _Text.Append(text); done = true; }
            return done;
        }

        // ------------------------------------------------

        /// <summary>
        /// Prevents invalid initial values.
        /// </summary>
        static void PreventInvalidValues(Item[] items)
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
        void PreventDuplicatedRangeNames(Item[] items)
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
        void PreventInvalidOrdinals(string text, Item[] items)
        {
            int pos = 0, index;

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
        /// Intercepts ranges with just one special value. If so, returns the modified range.
        /// Otherwise, returns the original one.
        /// <br/> This method assumes the range contains just one item.
        /// </summary>
        Item[] InterceptUniqueSpecialValue(Item[] items)
        {
            object?[]? values;
            switch (items[0].Payload) // We know the items collection contains just one item...
            {
                case IParameterList pars:
                    values = [.. pars];
                    items = Item.CaptureValues(values);
                    break;

                case IParameterList.IBuilder pars:
                    values = [.. pars];
                    items = Item.CaptureValues(values);
                    break;
            }
            return items;
        }

        // ------------------------------------------------

        // <summary>
        /// Invoked to scan the given sources to detect if there are collisions with the existing
        /// contents and, if so, to modify these sources to prevent these name collisions. Returns
        /// the modified source text.
        /// </summary>
        string AdjustSources(string text, Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                switch (item.Payload)
                {
                    case IParameter temp: Execute(i, temp.Name, temp.Value); break;
                    case AnonymousElement temp: Execute(i, temp.Name, temp.Value); break;
                    default: Execute(i, i.ToString(), item.Payload); break;
                }
            }

            return text;

            // Processes the given element...
            void Execute(int ordinal, string name, object? value)
            {
                int pos, index;
                string? str;

                // No collisions...
                var xname = name;
                if (!xname.StartsWith(Prefix, Comparison)) xname = Prefix + name;
                if (_Parameters.IndexOf(xname) > 0) return;

                // Processing '#...' sequences...
                pos = 0;
                while ((index = FindBracket(text, pos, name, out str)) >= 0)
                {
                    text = text.Remove(index, str!.Length);
                    text = text.Insert(index, name);
                    pos = index + name.Length;
                }

                // Processing '{...}' brackets...
                pos = 0;
                while ((index = FindBracket(text, pos, name, out str)) >= 0)
                {
                    text = text.Remove(index, str!.Length);
                    text = text.Insert(index, name);
                    pos = index + name.Length;
                }

                // Adjusting the element itself...
                var par = new Parameter(xname, value);
                var item = new Item(par);
                items[ordinal] = item;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to parse the contents of the given source str as an ordinal specification.
        /// <br/> Source can also be a '{...}' or a '#...' one.
        /// <br/> Any case, it must not be empty, and all its characters must be digits.
        /// </summary>
        bool ParseOrdinal(string str, out int value)
        {
            if (str.StartsWith('{') && str.EndsWith('}'))
            {
                str = str.Unwrap('{', '}', trim: true)!;
                return TryParse(str, out value);
            }

            if (str.StartsWith(Prefix, Comparison)) str = str[Prefix.Length..];
            return TryParse(str, out value);

            // The actual carry horse...
            static bool TryParse(string str, out int value)
            {
                if (str.Length == 0) { value = 0; return false; }
                foreach (var c in str) if (!char.IsAsciiDigit(c)) { value = 0; return false; }
                return int.TryParse(str, out value);
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
        /// Returns the index of the first ocurrence of a '{...}' bracket, starting from the given
        /// initial position, or -1 if any if found. When found, the bracket is returned it the
        /// out argument.
        /// </summary>
        static int FindBracket(string text, int pos, out string? str)
        {
            str = null;
            if (pos >= text.Length) return -1;

            var ini = text.IndexOf('{', pos); if (ini < 0) return -1;
            var end = text.IndexOf('}', ini); if (end < 0) return -1;

            str = text.Substring(ini, end - ini + 1);
            return ini;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a '{name}' bracket, starting from the given
        /// initial position, or -1 if any if found. When found, the bracket is returned it the
        /// out argument.
        /// <br/> The given 'name' is tested with and without the engine's prefix.
        /// </summary>
        int FindBracket(string text, int pos, string name, out string? str)
        {
            str = null;
            if (pos >= text.Length) return -1;

            if (name.Length == 0) return -1;
            var rname = name.StartsWith(Prefix, Comparison) ? name[Prefix.Length..] : name;

            int index;
            while ((index = FindBracket(text, pos, out str)) >= 0)
            {
                var temp = str.Unwrap('{', '}', trim: false)!;
                if (temp.Length == 0) continue;
                var rtemp = temp.StartsWith(Prefix, Comparison) ? temp[Prefix.Length..] : temp;

                if (string.Compare(name, temp, Comparison) == 0) return index;

                if (name != rname || temp != rtemp)
                    if (string.Compare(rname, rtemp, Comparison) == 0) return index;

                pos = index + str!.Length;
            }

            str = null;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a '{ordinal}' bracket, starting the the
        /// given inicial position, or -1 if not found. When found, both the bracket and the value
        /// of the ordinal are returned in the out arguments.
        /// </summary>
        int FindOrdinalBracket(string text, int pos, out string? str, out int value)
        {
            str = null;
            value = 0;
            if (pos >= text.Length) return -1;

            int index;
            while ((index = FindBracket(text, pos, out str)) >= 0)
            {
                var temp = str.Unwrap('{', '}', trim: false)!;
                if (ParseOrdinal(temp, out value)) return index;

                pos = index + str!.Length;
            }

            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index of the first ocurrence of a '#...' sequence, starting from the given
        /// initial position, or -1 if any if found. When found, the sequece is returned it the
        /// out argument.
        /// </summary>
        int FindSequence(string text, int pos, out string? str)
        {
            str = null;
            if (pos >= text.Length) return -1;

            var comparer = char.CharComparer(Comparison);
            var ini = text.IndexOf(Prefix, pos, Comparison);
            if (ini < 0) return -1;
            if (ini > 0)
            {
                var c = text[ini - 1];
                if (!IsolatedFinder.SEPARATORS.Contains(c, comparer)) return -1;
            }

            var span = text.AsSpan(ini + Prefix.Length);
            var end = span.IndexOfAny(IsolatedFinder.SEPARATORS, Comparison);

            if (end >= 0) // Embedded sequence...
            {
                str = text.Substring(ini, end + Prefix.Length);
                return IsEmbedded(str) ? -1 : ini;
            }
            else // Sequence spans till the end...
            {
                str = text[ini..];
                return IsEmbedded(str) ? -1 : ini;
            }

            // Validate it is not a '{#...}' sequence...
            bool IsEmbedded(string str)
            {
                end = ini + str.Length;

                return
                    ini > 0 && text[ini - 1] == '{' &&
                    end < (text.Length) && text[end] == '}';
            }
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a '#name' sequence, starting from the given
        /// initial position, or -1 if any if found. When found, the sequence is returned it the
        /// out argument.
        /// <br/> The given 'name' is tested with and without the engine's prefix.
        /// </summary>
        int FindSequence(string text, int pos, string name, out string? str)
        {
            str = null;
            if (pos >= text.Length) return -1;

            var needed = false;
            string reduced = name;
            if (reduced.StartsWith(Prefix, Comparison)) { needed = true; reduced = reduced[Prefix.Length..]; }

            int index;
            while ((index = FindSequence(text, pos, out str)) >= 0)
            {
                var temp = str![Prefix.Length..];
                if (string.Compare(name, temp, Comparison) == 0) return index;

                if (needed &&
                    string.Compare(reduced, temp, Comparison) == 0) return index;

                pos = index + str!.Length;
            }

            str = null;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a '#ordinal' sequence, starting the the
        /// given inicial position, or -1 if not found. When found, both the sequence and the
        /// value of the ordinal are returned in the out arguments.
        /// </summary>
        int FindOrdinalSequence(string text, int pos, out string? str, out int value)
        {
            str = null;
            value = 0;
            if (pos >= text.Length) return -1;

            int index;
            while ((index = FindSequence(text, pos, out str)) >= 0)
            {
                var temp = str![Prefix.Length..];
                if (ParseOrdinal(temp, out value)) return index;

                pos = index + str!.Length;
            }

            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// Represents an item in the range of captured arguments.
        /// </summary>
        class Item(object? payload, bool used = false)
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
            public static Item[] CaptureValues(object?[]? values)
            {
                values ??= [null];
                if (values.Length == 0) return [];

                var items = new Item[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    var value = values[i];

                    items[i] = AnonymousElement.TryCapture(value, out var element)
                        ? new Item(element)
                        : new Item(value);
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