#pragma warning disable IDE0057

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

        /// <inheritdoc cref="ICommandInfo.IBuilder.CreateInstance"/>
        public virtual CommandInfo CreateInstance() => new(this);
        ICommandInfo ICommandInfo.IBuilder.CreateInstance() => CreateInstance();

        /// <inheritdoc/>
        public IEngine Engine => _Parameters.Engine;

        StringComparison Comparison => Engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        string Prefix => Engine.ParameterPrefix;

        /// <inheritdoc/>
        public string Text => _Text.ToString();

        /// <inheritdoc/>
        public IParameterList Parameters => _Parameters.CreateInstance();

        /// <inheritdoc/>
        public bool IsEmpty => _Text.Length == 0 && _Parameters.Count == 0;

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Clear()
        {
            if (IsEmpty) return false;

            _Text.Clear();
            _Parameters.Clear();
            return true;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool ReplaceText(string? text)
        {
            if (_Text.Length == 0 && (text is null || text.Length == 0)) return false;

            if (text is not null && AreRemainingBrackets(text))
                throw new ArgumentException(
                    "No '{...}' bracket specifications allowed.")
                    .WithData(text);

            _Text.Clear();
            _Text.Append(text ?? string.Empty);
            return true;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool ReplaceValues(params object?[]? range)
        {
            range ??= [null];

            if (range.Length == 1)
            {
                if (range[0] is IParameterList xlist) range = xlist.ToArray();
                if (range[0] is IParameterList.IBuilder xbuilder) range = xbuilder.ToArray();
            }

            if (_Parameters.Count == 0 && range.Length == 0) return false;

            var old = _Parameters.Clone(); // Keeps track of the original ones...

            if (range.Length > 0) // Iterating...
            {
                var items = RangeElement.Capture(range);
                var captured = new ParameterList.Builder(Engine);

                for (int i = 0; i < items.Length; i++)
                {
                    var item = items[i]; switch (item.Value)
                    {
                        case IParameter par:
                            Capture(par, captured);
                            break;

                        case AnonymousElement anon:
                            var xpar = new Parameter(anon.Name, anon.Value);
                            Capture(xpar, captured);
                            break;

                        default:
                            _Parameters.AddNew(item.Value, out _);
                            break;
                    }
                }

                if (old.Equals(captured)) return false;
            }

            // Finishing...
            if (old.Count > 0) _Parameters.RemoveRange(0, old.Count);
            return true;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Add(ICommandInfo source)
        {
            source.ThrowWhenNull();

            var text = source.Text;
            var pars = source.Parameters;

            // If 'source.Text' is not null, we don't want dangling specs. But it may happen that
            // source carries unused values due to previous transformations...

            var noRemainingSpecs = text.Length > 0;
            var noUnusedValues = false;

            return Add(noRemainingSpecs, noUnusedValues, text, pars);
        }

        /// <inheritdoc/>
        public virtual bool Add(ICommandInfo.IBuilder source)
        {
            source.ThrowWhenNull();

            var text = source.Text;
            var pars = source.Parameters;

            // If 'source.Text' is not null, we don't want dangling specs. But it may happen that
            // source carries unused values due to previous transformations...

            var noRemainingSpecs = text.Length > 0;
            var noUnusedValues = false;

            return Add(noRemainingSpecs, noUnusedValues, text, pars);
        }

        /// <inheritdoc/>
        public virtual bool Add(string? text, params object?[]? range)
        {
            var noRemainingSpecs = true;
            var noUnusedValues = true;

            return Add(noRemainingSpecs, noUnusedValues, text, range);
        }

        // ------------------------------------------------

        /// <summary>
        /// Adds to this instance the given text and collection of parameters.
        /// <br/>- If <paramref name="noRemainingSpecs"/> then no dangling specs are allowed.
        /// <br/>- If <paramref name="noUnusedValues"/> the no unused values are allowed.
        /// </summary>
        bool Add(
            bool noRemainingSpecs,
            bool noUnusedValues,
            string? text, params object?[]? range)
        {
            var textnull = text is null;
            var rangeempty = range is not null && range.Length == 0;
            if ((textnull || text!.Length == 0) && rangeempty) return false;

            text ??= string.Empty;
            range ??= [null];

            // Capturing optional values...
            var items = RangeElement.Capture(range);

            if (items.Length == 1) // Cases when range is just one special element...
            {
                if (items[0].Value is IParameterList parList) // Special case...
                {
                    text = NamesToOrdinalBrackets(text, parList, Comparison);
                    range = parList.ToArray();
                    return Add(noRemainingSpecs, noUnusedValues, text, range);
                }
                if (items[0].Value is IParameterList.IBuilder parBuilder) // Special case...
                {
                    text = NamesToOrdinalBrackets(text, parBuilder, Comparison);
                    range = parBuilder.ToArray();
                    return Add(noRemainingSpecs, noUnusedValues, text, range);
                }
            }

            for (int i = 0; i < items.Length; i++) // Command-alike parameters not allowed...
            {
                var item = items[i];

                if (item.Value is ICommand) throw new ArgumentException("Element cannot be a command itself.").WithData(item);
                if (item.Value is ICommandInfo) throw new ArgumentException("Element cannot be a command info itself.").WithData(item);
                if (item.Value is ICommandInfo.IBuilder) throw new ArgumentException("Element cannot be a command info builder itself.").WithData(item);
            }

            // Iterating through the given range of optional values...
            var ret = !textnull;
            var captured = new ParameterList.Builder(Engine);

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                string name = null!;
                bool changed = false;
                IParameter par;

                // Capturing a suitable parameter...
                switch (item.Value)
                {
                    case IParameter xpar:
                        par = Capture(xpar, captured);
                        name = xpar.Name;
                        break;

                    case AnonymousElement anon:
                        par = new Parameter(anon.Name, anon.Value);
                        par = Capture(par, captured);
                        name = anon.Name;
                        break;

                    default:
                        _Parameters.AddNew(item.Value, out par);
                        captured.Add(par);
                        name = par.Name;
                        break;
                }

                // If no given text, no need to modify it...
                if (textnull)
                {
                    item.Used = true;
                    ret = true;
                    continue;
                }

                // Modifying text by named brackets, where 'name' is used to find the old name in
                // the text, and needs not to be the same as the new 'par.Name'...
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
            if (ret && !textnull) _Text.Append(text);
            return ret;
        }

        // ------------------------------------------------

        /// <summary>
        /// Determines if there are any dangling '{...}' brackets in the given text.
        /// </summary>
        static bool AreRemainingBrackets(string text)
        {
            var ini = text.IndexOf('{'); if (ini < 0) return false;
            var end = text.IndexOf('}', ini); if (end < 0) return false;
            return true;
        }

        /// <summary>
        /// Replaces the names of the given parameters in the given text to ordinal bracket
        /// specifications, maintaining the order of the parameters in that collection.
        /// </summary>
        static string? NamesToOrdinalBrackets(
            string? text,
            IEnumerable<IParameter> pars, StringComparison comparison)
        {
            if (text is not null && text.Length > 0)
            {
                var i = 0; foreach (var par in pars)
                {
                    var bracket = $"{{{i}}}"; i++;
                    var name = par.Name;
                    var pos = 0;

                    while ((pos = name.FindIsolated(text, pos, comparison)) >= 0)
                    {
                        text = text.Remove(pos, par.Name.Length);
                        text = text.Insert(pos, bracket);
                        pos += bracket.Length;
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of an ordinal '{value}' bracket in the given
        /// text, starting at the given position, or -1 if any is found. Otherwise, the found
        /// bracket itself is returned in the out parameter.
        /// </summary>
        static int FindOrdinalBracket(int value, string text, int ini, out string? bracket)
        {
            if (ini < text.Length)
            {
                bracket = $"{{{value}}}";

                var pos = text.IndexOf(bracket, ini);
                if (pos >= 0) return pos;
            }

            bracket = null;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a named '{name}' bracket in the given
        /// text, starting at the given position, or -1 if any is found. Otherwise, the found
        /// bracket itself is returned in the out parameter. The 'name' is tested with and
        /// without the engine's prefix as needed.
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
        /// Validates that the name of the given parameter starts with the engine's prefix and,
        /// if not, returns a new instance with a modified name.
        /// </summary>
        IParameter ValidateName(IParameter par)
        {
            if (par.Name.StartsWith(Prefix, Comparison))
            {
                return par;
            }
            else
            {
                return par.WithName(Prefix + par.Name);
            }
        }

        /// <summary>
        /// Captures the given parameter into both this instance and into the given collection
        /// of captured ones intended to keep track of the new ones, and to prevent new duplicated
        /// names. Returns either the given parameter, or a new one created to prevent collision
        /// of its name with any *original* ones.
        /// </summary>
        IParameter Capture(IParameter par, ParameterList.Builder captured)
        {
            par = ValidateName(par);

            if (captured.Contains(par.Name)) throw new DuplicateException(
                "Duplicated element name detected.")
                .WithData(par.Name);

            if (_Parameters.Contains(par.Name))
            {
                _Parameters.AddNew(par.Value, out par);
            }
            else _Parameters.Add(par);

            captured.Add(par);
            return par;
        }

        // ------------------------------------------------

        /// <summary>
        /// Represents an arbitrary element in a given range of values. 
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
        /// Represents an anonymous element in a range of values, as in 'new {Name = Value}'.
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