namespace Yotei.ORM.Code;

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
    /// Initializes a new instance with the given text.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    public CommandInfo(IEngine engine, string text)
    {
        Text = text.ThrowWhenNull();
        Parameters = new ParameterList(engine);
    }

    /// <summary>
    /// Initializes a new instance with the given list of parameters.
    /// </summary>
    /// <param name="range"></param>
    public CommandInfo(IParameterList range)
        : this(range.ThrowWhenNull().Engine)
        => AddInternal(range);

    /// <summary>
    /// Initializes a new instance using the given text and list of parameters.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="range"></param>
    public CommandInfo(string text, IParameterList range)
    {
        Text = string.Empty;
        Parameters = range.ThrowWhenNull().Clear();

        AddInternal(text, range);
    }

    /// <summary>
    /// Initializes a new instance with the given text and optional collection of arguments.
    /// If used, the arguments must be encoded using a '{n}' positional specification in the
    /// text, where 'n' refers to the index in that array of arguments. The specification is
    /// then changed based upon the concrete argument type:
    /// <br/>- regular parameter (its name is used),
    /// <br/>- anonymous type (the name of its unique property is used),
    /// <br/>- any other value (the next available parameter name).
    /// <br/> In addion, text cannot be null, and unused optional arguments are not allowed.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="args"></param>
    public CommandInfo(IEngine engine, string text, params object?[] args)
    {
        Text = string.Empty;
        Parameters = new ParameterList(engine);

        AddInternal(text, args);
    }

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
        : Text + $"; -- [{string.Join(", ", Parameters)}]";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Parameters.Engine;

    /// <inheritdoc/>
    public string Text { get; private set; }

    /// <inheritdoc/>
    public IParameterList Parameters { get; private set; }

    /// <inheritdoc/>
    public bool IsEmpty => Text.Length == 0 && Parameters.Count == 0;

    StringComparison Comparison => Engine.CaseSensitiveNames
        ? StringComparison.Ordinal
        : StringComparison.OrdinalIgnoreCase;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo Add(string text)
    {
        var clone = Clone();
        var done = clone.AddInternal(text);
        return done ? clone : this;
    }
    bool AddInternal(string text)
    {
        text.ThrowWhenNull();

        if (text.Length == 0) return false;

        Text += text;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo Add(IEnumerable<IParameter> range)
    {
        var clone = Clone();
        var done = clone.AddInternal(range);
        return done ? clone : this;
    }
    bool AddInternal(IEnumerable<IParameter> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<IParameter> trange && trange.Count == 0) return false;
        if (range is ICollection irange && irange.Count == 0) return false;

        if (range is IParameterList items && this.Parameters.Count == 0)
        {
            Parameters = items;
            return true;
        }

        Parameters = Parameters.AddRange(range);
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo Add(ICommandInfo source)
    {
        var clone = Clone();
        var done = clone.AddInternal(source);
        return done ? clone : this;
    }
    bool AddInternal(ICommandInfo source)
    {
        source.ThrowWhenNull();

        if (source.IsEmpty) return false;

        var same = string.Compare(Engine.ParameterPrefix, source.Engine.ParameterPrefix, Comparison) == 0;
        var text = source.Text;
        for (int i = 0; i < source.Parameters.Count; i++)
        {
            var old = source.Parameters[i];
            if (!same)
            {
                var index = old.Name.IndexOf(source.Engine.ParameterPrefix, source.Engine.CaseSensitiveNames);
                if (index >= 0)
                {
                    var name = old.Name.Remove(0, source.Engine.ParameterPrefix.Length);
                    old = new Parameter(name, old.Value);
                }
            }

            if (Parameters.Contains(old.Name))
            {
                Parameters = Parameters.AddNew(old.Value, out var par);
                text = ChangeItem(text, old.Name, par.Name);
            }
            else Parameters = Parameters.Add(old);
        }

        Text += text;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo Add(string text, IParameterList range)
    {
        var clone = Clone();
        var done = clone.AddInternal(text, range);
        return done ? clone : this;
    }
    bool AddInternal(string text, IParameterList range)
    {
        text.ThrowWhenNull();
        range.ThrowWhenNull();

        return AddInternal(text, range.ToArray());
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo Add(string text, params object?[] args)
    {
        var clone = Clone();
        var done = clone.AddInternal(text, args);
        return done ? clone : this;
    }

    [SuppressMessage("", "CA2249")]
    bool AddInternal(string text, params object?[] args)
    {
        text.ThrowWhenNull();

        args ??= [null];
        if (text.Length == 0 && args.Length == 0) return false;

        // Going through all '{...}' sequences...
        var pars = new IParameter?[args.Length];
        var start = 0;
        while (start < text.Length)
        {
            if (!FindNext(text, ref start, out var item)) break;

            var vitem = item[1..^1].NullWhenEmpty()
                ?? throw new ArgumentException("Empty or blank '{...}' sequence found.")
                .WithData(item)
                .WithData(text);

            var index = int.Parse(vitem);
            var arg = args[index];

            // Case: arg is a regular parameter...
            if (arg is IParameter par)
            {
                Parameters = Parameters.Add(par);
                pars[index] = par;

                text = ChangeItem(text, item, par.Name);
                continue;
            }

            // Case: arg is an anonymous type...
            var type = arg?.GetType();
            if (type != null && type.IsAnonymous())
            {
                var members = type.GetProperties();
                if (members.Length == 0) throw new ArgumentException("No properties in anonymous type.").WithData(arg);
                if (members.Length > 1) throw new ArgumentException("Too many properties in anonymous type.").WithData(arg);

                var member = members[0];
                var value = member.GetValue(arg);

                var pos = member.Name.IndexOf(Engine.ParameterPrefix, Comparison);
                var name = pos >= 0 ? member.Name : (Engine.ParameterPrefix + member.Name);

                Parameters = Parameters.Add(par = new Parameter(name, value));
                pars[index] = par;

                text = ChangeItem(text, item, par.Name);
                continue;
            }

            // Default case: arg is an arbitrary value...
            else
            {
                Parameters = Parameters.AddNew(arg, out par);
                pars[index] = par;

                text = ChangeItem(text, item, par.Name);
                continue;
            }
        }

        // Have we used all arguments?...
        if (pars.Any(x => x == null)) throw new ArgumentException(
            "Not all optional arguments are found in the given text.")
            .WithData(text)
            .WithData(args);

        // Finishing...
        Text += text;
        return true;
    }

    // Finds the next '{...}' sequence...
    static bool FindNext(string text, ref int start, out string item)
    {
        item = null!;

        var ini = text.IndexOf('{', start);
        if (ini < 0) return false;
        if (ini == (text.Length - 1)) return false;

        var end = text.IndexOf('}', ini + 1);
        var len = end - ini + 1;

        start = end + 1;
        item = text.Substring(ini, len);
        return true;
    }

    // Changes all the ocurrences of the old item to the new one...
    string ChangeItem(string text, string olditem, string newitem)
    {
        var found = false;
        var start = 0;
        while (start < text.Length)
        {
            var pos = text.IndexOf(olditem, start, Comparison);
            if (pos < 0)
            {
                if (!found) throw new ArgumentException(
                    $"Item '{olditem}' not found in the given text.")
                    .WithData(text);

                break;
            }

            found = true;
            text = text.Remove(pos, olditem.Length);
            text = text.Insert(pos, newitem);
            start = pos + newitem.Length;
        }
        return text;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ICommandInfo Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done ? clone : this;
    }
    bool ClearInternal()
    {
        if (IsEmpty) return false;

        Text = string.Empty;
        Parameters = Parameters.Clear();
        return true;
    }
}