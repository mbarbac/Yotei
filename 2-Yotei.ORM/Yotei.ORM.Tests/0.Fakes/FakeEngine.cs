namespace Yotei.ORM.Tests;

// ========================================================
public partial class FakeEngine : Engine, IEngine
{
    public FakeEngine() : base() => KnownTags = new FakeKnownTags(CASESENSITIVETAGS);
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";

    /// <inheritdoc/>
    public override FakeEngine Clone() => new(this);

    /// <inheritdoc/>
    public override FakeEngine WithCaseSensitiveNames(bool value) => new(this) { CaseSensitiveNames = value };

    /// <inheritdoc/>
    public override FakeEngine WithNullValueLiteral(string value) => new(this) { NullValueLiteral = value };

    /// <inheritdoc/>
    public override FakeEngine WithPositionalParameters(bool value) => new(this) { PositionalParameters = value };

    /// <inheritdoc/>
    public override FakeEngine WithParameterPrefix(string value) => new(this) { ParameterPrefix = value };

    /// <inheritdoc/>
    public override FakeEngine WithSupportsNativePaging(bool value) => new(this) { SupportsNativePaging = value };

    /// <inheritdoc/>
    public override FakeEngine WithUseTerminators(bool value) => new(this) { UseTerminators = value };

    /// <inheritdoc/>
    public override FakeEngine WithLeftTerminator(char value) => new(this) { LeftTerminator = value };

    /// <inheritdoc/>
    public override FakeEngine WithRightTerminator(char value) => new(this) { RightTerminator = value };

    /// <inheritdoc/>
    public override FakeEngine WithKnownTags(IKnownTags value) => new(this) { KnownTags = value };
}