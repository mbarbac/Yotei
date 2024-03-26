namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IIdentifierPart"/>
public class IdentifierPart : Identifier, IIdentifierPart
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierPart(IEngine engine) : base(engine) { }

    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierPart(IEngine engine, string? value) : this(engine)
    {
        if ((value = value.NullWhenEmpty()) == null) return;

        var nums = Engine.UnwrappedIndexes(value, '.');
        if (nums.Count > 0) throw new ArgumentException(
            "Unwrapped dots not allowed in single-part identifiers.")
            .WithData(value);

        nums = Engine.UnwrappedIndexes(value, ' ');
        if (nums.Count > 0) throw new ArgumentException(
            "Unwrapped spaces not allowed in single-part identifiers.")
            .WithData(value);

        if (Engine.UseTerminators && value.Length == 1)
        {
            if (value[0] == Engine.LeftTerminator) throw new ArgumentException(
                "Terminator cannot be used as the value.")
                .WithData(value);

            if (value[0] == Engine.RightTerminator) throw new ArgumentException(
                "Terminator cannot be used as the value.")
                .WithData(value);
        }

        UnwrappedValue = Engine.UseTerminators
            ? value.UnWrap(Engine.LeftTerminator, Engine.RightTerminator).NullWhenEmpty()
            : value;

        Value = Engine.UseTerminators && UnwrappedValue != null
            ? $"{Engine.LeftTerminator}{UnwrappedValue}{Engine.RightTerminator}"
            : UnwrappedValue;
    }

    // ----------------------------------------------------

    public override bool Equals(object? obj) => Equals(obj as IIdentifier);
    public static bool operator ==(IdentifierPart x, IIdentifier y) => x is not null && x.Equals(y);
    public static bool operator !=(IdentifierPart x, IIdentifier y) => !(x == y);
    public override int GetHashCode() => base.GetHashCode();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override string? Value { get; }

    /// <inheritdoc/>
    public string? UnwrappedValue { get; }
}