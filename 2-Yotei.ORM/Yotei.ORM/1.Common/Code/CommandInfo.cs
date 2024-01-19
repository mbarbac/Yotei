using System.ComponentModel.DataAnnotations;

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
        CommandText = string.Empty;
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
        CommandText = text ?? string.Empty;
        Parameters = new ParameterList(engine);
    }

    /// <summary>
    /// Initializes a new instance with the given text, and the parameters obtained from the
    /// given collection of arguments.
    /// <br/> Text can be null if not used.
    /// <br/> Arguments, if used, shall be encoded in the text using:
    /// <br/> - The '{name}' of a regular parameter included in the optional array.
    /// <br/> - The '{name}' of the unique property of an anonymous type in the optional array, as
    /// in 'new { name = value }'.
    /// <br/> - A '{n}' positional specification, where 'n' is the index of the value in the given
    /// optional array.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    /// <param name="args"></param>
    public CommandInfo(IEngine engine, string? text, params object?[] args)
        : this(engine)
        => Add(text, args);

    /// <summary>
    /// Initializes a new instance with the given text and collection of parameters.
    /// <br/> Text can be null if not used.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parameters"></param>
    public CommandInfo(string? text, IParameterList parameters)
    {
        CommandText = text ?? string.Empty;
        Parameters = parameters.ThrowWhenNull();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CommandInfo(CommandInfo source)
    {
        CommandText = source.CommandText;
        Parameters = source.Parameters;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Parameters.Count == 0
        ? CommandText
        : CommandText + $"; -- [{string.Join(", ", Parameters)}]";

    // ----------------------------------------------------

    IEngine Engine => Parameters.Engine;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string CommandText { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IParameterList Parameters { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    public void Add(string? text)
    {
        if (text != null) CommandText += text;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parameter"></param>
    public void Add(IParameter parameter)
    {
        Parameters = Parameters.Add(parameter);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    public void Add(IEnumerable<IParameter> range)
    {
        Parameters = Parameters.AddRange(range);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    public void Add(string? text, params object?[] args)
    {
        args ??= [null];

        var prefix = Engine.ParameterPrefix;
        int start;
        int index;
        string name;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            // Case: arg is a regular parameter...
            if (arg is IParameter par) 
            {
                Parameters = Parameters.Add(par);

                if (text != null)
                {
                    index = NameIndex(0, text, par.Name, Engine.CaseSensitiveNames);
                    if (index < 0) throw new ArgumentException(
                        "Parameter name not found in the given text.")
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
                if (members.Length == 0) throw new ArgumentException("No properties found in anonymous type.").WithData(arg);
                if (members.Length > 1) throw new ArgumentException("Too many properties found in anonymous type.").WithData(arg);

                var member = members[0];
                
                index = member.Name.IndexOf(prefix, Engine.CaseSensitiveNames);
                name = index >= 0 ? member.Name : prefix + member.Name;

                var value = member.GetValue(arg);
                Parameters = Parameters.Add(par = new Parameter(name, value));

                if (text != null)
                {
                    name = "{" + member.Name + "}";
                    if (text.Length == 0) throw new ArgumentException(
                        "Name of anonymous property not found in the given text.")
                        .WithData(name)
                        .WithData(text);

                    var found = false;
                    start = 0;
                    while (start < text.Length)
                    {
                        index = NameIndex(start, text, name, caseSensitive: false);
                        if (index < 0)
                        {
                            if (!found) throw new ArgumentException(
                                "Name of anonymous property not found in the given text.")
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

            // Case: numeric specification...
            Parameters = Parameters.AddNew(arg, out par!);

            if (text != null)
            {
                name = "{" + i.ToString() + "}";
                if (text.Length == 0) throw new ArgumentException(
                    "Numeric specification not found in the given text")
                    .WithData(name)
                    .WithData(text);
                
                var found = false;
                start = 0;
                while (start < text.Length)
                {
                    index = NameIndex(start, text, name, caseSensitive: false);
                    if (index < 0)
                    {
                        if (!found) throw new ArgumentException(
                            "Numeric specification not found in the given text")
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

        // Adding the given text...
        if (text != null) CommandText += text;
    }

    int NameIndex(int start, string text, string name, bool caseSensitive)
    {
        var span = text.AsSpan(start);
        var index = span.IndexOf(name, caseSensitive);
        return index < 0 ? -1 : index + start;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="source"></param>
    public void Add(ICommandInfo source)
    {
        source.ThrowWhenNull();

        var same = string.Compare(
            Engine.ParameterPrefix,
            source.Parameters.Engine.ParameterPrefix,
            !Engine.CaseSensitiveNames)
            == 0;

        var text = source.CommandText;
        for (int i = 0; i < source.Parameters.Count; i++)
        {
            var old = source.Parameters[i];

            // No name colision, we assume old text is correct...
            if (same && !Parameters.Contains(old.Name))
            {
                Parameters = Parameters.Add(old);
            }

            // Either name colision, or different prefix...
            else
            {
                Parameters = Parameters.AddNew(old.Value, out var par);

                var start = 0;
                while (start < text.Length)
                {
                    var name = old.Name;
                    var pos = text.IndexOf(name, start); if (pos >= 0)
                    {
                        text = text.Remove(pos, name.Length);
                        text = text.Insert(pos, par!.Name);
                        start = pos + par.Name.Length;
                    }
                }
            }
        }

        CommandText += text;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Clear()
    {
        CommandText = string.Empty;
        Parameters = Parameters.Clear();
    }
}