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
            get => throw null;
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool Add(string? text, params object?[]? range)
        {
            throw null;
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

            // Shortcut when range is empty...
            if (range.Length == 0)
            {
                if (_Parameters.Count == 0) return false;
                _Parameters.Clear();
                return true;
            }

            // Standard case...
            else
            {
                throw null;
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
    }
}