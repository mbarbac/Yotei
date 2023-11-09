using System.ComponentModel.DataAnnotations;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifierPart"/>
/// </summary>
public class IdentifierPart : IIdentifierPart
{
    string? _Value;
    string? _UnwrappedValue;

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
            _UnwrappedValue = null;

            if ((value = value.NullWhenEmpty()) == null) return;

            if (HasChar(value, '.')) throw new ArgumentException(
                "Not terminated single-part identifier cannot contain embedded dots.")
                .WithData(value);

            if (HasChar(value, ' ')) throw new ArgumentException(
                "Not terminated single-part identifier cannot contain embedded spaces.")
                .WithData(value);

            _UnwrappedValue = Engine.UseTerminators
                ? value.UnWrap(Engine.LeftTerminator, Engine.RightTerminator).NullWhenEmpty()
                : value.NullWhenEmpty();

            _Value = Engine.UseTerminators && _UnwrappedValue is not null
                ? $"{Engine.LeftTerminator}{_UnwrappedValue}{Engine.RightTerminator}"
                : _UnwrappedValue;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? UnwrappedValue
    {
        get => _UnwrappedValue;
        init => Value = value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance contains the given character, not protected by the engine
    /// terminators, if such are used.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="ch"></param>
    /// <returns></returns>
    bool HasChar(string value, char ch)
    {
        if (!Engine.UseTerminators) return value.Contains(ch);

        // Terminators have different value...
        if (Engine.LeftTerminator != Engine.RightTerminator)
        {
            var deep = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == Engine.LeftTerminator) { deep++; continue; }
                if (value[i] == Engine.RightTerminator) { deep--; if (deep < 0) deep = 0; continue; }
                if (value[i] == ch && deep == 0) return true;
            }
            return false;
        }

        // Terminators are the same character...
        else
        {
            var deep = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == Engine.LeftTerminator) { deep = deep == 0 ? 1 : 0; continue; }
                if (value[i] == '.' && deep == 0) return true;
            }
            return false;
        }
    }
}