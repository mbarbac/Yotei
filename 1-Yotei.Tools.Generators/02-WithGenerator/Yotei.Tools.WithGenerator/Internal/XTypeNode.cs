using System.Data;

namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
public class XTypeNode : TypeNode
{
    AttributeData Attribute = default!;

    /// <summary>
    /// Initializes a new instance.
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
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        if (Symbol.IsRecord) { TreeError.RecordsNotSupported.Report(Symbol, context); r = false; }

        if (Attributes.Count == 0) { TreeError.NoAttributes.Report(Symbol, context); r = false; }
        else if (Attributes.Count > 1) { TreeError.TooManyAttributes.Report(Symbol, context); r = false; }
        else Attribute = Attributes[0];

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnEmitCore(ref TreeContext context, CodeBuilder cb)
    {
        var r = base.OnEmitCore(ref context, cb);
        if (r)
        {
            CaptureInheritProperties();
            CaptureInheritFields();
        }
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited properties, if any.
    /// </summary>
    void CaptureInheritProperties()
    {
        foreach (var type in Symbol.AllBaseTypes) TryCaptureAt(type);
        foreach (var type in Symbol.AllInterfaces) TryCaptureAt(type);

        /// <summary>
        /// Tries to capture at the given type's level.
        /// </summary>
        void TryCaptureAt(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>();
            foreach (var member in members)
            {
                if (!member.HasWithAttribute(out _)) continue;

                var temp = ChildProperties.Find(x => x.Symbol.Name == member.Name);
                if (temp != null) continue;

                var node = new XPropertyNode(member) { Parent = this, Inherited = true };
                ChildProperties.Add(node);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited fields, if any.
    /// </summary>
    void CaptureInheritFields()
    {
        foreach (var type in Symbol.AllBaseTypes) TryCaptureAt(type);
        foreach (var type in Symbol.AllInterfaces) TryCaptureAt(type);

        /// <summary>
        /// Tries to capture at the given type's level.
        /// </summary>
        void TryCaptureAt(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>();
            foreach (var member in members)
            {
                if (!member.HasWithAttribute(out _)) continue;

                var temp = ChildFields.Find(x => x.Symbol.Name == member.Name);
                if (temp != null) continue;

                var node = new XFieldNode(member) { Parent = this, Inherited = true };
                ChildFields.Add(node);
            }
        }
    }
}