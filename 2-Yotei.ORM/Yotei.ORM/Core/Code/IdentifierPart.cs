namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifierPart"/>
/// </summary>
public sealed class IdentifierPart : IIdentifierPart
{
    string? _Value;
    string? _Unwrapped;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierPart(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierPart(IEngine engine, string? value) : this(engine) => Value = value;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value ?? string.Empty;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Value
    {
        get => _Value;
        init
        {
            _Value = null;
            _Unwrapped = null;

            if ((value = value.NullWhenEmpty(trim: true)) == null) return;

            var items = Engine.GetUnwrappedIndexes(value, '.');
            if (items.Count > 0) throw new ArgumentException(
                "Embedded dots not allowed for not-terminated single-part identifiers.")
                .WithData(value);

            items = Engine.GetUnwrappedIndexes(value, ' ');
            if (items.Count > 0) throw new ArgumentException(
                "Embedded spaces not allowed for not-terminated single-part identifiers.")
                .WithData(value);

            _Unwrapped = Engine.UseTerminators
                ? value.UnWrap(Engine.LeftTerminator, Engine.RightTerminator).NullWhenEmpty()
                : value.NullWhenEmpty();

            _Value = Engine.UseTerminators && _Unwrapped != null
                ? $"{Engine.LeftTerminator}{_Unwrapped}{Engine.RightTerminator}"
                : _Unwrapped;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? UnwrappedValue
    {
        get => _Unwrapped;
        init => _Value = value;
    }
}