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
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommand source) => throw null;

        /// <summary>
        /// Initializes a new instance using the contents from the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo source) => throw null;

        /// <summary>
        /// Initializes a new instance using the contents from the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo.IBuilder source) => throw null;

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
                var prefix = Engine.ParameterPrefix;

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
            throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommandInfo source)
        {
            throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommandInfo.IBuilder source)
        {
            throw null;
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool Add(string text, params object?[]? range)
        {
            throw null;
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool ReplaceText(string text, bool strict = true)
        {
            throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool ReplaceParameters(params object?[]? range)
        {
            throw null;
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