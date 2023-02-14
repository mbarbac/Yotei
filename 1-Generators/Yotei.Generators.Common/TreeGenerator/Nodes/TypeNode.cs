namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a type in the hierarchy.
/// </summary>
internal class TypeNode : CapturedType
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="capturedType"></param>
    /// <param name="index"></param>
    public TypeNode(ICapturedType captured, int index) : base(
        captured.SemanticModel,
        captured.Generator,
        captured.TypeSyntaxChain[index],
        captured.TypeSymbolChain[index])
    { }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child type nodes.
    /// </summary>
    List<TypeNode> ChildTypeNodes { get; } = new();

    TypeNode LocateChildTypeNode(ICaptured captured, int index)
    {
        var item = captured.AsCapturedType();
        var tpSyntax = item.TypeSyntaxChain[index];
        var tpSymbol = item.TypeSymbolChain[index];
        var tpName = tpSymbol.ShortName();

        var node = ChildTypeNodes.Find(x => x.Name == tpName);
        if (node == null)
        {
            node = new TypeNode(item, index);
            ChildTypeNodes.Add(node);
        }
        return node;
    }

    /// <summary>
    /// The collection of child captured types.
    /// </summary>
    List<ICapturedType> ChildCapturedTypes { get; } = new();

    /// <summary>
    /// The collection of child captured properties.
    /// </summary>
    List<ICapturedProperty> ChildProperties { get; } = new();

    /// <summary>
    /// The collection of child captured fields.
    /// </summary>
    List<ICapturedField> ChildFields { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to register into this instance the given captured element, at the level given
    /// by the namespace and type indexes.
    /// </summary>
    public void Register(ICaptured captured, int tpIndex)
    {
        var item = captured.AsCapturedType();
        var tpLen = item.TypeSyntaxChain.Length;
        var level = item.Generator.TreeLevel;

        // Terminal Type...
        if (tpIndex == (tpLen - 1) && level == TreeLevel.Type)
        {
            var node = ChildCapturedTypes.Find(x => x.Name == item.Name);
            if (node == null) ChildCapturedTypes.Add(item);
            return;
        }

        else
        {
            // Intermediate type...
            if (tpIndex < tpLen)
            {
                var node = LocateChildTypeNode(captured, tpIndex);
                node.Register(captured, tpIndex + 1);
                return;
            }

            // Terminal member...
            else
            {
                switch (level)
                {
                    case TreeLevel.Property: if (CaptureProperty()) return; break;
                    case TreeLevel.Field: if (CaptureField()) return; break;
                    case TreeLevel.PropertyOrField: if (CapturePropertyOrField()) return; break;
                }
                throw new ArgumentException($"Invalid terminal element: {captured}");

                bool CaptureProperty()
                {
                    if (captured is ICapturedProperty prop) { ChildProperties.Add(prop); return true; }
                    return false;
                }
                bool CaptureField()
                {
                    if (captured is ICapturedField field) { ChildFields.Add(field); return true; }
                    return false;
                }
                bool CapturePropertyOrField() => CaptureProperty() || CaptureField();
            }
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        foreach (var tpNode in ChildTypeNodes) if (!tpNode.Validate(context)) return false;
        foreach (var tpItem in ChildCapturedTypes) if (!tpItem.Validate(context)) return false;
        foreach (var prop in ChildProperties) if (!prop.Validate(context)) return false;
        foreach (var field in ChildFields) if (!field.Validate(context)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc>
    /// </inheritdoc>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder cb)
    {
        var num = 0;

        for (int i = 0; i < ChildTypeNodes.Count; i++)
        {
            if (num > 0) cb.AppendLine();
            num++;
            ChildTypeNodes[i].Print(context, cb);
        }

        for (int i = 0; i < ChildCapturedTypes.Count; i++)
        {
            if (num > 0) cb.AppendLine();
            num++;
            ChildCapturedTypes[i].Print(context, cb);
        }

        for (int i = 0; i < ChildProperties.Count; i++)
        {
            if (num > 0) cb.AppendLine();
            num++;
            ChildProperties[i].Print(context, cb);
        }

        for (int i = 0; i < ChildFields.Count; i++)
        {
            if (num > 0) cb.AppendLine();
            num++;
            ChildFields[i].Print(context, cb);
        }
    }
}