namespace Yotei.ORM.Code;

partial class CommandInfo
{
    // ====================================================
    /// <inheritdoc cref="ICommandInfo.IBuilder"/>
    [Cloneable]
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
        /// Initializes a new instance with the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo source)
        {
            source.ThrowWhenNull();

            _Text = new(source.Text);
            _Parameters = new(source.Engine, source.Parameters);
        }

        /// <summary>
        /// Initializes a new instance with the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo.IBuilder source)
        {
            source.ThrowWhenNull();

            _Text = new(source.Text);
            _Parameters = new(source.Engine, source.Parameters);
        }

        /// <summary>
        /// Initializes a new instance using the given text and the collection of parameters
        /// obtained from the given range of values, if any.
        /// <br/>- If <paramref name="text"/> is null, then the range of values is just captured
        /// without any attempts of matching their names with bracket specifications. Similarly,
        /// if <paramref name="range"/> is empty, then the text is captured without intercepting
        /// any dangling specifications.
        /// <br/>- Parameter specifications must always be bracket ones, either positional '{n}'
        /// or named '{name}' ones, where name contains the name of the parameter or the name of
        /// the unique property of an anonymous item. In both cases, 'name' may or may not start
        /// with the engine parameters' prefix.
        /// <br/>- No unused parameters are allowed, neither dangling bracket specifications.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="text"></param>
        /// <param name="range"></param>
        public Builder(IEngine engine, string? text, params object?[]? range)
            : this(engine)
            => Add(text, range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source)
        {
            source.ThrowWhenNull();

            _Text = new(source._Text.ToString());
            _Parameters = new(source.Engine, source._Parameters);
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
        public virtual CommandInfo ToInstance()
        {
            throw null;
        }
        ICommandInfo ICommandInfo.IBuilder.ToInstance() => ToInstance();

        /// <inheritdoc/>
        public IEngine Engine => _Parameters.Engine;

        StringComparison Comparison => Engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        string Prefix => Engine.ParameterPrefix;

        /// <inheritdoc/>
        public string Text => _Text.ToString();

        /// <inheritdoc/>
        public IParameterList Parameters
        {
            get => throw null;
        }

        /// <inheritdoc/>
        public bool IsEmpty => _Text.Length == 0 && _Parameters.Count == 0;

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool ReplaceText(string? text)
        {
            throw null;
        }

        /// <inheritdoc/>
        public virtual bool ReplaceValues(params object?[]? range)
        {
            throw null;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Clear()
        {
            throw null;
        }

        /// <inheritdoc/>
        public virtual bool Add(ICommandInfo source)
        {
            throw null;
        }

        /// <inheritdoc/>
        public virtual bool Add(ICommandInfo.IBuilder source)
        {
            throw null;
        }

        /// <inheritdoc/>
        public virtual bool Add(string? text, params object?[]? range)
        {
            throw null;
        }
    }
}