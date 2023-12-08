namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XFieldNode : FieldNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="candidate"></param>
    public XFieldNode(FieldCandidate candidate) : base(candidate) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    public XFieldNode(IFieldSymbol symbol) : base(symbol) { }

    /// <summary>
    /// The name of the method to generate.
    /// </summary>
    string MethodName => $"With{Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        if (!HostType.TypeIsNotRecord(context)) return false;
        if (!Symbol.FieldIsWrittable(context)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="file"></param>
    public override void Print(SourceProductionContext context, FileBuilder file)
    {
        // Intercepting explicit implementation...
        if (HasMethod(HostType) != null) return;

        // Common variables...
        var parentType = HostType.FullyQualifiedName(addNullable: false);
        var memberType = Symbol.Type.FullyQualifiedName(addNullable: true);
        var vname = $"v_{Symbol.Name}";

        // Interface...
        if (HostType.IsInterface())
        {
            PrintDocumentation(file, vname);
            PrintInterface();
            return;
        }

        // Implementation...
        var specs = TryGetSpecs(out var temp) ? temp.NullWhenEmpty() : null;
        var comparison = StringComparison.OrdinalIgnoreCase;

        if (string.Compare(specs, "this", comparison) == 0)
        {
            PrintDocumentation(file, vname);
            PrintThisSpecs();
        }
        else if (string.Compare(specs, "base", comparison) == 0)
        {
            PrintDocumentation(file, vname);
            PrintBaseSpecs();
        }
        else if (specs is null || string.Compare(specs, "copy", comparison) == 0)
        {
            PrintDocumentation(file, vname);

            if (HostType.IsAbstract) PrintAbstract();
            else PrintCopySpecs();
        }
        else
        {
            Symbol.InvalidSpecs(specs, context);
            return;
        }

        return;

        /// <summary>
        /// Emits code when the host type is an interface...
        /// </summary>
        void PrintInterface()
        {
            var modifiers = GetModifiers();

            file.AppendLine($"{modifiers}{parentType}");
            file.AppendLine($"{MethodName}({memberType} {vname});");
        }

        /// <summary>
        /// Emits code when the host type is abstract...
        /// </summary>
        void PrintAbstract()
        {
            var modifiers = "public abstract ";

            file.AppendLine($"{modifiers}{parentType}");
            file.AppendLine($"{MethodName}({memberType} {vname});");
        }

        /// <summary>
        /// Emits code when the 'this' specs are requested...
        /// </summary>
        void PrintThisSpecs()
        {
            var modifiers = GetModifiers();

            file.AppendLine($"{modifiers}{parentType}");
            file.AppendLine($"{MethodName}({memberType} {vname})");
            file.AppendLine("{");
            file.IndentLevel++;

            file.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
            file.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
            file.AppendLine();

            file.AppendLine($"{Symbol.Name} = {vname};");
            file.AppendLine("return this;");

            file.IndentLevel--;
            file.AppendLine("}");
        }

        /// <summary>
        /// Emits code when the 'base' specs are requested...
        /// </summary>
        void PrintBaseSpecs()
        {
            var modifiers = GetModifiers();

            file.AppendLine($"{modifiers}{parentType}");
            file.AppendLine($"{MethodName}({memberType} {vname})");
            file.AppendLine("{");
            file.IndentLevel++;

            var chain = FindBaseChain(); if (chain == null)
            {
                file.AppendLine("throw new NotImplementedException();");
                Symbol.NoBaseMethod(context);
            }
            else
            {
                file.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
                file.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
                file.AppendLine();

                file.AppendLine($"var v_temp = {chain}.{MethodName}({vname});");
                file.AppendLine($"return ({HostType.Name})v_temp;");
            }

            file.IndentLevel--;
            file.AppendLine("}");

            // Gets the base chain to use, or null if any...
            string? FindBaseChain()
            {
                var parent = HostType.BaseType;
                var num = parent == null ? 0 : FindBaseNum(parent);
                if (num > 0)
                {
                    var sb = new StringBuilder();
                    for (int i = 0; i < num; i++)
                    {
                        if (i > 0) sb.Append('.');
                        sb.Append("base");
                    }
                    return sb.ToString();
                }
                return null;
            }
            int FindBaseNum(ITypeSymbol type)
            {
                var parent = type.BaseType;
                var num = parent == null ? 0 : FindBaseNum(parent);

                if (HasDecoratedMember(type) != null ||
                    HasMethod(type) != null ||
                    (HasMember(type) != null && type.HasAttributes(WithGeneratorAttr.LongName)))
                {
                    num++;
                }
                return num;
            }
        }

        /// <summary>
        /// Emits code when the 'copy' specs are requested...
        /// </summary>
        void PrintCopySpecs()
        {
            var modifiers = GetModifiers();

            file.AppendLine($"{modifiers}{parentType}");
            file.AppendLine($"{MethodName}({memberType} {vname})");
            file.AppendLine("{");
            file.IndentLevel++;

            var method = HostType.GetCopyConstructor(true) ?? HostType.GetCopyConstructor(false);
            if (method == null)
            {
                file.AppendLine("throw new NotImplementedException();");
                HostType.NoCopyConstructor(context);
            }
            else
            {
                var vtemp = "v_temp";

                file.AppendLine($"var v_comparer = EqualityComparer<{memberType}>.Default;");
                file.AppendLine($"if (v_comparer.Equals({Symbol.Name}, {vname})) return this;");
                file.AppendLine();

                file.AppendLine($"var {vtemp} = new {HostType.Name}(this)");
                file.AppendLine("{");

                file.IndentLevel++;
                file.AppendLine($"{Symbol.Name} = {vname}");
                file.IndentLevel--;

                file.AppendLine("};");
                file.AppendLine($"return {vtemp};");
            }

            file.IndentLevel--;
            file.AppendLine("}");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints documentation.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="vname"></param>
    void PrintDocumentation(FileBuilder file, string vname) => file.AppendLine($$"""
        /// <summary>
        /// Returns an instance of the host type where the value of the decorated member has been
        /// replaced by the new given one.
        /// </summary>
        /// <param name ="{{vname}}"></param>
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Returns the appropriate method modifiers, or null if any.
    /// </summary>
    /// <returns></returns>
    string? GetModifiers()
    {
        // Interfaces...
        if (HostType.IsInterface())
        {
            return HostType.AllInterfaces.Any(x =>
                HasDecoratedMember(x) != null ||
                HasMethod(x) != null)
                ? "new "
                : null;
        }

        // Implementation...
        else
        {
            var prevent = TryGetPreventVirtual(out var temp) && temp;
            var appears = AppearsInChain(HostType, true);
            if (appears)
            {
                return prevent ? "public new " : "public override ";
            }
            else
            {
                return HostType.IsSealed || prevent
                    ? "public "
                    : "public virtual ";
            }

            // Determines if appears in chain...
            bool AppearsInChain(ITypeSymbol type, bool top)
            {
                if (!top)
                {
                    if (HasMethod(type) != null) return true;
                    if (HasDecoratedMember(type) != null) return true;

                    if (type.HasAttributes(WithGeneratorAttr.LongName) &&
                        HasMember(type) != null)
                        return true;
                }
                var parent = type.BaseType;
                return parent != null && AppearsInChain(parent, false);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type has a member with the appropriate name, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IFieldSymbol? HasMember(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IFieldSymbol>()
        .FirstOrDefault(x => x.Name == Symbol.Name);

    /// <summary>
    /// Determines if the type has a DECORATED member with the appropriate name, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IFieldSymbol? HasDecoratedMember(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IFieldSymbol>()
        .FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            x.HasAttributes(WithGeneratorAttr.LongName));

    /// <summary>
    /// Determines if the type implements a compatible method, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IMethodSymbol? HasMethod(ITypeSymbol type) => type
        .GetMembers()
        .OfType<IMethodSymbol>()
        .FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the <see cref="WithGeneratorAttr.Specs"/> setting.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetSpecs(out string? value)
    {
        if (WithGeneratorAttr.GetSpecs(Symbol, out value)) return true;
        if (WithGeneratorAttr.GetSpecs(HostType, out value)) return true;

        foreach (var parent in HostType.AllBaseTypes())
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        foreach (var parent in HostType.AllInterfaces)
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetSpecs(member, out value)) return true;

            if (WithGeneratorAttr.GetSpecs(parent, out value)) return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get the value of the <see cref="WithGeneratorAttr.PreventVirtual"/> setting.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetPreventVirtual(out bool value)
    {
        if (WithGeneratorAttr.GetPreventVirtual(Symbol, out value)) return true;
        if (WithGeneratorAttr.GetPreventVirtual(HostType, out value)) return true;

        foreach (var parent in HostType.AllBaseTypes())
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        foreach (var parent in HostType.AllInterfaces)
        {
            var member = HasDecoratedMember(parent);
            if (member != null &&
                WithGeneratorAttr.GetPreventVirtual(member, out value)) return true;

            if (WithGeneratorAttr.GetPreventVirtual(parent, out value)) return true;
        }

        return false;
    }
}