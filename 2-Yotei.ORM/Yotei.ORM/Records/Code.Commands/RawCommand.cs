#pragma warning disable IDE0044 // Make field readonly

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRawCommand"/>
/// </summary>
[WithGenerator]
public partial class RawCommand : ORM.Code.Command, IRawCommand
{
    StringBuilder _Builder;
    IParameterList _Parameters;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public RawCommand(IConnection connection) : base(connection)
    {
        _Builder = new();
        _Parameters = new ORM.Code.ParameterList(Connection.Engine);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected RawCommand(RawCommand source) : base(source.Connection)
    {
        _Builder = new(source._Builder.ToString());
        _Parameters = new ORM.Code.ParameterList(Connection.Engine, source._Parameters);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public ICommandEnumerator GetEnumerator()
        => Connection.Records.CreateCommandEnumerator(this);
    IEnumerator<IRecord?> IEnumerable<IRecord?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ICommandEnumerator GetAsyncEnumerator(CancellationToken token = default)
        => Connection.Records.CreateCommandEnumerator(this, token);
    IAsyncEnumerator<IRecord?> IAsyncEnumerable<IRecord?>.GetAsyncEnumerator(
        CancellationToken token)
        => GetAsyncEnumerator(token);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override string GetText(out IParameterList parameters)
    {
        parameters = _Parameters;
        return _Builder.ToString();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="iterable"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override string GetText(
        bool iterable, out IParameterList parameters) => GetText(out parameters);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public IRawCommand Append(string? specs, params object?[] args)
    {
        args ??= [null];

        // With arguments...
        if (args.Length != 0)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                // Case: arg is already a parameter...
                if (arg is IParameter par) _Parameters = _Parameters.Add(par);
                else
                {
                    // Case: arg is an anonymous type...
                    var type = arg?.GetType();
                    if (type != null && type.IsAnonymous())
                    {
                        var members = type.GetMembers().OfType<PropertyInfo>().ToList();
                        if (members.Count == 0) throw new ArgumentException("No members found in argument.").WithData(arg);
                        if (members.Count > 1) throw new ArgumentException("Many members found in argument.").WithData(arg);

                        var member = members[0];
                        var value = member.GetValue(arg);
                        par = new ORM.Code.Parameter(member.Name, value);

                        _Parameters = _Parameters.Add(par);
                    }

                    // Null value or regular one...
                    else
                    {
                        _Parameters = _Parameters.AddNew(arg, out par);

                        if (specs != null) // Adjusting the specs if needed...
                        {
                            var start = 0;
                            while (start < specs.Length)
                            {
                                var name = "{" + i.ToString() + "}";
                                var pos = specs.IndexOf(name, start);
                                if (pos >= 0)
                                {
                                    specs = specs.Remove(pos, name.Length);
                                    specs = specs.Insert(pos, par.Name);
                                    start = pos + par.Name.Length;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Adding the text and finalizing...
        if (specs != null) _Builder.Append(specs);
        return this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public IRawCommand Append(Func<dynamic, object> specs) => throw null;
}