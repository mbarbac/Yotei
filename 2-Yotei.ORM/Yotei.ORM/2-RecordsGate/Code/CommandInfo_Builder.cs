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
        /// <returns></returns>
        public virtual bool Add(string text, params object?[]? values)
        {
            ArgumentNullException.ThrowIfNull(text);
            values ??= [null];

            throw null;
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

            bool TryParse(string str, out int value)
            {
                if (str.Length == 0) { value = 0; return false; }
                foreach (var c in str) if (!char.IsAsciiDigit(c)) { value = 0; return false; }
                return int.TryParse(str, out value);
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index of the first ocurrence of a bracket (as in '{...}') starting at the
        /// given initial index, or -1 if any is found. When found, the bracket is returned in the
        /// out argument.
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
    }
}