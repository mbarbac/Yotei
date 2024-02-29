namespace Yotei.Tools.UpcastGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode(INode parent, TypeCandidate candidate) : TypeNodeEx(parent, candidate)
{
    readonly ChildList<UpcastType> UTypes = [];
    readonly ChildList<IPropertySymbol> UProperties = [];
    readonly ChildList<IMethodSymbol> UMethods = [];
    readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// We just need to validate we have a list to iterate. Later, we just pick the suitable
    /// ones.
    /// </remarks>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (Syntax.BaseList == null)
        {
            context.NoInheritanceList(Syntax);
            return false;
        }

        if (!base.OnValidate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// Capturing before the default invocation the actual list of inherited types, as it may
    /// happen we need to manually add them to the inheritance chain.
    /// </remarks>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        UTypes.Clear();
        UProperties.Clear();
        UMethods.Clear();

        foreach (var node in Syntax.BaseList!.Types)
        {
            if (node.Type is not GenericNameSyntax named) continue;
            if (named.Arity != 1) continue;
            if (named.Identifier.Text is not "IUpcast" and not "IUpcastEx") continue;

            if (named.TypeArgumentList.Arguments.Count == 0) { context.NoInheritType(named); return; }

            var syntax = named.TypeArgumentList.Arguments[0];
            var info = SemanticModel.GetTypeInfo(syntax);
            var symbol = (INamedTypeSymbol?)info.Type;

            if (symbol == null) { context.NoInheritType(named); return; }

            var props = named.Identifier.Text == "IUpcastEx";
            var item = new UpcastType(syntax, symbol, props);
            UTypes.Add(item, raiseDuplicates: false);
        }

        // Base method will emit the inheritance chain and the captured contents...
        base.Emit(context, cb);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// We may need to add those not actually implemented.
    /// </remarks>
    protected override string GetTypeHeader()
    {
        var temps = UTypes.ToList();

        foreach (var node in Syntax.BaseList!.Types)
        {
            var info = SemanticModel.GetTypeInfo(node.Type);
            var type = (INamedTypeSymbol?)info.Type;
            if (type == null) continue;

            var temp = temps.Find(x => Comparer.Equals(x.Symbol, type));
            if (temp != null) temps.Remove(temp);
        }

        var options = new EasyNameOptions(useGenerics: true);
        var name = Symbol.EasyName(options);

        var sb = new StringBuilder();
        sb.Append(name);

        if (temps.Count > 0)
        {
            sb.Append(" : "); for (int i = 0; i < temps.Count; i++)
            {
                if (i != 0) sb.Append(", ");

                name = temps[i].Symbol.EasyName(options);
                sb.Append(name);
            }
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void OnEmit(SourceProductionContext _, CodeBuilder cb)
    {
        var num = false;
        foreach (var utype in UTypes)
        {
            DoProperties(utype, cb, ref num);
            DoMethods(utype, cb, ref num);
        }
    }

    /// <summary>
    /// Determines if the two given symbols are the same.
    /// </summary>
    bool Same(INamedTypeSymbol x, INamedTypeSymbol y)
    {
        if (Comparer.Equals(x, y)) return true;

        var options = new EasyNameOptions(useFullName: true, useGenerics: true);
        var xname = x.EasyName(options);
        var yname = y.EasyName(options);

        var same = string.Compare(xname, yname) == 0;
        return same;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the code for the properties of the given upcasted type.
    /// </summary>
    void DoProperties(UpcastType utype, CodeBuilder cb, ref bool num)
    {
        if (!utype.WithProperties) return;

        var hname = Symbol.EasyName(new EasyNameOptions(useGenerics: true));
        var items = GetProperties(utype);
        foreach (var item in items)
        {
            if (item.IsGeneratedCode(out var tool, out _) && tool == "Yotei") continue;
            if (Symbol.GetMembers().OfType<IPropertySymbol>().Any(x => Same(x, item))) continue;

            UProperties.Add(item);
            if (num) cb.AppendLine(); num = true;

            var adnul = item.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "";
            var getter = item.GetMethod != null;
            var setter = item.SetMethod != null && !item.SetMethod.IsInitOnly;
            var initer = item.SetMethod != null && item.SetMethod.IsInitOnly;
            var signature = GetSignature(item);
            var invocation = GetInvocation(item);

            // Iface from iface: new HostType Name { ... }
            if (Symbol.IsInterface())
            {
                var name = item.IsIndexer ? "this" : item.Name;

                cb.AppendLine(GetDocumentation(utype, item));
                cb.AppendLine(XUpcastGenerator.GeneratedCode);

                cb.Append($"new {hname}{adnul} {name}{signature} {{");
                if (getter) cb.Append(" get;");
                if (setter) cb.Append(" set;");
                if (initer) cb.Append(" init;");
                cb.AppendLine(" }");

                continue;
            }

            // Regular from iface: UpcastType UpcastType.Name { ... }
            if (utype.Symbol.IsInterface())
            {
                var uname = utype.Symbol.EasyName(new EasyNameOptions(useGenerics: true));
                var name = item.IsIndexer ? "this" : item.Name;

                cb.AppendLine(XUpcastGenerator.GeneratedCode);

                cb.AppendLine($"{uname}{adnul} {uname}.{name}{signature}");
                cb.AppendLine("{");
                cb.IndentLevel++;

                if (getter) cb.AppendLine($"get => {name}{invocation};");
                if (setter) cb.AppendLine($"set => {name}{invocation} = ({hname}{adnul})value;");
                if (initer) cb.AppendLine($"set => {name}{invocation} = ({hname}{adnul})value;");

                cb.IndentLevel--;
                cb.AppendLine("}");

                continue;
            }

            // Regular from regular: public new HostType Name { ... }
            else
            {
                var name = item.IsIndexer ? "this" : item.Name;

                cb.AppendLine(GetDocumentation(utype, item));
                cb.AppendLine(XUpcastGenerator.GeneratedCode);

                cb.AppendLine($"public new {hname}{adnul} {name}{signature}");
                cb.AppendLine("{");
                cb.IndentLevel++;

                if (getter) cb.AppendLine($"get => ({hname}{adnul}){name}{invocation};");
                if (setter) cb.AppendLine($"set => {name}{invocation} = value;");
                if (initer) cb.AppendLine($"set => {name}{invocation} = value;");

                cb.IndentLevel--;
                cb.AppendLine("}");

                continue;
            }
        }
    }

    /// <summary>
    /// Determines if the two given symbols are the same, x being the most derived one.
    /// </summary>
    bool Same(IPropertySymbol x, IPropertySymbol y)
    {
        if (Comparer.Equals(x, y)) return true;

        if (x.Name != y.Name) return false;

        if (x.Parameters.Length != y.Parameters.Length) return false;
        for (int i = 0; i < x.Parameters.Length; i++)
        {
            var xtype = x.Parameters[i].Type;
            var ytype = y.Parameters[i].Type;
            if (!xtype.IsAssignableTo(ytype)) return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the collection of elements to upcast.
    /// </summary>
    List<IPropertySymbol> GetProperties(UpcastType utype)
    {
        var items = utype.Symbol.GetMembers().OfType<IPropertySymbol>()
            .Where(x => Comparer.Equals(x.Type, utype.Symbol))
            .ToList();

        var hierarchy = this.GetHierarchy();
        var node = hierarchy.Find(x =>
            x is XTypeNode temp &&
            Same(temp.Symbol, utype.Symbol));

        if (node != null)
        {
            foreach (var item in ((XTypeNode)node).UProperties)
            {
                var temp = items.Find(x => Same(x, item));
                if (temp == null) items.Add(item);
            }
        }
        return items;
    }

    /// <summary>
    /// Gets the signature of the parameters.
    /// </summary>
    string GetSignature(IPropertySymbol item)
    {
        if (!item.IsIndexer) return "";

        var sb = new StringBuilder();
        sb.Append('[');

        for (int i = 0; i < item.Parameters.Length; i++)
        {
            if (i != 0) sb.Append(", ");

            var arg = item.Parameters[i];
            sb.Append(arg.Type.EasyName(new EasyNameOptions(addNullable: true, useGenerics: true)));
            sb.Append(' ');
            sb.Append(arg.Name);
        }

        sb.Append(']');
        return sb.ToString();
    }

    /// <summary>
    /// Gets the invocation of the parameters.
    /// </summary>
    string GetInvocation(IPropertySymbol item)
    {
        if (!item.IsIndexer) return "";

        var sb = new StringBuilder();
        sb.Append('[');

        for (int i = 0; i < item.Parameters.Length; i++)
        {
            if (i != 0) sb.Append(", ");

            var arg = item.Parameters[i];
            sb.Append(arg.Name);
        }

        sb.Append(']');
        return sb.ToString();
    }

    /// <summary>
    /// Gets the documentation for the given element.
    /// </summary>
    string GetDocumentation(UpcastType utype, IPropertySymbol item)
    {
        var options = new EasyNameOptions(useGenerics: true);
        var uname = utype.Symbol.EasyName(options);

        var iname = item.Name;
        if (item.IsIndexer)
        {
            options = new EasyNameOptions(addNullable: true, useGenerics: true);
            var sb = new StringBuilder();
            sb.Append("this[");

            for (int i = 0; i < item.Parameters.Length; i++)
            {
                if (i != 0) sb.Append(", ");
                sb.Append(item.Parameters[i].Type.EasyName(options));
            }
            
            sb.Append(']');
            iname = sb.ToString();
        }

        return $"/// <inheritdoc cref=\"{uname}.{iname}\"/>";
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the code for the methods of the given upcasted type.
    /// </summary>
    void DoMethods(UpcastType utype, CodeBuilder cb, ref bool num)
    {
        var hname = Symbol.EasyName(new EasyNameOptions(useGenerics: true));
        var items = GetMethods(utype);
        foreach (var item in items)
        {
            if (!item.CanBeReferencedByName) continue;
            if (item.IsGeneratedCode(out var tool, out _) && tool == "Yotei") continue;
            if (Symbol.GetMembers().OfType<IMethodSymbol>().Any(x => Same(x, item))) continue;

            UMethods.Add(item);
            if (num) cb.AppendLine(); num = true;

            var adnul = item.ReturnNullableAnnotation == NullableAnnotation.Annotated ? "?" : "";
            var signature = GetSignature(item);
            var invocation = GetInvocation(item);

            // Iface from iface: new HostType Name(...)
            if (Symbol.IsInterface())
            {
                cb.AppendLine(GetDocumentation(utype, item));
                cb.AppendLine(XUpcastGenerator.GeneratedCode);

                cb.AppendLine($"new {hname}{adnul} {item.Name}{signature};");
                continue;
            }

            // Regular from iface: UpcastType UpcastType.Name(...) => Name(...);
            if (utype.Symbol.IsInterface())
            {
                var uname = utype.Symbol.EasyName(new EasyNameOptions(useGenerics: true));

                cb.AppendLine(XUpcastGenerator.GeneratedCode);

                cb.AppendLine(
                    $"{uname}{adnul} {uname}.{item.Name}{signature} => " +
                    $"{item.Name}{invocation};");

                continue;
            }

            // Regular from regular: public override HostType Name(...) => (HostType)base.Name(...);
            else
            {
                var modifier = GetAccesibility(item);
                var change = (!item.IsAbstract && !item.IsVirtual) ? "new" : "override";

                cb.AppendLine(GetDocumentation(utype, item));
                cb.AppendLine(XUpcastGenerator.GeneratedCode);

                cb.AppendLine(
                    $"{modifier}{change} {hname}{adnul}{item.Name}{signature} => " +
                    $"({hname}{adnul})base.{item.Name}{invocation};");

                continue;
            }
        }
    }

    /// <summary>
    /// Determines if the two given symbols are the same, x being the most derived one.
    /// </summary>
    bool Same(IMethodSymbol x, IMethodSymbol y)
    {
        if (Comparer.Equals(x, y)) return true;

        if (x.Name != y.Name) return false;

        if (x.TypeParameters.Length != y.TypeParameters.Length) return false;
        for (int i = 0; i < x.TypeParameters.Length; i++)
        {
            var xtype = x.TypeParameters[i];
            var ytype = y.TypeParameters[i];
            if (!xtype.IsAssignableTo(ytype)) return false;
        }

        if (x.Parameters.Length != y.Parameters.Length) return false;
        for (int i = 0; i < x.Parameters.Length; i++)
        {
            var xtype = x.Parameters[i].Type;
            var ytype = y.Parameters[i].Type;
            if (!xtype.IsAssignableTo(ytype)) return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the collection of elements to upcast.
    /// </summary>
    List<IMethodSymbol> GetMethods(UpcastType utype)
    {
        var items = utype.Symbol.GetMembers().OfType<IMethodSymbol>()
            .Where(x => Comparer.Equals(x.ReturnType, utype.Symbol))
            .ToList();

        var hierarchy = this.GetHierarchy();
        var node = hierarchy.Find(x =>
            x is XTypeNode temp &&
            Same(temp.Symbol, utype.Symbol));

        if (node != null)
        {
            foreach (var item in ((XTypeNode)node).UMethods)
            {
                var temp = items.Find(x => Same(x, item));
                if (temp == null) items.Add(item);
            }
        }
        return items;
    }

    /// <summary>
    /// Gets the accesibility of the method.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    string GetAccesibility(IMethodSymbol item)
    {
        return item.DeclaredAccessibility switch
        {
            Accessibility.Private => "private ",
            Accessibility.Protected => "protected ",
            Accessibility.ProtectedAndInternal => "protected internal ",
            Accessibility.Internal => "internal ",
            Accessibility.Public => "public ",
            _ => ""
        };
    }

    /// <summary>
    /// Gets the signature of the parameters.
    /// </summary>
    string GetSignature(IMethodSymbol item)
    {
        var sb = new StringBuilder();
        sb.Append('(');

        for (int i = 0; i < item.Parameters.Length; i++)
        {
            if (i != 0) sb.Append(", ");

            var arg = item.Parameters[i];
            sb.Append(arg.Type.EasyName(new EasyNameOptions(addNullable: true, useGenerics: true)));
            sb.Append(' ');
            sb.Append(arg.Name);
        }

        sb.Append(')');
        return sb.ToString();
    }

    /// <summary>
    /// Gets the invocation of the parameters.
    /// </summary>
    string GetInvocation(IMethodSymbol item)
    {
        var sb = new StringBuilder();
        sb.Append('(');

        for (int i = 0; i < item.Parameters.Length; i++)
        {
            if (i != 0) sb.Append(", ");

            var arg = item.Parameters[i];
            sb.Append(arg.Name);
        }

        sb.Append(')');
        return sb.ToString();
    }

    /// <summary>
    /// Gets the documentation for the given element.
    /// </summary>
    string GetDocumentation(UpcastType utype, IMethodSymbol item)
    {
        var options = new EasyNameOptions(useGenerics: true);
        var uname = utype.Symbol.EasyName(options);

        var iname = item.Name; if (item.Parameters.Length > 0)
        {
            var sb = new StringBuilder();
            sb.Append($"{item.Name}(");

            for (int i = 0; i < item.Parameters.Length; i++)
            {
                if (i != 0) sb.Append(", ");
                sb.Append(item.Parameters[i].Type.EasyName(options));
            }

            sb.Append(')');
            iname = sb.ToString();
        }

        return $"/// <inheritdoc cref=\"{uname}.{iname}\"/>";
    }
}