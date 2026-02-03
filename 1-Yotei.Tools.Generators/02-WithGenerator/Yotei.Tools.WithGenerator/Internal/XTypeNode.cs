using System.Reflection.Metadata.Ecma335;

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (Symbol.IsRecord) { Symbol.ReportError(CoreError.RecordsNotSupported, context); return false; }
        if (Attributes.Count == 0) { Symbol.ReportError(CoreError.NoAttributes, context); return false; }
        if (Attributes.Count > 1) { Symbol.ReportError(CoreError.TooManyAttributes, context); return false; }

        return base.Validate(context);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/> We use the fact that when this method is invoked the hierarchy has been
    /// captured already, so that we can check the captured members to check for duplicates.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureProperties();
        CaptureFields();

        base.EmitCore(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited properties, if any.
    /// </summary>
    void CaptureProperties()
    {
        foreach (var type in Symbol.AllBaseTypes) TryCaptureAt(type);
        foreach (var type in Symbol.AllInterfaces) TryCaptureAt(type);

        // Tries to capture inherited members at the given type's level...
        void TryCaptureAt(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().Where(static x =>
                x.GetAttributes().Any(y =>
                y.AttributeClass is not null &&
                y.AttributeClass.Name.StartsWith(nameof(WithAttribute))))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildProperties.Find(x => x.Symbol.Name == member.Name);
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
        foreach (var type in Symbol.AllBaseTypes) TryCaptureAt(type);
        foreach (var type in Symbol.AllInterfaces) TryCaptureAt(type);

        // Tries to capture inherited members at the given type's level...
        void TryCaptureAt(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>().Where(static x =>
                x.GetAttributes().Any(y =>
                y.AttributeClass is not null &&
                y.AttributeClass.Name.StartsWith(nameof(WithAttribute))))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildFields.Find(x => x.Symbol.Name == member.Name);
                if (temp is null)
                {
                    var node = new XFieldNode(this, member) { IsInherited = true };
                    ChildFields.Add(node);
                }
            }
        }
    }
}