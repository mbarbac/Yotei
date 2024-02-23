namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode(
    INode parent, INamedTypeSymbol symbol) : TypeNode(parent, symbol)
{
    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        if (!context.TypeIsNotRecord(Symbol)) return false;

        if (!base.Validate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the documentation and decorator.
    /// </summary>
    /// <param name="cb"></param>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        /// <returns></returns>
        {{XCloneGenerator.YoteiGenerated}}
        """);

    /// <inheritdoc/>
    protected override void OnEmit(SourceProductionContext context, CodeBuilder cb)
    {
        if (GetTypeMethod(Symbol) != null) return; // Already implemented!

        if (Symbol.IsInterface()) EmitInterface(context, cb);
        else if (Symbol.IsAbstract) EmitAbstract(context, cb);
        else EmitRegular(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Case: type is interface.
    /// </summary>
    void EmitInterface(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typeName = Symbol.EasyName(new EasyNameOptions(useGenerics: true));

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typeName} Clone();");
    }

    /// <summary>
    /// Case: type is abstract.
    /// </summary>
    void EmitAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var typeName = Symbol.EasyName(new EasyNameOptions(useGenerics: true));

        EmitDocumentation(cb);
        cb.AppendLine($"public abstract {typeName} Clone();");
        EmitInterfacesToImplement(cb);
    }

    /// <summary>
    /// Case: type is class or struct.
    /// </summary>
    void EmitRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var typeName = Symbol.EasyName(new EasyNameOptions(useGenerics: true));

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{typeName} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;

        var method = Symbol.GetCopyConstructor(true) ?? Symbol.GetCopyConstructor(false);
        if (method == null)
        {
            cb.AppendLine("// Cannot find a copy constructor.");
            context.NoCopyConstructor(Symbol);
        }
        else // Using the copy constructor...
        {
            cb.AppendLine($"var v_temp = new {typeName}(this);");
            cb.AppendLine("return v_temp;");
        }

        cb.IndentLevel--;
        cb.AppendLine("}");
        EmitInterfacesToImplement(cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the interfaces that need to be implemented.
    /// </summary>
    /// <param name="cb"></param>
    void EmitInterfacesToImplement(CodeBuilder cb)
    {
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
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
            else if (GetTypeMethod(iface) != null) done = true;

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
    /// Returns the appropriate method modifiers, or null if any.
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

                if (GetTypeMethod(type) != null) return true;
                if (type.HasAttributes(CloneableAttr.LongName)) return true;

                return AppearsInChain(type.BaseType);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type has already implemented a corresponding method.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    IMethodSymbol? GetTypeMethod(ITypeSymbol type, bool recursive = false)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);

        if (item != null) return item;

        if (recursive)
        {
            var parent = type.BaseType;
            if (parent != null) return GetTypeMethod(parent, recursive);
        }

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the effective value of the <see cref="CloneableAttr.PreventVirtual"/> setting,
    /// using the given type an its appropriate inheritance chain.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool GetPreventVirtual(ITypeSymbol type, out bool value)
    {
        // The symbol itself...
        if (CloneableAttr.GetPreventVirtual(type, out value)) return true;

        // It might be defined in the inheritance hierarchy...
        foreach (var parent in type.AllBaseTypes())
            if (CloneableAttr.GetPreventVirtual(parent, out value)) return true;

        foreach (var parent in type.AllInterfaces)
            if (CloneableAttr.GetPreventVirtual(parent, out value)) return true;

        // Not defined...
        return false;
    }
}