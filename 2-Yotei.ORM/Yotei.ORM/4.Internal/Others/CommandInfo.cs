namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the information needed to build a command.
/// </summary>
public class CommandInfo
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public CommandInfo(IEngine engine)
    {
        CommandText = string.Empty;
        Parameters = new Code.ParameterList(engine);
    }

    /// <summary>
    /// Initializes a new instance with the given text.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="text"></param>
    public CommandInfo(IEngine engine, string text)
    {
        CommandText = text.ThrowWhenNull();
        Parameters = new Code.ParameterList(engine);
    }

    /// <summary>
    /// Initializes a new instance with the given text and parameters.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parameters"></param>
    public CommandInfo(string text, IParameterList parameters)
    {
        CommandText = text.ThrowWhenNull();
        Parameters = parameters.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Parameters.Count == 0
            ? CommandText
            : CommandText + $"; -- [{string.Join(", ", Parameters)}]";
    }

    // ----------------------------------------------------

    /// <summary>
    /// The text carried by this instance.
    /// </summary>
    public string CommandText { get; private set; }

    /// <summary>
    /// The parameters carried by this instance.
    /// </summary>
    public IParameterList Parameters { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// Adds to the contents of this instance the given text and optional arguments.
    /// <br/> Text can be null if not used.
    /// <br/> Arguments, if used, shall be encoded in the given text using:
    /// <br/> - The '{name}' of a regular parameter included in the optional array.
    /// <br/> - The '{name}' of the unique property of an anonymous type included in the optional
    /// array, as in 'new { name = value }'.
    /// <br/> - A '{n}' positional specification, where 'n' is the index in the optional array
    /// from where to obtain the value.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    public void Add(string? text, object?[] args)
    {
        args ??= [null];

        // With arguments...
        if (args.Length != 0)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                // Case: arg is already a parameter...
                if (arg is IParameter par) Parameters = Parameters.Add(par);
                else
                {
                    // Case: arg is an anonyous type...
                    var type = arg?.GetType();
                    if (type != null && type.IsAnonymous())
                    {
                        var members = type.GetMembers().OfType<PropertyInfo>().ToList();
                        if (members.Count == 0) throw new ArgumentException("No members found in argument.").WithData(arg);
                        if (members.Count > 1) throw new ArgumentException("Many members found in argument.").WithData(arg);

                        var member = members[0];
                        var value = member.GetValue(arg);
                        par = new Code.Parameter(member.Name, value);

                        Parameters = Parameters.Add(par);
                    }

                    // Other cases...
                    else
                    {
                        Parameters = Parameters.AddNew(arg, out par);

                        if (text != null) // We may need to adjust the text to the new name...
                        {
                            var start = 0;
                            while (start < text.Length)
                            {
                                var name = "{" + i.ToString() + "}";
                                var pos = text.IndexOf(name, start);
                                if (pos >= 0)
                                {
                                    text = text.Remove(pos, name.Length);
                                    text = text.Insert(pos, par.Name);
                                    start = pos + par.Name.Length;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Adding the text if needed...
        if (text != null) CommandText += text;
    }

    /// <summary>
    /// Clears the contents of this instance.
    /// </summary>
    public void Clear()
    {
        CommandText = string.Empty;
        Parameters = Parameters.Clear();
    }
}