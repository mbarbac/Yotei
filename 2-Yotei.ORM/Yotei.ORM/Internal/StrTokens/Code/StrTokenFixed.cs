namespace Yotei.ORM.Code.Internal;

// ========================================================
/// <inheritdoc cref="IStrTokenFixed"/>
public class StrTokenFixed : IStrTokenFixed
{
    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public StrTokenFixed(string value) => Payload = value;

    /// <inheritdoc/>
    public override string ToString() => Payload;

    /// <inheritdoc/>
    public string Payload
    {
        get => _Payload;
        init => _Payload = value.NotNullNotEmpty(trim: false);
    }
    string _Payload = string.Empty;

    object? IStrToken.Payload => Payload;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Reduce(StringComparison comparison) => this;
}