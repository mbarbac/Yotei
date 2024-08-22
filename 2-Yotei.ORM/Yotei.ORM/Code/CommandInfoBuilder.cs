namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
public partial class CommandInfoBuilder : ICommandInfoBuilder
{
    readonly StringBuilder _Text;
    readonly ParameterListBuilder _Parameters;
    IEngine Engine => _Parameters.Engine;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfoBuilder(IEngine engine)
    {
        _Text = new();
        _Parameters = new(engine);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfoBuilder(ICommandInfo source) : this(source.Parameters.Engine)
    {
        var strict = false;
        Add(strict, source.Text, source.Parameters);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfoBuilder(IEngine engine, string? text, params object?[] range) : this(engine)
    {
        var strict = !(text is null || (range is not null && range.Length == 0));
        Add(strict, text, range!);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfoBuilder(CommandInfoBuilder source) : this(source.Engine)
    {
        _Text.Append(source._Text);
        _Parameters.AddRange(source._Parameters);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string str = _Text.ToString();
        if (_Text.Length > 0 && _Parameters.Count > 0) str += " : ";
        if (_Parameters.Count > 0) str += string.Join(", ", _Parameters);
        return str;
    }

    /// <inheritdoc/>
    public virtual ICommandInfo ToInstance() => new CommandInfo(Engine, Text, Parameters);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string Text => _Text.ToString();

    /// <inheritdoc/>
    public IParameterList Parameters => _Parameters.ToInstance();

    /// <inheritdoc/>
    public bool IsEmpty => _Text.Length == 0 && _Parameters.Count == 0;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool ReplaceText(string? text)
    {
        if (_Text.Length == 0 && (text is null || text.Length == 0)) return false;

        _Text.Clear();
        _Text.Append(text ?? string.Empty);
        return true;
    }

    /// <inheritdoc/>
    public bool ReplaceParameters(IEnumerable<IParameter> range)
    {
        var count = range switch
        {
            IParameterList items => items.Count,
            IParameterListBuilder items => items.Count,
            _ => range.Count()
        };

        if (_Parameters.Count == 0 && count == 0) return false;

        _Parameters.Clear();
        _Parameters.AddRange(range);
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Add(ICommandInfo source)
    {
        source.ThrowWhenNull();

        var strict = false;
        return Add(strict, source.Text, source.Parameters);
    }

    /// <inheritdoc/>
    public bool Add(ICommandInfoBuilder source)
    {
        source.ThrowWhenNull();

        var strict = false;
        return Add(strict, source.Text.ToString(), source.Parameters);
    }

    /// <inheritdoc/>
    public bool Add(string? text, params object?[] range)
    {
        var strict = true;
        return Add(strict, text, range);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Clear()
    {
        if (IsEmpty) return false;

        _Text.Clear();
        _Parameters.Clear();
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Factorizes code taking into consideration whether strict mode is requested or not.
    /// When in strict mode, there are no validations for the match between the specifications
    /// in the text, brackets, and element names.
    /// </summary>
    /// <param name="strict"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    bool Add(bool strict, string? text, params object?[] range)
    {
        var comparison = Engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        var textnull = text is null;
        text ??= string.Empty;
        range ??= [null];

        var done = !textnull;
        var novels = new ParameterListBuilder(Engine);

        // Intercepting single-valued elements...
        if (range.Length == 1 &&
            range[0] is IEnumerable<IParameter> xrange)
            return Add(strict, text, xrange.ToArray());

        // Capturing and validating elements...
        var items = RangeElement.Capture(range);
        for (int i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (item.Value is IEnumerable<IParameter>) throw new ArgumentException("Element in the range of values cannot be a parameters' enumeration.").WithData(item);
            if (item.Value is ICommandInfo) throw new ArgumentException("Element in the range of values cannot be a command info instance.").WithData(item);
        }

        // If text is null, inconditionally capturing the elements...
        if (textnull)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                if (item.Value is IParameter par) // Parameter specified...
                {
                    par = Capture(par);
                }
                else if (item.Value is AnonymousItem anon) // Anonymous specified...
                {
                    par = new Parameter(anon.Name, anon.Value);
                    par = Capture(par);
                }
                else // Value specified...
                {
                    _Parameters.AddNew(item.Value, out par);
                    novels.Add(par);
                }

                item.Used = true;
                done = true;
            }
        }

        // Otherwise, iterating through the elements...
        else
        {
            for (int i = 0; i < items.Length; i++)
            {
                string name = null!;
                var item = items[i];

                // Capturing the element, just once...
                if (item.Parameter == null)
                {
                    if (item.Value is IParameter par) // Parameter specified...
                    {
                        name = par.Name;
                        par = Capture(par);
                    }
                    else if (item.Value is AnonymousItem anon) // Anonymous specified...
                    {
                        name = anon.Name;
                        par = new Parameter(anon.Name, anon.Value);
                        par = Capture(par);
                    }
                    else // Value specified...
                    {
                        _Parameters.AddNew(item.Value, out par);
                        novels.Add(par);
                        name = par.Name;
                    }

                    item.Parameter = par;
                }

                throw NotImplementedException();
                // Problem: when the element comes from an alredy validated collection, its
                // name may alredy be in the correct form, so we need to find for that form
                // and not for a bracket!
                // Idea:
                // En FindNamedBracket, cuando NO strict, intentar como fall-back encontrar
                // el name, pero siempre que haya un separador tras el !!!

                // Named specifications...
                var pos = 0;
                while ((pos = FindNamedBracket(name, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, item.Parameter.Name);
                    pos += bracket.Length;

                    item.Used = true;
                    done = true;
                }

                // Ordinal specifications...
                pos = 0;
                while ((pos = FindOrdinalBracket(i, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, item.Parameter.Name);
                    pos += bracket.Length;

                    item.Used = true;
                    done = true;
                }
            }
        }

        // Validations in strict mode...
        if (strict)
        {
            // No remaining unused elements...
            if (items.Length > 0 && items.Any(x => !x.Used)) throw new ArgumentException(
                "There are unused elements in the range of values.")
                .WithData(range)
                .WithData(text);

            // No remaining brackets...
            if (!textnull && AreAnyBrackets(text)) throw new ArgumentException(
                "There are unused brackets in the given text.")
                .WithData(text);

            static bool AreAnyBrackets(string text)
            {
                var ini = text.IndexOf('{'); if (ini < 0) return false;
                var end = text.IndexOf('}', ini); if (end < 0) return false;
                return true;
            }
        }

        // Finishing...
        if (done && !textnull) _Text.Append(text);
        return done;

        /// <summary>
        /// Captures the given parameter, eventually creating a new one if its name does not
        /// begin with the parameter prefix, or if it name already exist, provided its not a
        /// duplicate coming from duplicated element names.
        /// </summary>
        IParameter Capture(IParameter par)
        {
            if (!par.Name.StartsWith(Engine.ParametersPrefix, comparison))
                par = new Parameter(Engine.ParametersPrefix + par.Name, par.Value);

            if (novels.Contains(par.Name)) throw new DuplicateException(
                "Duplicated element name detected.")
                .WithData(par);

            if (_Parameters.Contains(par.Name)) _Parameters.AddNew(par.Value, out par);
            else _Parameters.Add(par);

            novels.Add(par);
            return par;
        }

        /// <summary>
        /// Gets the index of the first ocurrence of the ordinal bracket with the given value,
        /// starting at the given position, or -1 if not found.
        /// </summary>
        int FindOrdinalBracket(int i, int ini, out string? bracket)
        {
            if (ini < text.Length)
            {
                bracket = $"{{{i}}}";

                if (ini >= text.Length) return -1;
                return text.IndexOf(bracket, ini, comparison);
            }

            bracket = null;
            return -1;
        }

        /// <summary>
        /// Gets the index of the first ocurrence of a named bracket with the given name starting
        /// at the given position, or -1 if not found. Name is tested with and without the engine
        /// prefix.
        /// </summary>
        int FindNamedBracket(string name, int ini, out string? bracket)
        {
            if (ini < text!.Length)
            {
                bracket = $"{{{name}}}";
                var pos = text.IndexOf(bracket, ini, comparison);
                if (pos >= 0) return pos;

                if (!name.StartsWith(Engine.ParametersPrefix, comparison))
                {
                    bracket = $"{{{Engine.ParametersPrefix + name}}}";
                    pos = text.IndexOf(bracket, ini, comparison);
                    if (pos >= 0) return pos;
                }

                else
                {
                    name = name.Remove(0, Engine.ParametersPrefix.Length);
                    if (name.Length >= 1)
                    {
                        bracket = $"{{{name}}}";
                        pos = text.IndexOf(bracket, ini, comparison);
                        if (pos >= 0) return pos;
                    }
                }
            }

            bracket = null;
            return -1;
        }
    }

    /// <summary>
    /// Represents an element in the given collecion of parameters.
    /// </summary>
    class RangeElement
    {
        public object? Value;
        public IParameter? Parameter;
        public bool Used;

        public RangeElement() { }
        public override string ToString()
        {
            string str;
            if (Parameter is not null) str = $"Par:{Parameter}";
            else if (Value is IParameter par) str = $"Par:{par}";
            else if (Value is AnonymousItem item) str = $"Anon:{item}";
            else str = $"'{Value.Sketch()}'";

            if (Used) str += " (Used)";
            return str;
        }

        /// <summary>
        /// Captures the elements of the given collection of parameters.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static RangeElement[] Capture(object?[] range)
        {
            if (range.Length == 0) return [];

            var items = new RangeElement[range.Length];
            for (int i = 0; i < range.Length; i++)
            {
                var temp = AnonymousItem.TryCapture(range[i]);
                items[i] = new() { Value = temp };
            }
            return items;
        }
    }

    /// <summary>
    /// Represents an anonymous item in the given collection of parameters.
    /// </summary>
    class AnonymousItem
    {
        public string Name;
        public object? Value;

        public AnonymousItem(string name, object? value) { Name = name; Value = value; }
        public override string ToString() => $"{Name}:'{Value ?? "-"}'";

        /// <summary>
        /// Tries to capture the given value as an anonymous item, or returns the given one.
        /// </summary>
        public static object? TryCapture(object? value)
        {
            if (value is not null)
            {
                var type = value.GetType();
                if (type.IsAnonymous())
                {
                    var members = type.GetProperties();
                    if (members.Length == 0) throw new ArgumentException("No properties in anonymous argument.").WithData(value);
                    if (members.Length > 1) throw new ArgumentException("Too many in anonymous argument.").WithData(value);

                    var member = members[0];
                    var name = member.Name;
                    var temp = member.GetValue(value);

                    value = new AnonymousItem(name, temp);
                }
            }
            return value;
        }
    }
}