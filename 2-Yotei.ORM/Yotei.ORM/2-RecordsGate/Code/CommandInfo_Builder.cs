using System.Runtime.Intrinsics.X86;

namespace Yotei.ORM.Records.Code;

partial class CommandInfo
{
    // ====================================================
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
            _Text = new();
            _Parameters = new(engine);
        }

        /// <summary>
        /// Initializes a new instance using the contents of the given source, using its default
        /// iterable mode.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommand source)
        {
            ArgumentNullException.ThrowIfNull(source);

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
        public IEngine Engine => _Parameters.Engine;

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

        // ------------------------------------------------

        /// <summary>
        /// Appends to this instance the given text, if not null, and the collection of parameters
        /// obtained from the given range of values. In strict mode, there must be a correspondence
        /// among the given parameters and their representation in the given text.
        /// </summary>
        bool Append(bool strict, string text, params object?[]? range)
        {
            ArgumentNullException.ThrowIfNull(text);
            range ??= [null];

            // Capturing range of values...
            var items = RangeElement.Capture(range);
            var changed = false;
            int pos, index, value;
            string? bracket;

            // Validations...
            PreventInvalidRangeValues();
            PreventInitialDuplicatedNames();
            if (strict) PreventInvalidOrdinals();

            // Intercepting ranges with just one special element...
            if (items.Length == 1)
            {
                switch (items[0].Value)
                {
                    case IParameterList pars:
                        if (pars.Count == 1) { items[0] = new RangeElement(pars[0]); break; }
                        //text = NamesToOrdinals(text, pars, Comparison);
                        range = [.. pars];
                        return Append(strict, text, range);

                    case IParameterList.IBuilder pars:
                        if (pars.Count == 1) { items[0] = new RangeElement(pars[0]); break; }
                        //text = NamesToOrdinals(text, pars, Comparison);
                        range = [.. pars];
                        return Append(strict, text, range);
                }
            }

            // Iterating to capture parameters, and to change brackets into sequences...
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                IParameter par;
                string name;

                // Capturing a parameter from the value...
                switch (item.Value)
                {
                    case IParameter temp:
                        par = Capture(temp, changeDuplicatedName: true);
                        name = temp.Name;
                        break;

                    case AnonymousElement temp:
                        par = new Parameter(temp.Name, temp.Value);
                        par = Capture(par, changeDuplicatedName: true);
                        name = temp.Name;
                        break;

                    default:
                        _Parameters.AddNew(item.Value, out par);
                        name = par.Name;
                        break;
                }

                // Not strict mode...
                if (!strict) item.Used = true;

                // Finding by named bracket ('name' may not be 'par.Name')...
                pos = 0;
                while ((pos = FindNamedBracket(text, pos, name, out bracket, strict: false)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);
                    pos += par.Name.Length;
                    item.Used = true;
                }

                // Finding by ordinal bracket...
                pos = 0;
                while ((index = FindOrdinalBracket(text, pos, out bracket, out value)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);
                    pos += par.Name.Length;
                    item.Used = true;
                }
            }

            // Finishing...
            if (strict)
            {
                ValidateNoUnusedValues();
                ValidateNoDanglingBrackets();
            }

            if (text.Length > 0) { _Text.Append(text); changed = true; }
            if (items.Any(x => x.Used)) changed = true;
            return changed;

            // --------------------------------------------

            /// <summary>
            /// Prevents receiving a range with invalid values.
            /// </summary>
            void PreventInvalidRangeValues()
            {
                foreach (var item in items)
                {
                    switch (item.Value)
                    {
                        case ICommand:
                        case ICommandInfo:
                        case ICommandInfo.IBuilder:
                            throw new ArgumentException("Element cannot be a command-alike one.")
                            .WithData(item.Value);
                    }
                }
            }

            /// <summary>
            /// Prevents receiving a range with duplicated names.
            /// </summary>
            void PreventInitialDuplicatedNames()
            {
                List<string> names = [];
                foreach (var item in items)
                {
                    // Only parameters and anonymous carry names...
                    var name = item.Value switch
                    {
                        IParameter temp => temp.Name,
                        AnonymousElement temp => temp.Name,
                        _ => null
                    };
                    if (name is null) continue;

                    // Normalizing (and maybe re-storing, no harm in this)...
                    if (!name.StartsWith(Prefix, Comparison))
                    {
                        name = Prefix + name;
                        switch (item.Value)
                        {
                            case IParameter temp: item.Value = new Parameter(name, temp.Value); break;
                            case AnonymousElement temp: item.Value = new Parameter(name, temp.Value); break;
                        }
                    }

                    // Intercepting...
                    if (names.Any(x => string.Compare(x, name, Comparison) == 0))
                        throw new DuplicateException(
                            "Range of given values contains duplicated names.").WithData(items);

                    names.Add(name);
                }
            }

            /// <summary>
            /// Prevents invalid ordinals to appear in the given text.
            /// </summary>
            void PreventInvalidOrdinals()
            {
                pos = 0;
                while ((index = FindOrdinalBracket(text, pos, out bracket, out value)) >= 0)
                {
                    if (value >= items.Length) throw new ArgumentException(
                            "Ordinal bracket value bigger than the number of values.")
                            .WithData(value)
                            .WithData(items);

                    pos = index + bracket!.Length;
                }
            }

            /// <summary>
            /// Validates that there are no remaining unused values.
            /// </summary>
            void ValidateNoUnusedValues()
            {
                if (items.Any(x => !x.Used)) throw new ArgumentException(
                    "There are unused values in the given range.")
                    .WithData(text)
                    .WithData(range);
            }

            /// <summary>
            /// Validates that there are no remaining dangling '{...}' brackets.
            /// </summary>
            void ValidateNoDanglingBrackets()
            {
                if (FindBracket(text, 0, out _) >= 0) throw new ArgumentException(
                    "There are unused '{...}' brackets in the given text.")
                    .WithData(text)
                    .WithData(range);
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Replaces the named '{name}' specifications in the given text with their corresponding
        /// ordinal '{n}' ones, where 'n' is the ordinal of the named specification in the given
        /// collection of parameters.
        /// </summary>
        /*static string NamesToOrdinals(
            string text, IEnumerable<IParameter> pars, StringComparison comparison)
        {
            text.ThrowWhenNull();
            pars.ThrowWhenNull();

            var finder = new IsolatedFinder();
            var i = 0;
            foreach (var par in pars)
            {
                var bracket = $"{{{i}}}"; i++;
                var pos = 0;

                while ((pos = finder.Find(text, pos, par.Name, comparison)) >= 0)
                {
                    text = text.Remove(pos, par.Name.Length);
                    text = text.Insert(pos, bracket);
                    pos += bracket.Length;
                }
            }
            return text;
        }*/

        // ------------------------------------------------

        /// <summary>
        /// Validates that the name of the given parameter starts with the engine's prefix, and
        /// if not, return a new instance that starts with it.
        /// </summary>
        IParameter WithValidatedName(IParameter par)
        {
            return par.Name.StartsWith(Prefix, Comparison)
                ? par
                : par.WithName(Prefix + par.Name);
        }

        /// <summary>
        /// Captures the given parameter into this instance. If its name already exists in this
        /// instance, either it is changed to a new one to prevent collisions, if requested, or
        /// an exception is thrown.
        /// </summary>
        IParameter Capture(IParameter par, bool changeDuplicatedName)
        {
            par = WithValidatedName(par);

            if (_Parameters.Contains(par.Name))
            {
                if (changeDuplicatedName) _Parameters.AddNew(par.Value, out par);
                else throw new DuplicateException(
                    "Name of parameter is a duplicated.")
                    .WithData(par)
                    .WithData(this);
            }
            else
            {
                _Parameters.Add(par);
            }

            return par;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index of the first '{...}' bracket in the given source text, starting
        /// from the given initial index, or -1 if any is found. If found, returns the bracket
        /// in the out argument, even if it is an empty one (only the '{}' bracket).
        /// </summary>
        int FindBracket(string text, int ini, out string? bracket)
        {
            bracket = null;

            var pos = text.IndexOf('{', ini); if (pos < 0) return -1;
            var end = text.IndexOf('}', pos); if (end < 0) return -1;

            bracket = text.Substring(pos, end - pos + 1);
            return pos;
        }

        /// <summary>
        /// Returns the index of the next ordinal bracket in the given source text, starting
        /// at the given initial index, or -1 if any is found. If found, then the bracket and
        /// its ordinal value are returned in the out arguments.
        /// </summary>
        int FindOrdinalBracket(string text, int ini, out string? bracket, out int value)
        {
            bracket = null;
            value = 0;

            int pos = ini;
            while ((pos = FindBracket(text, pos, out bracket)) >= 0)
            {
                var span = bracket.AsSpan(1, bracket!.Length - 2);
                var valid = true;
                foreach (var c in span) if (!char.IsAsciiDigit(c)) { valid = false; break; }

                if (valid && int.TryParse(span, out value)) return pos;
                pos += bracket.Length;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the next ocurrence of an ordinal '{value} bracket in the given
        /// source text, starting at the given initial index, or -1 if any is found.
        /// </summary>
        /*int FindOrdinalBracket(string text, int ini, int value)
        {
            if (ini < text.Length)
            {
                var bracket = $"{{{value}}}";

                var pos = text.IndexOf(bracket, ini);
                if (pos >= 0) return pos;
            }
            return -1;
        }*/

        /// <summary>
        /// Returns the index of the next named bracket in the given source text, starting at
        /// the given initial index, or -1 if any is found. If found, the bracket is returned
        /// in the out argument.
        /// </summary>
        /*int FindNamedBracket(string text, int ini, out string? bracket)
        {
            bracket = null;

            int pos = ini;
            while ((pos = FindBracket(text, pos, out bracket)) >= 0)
            {
                var span = bracket.AsSpan(1, bracket!.Length - 2);
                var valid = false;
                foreach (var c in span) if (!char.IsAsciiDigit(c)) { valid = true; break; }

                if (valid) return pos;
                pos += bracket.Length;
            }

            return -1;
        }*/

        /// <summary>
        /// Returns the index of the next ocurrence of a named '{name} bracket in the given source
        /// text, starting at the given initial index, or -1 if any is found. If found, the bracket
        /// is returned in the out argument.
        /// <br/> If strict mode is requested, then the name must match the bracket's contents.
        /// Otherwise, such contents are tested with and without the engine's prefix.
        /// </summary>
        int FindNamedBracket(string text, int ini, string name, out string? bracket, bool strict)
        {
            if (ini < text.Length)
            {
                bracket = $"{{{name}}}";
                var pos = text.IndexOf(bracket, ini, Comparison);
                if (pos >= 0) return pos;

                if (!strict)
                {
                    if (!name.StartsWith(Prefix, Comparison)) // Add the engine's prefix
                    {
                        bracket = $"{{{Prefix + name}}}";
                        pos = text.IndexOf(bracket, ini, Comparison);
                        if (pos >= 0) return pos;
                    }
                    else // Remove the engine's prefix...
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
            }

            bracket = null;
            return -1;
        }

        /// <summary>
        /// Returns the index of the first ocurrence of a named sequence, as in '#...' (where '#'
        /// is the engine prefix), in the given source text starting from the given initial index,
        /// or -1 if any. If found, returns that sequence in the out argument, even if it is an
        /// empty one (only the engine's prefix).
        /// </summary>
        /*int FindNameSequence(string text, int ini, out string? sequence)
        {
            sequence = null;
            var comparer = char.CharComparer(Comparison);

            var pos = text.IndexOf(Prefix, ini, Comparison);

            if (pos < 0) return -1;
            if (pos > 0)
            {
                var c = text[pos - 1];
                if (!IsolatedFinder.SEPARATORS.Contains(c, comparer)) return -1;
            }

            var span = text.AsSpan(pos + Prefix.Length);
            var end = span.IndexOfAny(IsolatedFinder.SEPARATORS, Comparison);

            if (end >= 0) // We found an embedded sequence...
            {
                sequence = text.Substring(pos, end + Prefix.Length);
                return pos;
            }
            else // We've reached the end...
            {
                sequence = text[pos..];
                return pos;
            }
        }*/

        // ------------------------------------------------

        /// <summary>
        /// Represents an arbitrary element in the range of values.
        /// </summary>
        class RangeElement(object? value, bool used = false)
        {
            public object? Value = value;
            public bool Used = used;
            public override string ToString()
            {
                var str = Value switch
                {
                    IParameter item => $"Parameter: {item}",
                    AnonymousElement item => $"Anonymous: {item}",
                    _ => $"Value: {Value.Sketch()}",
                };
                if (Used) str += " (Used)";
                return str;
            }

            /// <summary>
            /// Captures the given range of values into a collection of normalized elements.
            /// </summary>
            public static RangeElement[] Capture(object?[]? range)
            {
                range ??= [null];
                if (range.Length == 0) return [];

                var items = new RangeElement[range.Length];
                for (int i = 0; i < range.Length; i++)
                {
                    var temp = AnonymousElement.TryCapture(range[i]);
                    items[i] = new(temp, false);
                }
                return items;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Represents an anonymous element in the range of values, as in '{ Name = Value }'
        /// </summary>
        class AnonymousElement(string name, object? value)
        {
            public string Name = name;
            public object? Value = value;
            public override string ToString() => $"{Name}:'{Value}'";

            /// <summary>
            /// Tries to capture the given value into an anonymous item. If not possible, returns
            /// the original value itself.
            /// </summary>
            public static object? TryCapture(object? value)
            {
                if (value is not null)
                {
                    var type = value.GetType();
                    if (type.IsAnonymous)
                    {
                        var members = type.GetProperties();
                        if (members.Length == 0) throw new ArgumentException("Anonymous element with no properties.").WithData(value);
                        if (members.Length > 1) throw new ArgumentException("Anonymous element with too many properties.").WithData(value);

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
