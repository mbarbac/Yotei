namespace Yotei.ORM.Generators;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    const string IListNamespace = "Yotei.ORM.Tools", IListName = "IInvariantList";
    const string ListNamespace = "Yotei.ORM.Tools", ListName = "InvariantList";

    readonly SymbolComparer Comparer = SymbolComparer.Default;
    AttributeData ThisAttribute = default!;
    INamedTypeSymbol ReturnType = default!;
    RoslynNameOptions ReturnNameOptions = RoslynNameOptions.Default;

    int Arity = 0;
    INamedTypeSymbol KType = null!; bool IsKTypeNullable = false;
    INamedTypeSymbol TType = null!; bool IsTTypeNullable = false;

    Type Template = default!;
    string ThisAttributeName = default!;
    string KTypeName = default!;
    string TTypeName = default!;
    string BracketName = default!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        bool r = true;

        if (!base.Validate(context)) r = false;
        if (!ValidateSingleAttribute(context)) r = false;
        if (!ValidateArityAndTypes(context)) r = false;
        if (!r) return false;

        Template = Arity == 1 ? typeof(IChainTemplate<>) : typeof(IChainTemplate<,>);
        ThisAttributeName = ThisAttribute.AttributeClass!.Name.RemoveEnd("Attribute");
        KTypeName = Arity == 1 ? null! : KType.EasyName(RoslynNameOptions.Full) + (IsKTypeNullable ? "?" : "");
        TTypeName = TType.EasyName(RoslynNameOptions.Full) + (IsTTypeNullable ? "?" : "");
        BracketName = Arity == 1 ? $"<{TTypeName}>" : $"<{KTypeName}, {TTypeName}>";
        return true;
    }

    /// <summary>
    /// Custom validations.
    /// </summary>
    bool ValidateSingleAttribute(SourceProductionContext context)
    {
        var found = FindAttributes(Symbol, out var items, out var unique);

        if (!found || unique is null || items.Count == 0 || items.Count > 1)
        {
            switch (items.Count)
            {
                case 0: TreeDiagnostics.NoAttributes(Symbol).Report(context); return false;
                default: TreeDiagnostics.TooManyAttributes(Symbol).Report(context); return false;
            }
        }

        ThisAttribute = unique;
        return true;
    }

    /// <summary>
    /// Custome validations.
    /// </summary>
    bool ValidateArityAndTypes(SourceProductionContext context)
    {
        if (ThisAttribute is null) return false;

        var atc = ThisAttribute.AttributeClass;
        if (atc is not null)
        {
            // Using no generics...
            if (ThisAttribute.AttributeClass!.Arity == 0)
            {
                var args = ThisAttribute.ConstructorArguments
                    .Where(x => !x.IsNull && x.Kind == TypedConstantKind.Type)
                    .Select(x => (INamedTypeSymbol)x.Value!)
                    .ToArray();

                // (Type ttype)...
                if (args.Length == 1)
                {
                    TType = args[0].UnwrapNullable(out IsTTypeNullable);
                    Arity = 1;
                    return true;
                }

                // (Type ktype, Type ttype)...
                else if (args.Length == 2)
                {
                    KType = args[0].UnwrapNullable(out IsKTypeNullable);
                    TType = args[1].UnwrapNullable(out IsTTypeNullable);
                    Arity = 2;
                    return true;
                }
            }

            // Using one generics: <T>...
            else if (ThisAttribute.AttributeClass!.Arity == 1)
            {
                TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out IsTTypeNullable);
                Arity = 1;
                return true;
            }

            // Using two generics: <K, T>...
            else if (ThisAttribute.AttributeClass!.Arity == 2)
            {
                KType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out IsKTypeNullable);
                TType = ((INamedTypeSymbol)atc.TypeArguments[1]).UnwrapNullable(out IsTTypeNullable);
                Arity = 2;
                return true;
            }
        }

        // Invalid attribute...
        TreeDiagnostics.InvalidAttribute(Symbol).Report(context);
        return false;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var nsname = Symbol.IsInterface() ? IListNamespace : ListNamespace;
        var extra = $"{nsname}.{ThisAttributeName}{BracketName}";
        var head = base.GetHeader(context) + $" : {extra}";
        return head;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Finding the return type...
        if (!FindReturnTypeValue(ThisAttribute, out ReturnType, out _)) ReturnType = Symbol;
        if (!Comparer.Equals(Symbol, ReturnType)) ReturnNameOptions = RoslynNameOptions.Full;

        // Emitting 'Clone()' if needed...
        TryEmitClone(context, cb);

        // Capturing common variables...
        var returntype = ReturnType.EasyName(ReturnNameOptions);
        var headdoc = Symbol.IsInterface() ? IListName : ListName;

        var ifaceoptions = EasyNameOptions.Default with
        {
            UseMemberArgumentsTypes = EasyNameOptions.Full,
            UseMemberArgumentsNames = true,
        };
        var methodoptions = ifaceoptions with
        {
            UseMemberArgumentsTypes = null
        };

        // Iterating through the template methods, unless it is implemented...
        var methods = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);
        var existing = Symbol.GetMembers().OfType<IMethodSymbol>().ToArray();

        foreach (var method in methods)
        {
            if (!CanEmit(method, existing)) continue;

            var mname = method.EasyName(ifaceoptions);
            mname = mname.Replace("K ", $"{KTypeName} "); // K key...
            mname = mname.Replace("<K", $"<{KTypeName}"); // IComparer<K> comparer...
            mname = mname.Replace("T ", $"{TTypeName} "); // T item...
            mname = mname.Replace("T>", $"{TTypeName}>"); // IEnumerable<T> range...

            // Documentation...
            var doc = InheritDoc(method);
            cb.AppendLine();
            cb.AppendLine(doc);
            cb.AppendLine(AttributeDoc);

            // Host is interface...
            if (Symbol.IsInterface())
            {
                cb.AppendLine($"new {returntype} {mname};");
            }

            // Or otherwise. Note we need not to implemente InvariantList explicit interface as
            // it is already the base class return type...
            else
            {
                var args = method.EasyName(methodoptions);
                cb.AppendLine($"public override {returntype} {mname}");
                cb.AppendLine($"=> ({returntype})base.{args};");
            }
        }

        // Obtains the documentation string...
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
    /// Determines if the given method can be emitted, or not because it has been implemented as
    /// one of the given existing ones.
    /// </summary>
    bool CanEmit(MethodInfo method, IMethodSymbol[] existing)
    {
        // Comparing the method against the existing ones...
        foreach (var item in existing)
        {
            var mname = method.Name;
            var ename = item.Name;
            if (mname != ename) continue; // Names differ, no impediment...

            var mpars = method.GetParameters();
            var epars = item.Parameters;
            if (mpars.Length != epars.Length) continue; // Differ, so no impediment...

            var count = mpars.Length;
            for (int i = 0; i < mpars.Length; i++)
            {
                var mpar = mpars[i]; var mtype = mpar.ParameterType;
                var epar = epars[i]; var etype = (INamedTypeSymbol)epar.Type;

                // Ej: Add(T item), Remove(K key)...
                if (mtype.IsGenericParameter)
                {
                    if (mtype.Name == "T" && Comparer.Equals(etype, TType)) count--; // Found...
                    if (mtype.Name == "K" && Comparer.Equals(etype, KType)) count--; // Found...
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

                        if (mtemp.Name == "T" && Comparer.Equals(etype, TType)) count--; // Found...
                        if (mtemp.Name == "K" && Comparer.Equals(etype, KType)) count--; // Found...
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

                        if (mtemp.Name == "T" && Comparer.Equals(etype, TType)) count--; // Found...
                        if (mtemp.Name == "K" && Comparer.Equals(etype, KType)) count--; // Found...
                    }
                }

                // Ej: RemoveAt(int index)...
                else
                {
                    // This check prevents matching 'IEnumerable<IItem>' and 'IEnumerable<string>'
                    // (string or any other), which is common when adding functionality...

                    if (mtype.GenericTypeArguments.Length > 0 &&
                        !mtype.GenericTypeArguments[0].IsGenericParameter)
                    {
                        if (etype.Match(mtype)) count--; // Found...
                    }
                }
            }

            if (count == 0) return false; // All have matched, so is the same method...
        }

        // No impediments found, we can emit the method...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to emit a 'Clone()' method.
    /// </summary>
    void TryEmitClone(SourceProductionContext __, CodeBuilder cb)
    {
        // If implemented or requested, we are done...
        if (FindCloneMethod(Symbol, out _)) return;
        if (FindCloneableAttribute(Symbol, out _)) return;

        // Documentation...
        cb.AppendLine($"/// <inheritdoc cref=\"ICloneable.Clone\"/>");

        // Host is an interface...
        if (Symbol.IsInterface())
        {
            var name = Symbol.EasyName();
            cb.AppendLine($"new {name} Clone();");
        }

        // Otherwise we are inheriting from an abstract class...
        else
        {
            var returntype = ReturnType.EasyName(ReturnNameOptions);
            var thisname = Symbol.EasyName();
            cb.AppendLine($"public override {returntype} Clone() => new {thisname}(this);");

            foreach (var iface in GetCloneInterfaces())
            {
                var name = iface.EasyName(RoslynNameOptions.Full);

                cb.AppendLine();
                cb.AppendLine($"{name}");
                cb.AppendLine($"{name}.Clone() => Clone();");
            }
        }
    }

    /// <summary>
    /// Gets the collection of clone interfaces that need explicit implementation.
    /// </summary>
    List<ITypeSymbol> GetCloneInterfaces()
    {
        var list = new List<ITypeSymbol>();

        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
        return list;

        /// <summary>
        /// Tries to capture the given interface as an explicit one.
        /// </summary>
        bool TryCapture(INamedTypeSymbol iface)
        {
            var found = false;

            foreach (var child in iface.Interfaces) // First child ones...
            {
                var temp = TryCapture(child);
                if (temp) found = true;
            }

            // Then, the given one if needed...
            if (!found)
                found = FindCloneMethod(iface, out _) || FindCloneableAttribute(iface, out _);

            if (found) // Adding if needed and not duplicated...
            {
                var temp = list.Find(x => Comparer.Equals(x, iface));
                if (temp is null) list.Add(iface);
            }

            return found;
        }
    }

    /// <summary>
    /// Tries to find a 'Cloneable' attribute decorating the given type, or on the first valid one
    /// in the given chains.
    /// </summary>
    static bool FindCloneableAttribute(
        INamedTypeSymbol type,
        out AttributeData value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out AttributeData value) =>
        {
            var items = type.GetAttributes().Where(x =>
                x.AttributeClass is not null &&
                x.AttributeClass.Name.StartsWith("CloneableAttribute")).ToArray();

            value = items.Length > 0 ? items[0] : null!;
            return items.Length > 0;
        },
        out value, chains);
    }

    /// <summary>
    /// Tries to find a 'Clone()' method on the given type, or on the first valid one in the
    /// given chains.
    /// </summary>
    static bool FindCloneMethod(
        INamedTypeSymbol type,
        out IMethodSymbol value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        return type.Recursive((INamedTypeSymbol type, out IMethodSymbol value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.Parameters.Length == 0 &&
                x.ReturnsVoid == false);

            return value is not null;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds the 'InvariantList' alike attributes that decorate the given type, or the first
    /// valid one in the given chains.
    /// </summary>
    static bool FindAttributes(
        INamedTypeSymbol type,
        out List<AttributeData> items, out AttributeData? unique,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        var found = type.Recursive((INamedTypeSymbol type, out List<AttributeData> items) =>
        {
            items = [..type.GetAttributes().Where(x =>
                x.AttributeClass is not null && (
                x.AttributeClass.Name.StartsWith(ListName) ||
                x.AttributeClass.Name.StartsWith(IListName)))];

            return items.Count > 0;
        },
        out items, chains);
        unique = found && items.Count == 1 ? items[0] : null;
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds the value of the <see cref="IInvariantListAttribute.ReturnType"/> property.
    /// </summary>
    static bool FindReturnTypeValue(
        AttributeData data, out INamedTypeSymbol value, out bool isnullable)
    {
        var name = nameof(IInvariantListAttribute.ReturnType);

        if (data.GetNamedArgument(name, out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
            {
                value = temp.UnwrapNullable(out isnullable);
                return true;
            }
        }

        value = null!;
        isnullable = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds the value of the <see cref="IInvariantListAttribute.VirtualMethod"/> property.
    /// </summary>
    static bool FindVirtualMethodValue(AttributeData data, out bool value)
    {
        var name = nameof(IInvariantListAttribute.VirtualMethod);

        if (data.GetNamedArgument(name, out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }

        value = false;
        return false;
    }

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(InvariantListGenerator)}}", "{{VersionDoc}}")]
        """;
}