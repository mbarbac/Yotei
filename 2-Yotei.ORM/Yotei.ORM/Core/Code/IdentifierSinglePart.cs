namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifierSinglePart"/>
/// </summary>
public class IdentifierSinglePart : Identifier, IIdentifierSinglePart
{
    string? _Value = null;
    string? _NonTerminatedValue = null;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierSinglePart(IEngine engine) : base(engine) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierSinglePart(IEngine engine, string? value) : this(engine) => Value = value;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string? Value
    {
        get => _Value;
        init
        {
            var parts = Engine.GetDotted(value);
            _Value = null;
            _NonTerminatedValue = null;

            if (parts.Length > 1) throw new ArgumentException(
                "Single-part identifiers cannot have dot-separated parts.")
                .WithData(value);

            if (parts.Length == 0) return;

            if (Engine.UseTerminators) value = value.UnWrap(Engine.LeftTerminator, Engine.RightTerminator);
            if ((value = value.NullWhenEmpty()) == null) return;

            if (value != null && !Engine.UseTerminators)
            {
                if (value.Contains('.')) throw new ArgumentException(
                    "Non-terminated single-part identifiers cannot have embedded dots.")
                    .WithData(value);

                if (value.Contains(' ')) throw new ArgumentException(
                    "Non-terminated single-part identifiers cannot have embedded spaces.")
                    .WithData(value);
            }

            _NonTerminatedValue = value;
            _Value = Engine.UseTerminators && value != null
                ? $"{Engine.LeftTerminator}{value}{Engine.RightTerminator}"
                : value;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? NonTerminatedValue
    {
        get => _NonTerminatedValue;
        init => Value = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override IIdentifier Reduce() => this;
}