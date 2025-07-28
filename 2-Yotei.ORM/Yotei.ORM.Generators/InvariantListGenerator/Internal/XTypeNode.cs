namespace Yotei.ORM.Generators;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    const string IListName = "IInvariantList";
    const string ListName = "InvariantList";

    const string IListNamespace = "Yotei.ORM.Tools";
    const string ListNamespace = "Yotei.ORM.Tools.Code";

    INamedTypeSymbol ReturnType = null!;
    AttributeData AttributeData = null!;
    INamedTypeSymbol AttributeClass = null!;
    int Arity = 0;
    INamedTypeSymbol KType = null!; bool IsKTypeNullable = false; // Arity == 2, might not be used
    INamedTypeSymbol TType = null!; bool IsTTypeNullable = false;

    Type Template = null!;
    string KTypeName = null!;
    string TTypeName = null!;
    string AttributeName = null!;
    string BracketName = null!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        // Base validations...
        if (!base.Validate(context)) return false;

        // Other validations (we can capture here because this node is not captured off-line)...
        if ((AttributeData = FindAttribute(this, context, Symbol)!) == null) return false;
        if ((AttributeClass = FindAttributeClass(AttributeData, context, Symbol)!) == null) return false;
        if (!CaptureArityAndTypes(context)) return false;
        if (!ValidateCopyConstructor(context)) return false;
        if (!CaptureReturnType(out ReturnType, context)) return false;

        // Other captures...
        Template = Arity == 1 ? typeof(IChainTemplate<>) : typeof(IChainTemplate<,>);
        KTypeName = KType?.EasyName(RoslynNameOptions.Full)!; if (IsKTypeNullable) KTypeName += "?";
        TTypeName = TType?.EasyName(RoslynNameOptions.Full)!; if (IsTTypeNullable) TTypeName += "?";
        AttributeName = AttributeClass.Name.RemoveEnd("Attribute");
        BracketName = Arity == 1 ? $"<{TTypeName}>" : $"<{KTypeName}, {TTypeName}>";

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Validates and captures the return type of the generated methods. By default, it will be
    /// the interface itself or, if it is not an interface, then the first direct interface that
    /// is itself an invariant-alike one. If there is not such interface, then the host type as
    /// the default one.
    /// </summary>
    bool CaptureReturnType(out INamedTypeSymbol type, SourceProductionContext context)
    {
        // Interfaces are valid per-se...
        if (Symbol.IsInterface()) { type = Symbol; return true; }

        // Validating no base type with concrete return type...
        var host = Symbol;
        while ((host = host.BaseType) != null)
        {
            // To simplify, assume that if no decoration no method implementation...
            var attr = FindAttribute(host);
            if (attr == null) continue;

            // If no direct inteface in a decorated host, returns concrete...
            var temp = FirstInvariant(host);
            if (temp == null)
            {
                TreeDiagnostics.InvalidReturnType(Symbol).Report(context);
                type = null!;
                return false;
            }
        }

        // Finding a suitable direct interface...
        var iface = FirstInvariant(Symbol);
        if (iface != null) { type = iface; return true; }

        // By default the host type itself...
        type = Symbol;
        return true;
    }

    /// <summary>
    /// Gets the first direct interface that is an invariant one, or null if any.
    /// </summary>
    INamedTypeSymbol? FirstInvariant(INamedTypeSymbol type)
    {
        foreach (var iface in type.Interfaces)
        {
            var found = iface.Recursive(iface =>
            {
                if (iface.Name.StartsWith(IListName)) return true;
                if (FindAttribute((INamedTypeSymbol)iface) != null) return true;
                return false;
            },
            allifaces: true);

            if (found) return iface;
        }
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the logical arity of the decorating attribute and the types used.
    /// Returns false and reports an appropriate error if they cannot be obtained.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool CaptureArityAndTypes(SourceProductionContext context)
    {
        // No generic parameters used...
        if (AttributeClass.Arity == 0)
        {
            var args = AttributeData.ConstructorArguments;

            if (args.Length == 1) // (Type ttype)...
            {
                TType = GetUnderlyingType(args[0], out IsTTypeNullable, context, Symbol)!;
                if (TType != null) { Arity = 1; return true; }
            }
            else if (args.Length == 2) // (Type ktype, Type ttype)...
            {
                KType = GetUnderlyingType(args[0], out IsKTypeNullable, context, Symbol)!;
                TType = GetUnderlyingType(args[1], out IsTTypeNullable, context, Symbol)!;
                if (KType != null && TType != null) { Arity = 2; return true; }
            }
        }

        // Used one generic <T> parameter...
        else if (AttributeClass.Arity == 1)
        {
            var ttype = (INamedTypeSymbol)AttributeClass.TypeArguments[0];
            TType = GetUnderlyingType(ttype, out IsTTypeNullable, context, Symbol)!;
            if (TType != null) { Arity = 1; return true; }
        }

        // Used two generic <K,T> parameters...
        else if (AttributeClass.Arity == 2)
        {
            var ktype = (INamedTypeSymbol)AttributeClass.TypeArguments[0];
            var ttype = (INamedTypeSymbol)AttributeClass.TypeArguments[1];
            KType = GetUnderlyingType(ktype, out IsKTypeNullable, context, Symbol)!;
            TType = GetUnderlyingType(ttype, out IsTTypeNullable, context, Symbol)!;
            if (KType != null && TType != null) { Arity = 2; return true; }
        }

        // Invalid attribute...
        TreeDiagnostics.InvalidAttribute(Symbol).Report(context);
        return false;
    }

    /// <summary>
    /// Obtains the actual underlying type, and if it is nullable or not. If not obtained,
    /// reports an appropriate error if the context and the symbol are not null.
    /// </summary>
    static INamedTypeSymbol? GetUnderlyingType(
        TypedConstant source, out bool nullable,
        SourceProductionContext? context = null, INamedTypeSymbol? symbol = null)
    {
        if (source.IsNull || source.Kind != TypedConstantKind.Type)
        {
            if (context.HasValue && symbol != null) TreeDiagnostics.InvalidAttribute(symbol).Report(context.Value);
            nullable = false;
            return null;
        }

        var type = (INamedTypeSymbol)source.Value!;
        return GetUnderlyingType(type, out nullable, context);
    }

    /// <summary>
    /// Obtains the actual underlying type, and if it is nullable or not. If not obtained,
    /// reports an appropriate error if the context and the symbol are not null.
    /// </summary>
    static INamedTypeSymbol? GetUnderlyingType(
        INamedTypeSymbol source, out bool nullable,
        SourceProductionContext? context = null, INamedTypeSymbol? symbol = null)
    {
        if (source.Name == "Nullable")
        {
            if (source.TypeArguments.Length == 1)
            {
                source = (INamedTypeSymbol)source.TypeArguments[0];
                nullable = true;
                return source;
            }

            if (context.HasValue) TreeDiagnostics.InvalidAttribute(symbol ?? source).Report(context.Value);
            nullable = false;
            return null;
        }
        else
        {
            nullable = false;
            return source;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the attribute data that decorates the element associated with the given node, or
    /// null if any. If not obtained, reports an appropriate error if the context and the symbol
    /// are not null.
    /// </summary>
    static AttributeData? FindAttribute(
        XTypeNode node,
        SourceProductionContext? context = null, INamedTypeSymbol? symbol = null)
    {
        if (node.Candidate != null)
        {
            var len = node.Candidate.Attributes.Length;

            if (len == 0)
            {
                if (context.HasValue) TreeDiagnostics.NoAttributes(symbol ?? node.Symbol).Report(context.Value);
                return null;
            }
            if (len > 1)
            {
                if (context.HasValue) TreeDiagnostics.TooManyAttributes(symbol ?? node.Symbol).Report(context.Value);
                return null;
            }

            return node.Candidate.Attributes[0];
        }

        return FindAttribute(node.Symbol, context, symbol);
    }

    /// <summary>
    /// Gets the attribute data that decorates the given type, or null if any. If not obtained,
    /// reports an appropriate error if the context and the symbol are not null.
    /// </summary>
    static AttributeData? FindAttribute(
        INamedTypeSymbol type,
        SourceProductionContext? context = null, INamedTypeSymbol? symbol = null)
    {
        var ats = type.GetAttributes().Where(x =>
            x.AttributeClass != null && (
            x.AttributeClass.Name.StartsWith(ListName) ||
            x.AttributeClass.Name.StartsWith(IListName)))
            .ToArray();

        if (ats.Length == 0)
        {
            if (context.HasValue) TreeDiagnostics.NoAttributes(symbol ?? type).Report(context.Value);
            return null;
        }
        else if (ats.Length > 1)
        {
            if (context.HasValue) TreeDiagnostics.TooManyAttributes(symbol ?? type).Report(context.Value);
            return null;
        }

        return ats[0];
    }

    /// <summary>
    /// Obtains the attribute class of the given attribute data, or null if any. If not obtained,
    /// reports an appropriate error if the context and the symbol are not null.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    static INamedTypeSymbol? FindAttributeClass(
        AttributeData data,
        SourceProductionContext? context, INamedTypeSymbol? symbol)
    {
        if (data.AttributeClass != null) return data.AttributeClass;

        if (context.HasValue && symbol != null) TreeDiagnostics.InvalidAttribute(symbol).Report(context.Value);
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// If the host type is not an interface, validates we have a valid constructor.
    /// </summary>
    bool ValidateCopyConstructor(SourceProductionContext context)
    {
        if (Symbol.IsInterface()) return true;

        var cons = Symbol.GetCopyConstructor();
        if (cons == null)
        {
            TreeDiagnostics.NoCopyConstructor(Symbol).Report(context);
            return false;
        }
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var nsname = Symbol.IsInterface() ? IListNamespace : ListNamespace;
        var extra = $"{nsname}.{AttributeName}{BracketName}";
        var head = base.GetHeader(context) + $" : {extra}";

        return head;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Emitting 'Clone()' if needed...
        TryEmitClone(cb);

        // Capturing common variables...
        var hostname = Symbol.EasyName();
        var options = ReturnType.IsInterface() ? RoslynNameOptions.Full with { UseTypeNullable = false } : RoslynNameOptions.Default;
        var retname = ReturnType.EasyName(options);

        var ioptions = EasyNameOptions.Default with // Interface method names...
        {
            UseMemberArgumentsTypes = EasyNameOptions.Full,
            UseMemberArgumentsNames = true,
        };
        var coptions = ioptions with // Base methods' arguments...
        {
            UseMemberArgumentsTypes = null,
        };
        var headdoc = Symbol.IsInterface() ? IListName : ListName;

        // Iterating through template's methods...
        var methods = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);
        var existing = Symbol.GetMembers().OfType<IMethodSymbol>().ToArray();

        foreach (var method in methods)
        {
            if (!CanEmit(method, existing)) continue;

            var mname = method.EasyName(ioptions);
            mname = mname.Replace("K ", $"{KTypeName} "); // K key...
            mname = mname.Replace("<K", $"<{KTypeName}"); // IComparer<K> comparer...
            mname = mname.Replace("T ", $"{TTypeName} "); // T item...
            mname = mname.Replace("T>", $"{TTypeName}>"); // IEnumerable<T> range...

            // Interfaces...
            if (Symbol.IsInterface())
            {
                var doc = InheritDoc(method);

                cb.AppendLine();
                cb.AppendLine(doc);
                cb.AppendLine(AttributeDoc);

                cb.AppendLine($"new {hostname} {mname};");
            }

            // Reference types...
            else
            {
                var doc = InheritDoc(method);

                cb.AppendLine();
                cb.AppendLine(doc);
                cb.AppendLine(AttributeDoc);

                var args = method.EasyName(coptions);
                cb.AppendLine($"public override {retname} {mname}");
                cb.AppendLine($"=> ({retname})base.{args};");

                // We need not to implement the explicit invariant interfaces, because either
                // it is already the return type, or such reimplementation is not needed as
                // the interface is not in the host type.
            }
        }

        // Obtains the string to use with '<inheritdoc...>'
        string InheritDoc(MethodInfo method)
        {
            var name = method.EasyName();
            name = $"{headdoc}.{name}";
            name = name.Replace('<', '{').Replace('>', '}');
            name = $"/// <inheritdoc cref=\"{name}\"/>";
            return name;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given method can be emitted or not (because it has been explicitly
    /// implemented in the host).
    /// </summary>
    bool CanEmit(MethodInfo method, IMethodSymbol[] existing)
    {
        // Comparing the method against each existin one...
        foreach (var item in existing)
        {
            var mname = method.Name;
            var ename = item.Name;
            if (mname != ename) continue; // Names differ, no impediment...

            var mpars = method.GetParameters();
            var epars = item.Parameters;
            if (mpars.Length != epars.Length) continue; // Differ, so no impediment...

            var comparer = SymbolComparer.Default;
            var count = mpars.Length;

            for (int i = 0; i < mpars.Length; i++)
            {
                var mpar = mpars[i]; var mtype = mpar.ParameterType;
                var epar = epars[i]; var etype = (INamedTypeSymbol)epar.Type;

                // Ej: Add(T item), Remove(K key)...
                if (mtype.IsGenericParameter)
                {
                    if (mtype.Name == "T" && comparer.Equals(etype, TType)) count--; // Found...
                    if (mtype.Name == "K" && comparer.Equals(etype, KType)) count--; // Found...
                }

                // Ej: AddRange(IEnumerable<T> range)...
                else if (
                    mtype.Name == "IEnumerable´1" &&
                    mtype.FullName == null &&
                    mtype.GenericTypeArguments[0].IsGenericParameter)
                {
                    if (etype.Name == "IEnumerable" && etype.TypeArguments.Length == 1)
                    {
                        var mtemp = mtype.GenericTypeArguments[0];

                        if (mtemp.Name == "T" && comparer.Equals(etype, TType)) count--; // Found...
                        if (mtemp.Name == "K" && comparer.Equals(etype, KType)) count--; // Found...
                    }
                }

                // Ej: Remove(Predicate<T> predicate)...
                else if (
                    mtype.Name == "Predicate`1" &&
                    mtype.FullName == null &&
                    mtype.GenericTypeArguments[0].IsGenericParameter)
                {
                    if (etype.Name == "Predicate" && etype.TypeArguments.Length == 1)
                    {
                        var mtemp = mtype.GenericTypeArguments[0];

                        if (mtemp.Name == "T" && comparer.Equals(etype, TType)) count--; // Found...
                        if (mtemp.Name == "K" && comparer.Equals(etype, KType)) count--; // Found...
                    }
                }

                // Ej: RemoveAt(int index)...
                else
                {
                    if (etype.Match(mtype)) count--; // Found...
                }
            }

            if (count == 0) return false; // All have matched, so is the same method...
        }

        // No impediments found, we can emit the method...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to emit a 'Clone()' method, if such is needed.
    /// </summary>
    /// <param name="cb"></param>
    void TryEmitClone(CodeBuilder cb)
    {
        // If already declared or implemented we are done...
        if (HasCloneMethod(Symbol)) return;

        // Documenting...
        cb.AppendLine("/// <inheritdoc cref=\"ICloneable.Clone\"/>");
        cb.AppendLine(AttributeDoc);

        // Host is an interface...
        if (Symbol.IsInterface())
        {
            var name = Symbol.EasyName();
            cb.AppendLine($"new {name} Clone();");
        }

        // Otherwise we are ultimately inheriting from an abstract class...
        else
        {
            var name = Symbol.EasyName();
            var retname = ReturnType.EasyName();
            cb.AppendLine($"public override {retname} Clone() => new {name}(this);");

            foreach (var iface in FindCloneInterfaces(Symbol))
            {
                name = iface.EasyName();

                cb.AppendLine();
                cb.AppendLine($"{name}");
                cb.AppendLine($"{name}.Clone() => Clone();");
            }
        }
    }

    /// <summary>
    /// Returns the collection of interfaces that need an explicit 'Clone()' implementation.
    /// </summary>
    IEnumerable<INamedTypeSymbol> FindCloneInterfaces(INamedTypeSymbol type)
    {
        var comparer = SymbolComparer.Default;
        List<INamedTypeSymbol> items = [];

        foreach (var iface in type.Interfaces) Capture(iface);
        return items;

        // Tries to capture the given interface.
        bool Capture(INamedTypeSymbol iface)
        {
            var need = false;
            foreach (var child in iface.Interfaces) if (Capture(child)) need = true;

            need |= HasCloneMethod(iface);

            if (need)
            {
                var temp = items.Find(x => comparer.Equals(x, iface));
                if (temp == null) items.Add(iface);
            }
            return need;
        }
    }

    /// <summary>
    /// Determines if the given type has a 'Clone()' alike method, or in its chain of base types
    /// and direct or all implemented interfaces if such is explicitly requested. Returns null
    /// if not found, or the found method otherwise.
    /// </summary>
    bool HasCloneMethod(
        INamedTypeSymbol type,
        bool chain = false, bool ifaces = false, bool allifaces = false)
    {
        return type.Recursive(type =>
        {
            if (type.Name == "ICloneable") return true;
            if (FindCloneMethod((INamedTypeSymbol)type) != null) return true;

            var ats = type.GetAttributes();
            if (ats.Any(x =>
                x.AttributeClass != null &&
                x.AttributeClass.Name.Contains("Cloneable"))) return true;

            return false;
        },
        chain, ifaces, allifaces);
    }

    /// <summary>
    /// Find the 'Clone()' method declared or implemented in the given type, or null if any.
    /// </summary>
    static IMethodSymbol? FindCloneMethod(INamedTypeSymbol type)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0 &&
            x.ReturnsVoid == false);

        return item;
    }

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(InvariantListGenerator)}}", "{{VersionDoc}}")]
        """;
}