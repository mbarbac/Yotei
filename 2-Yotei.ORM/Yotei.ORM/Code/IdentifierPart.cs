namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierPart"/>
public class IdentifierPart : IIdentifierPart
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierPart(IEngine engine) : this(engine, null) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierPart(IEngine engine, string? value)
    {
        Engine = engine.ThrowWhenNull();

        var parts = IdentifierCode.GetParts(Engine, value);
        switch (parts.Count)
        {
            case 0: return;

            case 1:
                value = parts[0];
                UnwrappedValue = value;
                Value = Engine.UseTerminators && value is not null
                    ? $"{Engine.LeftTerminator}{value}{Engine.RightTerminator}"
                    : null;
                return;

            default: throw new ArgumentException(
                "More than one part detected in the given single-part value.")
                .WithData(value);
        };
    }

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public string? Value { get; }

    /// <inheritdoc/>
    public string? UnwrappedValue { get; }

    /// <inheritdoc/>
    public bool Match(string? specs) => IdentifierCode.Match(this, specs);
}