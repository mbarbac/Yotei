using System.ComponentModel.DataAnnotations;
using StrSpan = System.ReadOnlySpan<char>;
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
        /// <param name="iterable"></param>
        public Builder(ICommand source, bool iterable)
            : this(source.ThrowWhenNull().GetCommandInfo(iterable)) { }

        /// <summary>
        /// Initializes a new instance using the contents from the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo source)
        {
            Connection = source.ThrowWhenNull().Connection;
            _Text = new(source.Text);
            _Parameters = new(Engine, source.Parameters);
        }

        /// <summary>
        /// Initializes a new instance using the contents from the given source.
        /// </summary>
        /// <param name="source"></param>
        public Builder(ICommandInfo.IBuilder source)
        {
            Connection = source.ThrowWhenNull().Connection;
            _Text = new(source.Text);
            _Parameters = new(Engine, source.Parameters);
        }

        /// <summary>
        /// Initializes a new instance with the given text and the collection of parameters
        /// obtained from the given range of values, if any.
        /// <br/> If both text and values are used, then the later shall be encoded in the text
        /// using either a positional '{n}' or a named '{name}' specification (where 'name' may
        /// or may not start with the engine prefix).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// We call 'Add()' to accept its limitations (no unused values, no dangling brackets...).
        public Builder(IConnection connection, string? text, params object?[]? range)
            : this(connection)
            => Add(text, range);

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
        string Prefix => Engine.ParameterPrefix;
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
                var text = Text;
                var finder = new IsolatedFinder();
                int count, pos;
                bool found;

                // Shortcut for empty instances...
                if (IsEmpty) return true;

                // No dangling '{...}' brackets...
                if (AreRemainingBrackets(text)) return false;

                // No unused parameters, removing names of existing for simplicity...
                count = 0;
                foreach (var par in _Parameters)
                {
                    found = false;
                    pos = 0;
                    while ((pos = finder.Find(text, pos, par.Name, Comparison)) >= 0)
                    {
                        text = text.Remove(pos, par.Name.Length);
                        found = true;
                    }
                    if (found) count++;
                }
                if (count != _Parameters.Count) return false;

                // No dangling '#...' sequences, '#' being the parameter prefix...
                pos = 0;
                while ((pos = text.IndexOf(Prefix, pos, Comparison)) >= 0)
                {
                    if (pos == 0) return false;
                    if (IsolatedFinder.SEPARATORS.Contains(text[pos - 1])) return false;
                    pos += Prefix.Length;
                }

                // Finishing without impediments...
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
            return Add(source.ThrowWhenNull().GetCommandInfo(iterable));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommandInfo source)
        {
            source.ThrowWhenNull();

            if (source.IsEmpty) return false;

            var strictNames = false;
            var strictValues = false;
            return Append(strictNames, strictValues, source.Text, source.Parameters);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool Add(ICommandInfo.IBuilder source)
        {
            source.ThrowWhenNull();

            if (source.IsEmpty) return false;

            var strictNames = false;
            var strictValues = false;
            return Append(strictNames, strictValues, source.Text, source.Parameters);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool Add(string? text, params object?[]? range)
        {
            var strictNames = true;
            var strictValues = text is not null; // Null => only capture values...

            return Append(strictNames, strictValues, text, range);
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool ReplaceText(string? text)
        {
            text ??= string.Empty;

            if (text.Length == 0 && TextLen == 0) return false;
            if (string.Compare(text, Text, Comparison) == 0) return false;

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

            // Shortcut when the given range is empty...
            if (range.Length == 0)
            {
                if (_Parameters.Count == 0) return false;

                _Parameters.Clear();
                return true;
            }

            // Standard case...
            var old = _Parameters.Clone();
            var strictNames = false;
            var strictValues = false;
            var changed = Append(strictNames, strictValues, null, range);

            if (changed) { for (int i = 0; i < old.Count; i++) _Parameters.RemoveAt(0); }
            else { _Parameters.Clear(); _Parameters.AddRange(old); }
            return changed;
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
        /// Appends to this instance the given text, and the collection of parameters obtained
        /// from the given range of values.
        /// <br/> If text is null, then the range of values is captured without validanting that
        /// their names are encoded in the text. Otherwise, if both text and values are used, then
        /// they must be encoded in the text using either a positional '{n}' or a named '{name}'
        /// specification, where 'name' may or may not start with the engine prefix.
        /// <br/> If a 'name' already exits, then it is changed in both the text and parameter
        /// names to prevent colisions (so enabling using positional '{0}' in the input text).
        /// <br/> This method may render this instance in an inconsitent state, where there may be
        /// unused parameters, dangling '{...}' bracket specifications, or dangling '#...' name
        /// ones ('#' stands for the engine prefix). This is acceptable while this instance is
        /// being constructed. When obtaining <see cref="CommandInfo"/> instances, the constructor
        /// will throw if inconsistencies are detected.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        bool Append(
            bool strictNames,
            bool strictValues,
            string? text, params object?[]? range)
        {
            bool textnull = text is null;
            text ??= string.Empty;
            range ??= [null];

            var items = RangeElement.Capture(range);
            var changed = false;
            var pos = 0;

            /// <summary>
            /// Intercepting ranges with just one special element...
            /// </summary>
            if (items.Length == 1)
            {
                switch (items[0].Value)
                {
                    case IParameterList parsList:
                        text = NamesToOrdinals(text, parsList, Comparison);
                        range = [.. parsList];
                        return Append(strictNames, strictValues, text, range);

                    case IParameterList.IBuilder parsBuilder:
                        text = NamesToOrdinals(text, parsBuilder, Comparison);
                        range = [.. parsBuilder];
                        return Append(strictNames, strictValues, text, range);
                }
            }

            /// <summary>
            /// Command-alike parameters are not allowed...
            /// </summary>
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

            /// <summary>
            /// Before using the range, validate there are no duplicate names in it...
            /// </summary>
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

                // Normalizing (and maybe re-storing) the names...
                if (!name.StartsWith(Prefix, Comparison))
                {
                    name = Prefix + name;
                    switch (item.Value)
                    {
                        case IParameter temp: item.Value = new Parameter(name, temp.Value); break;
                        case AnonymousElement temp: item.Value = new Parameter(name, temp.Value); break;
                    }
                }

                // Intercepting duplicates...
                if (names.Any(x => string.Compare(x, name, Comparison) == 0))
                    throw new DuplicateException(
                        "Range of given values contains duplicated names.").WithData(items);

                names.Add(name);
            }

            /// <summary>
            /// Before using the text, verify that ordinal brackets '{n}' are valid ones..
            /// </summary>
            if (text.Length > 0 && strictNames)
            {
                pos = 0;
                while (NextOrdinal(pos, out var span))
                {
                    if (int.TryParse(span, out var value))
                    {
                        if (value >= items.Length) throw new ArgumentException(
                            "Ordinal bracket value bigger than number of values.")
                            .WithData(text);
                    }
                    pos += span.Length + 2 + 1;
                }

                // Gets the next ordinal span from the ini position, if any. Empty brackets are
                // ignored and not reported.
                bool NextOrdinal(int pos, out StrSpan span)
                {
                    while (pos < text.Length)
                    {
                        var ini = text.IndexOf('{', pos); if (ini < 0) break;
                        var end = text.IndexOf('}', ini); if (end < 0) break;
                        span = text.AsSpan(ini + 1, end - ini - 1);

                        if (span.Length == 0) { pos = ini + 1; continue; }
                        return true;
                    }
                    span = StrSpan.Empty;
                    return false;
                }
            }

            /// <summary>
            /// Before using the text, validate there are no '#...' specs in it (#: prefix), as far
            /// as they are not protected in a bracket '{#...}' which is perfectly acceptable...
            /// </summary>
            if (text.Length > 0 && strictNames)
            {
                pos = 0;
                while ((pos = NextPrefix(pos)) >= 0)
                {
                    if (pos == 0 ||
                        IsolatedFinder.SEPARATORS.Contains(text[pos - 1]))
                        throw new ArgumentException(
                            "Given text contains dangling name specifications.")
                            .WithData(text);

                    pos += Prefix.Length;
                }

                // Gets the index of the next prefix from the ini position, that is not protected
                // between brackets ('{#...}' is absolutely acceptable)...
                int NextPrefix(int ini)
                {
                    var inside = 0;
                    for (int i = ini; i < text.Length; i++)
                    {
                        char c = text[i];
                        if (c == '{') { inside++; continue; }
                        if (c == '}') { inside--; if (inside < 0) inside = 0; continue; }

                        if (inside > 0) continue;
                        var span = text.AsSpan(i);
                        if (span.StartsWith(Prefix, Comparison)) return i;
                    }
                    return -1;
                }
            }

            /// <summary>
            /// Iterating though the range of values...
            /// </summary>
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                IParameter par;
                string name;

                // Capturing a parameter from the value...
                switch (item.Value)
                {
                    case IParameter temp:
                        par = Capture(temp);
                        name = temp.Name;
                        break;

                    case AnonymousElement temp:
                        par = new Parameter(temp.Name, temp.Value);
                        par = Capture(par);
                        name = temp.Name;
                        break;

                    default:
                        _Parameters.AddNew(item.Value, out par);
                        name = par.Name;
                        break;
                }

                // If text is null, then only capture values...
                if (textnull) { item.Used = true; continue; }

                // Finding by named bracket (name may not be par.Name)...
                pos = 0;
                while ((pos = FindNamedBracket(text, name, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);
                    pos += par.Name.Length;
                    item.Used = true;
                }

                // Finding by ordinal bracket...
                pos = 0;
                while ((pos = FindOrdinalBracket(text, i, pos, out var bracket)) >= 0)
                {
                    text = text.Remove(pos, bracket!.Length);
                    text = text.Insert(pos, par.Name);
                    pos += par.Name.Length;
                    item.Used = true;
                }
            }

            /// <summary>
            /// No remaining specs....
            /// </summary>
            if (strictNames &&
                text.Length > 0 &&
                AreRemainingBrackets(text))
            {
                throw new ArgumentException(
                    "There are unused brackets in the given text.")
                    .WithData(text);
            }

            /// <summary>
            /// No unused values...
            /// </summary>
            if (strictValues &&
                !textnull &&
                items.Length > 0 &&
                items.Any(static x => !x.Used))
            {
                throw new ArgumentException(
                    "There are unused values in the given range.")
                    .WithData(text);
            }

            /// <summary>
            /// Finishing...
            /// </summary>
            if (text.Length > 0) { _Text.Append(text); changed = true; }
            if (items.Length > 0) changed = true;
            return changed;
        }

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the named '{name}' specifications of the given parameters in the given text
        /// with their ordinal '{n}' ones.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pars"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        /// This method needs to be public as it is used in other places.
        public static string NamesToOrdinals(
            string text,
            IEnumerable<IParameter> pars,
            StringComparison comparison)
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
        }

        /// <summary>
        /// Returns the index of the first ocurrence of an ordinal '{n}' bracket in the given
        /// text, starting at the given position, or -1 if the bracket was not found. The out
        /// argument contains the found bracket, if any.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="ini"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
        static int FindOrdinalBracket(string text, int value, int ini, out string? bracket)
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
        /// Returns the index of the first ocurrence of an ordinal '{name}' bracket in the given
        /// text, starting at the given position, or -1 if the bracket was not found. The 'name'
        /// literal is tested with and without the engine prefix, if needed. The out argument
        /// contains the found bracket, if any.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <param name="ini"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
        [SuppressMessage("", "IDE0057")]
        int FindNamedBracket(string text, string name, int ini, out string? bracket)
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
        /// Determines if the given text has any dangling '{...}' braket specification.
        /// </summary>
        static bool AreRemainingBrackets(string text)
        {
            if (text.Length == 0) return false;

            var ini = text.IndexOf('{'); if (ini < 0) return false;
            var end = text.IndexOf('}', ini); if (end < 0) return false;
            return true;
        }

        // ----------------------------------------------------

        /// <summary>
        /// Validates that the name of the given parameter starts with the engine's prefix, and
        /// if not, creates a new instance that complies.
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        IParameter ValidateName(IParameter par)
        {
            return par.Name.StartsWith(Prefix, Comparison)
                ? par
                : par.WithName(Prefix + par.Name);
        }

        /// <summary>
        /// Captures the given parameter into both this instance. Returns the captured parameter,
        /// which can either be the original one, or a new one created to prevent name duplicates
        /// with the *existing* ones.
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        IParameter Capture(IParameter par)
        {
            par = ValidateName(par);

            if (_Parameters.Contains(par.Name)) _Parameters.AddNew(par.Value, out par);
            else _Parameters.Add(par);

            return par;
        }

        // ----------------------------------------------------

        /// <summary>
        /// Represents an arbitrary element in a given range of values.
        /// </summary>
        class RangeElement(object? value, bool used)
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
            /// <param name="range"></param>
            /// <returns></returns>
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

        // ----------------------------------------------------

        /// <summary>
        /// Represents an anonymous element in a range of values, as in 'new {Name = Value}'.
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