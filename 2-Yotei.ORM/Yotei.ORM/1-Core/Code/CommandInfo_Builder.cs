namespace Yotei.ORM.Code;

partial class CommandInfo
{
    // ========================================================
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
            throw null;
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
            throw null;
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source, using its default
        /// iterable mode.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommand source)
        {
            throw null;
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source, using the requested
        /// iterable mode.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommand source, bool iterable)
        {
            throw null;
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo source)
        {
            throw null;
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo.IBuilder source)
        {
            throw null;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual ICommandInfo ToInstance()
        {
            throw null;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine => _Parameters.Engine;
        string Prefix => Engine.ParameterPrefix;
        bool IgnoreCase => Engine.IgnoreCase;

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
        public virtual bool Add(ICommand source)
        {
            throw null;
        }

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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="values"></param>
        public virtual bool Add(string text, params object?[]? values)
        {
            throw null;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool AddText(string text)
        {
            throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool AddValues(params object?[]? values)
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
            throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReplaceValues(params object?[]? values)
        {
            throw null;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear()
        {
            throw null;
        }

        // ------------------------------------------------

        bool Append(string? text, params object?[]? values)
        {
            values ??= [null];

            // Preparing...
            var args = IElement.CaptureArguments(values);
            var done = false;

            // Iterating...
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                IParameter par;
                string name;

                // Capturing as a temporary parameter...
                switch (arg)
                {
                    case ValueElement item:
                        _Parameters.AddNew(item.Value, out par);
                        name = par.Name;
                        par = Capture(par);
                        break;

                    case ParameterElement item:
                        name = item.Payload.Name;
                        par = Capture(item.Payload);
                        break;

                    case AnonymousElement item:
                        name = item.Name;
                        par = new Parameter(item.Name, item.Value);
                        par = Capture(par);
                        break;

                    default:
                        throw new UnreachableException("Unknown argument.").WithData(arg);
                }

                // Processing named '#name' sequences...

                // Processing named '{name}' brackets...

                // Processing ordinal '#n' sequences...

                // Processing ordinal '{n}' brackets...
            }

            // Finishing...
            throw null;
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
    }
}

// ========================================================
/// <summary>
/// Represents an argument passed to the builder.
/// </summary>
file interface IElement
{
    string Name { get; }
    object? Value { get; }
    bool Used { get; set; }

    /// <summary>
    /// Captures a list of arguments based upon the given values.
    /// </summary>
    public static IElement[] CaptureArguments(object?[]? values)
    {
        values ??= [null];

        var list = new List<IElement>();
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
            if (value is IEnumerable<IParameter> pars)
            {
                foreach (var temp in pars) list.Add(new ParameterElement(temp));
                continue;
            }
            list.Add(new ValueElement(value)); // Default...
        }
        return [.. list];
    }
}

// ========================================================
/// <summary>
/// Represents a parameter argument passed to the builder.
/// </summary>
file class ValueElement : IElement
{
    public ValueElement(object? value) { Name = null!; Value = value; }
    public ValueElement(string name, object? value) { Name = name; Value = value; }

    public string Name
    {
        get;
        set => field = value is null ? null! : value.NotNullNotEmpty(trim: true);
    }
    public object? Value { get; set; }
    public bool Used { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString()
    {
        var value = Value.Sketch();
        var str = Name is null ? $"Value(-='{value}')" : $"Value({Name}='{value}')";
        return Used ? $"{str}:Used" : str;
    }
}

// ========================================================
/// <summary>
/// Represents a parameter argument passed to the builder.
/// </summary>
file class ParameterElement(IParameter par) : IElement
{
    public IParameter Payload { get; set => field = value.ThrowWhenNull(); } = par;
    public string Name => Payload.Name;
    public object? Value => Payload.Value;
    public bool Used { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString()
    {
        var str = $"Parameter({Name}='{Value.Sketch()}')";
        return Used ? $"{str}:Used" : str;
    }
}

// ========================================================
/// <summary>
/// Represents a parameter argument passed to the builder.
/// </summary>
file class AnonymousElement(string name, object? value) : IElement
{
    public string Name { get; set => field = value.ThrowWhenNull(); } = name;
    public object? Value { get; set; } = value;
    public bool Used { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString()
    {
        var str = $"Anonymous({Name}='{Value.Sketch()}')";
        return Used ? $"{str}:Used" : str;
    }

    /// <summary>
    /// Tries to capture the given element as an anonymous one.
    /// </summary>
    public static bool TryCapture(object? source, [NotNullWhen(true)] out AnonymousElement? element)
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