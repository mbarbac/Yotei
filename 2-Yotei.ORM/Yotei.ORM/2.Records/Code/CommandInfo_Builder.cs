#pragma warning disable IDE0057

using System.Data;

namespace Yotei.ORM.Records.Code;

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
        /// Initializes a new instance with the given text and the collection of parameters
        /// obtained from the given range of values.
        /// <br/> If text is null, then the range of value is captured without any attempts of
        /// matching their names with any text specifications. Similarly, if there are no items
        /// in the range of values, then the text is captured without intercepting any dangling
        /// specifications.
        /// <br/> Parameter specifications in the given text must always be bracket ones, either
        /// positional '{n}' or named '{name}' ones. Positional ones refer to the ordinal of the
        /// element in the range of values. Named ones contain the name of the parameter, or the
        /// name of the unique property of the given anonymous item. In both cases, 'name' may or
        /// may not start with the engine parameters' prefix, which is always used in the captured
        /// ones. If no bracketed, you can use raw parameter names as you wish.
        /// <br/> No unused parameters are allowed, neither dangling specifications in the text.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="text"></param>
        /// <param name="range"></param>
        public Builder(IEngine engine, string? text, params object?[]? range) : this(engine)
        {
            var noRemainingSpecs = true;
            var noUnusedValues = true;
            var space = true;
            Add(noRemainingSpecs, noUnusedValues, space, text, range);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source)
        {
            source.ThrowWhenNull();

            _Text = new(source._Text.ToString());
            _Parameters = new(source.Engine, source._Parameters.ToInstance());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (_Parameters.Count == 0) return _Text.ToString();

            var pars = $"[{string.Join(", ", _Parameters)}]";
            return _Text.Length == 0 ? pars : $"{_Text} : {pars}";
        }

        /// <inheritdoc/>
        public ICommandInfo ToInstance() => new CommandInfo(this);

        // ------------------------------------------------

        /// <inheritdoc/>
        public IEngine Engine => _Parameters.Engine;

        /// <inheritdoc/>
        public string Text => _Text.ToString();

        /// <inheritdoc/>
        public IParameterList Parameters => _Parameters.ToInstance();

        /// <inheritdoc/>
        public bool IsEmpty => _Text.Length == 0 && _Parameters.Count == 0;

        // ------------------------------------------------

        StringComparison Comparison => Engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        string Prefix => Engine.ParameterPrefix;

        // ------------------------------------------------

        /// <inheritdoc/>
        public bool ReplaceText(string? text)
        {
            if (_Text.Length == 0 && (text is null || text.Length == 0)) return false;

            if (text is not null && AreRemainingBrackets(text)) throw new ArgumentException(
                "No '{...}' bracket specifications allowed.")
                .WithData(text);

            _Text.Clear();
            _Text.Append(text ?? string.Empty);
            return true;
        }

        /// <inheritdoc/>
        public bool ReplaceValues(params object?[]? range)
        {
            range ??= [null];

            if (_Parameters.Count == 0 && range.Length == 0) return false;

            _Parameters.Clear(); if (range.Length > 0)
            {
                var items = RangeElement.Capture(range);
                var captured = new ParameterList.Builder(Engine);
                
                for (int i = 0; i < items.Length; i++)
                {
                    var item = items[i];
                    switch (item.Value)
                    {
                        case IParameter xpar:
                            Capture(xpar, captured);
                            break;

                        case AnonymousElement anon:
                            var par = new Parameter(anon.Name, anon.Value);
                            Capture(par, captured);
                            break;
                        
                        default:
                            _Parameters.AddNew(item.Value, out _);
                            break;
                    }
                }
            }

            return true;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public bool Clear()
        {
            if (IsEmpty) return false;

            _Text.Clear();
            _Parameters.Clear();
            return true;
        }

        /// <inheritdoc/>
        public bool Add(ICommandInfo source)
        {
            source.ThrowWhenNull();

            var comparison = source.Engine.CaseSensitiveNames
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            var pars = source.Parameters.ToArray();
            var text = TextToBrackets(source.Text, pars, comparison);
            if (text.Length == 0) text = null;

            var noRemainingSpecs = text is not null;
            var noUnusedValues = true;
            var space = true;
            return Add(noRemainingSpecs, noUnusedValues, space, text, pars);
        }

        /// <inheritdoc/>
        public bool Add(ICommandInfo.IBuilder source)
        {
            source.ThrowWhenNull();

            var comparison = source.Engine.CaseSensitiveNames
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            var pars = source.Parameters.ToArray();
            var text = TextToBrackets(source.Text, pars, comparison);
            if (text.Length == 0) text = null;

            var noRemainingSpecs = text is not null;
            var noUnusedValues = true;
            var space = true;
            return Add(noRemainingSpecs, noUnusedValues, space, text, pars);
        }

        /// <inheritdoc/>
        public bool Add(string? text, params object?[]? range)
        {
            var noRemainingSpecs = true;
            var noUnusedValues = true;
            var space = true;
            return Add(noRemainingSpecs, noUnusedValues, space, text, range);
        }

        /// <inheritdoc/>
        public bool Add(bool space, string? text, params object?[]? range)
        {
            var noRemainingSpecs = true;
            var noUnusedValues = true;
            return Add(noRemainingSpecs, noUnusedValues, space, text, range);
        }

        // ------------------------------------------------

        /// <summary>
        /// Factorizes common adding code.
        /// </summary>
        bool Add(
            bool noRemainingSpecs,
            bool noUnusedValues,
            bool space,
            string? text, params object?[]? range)
        {
            var textnull = text is null;
            var rangeempty = range is not null && range.Length == 0;

            if ((textnull || text!.Length == 0) && rangeempty) return false;

            text ??= string.Empty;
            range ??= [null];

            // Capturing and validating...
            var items = RangeElement.Capture(range);

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                if (item.Value is ICommand) throw new ArgumentException("Element cannot be a command itself.").WithData(item);
                if (item.Value is ICommandInfo) throw new ArgumentException("Element cannot be a command info itself.").WithData(item);
                if (item.Value is ICommandInfo.IBuilder) throw new ArgumentException("Element cannot be a command info builder itself.").WithData(item);
            }

            // Iterating through the given range of elements...
            var ret = !textnull;
            var captured = new ParameterList.Builder(Engine);

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                string name = null!;
                IParameter par;

                // Capturing a suitable parameter...
                switch (item.Value)
                {
                    case IParameter xpar:
                        par = Capture(xpar, captured);
                        name = par.Name;
                        break;

                    case AnonymousElement anon:
                        par = new Parameter(anon.Name, anon.Value);
                        par = Capture(par, captured);
                        name = par.Name;
                        break;

                    default:
                        _Parameters.AddNew(item.Value, out par);
                        captured.Add(par);
                        name = par.Name;
                        break;
                }

                // If no given text no need to modify it ;)...
                if (textnull)
                {
                    item.Used = true;
                    ret = true;
                    continue;
                }

                // Modifying text by named brackets...
                var pos = 0;
                while ((pos = FindNamedBracket(name, text, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);

                    pos += par.Name.Length;
                    item.Used = true;
                    ret = true;
                }

                // Modifying text by ordinal brackets...
                pos = 0;
                while ((pos = FindOrdinalBracket(i, text, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);

                    pos += par.Name.Length;
                    item.Used = true;
                    ret = true;
                }
            }

            // Validating no dangling specifications...
            if (noRemainingSpecs &&
                !textnull && AreRemainingBrackets(text)) throw new ArgumentException(
                    "There are unused brackets in the given text.")
                    .WithData(text);

            // Validating no remaining unused elements...
            if (noUnusedValues &&
                items.Length > 0 && items.Any(x => !x.Used)) throw new ArgumentException(
                    "There are unused elements in the range of values.")
                    .WithData(range)
                    .WithData(text);

            // Finishing...
            if (ret && !textnull)
            {
                if (space && _Text.Length > 0)
                {
                    if (text.Length > 0 && text[0] != ' ') _Text.Append(' ');
                }
                _Text.Append(text);
            }
            return ret;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the given text modified with the appropriate brackets to refer to the given
        /// collection of parameters. We are assuming that the name of all parameters start with
        /// the engine parameters' prefix.
        /// </summary>
        static string TextToBrackets(
            string text, IEnumerable<IParameter> pars, StringComparison comparison)
        {
            var i = 0;
            foreach (var par in pars)
            {
                var bracket = $"{{{i}}}";
                i++;

                var pos = 0;
                while ((pos = text.IndexOf(par.Name, pos, comparison)) >= 0)
                {
                    text = text.Remove(pos, par.Name.Length);
                    text = text.Insert(pos, bracket);
                    pos += bracket.Length;
                }
            }

            return text;
        }

        /// <summary>
        /// Determines if there are any dangling brackets in the given text.
        /// </summary>
        static bool AreRemainingBrackets(string text)
        {
            var ini = text.IndexOf('{'); if (ini < 0) return false;
            var end = text.IndexOf('}', ini); if (end < 0) return false;
            return true;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of an ordinal '{i}' bracket in the given
        /// text, starting from the given initial position, or -1 if such is not found. If found,
        /// the bracket itself is returned in the out argument.
        /// </summary>
        static int FindOrdinalBracket(int i, string text, int ini, out string? bracket)
        {
            if (ini < text.Length)
            {
                bracket = $"{{{i}}}";

                var pos = text.IndexOf(bracket, ini);
                if (pos >= 0) return pos;
            }

            bracket = null;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a named '{name}' bracket in the given
        /// text, starting from the given initial position, or -1 if such is not found. If found,
        /// the bracket itself is returned in the out argument.
        /// <br/> 'name' is tested with and without the engine parameters' prefix, as needed.
        /// </summary>
        int FindNamedBracket(string name, string text, int ini, out string? bracket)
        {
            if (ini < text.Length)
            {
                bracket = $"{{{name}}}";
                var pos = text.IndexOf(bracket, ini, Comparison);
                if (pos >= 0) return pos;

                if (!name.StartsWith(Prefix, Comparison))
                {
                    bracket = $"{{{Prefix + name}}}";
                    pos = text.IndexOf(bracket, ini, Comparison);
                    if (pos >= 0) return pos;
                }

                else
                {
                    name = name.Remove(0, Prefix.Length);
                    if (name.Length > 0)
                    {
                        bracket = $"{{{name}}}";
                        pos = text.IndexOf(bracket, ini, Comparison);
                        if (pos >= 0) return pos;
                    }
                }
            }

            bracket = null;
            return -1;
        }

        /// <summary>
        /// Captures the given parameter into both the collection of parameters of this instance
        /// and the 'captured' collection that keeps track of the new added ones to prevent any
        /// duplication in the new names. Returns either the given parameter, or a new one whose
        /// name may have changed if it didn't start with the engine parameters' prefix, or if
        /// that name collides with the name of an existing original parameter.
        /// </summary>
        IParameter Capture(IParameter par, ParameterList.Builder captured)
        {
            par = Validate(par);

            if (captured.Contains(par.Name)) throw new DuplicateException(
                "Duplicated element name detected.")
                .WithData(par.Name);

            if (_Parameters.Contains(par.Name)) _Parameters.AddNew(par.Value, out par);
            else _Parameters.Add(par);

            captured.Add(par);
            return par;
        }

        /// <summary>
        /// Validates that the name of the given parameter starts with the given prefix and,
        /// if not, returns a new one with a modified name. Otherwise, returns the given one.
        /// </summary>
        IParameter Validate(IParameter par)
        {
            return par.Name.StartsWith(Prefix, Comparison)
                ? par
                : new Parameter(Prefix + par.Name, par.Value);
        }

        // ------------------------------------------------

        /// <summary>
        /// Represents an arbitrary element of the given range of values. 
        /// </summary>
        class RangeElement
        {
            public object? Value;
            public bool Used;

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            public RangeElement(object? value, bool used = false)
            {
                Value = value;
                Used = used;
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                var sb = new StringBuilder(); switch (Value)
                {
                    case IParameter item: sb.Append($"Par: {item}"); break;
                    case AnonymousElement item: sb.Append($"Anon: {item}"); break;
                    default: sb.Append($"'{Value.Sketch()}'"); break;
                }
                if (Used) sb.Append($" (Used)");
                return sb.ToString();
            }

            /// <summary>
            /// Captures the elements of the given normalized range of values.
            /// </summary>
            public static RangeElement[] Capture(object?[] range)
            {
                if (range.Length == 0) return [];

                var items = new RangeElement[range.Length];
                for (int i = 0; i < range.Length; i++)
                {
                    var temp = AnonymousElement.TryCapture(range[i]);
                    items[i] = new(temp);
                }
                return items;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Represents an anonymous element in range of values, as in 'new {Name = "..." }'.
        /// </summary>
        class AnonymousElement
        {
            public string Name;
            public object? Value;

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            public AnonymousElement(string name, object? value)
            {
                Name = name.NotNullNotEmpty();
                Value = value;
            }

            /// <inheritdoc/>
            public override string ToString() => $"{Name}:'{Value}'";

            /// <summary>
            /// Tries to capture the given value as an anonymous item, if that is possible, or
            /// otherwise returns the original value itself.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static object? TryCapture(object? value)
            {
                if (value is not null)
                {
                    var type = value.GetType();
                    if (type.IsAnonymous())
                    {
                        var members = type.GetProperties();
                        if (members.Length == 0) throw new ArgumentException("No properties in anonymous argument.").WithData(value);
                        if (members.Length > 1) throw new ArgumentException("Too many properties in anonymous argument.").WithData(value);

                        var member = members[0];
                        var name = member.Name;
                        var temp = member.GetValue(value);

                        value = new AnonymousElement(name, temp);
                    }
                }

                return value;
            }
        }
    }
}
