namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode(
    SemanticModel model, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
    : TypeNode(model, syntax, symbol)
{
    /// <inheritdoc/>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (!base.OnValidate(context)) return false;
        if (!context.TypeIsNotRecord(Syntax)) return false;

        return true;
    }

    // ----------------------------------------------------

    void PrintDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        /// <returns></returns>
        """);

    /// <summary>
    /// Invoked to emit the source code of this type, not its hierarchical childs.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void OnEmit(SourceProductionContext context, CodeBuilder cb)
    {
        if (GetNodeMethod(Symbol) != null) return;

        PrintDocumentation(cb);
        cb.AppendLine(YoteiGeneratedAttr.GetDecorator(GetType()));

        if (Symbol.IsInterface()) PrintInterface(context, cb);
        else if (Symbol.IsAbstract) PrintAbstract(context, cb);
        else PrintRegular(context, cb);
    }

    /// <summary>
    /// Case: interface
    /// </summary>
    void PrintInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typeName = Symbol.EasyName(new EasyNameOptions(useGenerics: true));

        cb.AppendLine($"{modifiers}{typeName} Clone();");
    }

    /// <summary>
    /// Case: abstract
    /// </summary>
    void PrintAbstract(SourceProductionContext _, CodeBuilder cb)
    {
        var typeName = Symbol.EasyName(new EasyNameOptions(useGenerics: true));

        cb.AppendLine($"public abstract {typeName} Clone();");
        PrintInterfacesToImplement(cb);
    }

    /// <summary>
    /// Case: abstract
    /// </summary>
    void PrintRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typeName = Symbol.EasyName(new EasyNameOptions(useGenerics: true));

        cb.AppendLine($"{modifiers}{typeName} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;

        var method = Symbol.GetCopyConstructor(true) ?? Symbol.GetCopyConstructor(false);
        if (method == null)
        {
            cb.AppendLine("// Cannot find a copy constructor.");
            context.NoCopyConstructor(Syntax);
        }
        else // Using the copy constructor...
        {
            cb.AppendLine($"var v_temp = new {typeName}(this);");
            cb.AppendLine("return v_temp;");
        }

        cb.IndentLevel--;
        cb.AppendLine("}");
        PrintInterfacesToImplement(cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Case: interfaces to implement.
    /// </summary>
    void PrintInterfacesToImplement(CodeBuilder cb)
    {
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces )
        {
            var typeName = iface.EasyName(new EasyNameOptions(
                useFullName: true,
                useGenerics: true));

            var valueName = iface.Name == "ICloneable" ? "object" : typeName;

            cb.AppendLine();
            cb.AppendLine(valueName);
            cb.AppendLine($"{typeName}.Clone() => Clone();");
        }
    }

    [SuppressMessage("", "IDE0305")]
    ITypeSymbol[] GetInterfacesToImplement()
    {
        var comparer = SymbolEqualityComparer.Default;
        var list = new List<ITypeSymbol>();

        foreach (var iface in Symbol.Interfaces) Populate(iface);
        return list.ToArray();

        // Populates the interfaces in the inheritance chain...
        bool Populate(ITypeSymbol iface)
        {
            var done = false;

            if (iface.HasAttributes(CloneableAttr.LongName)) done = true;
            else if (iface.Name == "ICloneable") done = true;
            else if (GetNodeMethod(iface) != null) done = true;

            foreach (var child in iface.Interfaces) if (Populate(child)) done = true;

            if (done)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }
            return done;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the appropriate modifiers, or null if any.
    /// <br/> If so, an space is added to the returned string.
    /// </summary>
    /// <returns></returns>
    string? GetModifiers()
    {
        // Interfaces...
        if (Symbol.IsInterface())
        {
            return Symbol.AllInterfaces.Any(x =>
                x.Name == "ICloneable" ||
                x.HasAttributes(CloneableAttr.LongName))
                ? "new "
                : null;
        }

        // Others...
        else
        {
            var prevent = GetPreventVirtual(Symbol, out var temp) && temp;
            var appears = AppearsInChain(Symbol.BaseType);

            if (appears)
            {
                return prevent ? "public new " : "public override ";
            }
            else
            {
                return Symbol.IsSealed || prevent
                    ? "public "
                    : "public virtual ";
            }

            // Determines if appears in chain...
            bool AppearsInChain(ITypeSymbol? type)
            {
                if (type is null) return false;

                if (GetNodeMethod(type) != null) return true;
                if (type.HasAttributes(CloneableAttr.LongName)) return true;

                return AppearsInChain(type.BaseType);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the method implemented by the type, or null if any.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IMethodSymbol? GetNodeMethod(ITypeSymbol type, bool recursive = false)
    {
        var method = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);

        if (method != null) return method;

        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return GetNodeMethod(parent, recursive);
        }

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the effective value of the <see cref="CloneableAttr.PreventVirtual"/> setting.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetPreventVirtual(ITypeSymbol type, out bool value)
    {
        // The symbol itself...
        if (CloneableAttr.GetPreventVirtual(type, out value)) return value;

        // It might be defined in the inheritance hierarchy...
        foreach(var parent in type.AllBaseTypes())
            if (CloneableAttr.GetPreventVirtual(parent, out value)) return value;

        foreach (var parent in type.Interfaces)
            if (CloneableAttr.GetPreventVirtual(parent, out value)) return value;

        // Not defined...
        return false;
    }
}