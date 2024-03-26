using ParameterList = Yotei.ORM.Code.ParameterList;
using Parameter = Yotei.ORM.Code.Parameter;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ICommandInfo"/>
[Cloneable]
public sealed partial class CommandInfo : ICommandInfo
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine)
    {
        Text = string.Empty;
        Parameters = new ParameterList(engine);
    }

    /// <summary>
    /// Initializes a new instance with the given text and optional arguments.
    /// <br/> If the text is null, then it is ignored.
    /// <br/> If the optional list of arguments is used, then their associated names or positions
    /// must be referenced in the given text, and all arguments must be used. Each can be:
    /// <br/> - a raw value, that appears in the text agains a '{n}' positional specification. A
    /// new parameter is added using a name generated automatically.
    /// <br/> - a regular parameter, that appears in the text either by its '{name}' or by its
    /// '{n}' positional specification. If that name is a duplicated one, then an exception is
    /// thrown.
    /// <br/> - an anonymous type, that appears in the text either by the '{name}' of its unique
    /// property, or by its '{n}' positional specification. If that name does not start by the
    /// engine's parameter prefix, then it is added automatically.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfo(IEngine engine, string? text, params object?[] range)
        : this(engine)
        => AddInternal(text, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    CommandInfo(CommandInfo source)
    {
        Text = source.Text;
        Parameters = source.Parameters;
    }

    /// <inheritdoc/>
    public override string ToString() => Parameters.Count == 0
        ? Text
        : Text + $" -- [{string.Join(", ", Parameters)}]";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string Text { get; private set; }

    /// <inheritdoc/>
    public IParameterList Parameters { get; private set; }

    /// <inheritdoc/>
    public bool IsEmpty => Text.Length == 0 && Parameters.Count == 0;

    IEngine Engine => (IEngine)Parameters.Engine;

    StringComparison Comparison => Engine.CaseSensitiveNames
        ? StringComparison.Ordinal
        : StringComparison.OrdinalIgnoreCase;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo AddText(string? text)
    {
        if (text is null || text.Length == 0) return this;

        var clone = Clone(); clone.Text += text;
        return clone;
    }

    /// <inheritdoc/>
    public ICommandInfo AddParameters(params IParameter[] range)
    {
        range.ThrowWhenNull();

        if (range.Length == 0) return this;

        var clone = Clone(); clone.Parameters = clone.Parameters.AddRange(range);
        return clone;
    }

    /// <inheritdoc/>
    public ICommandInfo Clear()
    {
        if (IsEmpty) return this;

        var clone = Clone();
        clone.Text = string.Empty;
        clone.Parameters = new ParameterList(Engine);
        return clone;
    }

    // ----------------------------------------------------

    private struct AnonymousItem
    {
        public string Name;
        public object? Value;
    }

    /// <inheritdoc/>
    public ICommandInfo Add(string? text, params object?[] range)
    {
        var clone = Clone();
        var done = clone.AddInternal(text, range);
        return done ? clone : this;
    }
    bool AddInternal(string? text, params object?[] range)
    {
        // We may have a first element whose value is null...
        range ??= [null];

        // If no text, we cannot have arguments...
        if (text is null || text.Length == 0)
        {
            if (range.Length > 0) throw new ArgumentException(
                "Arguments with no match in null or empty text.")
                 .WithData(text)
                 .WithData(range);

            return false;
        }

        // Finding anonymous arguments...
        for (int i = 0; i < range.Length; i++)
        {
            var arg = range[i];
            var type = arg?.GetType();
            if (type != null && type.IsAnonymous())
            {
                var members = type.GetProperties();
                if (members.Length == 0) throw new ArgumentException("No property found in anonymous argument.").WithData(arg);
                if (members.Length > 1) throw new ArgumentException("Too many propertis found in anonymous argument.").WithData(arg);

                var member = members[0];
                var value = member.GetValue(arg);
                var name = member.Name;

                range[i] = new AnonymousItem() { Name = name, Value = value };
            }
        }

        // Going through all '{...}' sequences...
        var used = new bool[range.Length];
        var ini = 0;

        while (ini < text.Length)
        {
            if (!FindNext(text, ref ini, out var terminated)) break;

            var inner = terminated[1..^1].NullWhenEmpty() ??
                throw new ArgumentException("Empty or blank '{...}' sequence found.").WithData(text);

            // Case: sequence is '{n}'...
            if (int.TryParse(inner, out var index))
            {
                if (index < 0) throw new IndexOutOfRangeException($"Index '{index}' cannot be negative.");
                if (index >= range.Length) throw new IndexOutOfRangeException($"Index '{index}' is equal or bigger than arguments length.");

                var value = range[index];

                if (value is IParameter par)
                {
                    Parameters = Parameters.Add(par);
                    used[index] = true;
                    text = ReplaceAll(text, terminated, par.Name);
                }
                else if (value is AnonymousItem item)
                {
                    var name = item.Name;
                    if (!name.StartsWith(Engine.ParameterPrefix, Comparison))
                        name = Engine.ParameterPrefix + name;

                    Parameters = Parameters.Add(par = new Parameter(name, item.Value));
                    used[index] = true;
                    text = ReplaceAll(text, terminated, par.Name);
                }
                else
                {
                    Parameters = Parameters.AddNew(value, out par);
                    used[index] = true;
                    text = ReplaceAll(text, terminated, par.Name);
                }
            }

            // Case: sequence is '{name}'...
            else
            {
                for (int i = 0; i < range.Length; i++)
                {
                    var value = range[i];

                    if (value is IParameter par && Compare(inner, ref par))
                    {
                        Parameters = Parameters.Add(par);
                        used[i] = true;
                        text = ReplaceAll(text, terminated, par.Name);
                        break;
                    }
                    else if (value is AnonymousItem item && Compare(inner, ref item))
                    {
                        Parameters = Parameters.Add(par = new Parameter(item.Name, item.Value));
                        used[i] = true;
                        text = ReplaceAll(text, terminated, par.Name);
                        break;
                    }
                }
            }
        }

        // Have we used all arguments?
        if (used.Any(x => x == false)) throw new ArgumentException(
            "Not all optional arguments are found in the given text.")
            .WithData(text)
            .WithData(range);

        // Finishing...
        Text += text;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo Add(ICommandInfo source)
    {
        source.ThrowWhenNull();
        if (source.IsEmpty) return this;

        var clone = Clone();
        var done = clone.AddInternal(source);
        return done ? clone : this;
    }
    bool AddInternal(ICommandInfo source)
    {
        if (source.IsEmpty) return false;

        var oldEngine = source.Parameters.Engine;
        var oldPrefix = oldEngine.ParameterPrefix;
        var oldComparison = oldEngine.CaseSensitiveNames ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        var same = string.Compare(Engine.ParameterPrefix, oldPrefix, Comparison) == 0;
        var text = source.Text;

        for (int i = 0; i < source.Parameters.Count; i++)
        {
            var old = source.Parameters[i];
            if (!same)
            {
                if (old.Name.StartsWith(oldPrefix, oldComparison))
                {
                    var name = old.Name.Remove(0, oldPrefix.Length);
                    if (name.Length == 0) throw new ArgumentException(
                        "Old parameter name is just its prefix.")
                        .WithData(old);

                    name = Engine.ParameterPrefix + name;
                    old = new Parameter(name, old.Value);
                }
            }

            if (Parameters.Contains(old.Name))
            {
                Parameters = Parameters.AddNew(old.Value, out var par);
                text = ReplaceAll(text, old.Name, par.Name);
            }
            else
            {
                Parameters = Parameters.Add(old);
            }
        }

        Text += text;
        return true;
    }

    /// <inheritdoc/>
    public ICommandInfo Add(string? text, ICommandInfo source)
    {
        source.ThrowWhenNull();
        if ((text is null || text.Length == 0) && source.IsEmpty) return this;

        var clone = Clone();
        var done = false;
        if (text is not null) { clone.Text += text; done = true; }

        var temp = clone.AddInternal(source);
        done = done || temp;

        return done ? clone : this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Detremines if the name of the parameter is the given one.
    /// </summary>
    bool Compare(string inner, ref IParameter par)
    {
        var name = par.Name;
        if (string.Compare(inner, name, Comparison) == 0) return true;

        if (!inner.StartsWith(Engine.ParameterPrefix, Comparison))
            inner = Engine.ParameterPrefix + inner;

        if (!name.StartsWith(Engine.ParameterPrefix, Comparison))
            name = Engine.ParameterPrefix + name;

        if (string.Compare(inner, name, Comparison) == 0)
        {
            par = new Parameter(name, par.Value);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if the name of the anonymous item is the given one.
    /// </summary>
    bool Compare(string inner, ref AnonymousItem item)
    {
        var name = item.Name;
        if (!name.StartsWith(Engine.ParameterPrefix))
        {
            name = Engine.ParameterPrefix + name;
            item = new AnonymousItem() { Name = name, Value = item.Value };
        }
        if (string.Compare(inner, name, Comparison) == 0) return true;

        if (!inner.StartsWith(Engine.ParameterPrefix, Comparison))
            inner = Engine.ParameterPrefix + inner;

        if (string.Compare(inner, name, Comparison) == 0) return true;
        return false;
    }

    /// <summary>
    /// Tries to find the next '{...}' sequence in the given text, starting at the given index.
    /// </summary>
    static bool FindNext(string text, ref int start, [NotNullWhen(true)] out string? terminated)
    {
        terminated = null;

        var ini = text.IndexOf('{', start);
        if (ini < 0) return false;
        if (ini == (text.Length - 1)) return false;

        var end = text.IndexOf('}', ini + 1);
        if (end < 0) return false;

        var len = end - ini + 1;
        start = end + 1;
        terminated = text.Substring(ini, len);
        return true;
    }

    /// <summary>
    /// Replaces all the ocurrences of the old string in the given text with the new given string.
    /// </summary>
    string ReplaceAll(string text, string oldstr, string newstr)
    {
        var start = 0;

        while (start < text.Length)
        {
            var pos = text.IndexOf(oldstr, start, Comparison);
            if (pos < 0) break;

            text = text.Remove(pos, oldstr.Length);
            text = text.Insert(pos, newstr);
            start = pos + newstr.Length;
        }

        return text;
    }
}