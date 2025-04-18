﻿namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="FieldNode"/>
internal class XFieldNode : FieldNode
{
    public XFieldNode(TypeNode parent, IFieldSymbol symbol) : base(parent, symbol) { }
    public XFieldNode(TypeNode parent, FieldCandidate candidate) : base(parent, candidate) { }

    INamedTypeSymbol Host => ParentNode.Symbol;
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        var r = true;

        if (Host.IsRecord)
        {
            TreeDiagnostics.KindNotSupported(Host).Report(context);
            r = false;
        }
        if (!Host.IsInterface() && !Symbol.IsWrittable())
        {
            TreeDiagnostics.NotWrittable(Symbol).Report(context);
            r = false;
        }

        if (!base.Validate(context)) r = false;

        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        // Declared or implemented explicitly...
        if (FindMethod(Host) != null) return;

        // Dispatching...
        if (Host.IsInterface()) EmitAsInterface(context, cb);
        else if (Host.IsAbstract) EmitAsAbstract(context, cb);
        else EmitAsConcrete(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an interface.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitAsInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var parentType = Host.EasyName(RoslynNameOptions.Default);
        var memberType = Symbol.Type.EasyName(RoslynNameOptions.Full);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName});");

        /// <summary>
        /// Gets the method modifiers, with a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            foreach (var iface in Host.AllInterfaces)
            {
                var member = FindDecoratedMember(iface); // Implies it being implemented...
                if (member != null) return "new ";

                var method = FindMethod(iface);
                if (method != null) return "new ";
            }

            return null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is an abstract one.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="cb"></param>
    protected void EmitAsAbstract(SourceProductionContext _, CodeBuilder cb)
    {
        var modifiers = GetModifiers();
        var parentType = Host.EasyName(RoslynNameOptions.Default);
        var memberType = Symbol.Type.EasyName(RoslynNameOptions.Full);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName});");

        /// <summary>
        /// Gets the method modifiers, with a space separator, or null if any.
        /// Here we don't care about <see cref="WithAttribute.PreventVirtual"/>.
        /// </summary>
        string? GetModifiers()
        {
            // Having a base type...
            if (Host.BaseType != null && Host.BaseType.Name != "Object")
            {
                // If there is a base method...
                var method = FindMethod(Host.BaseType, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility.ToCSharpString(addspace: true);
                    return $"{access}abstract override ";
                }

                // Or if it is being implemented...
                var at = FindMemberWithAttribute(Host.BaseType, chain: true, ifaces: true);
                var inherit = XTypeNode.FindInheritWithsAttribute(Host, chain: true, ifaces: true);

                if (at != null && inherit != null)
                {
                    return "public abstract override ";
                }
            }

            // Default...
            return "public abstract ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the type is a concrete one.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected void EmitAsConcrete(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Host.GetCopyConstructor(strict: false);
        if (ctor == null)
        {
            TreeDiagnostics.NoCopyConstructor(Host).Report(context);
            return;
        }

        var modifiers = GetModifiers();
        var parentType = Host.EasyName(RoslynNameOptions.Default);
        var memberType = Symbol.Type.EasyName(RoslynNameOptions.Full);

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{parentType}");
        cb.AppendLine($"{MethodName}({memberType} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var xtemp = "x_temp";
            cb.AppendLine($"var {xtemp} = new {parentType}(this)");
            cb.AppendLine("{");
            cb.IndentLevel++;
            {
                cb.AppendLine($"{Symbol.Name} = {ArgumentName}");
            }
            cb.IndentLevel--;
            cb.AppendLine("};");
            cb.AppendLine($"return {xtemp};");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        /// <summary>
        /// Gets the method modifiers, with a space separator, or null if any.
        /// </summary>
        string? GetModifiers()
        {
            var at =
                FindMemberWithAttribute(Host, chain: true, ifaces: true) ??
                XTypeNode.FindInheritWithsAttribute(Host, chain: true, ifaces: true);

            var prevent = at != null && GetPreventVirtualValue(at, out var temp) && temp;
            var issealed = Host.IsSealed;

            // Having a base type...
            if (Host.BaseType != null && Host.BaseType.Name != "Object")
            {
                // If there is a base method...
                var method = FindMethod(Host.BaseType, chain: true);
                if (method != null)
                {
                    var access = method.DeclaredAccessibility;
                    if (access == Accessibility.Private) return null; // Cannot override...

                    var accstr = access.ToCSharpString(addspace: true);
                    var isvirtual = method.IsVirtual || method.IsOverride || method.IsAbstract;

                    return isvirtual switch
                    {
                        true => $"{accstr}override ",
                        false => $"{accstr}new ",
                    };
                }

                // Or if it is being implemented...
                var inherit = XTypeNode.FindInheritWithsAttribute(Host);
                if (inherit != null)
                {
                    var host = Host.BaseType;
                    while (host != null)
                    {
                        at = FindMemberWithAttribute(host, chain: true, ifaces: true);
                        if (at != null)
                        {
                            prevent = GetPreventVirtualValue(at, out temp) && temp;
                            return prevent ? "public new " : "public override ";
                        }

                        host = host.BaseType;
                    }
                }
            }

            // Default...
            return prevent || issealed ? "public " : "public virtual ";
        }
    }

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated code.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        {{AttributeDoc}}
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a '<c>With[name](value)</c>' method in the given type, including also its
    /// base types and interfaces if requested. Returns null if not found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public IMethodSymbol? FindMethod(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == MethodName &&
            x.Parameters.Length == 1 &&
            Symbol.Type.IsAssignableTo(x.Parameters[0].Type));

        if (item == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                item = FindMethod(temp);
                if (item != null) break;
            }
        }

        if (item == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                item = FindMethod(temp);
                if (item != null) break;
            }
        }

        return item;
    }

    /// <summary>
    /// Tries to find a compatible member in in the given type, including also its base types
    /// and interfaces, if requested. Returns null if not found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public IFieldSymbol? FindMember(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            Symbol.Type.IsAssignableTo(x.Type));

        if (item == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                item = FindMember(temp);
                if (item != null) break;
            }
        }

        if (item == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                item = FindMember(temp);
                if (item != null) break;
            }
        }

        return item;
    }

    /// <summary>
    /// Tries to find a compatible member in in the given type, decorated with the appropriate
    /// attribute, including also its base types and interfaces, if requested. Returns null if
    /// not found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public IFieldSymbol? FindDecoratedMember(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(x =>
            x.Name == Symbol.Name &&
            Symbol.Type.IsAssignableTo(x.Type) &&
            x.HasAttributes(typeof(WithAttribute)));

        if (item == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                item = FindMember(temp);
                if (item != null) break;
            }
        }

        if (item == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                item = FindMember(temp);
                if (item != null) break;
            }
        }

        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a '<see cref="WithAttribute"/>' attribute in a member of the given type,
    /// or in members of its host base types and interfaces, if requested. Returns null if not
    /// found.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    public AttributeData? FindMemberWithAttribute(
        ITypeSymbol type,
        bool chain = false, bool ifaces = false)
    {
        var member = FindDecoratedMember(type, chain, ifaces);
        var at = member?.GetAttributes(typeof(WithAttribute)).FirstOrDefault();
        return at;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value of the '<see cref="WithAttribute.PreventVirtual"/>' or the
    /// <see cref="InheritWithsAttribute.PreventVirtual"/> named argument from the given
    /// attribute data, using the fact that both are named the same.  Returns <c>true</c> if
    /// the value is found, and the value itself in the <paramref name="value"/> parameter, or
    /// false otherwise.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool GetPreventVirtualValue(AttributeData at, out bool value)
    {
        if (at.GetNamedArgument(nameof(WithAttribute.PreventVirtual), out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }

        value = default;
        return false;
    }
}