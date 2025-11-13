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
        public Builder(IConnection connection, string text, params object?[]? range)
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
                var text = _Text.ToString();
                var finder = new IsolatedFinder();
                var sensitive = Engine.CaseSensitiveNames;

                // Empty instances are consistent by definition...
                if (IsEmpty) return true;

                // Finding dangling brackets...
                if (AreRemainingBrackets(text)) return false;

                // Finding unused parameters...
                var count = 0;
                foreach (var par in _Parameters)
                    if (finder.Find(text, 0, par.Name, !sensitive) >= 0) count++;

                if (_Parameters.Count != count) return false;               

                // Finishing...
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
            text ??= string.Empty;
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
        public virtual bool ReplaceText(string text, bool strict = true)
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
                if (_Parameters.Count != 0) return false;

                _Parameters.Clear();
                return true;
            }

            // Standard case...
            var old = _Parameters.ToArray(); _Parameters.Clear();
            var noRemainingSpecs = false;
            var noUnusedValues = false;
            var changed = Append(noRemainingSpecs, noUnusedValues, null, range);

            if (!changed)
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
            throw null;
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
    }
}