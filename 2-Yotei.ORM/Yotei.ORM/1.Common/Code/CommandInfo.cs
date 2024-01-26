using System.Data;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICommandInfo"/>
/// </summary>
[Cloneable]
public partial class CommandInfo : ICommandInfo
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
    /// <br/> Text can be null if not used.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    public CommandInfo(IEngine engine, string? text)
    {
        Text = text ?? string.Empty;
        Parameters = new ParameterList(engine);
    }

    /// <summary>
    /// Initializes a new instance with the given text and parameters.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parameters"></param>
    public CommandInfo(string? text, IParameterList parameters)
    {
        Text = text ?? string.Empty;
        Parameters = parameters.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc cref="ICommandInfo.Add(string?, object?[])"/>
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="args"></param>
    public CommandInfo(IEngine engine, string? text, params object?[] args)
        : this(engine)
        => AddInternal(text, args);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source)
    {
        Text = source.Text;
        Parameters = source.Parameters;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Parameters.Count == 0
        ? Text
        : Text + $"; -- [{string.Join(", ", Parameters)}]";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine => Parameters.Engine;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IParameterList Parameters { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual bool IsEmpty => Text.Length == 0 && Parameters.Count == 0;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(string? text)
    {
        if (text is null) return this;

        var clone = Clone(); clone.Text += text;
        return clone;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(IEnumerable<IParameter> range)
    {
        range.ThrowWhenNull();
        if (range is ICollection<IParameter> trange && trange.Count == 0) return this;
        if (range is ICollection irange && irange.Count == 0) return this;

        var clone = Clone(); clone.Parameters = clone.Parameters.AddRange(range);
        return clone;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(string? text, params object?[] args)
    {
        args ??= [null];
        if (text is null && args.Length == 0) return this;

        var clone = Clone(); clone.AddInternal(text, args);
        return clone;
    }
    void AddInternal(string? text, params object?[] args)
    {
        var comp = Engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        // Looping though the arguments...
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            // Case: arg is regular parameter...
            if (arg is IParameter par)
            {
                Parameters = Parameters.Add(par);

                if (text is not null)
                {
                    if (text.Length == 0) throw new ArgumentException(
                        "Parameter name not found in text.")
                        .WithData(par)
                        .WithData(text);

                    var index = text.IndexOf(par.Name, comp);
                    if (index < 0 ) throw new ArgumentException(
                        "Parameter name not found in text.")
                        .WithData(par)
                        .WithData(text);
                }

                continue;
            }

            // Case: arg is anonymous type...
            var type = arg?.GetType();
            if (type != null && type.IsAnonymous())
            {
                var members = type.GetProperties();
                if (members.Length == 0) throw new ArgumentException("No properties in anonymous type.").WithData(arg);
                if (members.Length > 1) throw new ArgumentException("Too many properties in anonymous type.").WithData(arg);

                var member = members[0];
                var index = member.Name.IndexOf(Engine.ParameterPrefix, Engine.CaseSensitiveNames);
                var name = index >= 0 ? member.Name : (Engine.ParameterPrefix + member.Name);

                var value = member.GetValue(arg);
                Parameters = Parameters.Add(par = new Parameter(name, value));

                if (text is not null)
                {
                    if (text.Length == 0) throw new ArgumentException(
                        "Name of anonymous property not found in text.")
                        .WithData(name)
                        .WithData(text);

                    name = "{" + member.Name + "}";
                    
                    var found = false;
                    var start = 0;
                    while (start < text.Length)
                    {
                        index = text.IndexOf(name, start, comp);
                        if (index < 0)
                        {
                            if (!found) throw new ArgumentException(
                                "Name of anonymous property not found in text.")
                                .WithData(name)
                                .WithData(text);

                            break;
                        }
                        found = true;

                        text = text.Remove(index, name.Length);
                        text = text.Insert(index, par.Name);
                        start = index + par.Name.Length;
                    }
                }

                continue;
            }

            // Default case: positional specification...
            else
            {
                Parameters = Parameters.AddNew(arg, out par!);

                if (text is not null)
                {
                    var name = "{" + i.ToString() + "}";

                    if (text.Length == 0) throw new ArgumentException(
                        "Positional specification not found in text.")
                        .WithData(name)
                        .WithData(text);

                    var found = false;
                    var start = 0;
                    while (start < text.Length)
                    {
                        var index = text.IndexOf(name, start, comp);
                        if (index < 0)
                        {
                            if (!found) throw new ArgumentException(
                                "Name of anonymous property not found in text.")
                                .WithData(name)
                                .WithData(text);

                            break;
                        }
                        found = true;

                        text = text.Remove(index, name.Length);
                        text = text.Insert(index, par.Name);
                        start = index + par.Name.Length;
                    }
                }
            }
        }

        // Adding the text...
        if (text is not null) Text += text;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual ICommandInfo Add(ICommandInfo source)
    {
        source.ThrowWhenNull();
        if (source.IsEmpty) return this;

        var clone = Clone(); clone.AddInternal(source);
        return clone;
    }
    void AddInternal(ICommandInfo source)
    {
        var comp = Engine.CaseSensitiveNames
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        var samePrefix = string.Compare(
            Engine.ParameterPrefix, source.Engine.ParameterPrefix, comp) == 0;

        var text = source.Text;
        for (int i = 0; i < source.Parameters.Count; i++)
        {
            var old = source.Parameters[i];

            // No name colission, we assume old text is correct...
            if (samePrefix && !Parameters.Contains(old.Name))
                Parameters = Parameters.Add(old);

            // Either different prefix or name colission...
            else
            {
                Parameters = Parameters.AddNew(old.Value, out var par);

                var name = old.Name;
                var start = 0;
                while (start < text.Length)
                {
                    var pos = text.IndexOf(name, start, comp);
                    if (pos >= 0)
                    {
                        text = text.Remove(pos, name.Length);
                        text = text.Insert(pos, par!.Name);
                        start = pos + par.Name.Length;
                    }
                }
            }
        }

        Text += text;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ICommandInfo Clear()
    {
        if (IsEmpty) return this;

        var clone = Clone();
        clone.Text = string.Empty;
        clone.Parameters = clone.Parameters.Clear();
        return clone;
    }
}