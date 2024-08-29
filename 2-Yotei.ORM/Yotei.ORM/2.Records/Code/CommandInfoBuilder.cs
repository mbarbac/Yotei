using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfoBuilder"/>
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
        var brackets = false;
        Add(brackets, source.Text, source.Parameters);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfoBuilder(IEngine engine, string? text, params object?[] range) : this(engine)
    {
        var brackets = true;
        Add(brackets, text, range);
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

    /// <inheritdoc/>
    public bool ReplaceValues(params object?[] range)
    {
        range ??= [null];

        if (_Parameters.Count == 0 && range.Length == 0) return false;

        _Parameters.Clear();

        var brackets = true;
        Add(brackets, null, range);
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Add(ICommandInfo source)
    {
        source.ThrowWhenNull();

        var brackets = false;
        return Add(brackets, source.Text, source.Parameters);
    }

    /// <inheritdoc/>
    public bool Add(ICommandInfoBuilder source)
    {
        source.ThrowWhenNull();

        var brackets = false;
        return Add(brackets, source.Text, source.Parameters);
    }

    /// <inheritdoc/>
    public bool Add(string? text, params object?[] range)
    {
        var brackets = true;
        return Add(brackets, text, range);
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
    /// Factorizes common add code.
    /// <br/> If text is null, then just capture the elements without trying to match specs.
    /// <br/> If not elements, then just capture the text without trying to match specs.
    /// <br/> Specs can be '{n}' ordinal or '{name}' ones, name using or not engine prefix.
    /// <br/> brackets: Find element names using brackets or ordinals, or just the raw name.
    /// </summary>
    /// <param name="brackets"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    bool Add(bool brackets, string? text, params object?[] range)
    {
        var match = !(text is null || (range != null && range.Length == 0));
        var textnull = text is null;
        var comparison = Engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        text ??= string.Empty;
        range ??= [null];

        var done = !textnull;
        var novels = new ParameterListBuilder(Engine);

        // Intercepting single-valued elements...
        if (range.Length == 1 &&
            range[0] is IEnumerable<IParameter> xrange) return Add(brackets, text, xrange.ToArray());

        // Capturing and validating elements...
        var items = RangeElement.Capture(range);
        for (int i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (item.Value is IEnumerable<IParameter>) throw new ArgumentException("Element in the range of values cannot be a parameters' enumeration.").WithData(item);
            if (item.Value is ICommandInfo) throw new ArgumentException("Element in the range of values cannot be a command info instance.").WithData(item);
        }

        // Iterating the given elements...
        for (int i = 0; i < items.Length; i++)
        {
            string name = null!;
            var item = items[i];

            if (item.Value is IParameter par) // Element is a parameter...
            {
                name = par.Name;
                par = Capture(par);
            }
            else if (item.Value is AnonymousItem anon) // Element is an anonymous type...
            {
                name = anon.Name;
                par = new Parameter(anon.Name, anon.Value);
                par = Capture(par);
            }
            else // Element is an arbitrary value...
            {
                _Parameters.AddNew(item.Value, out par);

                novels.Add(par);
                name = par.Name;
            }

            if (textnull) // We don't need to modify the given text...
            {
                item.Used = true;
                done = true;
                continue;
            }

            if (brackets) // Finding named or ordinal specs...
            {
                var pos = 0;
                while ((pos = FindNamedBracket(name, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);

                    pos += par.Name.Length;
                    item.Used = true;
                    done = true;
                }

                pos = 0;
                while ((pos = FindOrdinalBracket(i, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);

                    pos += par.Name.Length;
                    item.Used = true;
                    done = true;
                }
            }

            else // Finding raw element names...
            {
                var pos = 0;
                while ((pos = FindRawName(name, pos)) >= 0)
                {
                    text = text.Remove(pos, name.Length);
                    text = text.Insert(pos, par.Name);

                    pos += par.Name.Length;
                    item.Used = true;
                    done = true;
                }
            }
        }

        // Validations...
        if (match)
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
        /// Captures the given parameter, eventually creating a new one if:
        /// - Its name does not belong with the engine's prefix.
        /// - Its name collides with an original one.
        /// In addition, this method validates that the name of the captured parameter is not a
        /// duplicate of any of the new parameters.
        /// </summary>
        IParameter Capture(IParameter par)
        {
            var prefix = Engine.ParametersPrefix;

            if (!par.Name.StartsWith(prefix, comparison))
                par = new Parameter(prefix + par.Name, par.Value);

            if (novels.Contains(par.Name)) throw new DuplicateException(
                "Duplicated element name detected.")
                .WithData(par);

            if (_Parameters.Contains(par.Name)) _Parameters.AddNew(par.Value, out par);
            else _Parameters.Add(par);

            novels.Add(par);
            return par;
        }

        /// <summary>
        /// Gets the index of the first ocurrence or the given name, starting from the given
        /// index, provided there are suitable heading and trailing separators.
        /// </summary>
        int FindRawName(string name, int ini)
        {
            var pos = text.IndexOf(name, ini, comparison);
            
            if (pos >= 0)
            {
                if (pos > 0)
                {
                    var c = text[pos - 1];
                    if (!Separators.Contains(c)) return -1;
                }

                var len = pos + name.Length;
                if (len < (text.Length - 1))
                {
                    var c = text[pos - 1];
                    if (!Separators.Contains(c)) return -1;
                }
            }

            return pos;
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
    /// Used to identify raw element names.
    /// </summary>
    static char[] Separators = " ()[]{}.,:;-+*/^!?=&%´'\"".ToCharArray();

    /// <summary>
    /// Represents an element in the given collecion of parameters.
    /// </summary>
    class RangeElement
    {
        public object? Value;
        public bool Used;

        public RangeElement() { }
        public override string ToString()
        {
            string str;
            if (Value is IParameter par) str = $"Par:{par}";
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