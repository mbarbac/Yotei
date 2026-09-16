namespace Yotei.ORM.Code;

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
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        [SuppressMessage("", "IDE0290")]
        public Builder(IEngine engine)
        {
            _Text = new();
            _Parameters = new(engine);
        }

        /// <summary>
        /// Initializes a new instance with the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo source)
            : this(source.ThrowWhenNull().Engine)
            => Add(source);

        /// <summary>
        /// Initializes a new instance with the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo.IBuilder source)
            : this(source.ThrowWhenNull().Engine)
            => Add(source);

        /// <summary>
        /// Intializes a new instance with the given text and the collection of parameters obtained
        /// from the given optional values have been added to it.
        /// <br/> If the text is null, then it is ignored.
        /// <br/> If text is not null, then the values must be encoded using either a positional
        /// '{n}' specification, or a named '{name}' one (where if 'name' is not prefixed with
        /// engine's prefix, it is added automatically).
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="text"></param>
        /// <param name="values"></param>
        public Builder(IEngine engine, string? text, params object?[]? values)
            : this(engine)
            => Add(text, values);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
            : this(other.ThrowWhenNull().Engine)
            => Add(other);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
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
            => IsEmpty ? new CommandInfo(Engine) : new CommandInfo(this);

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine => _Parameters.Engine;
        string Prefix => Engine.ParameterPrefix;
        bool IgnoreCase => Engine.IgnoreCase;
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
                int pos;

                // Empty instances are consistent by definition...
                if (IsEmpty) return true;

                // No unused parameters...
                foreach (var par in _Parameters)
                {
                    pos = FindNamedSequence(text, 0, par.Name, out _);
                    if (pos >= 0) continue;
                    return false;
                }

                // No remaining brackets...
                pos = FindBracket(text, 0, out _);
                if (pos >= 0) return false;

                // No remaining sequences...
                pos = 0;
                while ((pos = FindSequence(text, pos, out var str)) >= 0)
                {
                    var temp = _Parameters.IndexOf(str!);
                    if (temp < 0) return false;
                    pos += str!.Length;
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
        /// <returns><inheritdoc/></returns>
        public virtual bool Add(ICommandInfo source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var text = source.Text;
            var pars = source.Parameters;
            return Append(text, pars);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool Add(ICommandInfo.IBuilder source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var text = source.Text;
            var pars = source.Parameters;
            return Append(text, pars);
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="values"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool Add(
            string? text, params object?[]? values) => Append(text, values);

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool ReplaceText(string? text)
        {
            if (text is null && _Text.Length == 0) return false;

            var old = Text;
            _Text.Clear();

            if (!Append(text, []))
            {
                _Text.Clear();
                _Text.Append(old);
                return false;
            }
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="values"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool ReplaceValues(params object?[]? values)
        {
            values ??= [null];

            if (values.Length == 0 && _Parameters.Count == 0) return false;

            var old = Parameters;
            _Parameters.Clear();

            if (!Append(null, values))
            {
                _Parameters.Clear();
                _Parameters.AddRange(old);
                return false;
            }
            return true;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public virtual bool Clear()
        {
            if (IsEmpty) return false;

            _Text.Clear();
            _Parameters.Clear();
            return true;
        }

        // ------------------------------------------------

        /// <summary>
        /// Captures into this instance the given text and the collection of parameters obtained
        /// from the given optional values have been added to it.
        /// <br/> If the text is null, then it is ignored.
        /// <br/> If text is not null, then the values must be encoded using either a positional
        /// '{n}' specification, or a named '{name}' one (where if 'name' is not prefixed with
        /// engine's prefix, it is added automatically).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        bool Append(string? text, params object?[]? values)
        {
            values ??= [null];

            // Preparing...
            var args = IElement.CaptureArguments(values);
            var done = false;

            PreventDuplicatedNames(args);
            PreventInvalidOrdinals(text, args);

            // Iterating through the arguments...
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                IParameter par;
                string name;

                // Capturing...
                switch (arg)
                {
                    case ValueElement item:
                        _Parameters.AddNew(item.Value, out par);
                        name = par.Name;
                        par = Capture(par);
                        done = true;
                        break;

                    case ParameterElement item:
                        name = item.Payload.Name;
                        par = Capture(item.Payload);
                        done = true;
                        break;

                    case AnonymousElement item:
                        name = item.Name;
                        par = new Parameter(item.Name, item.Value);
                        par = Capture(par);
                        done = true;
                        break;

                    default:
                        throw new UnreachableException("Unknown argument type.").WithData(arg);
                }

                // No text, no need to adjust...
                if (text is null) continue;

                string? str;
                int pos, ordinal;

                // Processing named '{name}' brackets...
                pos = 0;
                while ((pos = FindNamedBracket(text, pos, name, out str)) >= 0)
                {
                    text = text.Remove(pos, str!.Length);
                    text = text.Insert(pos, par.Name);
                    pos += par.Name.Length;
                }

                // Processing ordinal '{n}' brackets...
                pos = 0;
                while ((pos = FindOrdinalBracket(text, pos, out str, out ordinal)) >= 0)
                {
                    if (ordinal == i)
                    {
                        text = text.Remove(pos, str!.Length);
                        text = text.Insert(pos, par.Name);
                        pos += par.Name.Length;
                    }
                    else pos += str!.Length;
                }

                // Processing named '#name' sequences...
                pos = 0;
                while ((pos = FindNamedSequence(text, pos, name, out str)) >= 0)
                {
                    text = text.Remove(pos, str!.Length);
                    text = text.Insert(pos, par.Name);
                    pos += par.Name.Length;
                }

                // Processing ordinal '#n' sequences...
                pos = 0;
                while ((pos = FindOrdinalSequence(text, pos, out str, out ordinal)) >= 0)
                {
                    if (ordinal == i)
                    {
                        text = text.Remove(pos, str!.Length);
                        text = text.Insert(pos, par.Name);
                        pos += par.Name.Length;
                    }
                    else pos += str!.Length;
                }
            }

            // Finishing...
            if (text is not null && text.Length > 0) { _Text.Append(text); done = true; }
            return done;
        }

        // ------------------------------------------------

        /// <summary>
        /// Prevents duplicated names in the given range of arguments.
        /// </summary>
        void PreventDuplicatedNames(IElement[] args)
        {
            List<string> names = [];

            foreach (var arg in args)
            {
                var name = arg switch
                {
                    ValueElement item => null,
                    ParameterElement item => item.Payload.Name,
                    AnonymousElement item => item.Name,
                    _ => throw new ArgumentException("Invalid argument type.").WithData(arg)
                };
                if (name != null)
                {
                    if (!name.StartsWith(Prefix, IgnoreCase)) name = Prefix + name;

                    var temp = names.Find(x => string.Compare(x, name, IgnoreCase) == 0);
                    if (temp != null) throw new DuplicateException(
                        "Range of arguments contains duplicated names.")
                        .WithData(name)
                        .WithData(args);

                    names.Add(name);
                }
            }
        }

        /// <summary>
        /// Prevents invalid initial ordinals.
        /// <br/> If the text is null, then this method is ignored.
        /// </summary>
        void PreventInvalidOrdinals(string? text, IElement[] args)
        {
            if (text is null) return;

            var pos = 0;
            while ((pos = FindOrdinalBracket(text, pos, out var str, out var value)) >= 0)
            {
                if (value >= args.Length) throw new ArgumentException(
                    "Ordinal bracket value bigger than the number of arguments.")
                    .WithData(value)
                    .WithData(args);

                pos += str!.Length;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Guarantees that the name of the given parameter starts with the engine's prefix, or
        /// creates and returns a new one that does it.
        /// </summary>
        IParameter WithPrefixName(IParameter par)
        {
            return par.Name.StartsWith(Prefix, IgnoreCase)
                ? par
                : par.WithName(Prefix + par.Name);
        }

        /// <summary>
        /// Captures the given parameter into this instance. If its name already exist, then
        /// it is changed to prevent name collisions.
        /// </summary>
        IParameter Capture(IParameter par)
        {
            par = WithPrefixName(par);

            if (_Parameters.Contains(par.Name)) _Parameters.AddNew(par.Value, out par);
            else _Parameters.Add(par);

            return par;
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to obtain an ordinal value from the given specification, which can be a plain
        /// number, a '{...}' bracket, or a '#...' sequence.
        /// </summary>
        bool ParseOrdinal(string str, out int value)
        {
            if (str.StartsWith('{') && str.EndsWith('}')) str = str.Unwrap('{', '}', trim: true)!;
            if (str.StartsWith(Prefix, IgnoreCase)) str = str[Prefix.Length..];
            str = str.Trim();

            value = -1;
            if (str.Length == 0) return false;
            foreach (var c in str) if (!char.IsAsciiDigit(c)) return false;
            return int.TryParse(str, out value);
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index of the first ocurrence of a '{...}' bracket, starting from the given
        /// initial position, or -1 if any is found. If found, it is returned in the out argument.
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
        /// initial position, or -1 if any is found. If found, it is returned in the out argument.
        /// The given 'name' is tested with and without the engine's prefix.
        /// </summary>
        int FindNamedBracket(string text, int pos, string name, out string? str)
        {
            str = null;
            if (pos >= text.Length) return -1;
            if (name.Length == 0) return -1;

            var rname = name.StartsWith(Prefix, IgnoreCase) ? name[Prefix.Length..] : name;
            var xname = name.StartsWith(Prefix, IgnoreCase) ? name : (Prefix + name);

            int index = pos;
            while ((index = FindBracket(text, index, out str)) >= 0)
            {
                var temp = str.Unwrap('{', '}', trim: false)!.Trim();
                if (temp.Length == 0) continue;

                if (string.Compare(temp, xname, IgnoreCase) == 0) return index;
                if (string.Compare(temp, rname, IgnoreCase) == 0) return index;

                index += str!.Length;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a '{n}' bracket, starting from the given
        /// initial position, or -1 if any is found. If found, both the bracket and the ordinal
        /// value are returned in the out arguments.
        /// </summary>
        int FindOrdinalBracket(string text, int pos, out string? str, out int value)
        {
            str = null;
            value = -1;
            if (pos >= text.Length) return -1;

            int index = pos;
            while ((index = FindBracket(text, index, out str)) >= 0)
            {
                var temp = str.Unwrap('{', '}', trim: false)!.Trim();
                if (temp.Length == 0) continue;

                if (ParseOrdinal(temp, out value)) return index;

                index += str!.Length;
            }

            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index of the first ocurrence of a '#...' sequence, starting from the given
        /// initial position, or -1 if any is found. If found, it is returned in the out argument.
        /// </summary>
        int FindSequence(string text, int pos, out string? str)
        {
            str = null;
            if (pos >= text.Length) return -1;

            var ini = text.IndexOf(Prefix, pos, Comparison);
            if (ini < 0) return -1;

            if (ini > 0)
            {
                var comparer = char.CharComparer(IgnoreCase);
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
            else // Sequence spans till end...
            {
                str = text[ini..];
                return IsEmbedded(str) ? -1 : ini;
            }

            /// <summary>
            /// Validates it is not a '{#...}' sequence.
            /// </summary>
            bool IsEmbedded(string str)
            {
                end = ini + str.Length;

                return
                    ini > 0 && text[ini - 1] == '{' &&
                    end < (text.Length) && text[end] == '}';
            }
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a '#name' sequece, starting from the given
        /// initial position, or -1 if any is found. If found, it is returned in the out argument.
        /// The given 'name' is tested with and without the engine's prefix.
        /// </summary>
        int FindNamedSequence(string text, int pos, string name, out string? str)
        {
            str = null;
            if (pos >= text.Length) return -1;
            if (name.Length == 0) return -1;

            var rname = name.StartsWith(Prefix, IgnoreCase) ? name[Prefix.Length..] : name;
            var xname = name.StartsWith(Prefix, IgnoreCase) ? name : (Prefix + name);

            int index = pos;
            while ((index = FindSequence(text, index, out str)) >= 0)
            {
                var temp = str![Prefix.Length..].Trim(); ;
                if (temp.Length == 0) continue;

                if (string.Compare(temp, xname, IgnoreCase) == 0) return index;
                if (string.Compare(temp, rname, IgnoreCase) == 0) return index;

                index += str!.Length;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a '#n' sequence, starting from the given
        /// initial position, or -1 if any is found. If found, both the bracket and the ordinal
        /// value are returned in the out arguments.
        /// </summary>
        int FindOrdinalSequence(string text, int pos, out string? str, out int value)
        {
            str = null;
            value = -1;
            if (pos >= text.Length) return -1;

            int index = pos;
            while ((index = FindSequence(text, index, out str)) >= 0)
            {
                var temp = str![Prefix.Length..].Trim();
                if (temp.Length == 0) continue;

                if (ParseOrdinal(temp, out value)) return index;

                index += str!.Length;
            }

            return -1;
        }

        // ------------------------------------------------
        /// <summary>
        /// Represents an argument passed to the builder.
        /// </summary>
        interface IElement
        {
            string Name { get; }
            object? Value { get; }

            /// <summary>
            /// Captures the given list of values into a collection of arguments.
            /// </summary>
            public static IElement[] CaptureArguments(object?[]? values)
            {
                values ??= [null];

                List<IElement> list = [];
                for (int i = 0; i < values.Length; i++)
                {
                    var value = values[i];

                    if (AnonymousElement.TryCapture(value, out var element))
                    {
                        list.Add(element);
                        continue;
                    }
                    if (value is IParameter par)
                    {
                        list.Add(new ParameterElement(par));
                        continue;
                    }
                    if (value is IEnumerable<IParameter> range)
                    {
                        foreach (var temp in range) list.Add(new ParameterElement(temp));
                        continue;
                    }
                    switch (value)
                    {
                        case ICommand:
                        case ICommandInfo:
                        case ICommandInfo.IBuilder:
                            throw new ArgumentException("Argument cannot be a command-alike one.")
                            .WithData(value);

                        default:
                            list.Add(new ValueElement(value));
                            break;
                    }
                }

                return [.. list];
            }
        }

        // ------------------------------------------------
        /// <summary>
        /// Represents an argument passed as a plain value.
        /// </summary>
        class ValueElement : IElement
        {
            public ValueElement(object? value) { Value = value; Name = null!; }
            public ValueElement(string name, object? value) { Value = value; Name = name; }
            public override string ToString()
            {
                var value = Value.Sketch();
                var str = Name is null ? $"Value(-='{value}')" : $"Value({Name}='{value}')";
                return str;
            }
            public string Name
            {
                get;
                set => field = value is null ? null! : value.NotNullNotEmpty(trim: true);
            }
            public object? Value { get; set; }
        }

        // ------------------------------------------------
        /// <summary>
        /// Represents an argument passed as a parameter.
        /// </summary>
        class ParameterElement(IParameter par) : IElement
        {
            public override string ToString() => $"Parameter({Name}='{Value.Sketch()}')";
            public IParameter Payload { get; set => field = value.ThrowWhenNull(); } = par;
            public string Name => Payload.Name;
            public object? Value => Payload.Value;
        }

        // ------------------------------------------------
        /// <summary>
        /// Represents an argument passed as an anonymous value.
        /// </summary>
        class AnonymousElement(string name, object? value) : IElement
        {
            public override string ToString() => $"Anonymous({Name}='{Value.Sketch()}')";
            public string Name { get; set => field = value.NotNullNotEmpty(trim: true); } = name;
            public object? Value { get; set; } = value;

            /// <summary>
            /// Tries to capture the given element as an anonymous one.
            /// </summary>
            public static bool TryCapture(
                object? source, [NotNullWhen(true)] out AnonymousElement? element)
            {
                if (source is not null)
                {
                    var type = source.GetType();
                    if (type.IsAnonymous)
                    {
                        var members = type.GetProperties();
                        if (members.Length == 0) throw new ArgumentException("Anonymous argument with no properties.").WithData(source);
                        if (members.Length > 1) throw new ArgumentException("Anonymous argument with too many properties.").WithData(source);

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
