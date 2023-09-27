namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    public XTypeNode(Node parent, TypeCandidate candidate) : base(parent, candidate) { }
    public XTypeNode(Node parent, ITypeSymbol symbol):base(parent, symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        // Base validations...
        if (!base.Validate(context)) return false;
        if (!ValidateNotRecord(context, Symbol)) return false;

        // Passed...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints the appropriate documentation.
    /// </summary>
    void PrintDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        /// <returns></returns>
        """);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder cb)
    {
        // Initiating...
        if (HasMethod(Symbol) != null) return;
        PrintDocumentation(cb);

        var modifiers = GetModifiers();

        // Interfaces...
        if (IsInterface)
        {
            cb.AppendLine($"{modifiers}{Symbol.Name} Clone();");
            return;
        }

        // Implementation...
        if (Symbol.IsAbstract)
        {
            modifiers = "public abstract ";
            cb.AppendLine($"{modifiers}{Symbol.Name} Clone();");
        }
        else
        {
            cb.AppendLine($"{modifiers}{Symbol.Name} Clone()");
            cb.AppendLine("{");
            cb.IndentLevel++;

            cb.AppendLine("throw new NotImplementedException();");

            cb.IndentLevel--;
            cb.AppendLine("}");
        }

        // Interfaces to implement...
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            var name = iface.FullyQualifiedName(addNullable: false);
            var type = iface.Name == "ICloneable" ? "object" : name;

            cb.AppendLine();
            cb.AppendLine(type);
            cb.AppendLine($"{name}.Clone() => Clone();");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string with the method modifiers, followed by a 'space' if needed.
    /// </summary>
    /// <returns></returns>
    string GetModifiers()
    {
        // Interfaces...
        if (IsInterface)
        {
            return Symbol.AllInterfaces.Any(x =>
                x.Name == "ICloneable" ||
                x.HasAttributes(CloneableAttr.LongName))
                ? "new "
                : string.Empty;
        }

        // Implementation...
        else
        {
            foreach (var type in Symbol.AllBaseTypes())
            {
                var method = HasMethod(type);
                if (method != null)
                {
                    return method.IsVirtual || method.IsOverride || method.IsAbstract
                        ? "public override "
                        : "public new ";
                }

                if (type.HasAttributes(CloneableAttr.LongName))
                {
                    return !CloneableAttr.GetPreventVirtual(type)
                        ? "public override "
                        : "public new ";
                }
            }

            if (Symbol.IsSealed) return "public ";

            return !CloneableAttr.GetPreventVirtual(Symbol)
                ? "public virtual "
                : "public ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the interfaces that need implementation. This method is only called
    /// for implementation purposes.
    /// </summary>
    /// <returns></returns>
    List<ITypeSymbol> GetInterfacesToImplement()
    {
        var list = new NoDuplicatesList<ITypeSymbol>()
        {
            ThrowDuplicates = false,
            Comparer = SymbolEqualityComparer.Default.Equals,
        };

        foreach (var iface in Symbol.Interfaces) Populate(iface);
        return list.ToList();

        // Recursive...
        bool Populate(ITypeSymbol iface)
        {
            bool done = false;

            if (iface.Name == "ICloneable") done = true;
            if (HasMethod(iface) != null) done = true;
            if (iface.HasAttributes(CloneableAttr.LongName)) done = true;

            foreach (var child in iface.Interfaces)
            {
                if (Populate(child)) done = true;
            }

            if (done) list.Add(iface);
            return done;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type has a 'Clone' method, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static IMethodSymbol? HasMethod(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);
    }
}