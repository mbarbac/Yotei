namespace Yotei.Tools.WithGenerator;

// ========================================================
internal partial class WithGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override Type[] TypeAttributes { get; } = [
        typeof(InheritWithsAttribute),
        typeof(InheritWithsAttribute<>)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override TypeNode CreateNode(
        TypeCandidate candidate)
        => new XTypeNode(candidate.Symbol)
        { Syntax = candidate.Syntax, Attributes = candidate.Attributes };
}

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool Validate(SourceProductionContext context)
    {
        var r = base.Validate(context);

        if (Symbol.IsRecord) { CoreDiagnostics.RecordsNotSupported(Symbol).Report(context); r = false; }
        if (Attributes.Length == 0) { CoreDiagnostics.NoAttributes(Symbol).Report(context); r = false; }
        if (Attributes.Length > 1) { CoreDiagnostics.TooManyAttributes(Symbol).Report(context); r = false; }

        return r;
    }

    /// <summary>
    /// <inheritdoc/>
    /// We use the fact that this method is invoked AFTER the hierarchy is built to capture the
    /// inherited members knowing that we can now check for duplicates and prevent adding them.
    /// </summary>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureProperties();
        CaptureFields();

        base.Emit(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited properties, if any.
    /// </summary>
    void CaptureProperties()
    {
        foreach (var type in Symbol.AllBaseTypes) TryCapture(type);
        foreach (var type in Symbol.AllInterfaces) TryCapture(type);

        // Tries to capture members are the given type level...
        void TryCapture(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().Where(x =>
                x.GetAttributes().Any(y =>
                y.AttributeClass is not null &&
                y.AttributeClass.Name.StartsWith("WithAttribute")))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildProperties.Find(x => member.Name == x.Symbol.Name);
                if (temp is null)
                {
                    var node = new XPropertyNode(this, member) { IsInherited = true };
                    ChildProperties.Add(node);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited fields, if any.
    /// </summary>
    void CaptureFields()
    {
        foreach (var type in Symbol.AllBaseTypes) TryCapture(type);
        foreach (var type in Symbol.AllInterfaces) TryCapture(type);

        // Tries to capture members are the given type level...
        void TryCapture(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>().Where(x =>
                x.GetAttributes().Any(y =>
                y.AttributeClass is not null &&
                y.AttributeClass.Name.StartsWith("WithAttribute")))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildFields.Find(x => member.Name == x.Symbol.Name);
                if (temp is null)
                {
                    var node = new XFieldNode(this, member) { IsInherited = true };
                    ChildFields.Add(node);
                }
            }
        }
    }
}