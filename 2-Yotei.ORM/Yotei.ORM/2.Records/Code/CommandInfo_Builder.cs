#pragma warning disable IDE0057

using System.ComponentModel.DataAnnotations;

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
        public Builder(ICommandInfo source) : this(source.Engine) => Add(source);

        /// <summary>
        /// Initializes a new instance with the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo.IBuilder source) : this(source.Engine) => Add(source);

        /// <summary>
        /// Initializes a new instance with the given text and parameters obtained from the given
        /// range of values.
        /// <br/> If text is null, then the range of values is just captured without any attempt
        /// of matching their names with any text specifications. Conversely, if no elements are
        /// given in the range, then the text is just captured without intercepting any dangling
        /// specifications.
        /// <br/> Parameter specifications in the text are always bracket ones, and can either be
        /// positional '{n}' or named '{name}' ones. Positional one refer to the ordinal of the
        /// element in the range of values. Named ones contain the name of the parameter or the
        /// unique property of the given anonymous item, and 'name' may or may not start with
        /// the engine parameters' prefix.
        /// <br/> No unused parameters are allowed, neither dangling specifications in the text.
        /// <br/> You can use raw (non-bracketed) parameter names as you wish.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="text"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, string? text, params object?[]? range) : this(engine)
            => AddCore(text, range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.Engine) => Add(source);

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

        /// <inheritdoc/>
        public bool Clear()
        {
            if (IsEmpty) return false;

            _Text.Clear();
            _Parameters.Clear();
            return true;
        }

        /// <inheritdoc/>
        public bool ReplaceText(string? text)
        {
            if (_Text.Length == 0 && (text is null || text.Length == 0)) return false;

            if (text is not null && AreAnyBrackets(text)) throw new ArgumentException(
                "No '{...}' bracket specifications allowed.")
                .WithData(text);

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
                IParameterList.IBuilder items => items.Count,
                List<IParameter> items => items.Count,
                _ => range.Count(),
            };

            if (_Parameters.Count == 0 && count == 0) return false;

            _Parameters.Clear();
            _Parameters.AddRange(range);
            return true;
        }

        /// <inheritdoc/>
        public bool ReplaceValues(params object?[]? range)
        {
            range ??= [null];

            if (_Parameters.Count == 0 && range.Length == 0) return false;

            _Parameters.Clear();
            AddCore(null, range);
            return true;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public bool Add(ICommandInfo source)
        {
            source.ThrowWhenNull();

            var comparison = Engine.CaseSensitiveNames
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            var pars = source.Parameters;
            var text = TextToBrackets(source.Text, pars, comparison);
            return AddCore(text, pars);
        }

        /// <inheritdoc/>
        public bool Add(ICommandInfo.IBuilder source)
        {
            source.ThrowWhenNull();

            var comparison = Engine.CaseSensitiveNames
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            var pars = source.Parameters;
            var text = TextToBrackets(source.Text, pars, comparison);
            return AddCore(text, pars);
        }

        /// <inheritdoc/>
        public bool Add(string? text, params object?[]? range) => AddCore(text, range);

        // ------------------------------------------------

        /// <summary>
        /// Factorizes common code to add to this instance the given text and the collection of
        /// parameters obtained from the given range of values.
        /// <br/> If text is null, then the range of values is just captured without any attempt
        /// of matching their names with any text specifications. Conversely, if no elements are
        /// given in the range, then the text is just captured without intercepting any dangling
        /// specifications.
        /// <br/> Parameter specifications in the text are always bracket ones, and can either be
        /// positional '{n}' or named '{name}' ones. Positional one refer to the ordinal of the
        /// element in the range of values. Named ones contain the name of the parameter or the
        /// unique property of the given anonymous item, and 'name' may or may not start with
        /// the engine parameters' prefix.
        /// <br/> No unused parameters are allowed, neither dangling specifications in the text.
        /// <br/> You can use raw (non-bracketed) parameter names as you wish.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns>Whether changes has been made or not.</returns>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddCore(string? text, params object?[]? range) 
        {
            var textnull = text is null;
            var rangeempty = range is not null && range.Length == 0;

            if ((textnull || text!.Length == 0) && rangeempty) return false;
            
            text ??= string.Empty;
            range ??= [null];

            // Capturing and validating...
            if (range.Length == 1)
            {
                var item = range[0];
                if (item is IEnumerable<IParameter> xrange) return AddCore(text, xrange.ToArray());
            }

            var items = RangeElement.Capture(range);
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                switch (item.Value)
                {
                    case IEnumerable<IParameter>: throw new ArgumentException("Element in range cannot be a parameters' collection.").WithData(item);
                    case ICommandInfo.IBuilder: throw new ArgumentException("Element in range cannot be a command info builder.").WithData(item);
                    case ICommandInfo: throw new ArgumentException("Element in range cannot be a command info.").WithData(item);
                    case ICommand: throw new ArgumentException("Element in range cannot be a command.").WithData(item);
                }
            }

            // Iterating through the given range of elements...
            var ret = !textnull;
            var captured = new ParameterList.Builder(Engine);
            var comparison = Engine.CaseSensitiveNames
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                string name = null!;
                IParameter par;

                // Capturing a suitable parameter...
                switch (item.Value)
                {
                    case IParameter xpar:
                        par = Capture(xpar, captured, comparison);
                        name = par.Name;
                        break;

                    case AnonymousElement anon:
                        par = new Parameter(anon.Name, anon.Value);
                        par = Capture(par, captured, comparison);
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
                while ((pos = FindNamedBracket(name, text, pos, comparison, out var bracket)) >= 0)
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

            // Validating no remaining unused elements...
            if (items.Length > 0 && items.Any(x => !x.Used)) throw new ArgumentException(
                "There are unused elements in the range of values.")
                .WithData(range)
                .WithData(text);

            // Validating no dangling specifications...
            if (!textnull && AreAnyBrackets(text)) throw new ArgumentException(
                "There are unused brackets in the given text.")
                .WithData(text);

            // Finishing...
            if (ret && !textnull) _Text.Append(text);
            return ret;
        }

        // ------------------------------------------------

        /// <summary>
        /// Finds in the given text the raw parameter names of the given collection, and transform
        /// them into bracket ordinal ones.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        static string TextToBrackets(
            string text, IEnumerable<IParameter> pars, StringComparison comparison)
        {
            var i = 0;
            var pos = 0;
            
            foreach (var par in pars)
            {
                var bracket = $"{{{i}}}";

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
        static bool AreAnyBrackets(string text)
        {
            var ini = text.IndexOf('{'); if (ini < 0) return false;
            var end = text.IndexOf('}', ini); if (end < 0) return false;
            return true;
        }

        /// <summary>
        /// Gets the index of the first ocurrence of an ordinal bracket '{i}', starting from the
        /// given position, or -1 if not found. If found, returns the complete bracket in the out
        /// argument.
        /// </summary>
        static int FindOrdinalBracket(
            int i, 
            string text, int ini,
            out string? bracket)
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
        /// Gets the index of the first ocurrence of a named bracket '{name}', starting from
        /// the given position, or -1 if not found. 'name' itself is tested with and without
        /// the engine parameters' prefix. If found, returns the complete bracket in the out
        /// argument.
        /// </summary>
        int FindNamedBracket(
            string name,
            string text, int ini, StringComparison comparison,
            out string? bracket)
        {
            if (ini < text.Length)
            {
                bracket = $"{{{name}}}";
                var pos = text.IndexOf(bracket, ini, comparison);
                if (pos >= 0) return pos;

                if (!name.StartsWith(Engine.ParameterPrefix, comparison))
                {
                    bracket = $"{{{Engine.ParameterPrefix + name}}}";
                    pos = text.IndexOf(bracket, ini, comparison);
                    if (pos >= 0) return pos;
                }

                else
                {
                    name = name.Remove(0, Engine.ParameterPrefix.Length);
                    if (name.Length > 0)
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

        /// <summary>
        /// Captures the given parameter in both the 'captured' collection and in the original
        /// one, intercepting duplicate names in the new added parameters, but also eventually 
        /// creating a new one if:
        /// - The name does not start with the engine parameters' prefix.
        /// - The name collides with an existing one.
        /// </summary>
        IParameter Capture(
            IParameter par,
            ParameterList.Builder captured, StringComparison comparison)
        {
            var prefix = Engine.ParameterPrefix;

            if (!par.Name.StartsWith(prefix, comparison))
                par = new Parameter(prefix + par.Name, par.Value);

            if (captured.Contains(par.Name)) throw new DuplicateException(
                "Duplicated element name detected.")
                .WithData(par.Name);

            if (_Parameters.Contains(par.Name)) _Parameters.AddNew(par.Value, out par);
            else _Parameters.Add(par);

            captured.Add(par);
            return par;
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
                ;
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