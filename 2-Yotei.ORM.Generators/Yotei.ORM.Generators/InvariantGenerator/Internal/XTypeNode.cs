namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol)
    {
        ReturnType = Symbol;
        ReturnNullable = false;
        ReturnOptions = EasyNameOptions.Default;
        Arity = 0;
        KType = null!; KTypeNullable = false; KTypeName = null!;
        TType = null!; TTypeNullable = false; TTypeName = null!;
        Template = null!;
        Bracket = null!;
    }
    INamedTypeSymbol ReturnType;
    bool ReturnNullable;
    EasyNameOptions ReturnOptions;
    int Arity;
    INamedTypeSymbol KType; bool KTypeNullable; string KTypeName;
    INamedTypeSymbol TType; bool TTypeNullable; string TTypeName;
    Type Template;
    string Bracket;

    const string TheNamespace = "Yotei.ORM.Tools";
    const string IListName = "IInvariantList";
    const string ListName = "InvariantList";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        if (Symbol.IsRecord) { CoreDiagnostics.RecordsNotSupported(Symbol).Report(context); return false; }
        if (Attributes.Length == 0) { CoreDiagnostics.NoAttributes(Symbol).Report(context); return false; }
        if (Attributes.Length > 1) { CoreDiagnostics.TooManyAttributes(Symbol).Report(context); return false; }

        var at = Attributes[0];
        var atc = at.AttributeClass;
        if (atc is null) { CoreDiagnostics.InvalidAttribute(Symbol).Report(context); return false; }

        // Getting the return type, if any is requested...
        if (GetReturnType(at, out var type, out var nullable))
        {
            ReturnType = type;
            ReturnNullable = nullable;

            var same = SymbolEqualityComparer.Default.Equals(Symbol, type);
            if (!same) ReturnOptions = EasyNameOptions.Full with
            { TypeUseNullable = false };
        }

        // Attribute uses no generic arguments...
        if (atc.Arity == 0)
        {
            var args = at.ConstructorArguments
                .Where(x => !x.IsNull && x.Kind == TypedConstantKind.Type)
                .Select(x => (INamedTypeSymbol)x.Value!)
                .ToArray();

            if (args.Length == 1) // One type argument: <T>...
            {
                Template = typeof(IChainTemplate<>);
                Arity = 1;
                TType = args[0].UnwrapNullable(out TTypeNullable);
                TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
                Bracket = $"<{TTypeName}>";
            }

            if (args.Length == 2) // Two type arguments: <K, T>...
            {
                Template = typeof(IChainTemplate<,>);
                Arity = 2;
                KType = args[0].UnwrapNullable(out KTypeNullable);
                TType = args[1].UnwrapNullable(out TTypeNullable);

                KTypeName = KType.EasyName(EasyNameOptions.Full) + (KTypeNullable ? "?" : "");
                TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
                Bracket = $"<{KTypeName}, {TTypeName}>";
            }
        }

        // Attribute uses one generic argument: <T>...
        if (atc.Arity == 1)
        {
            Template = typeof(IChainTemplate<>);
            Arity = 1;
            TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out TTypeNullable);
            TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
            Bracket = $"<{TTypeName}>";
        }

        // Attribute uses two generic arguments: <K, T>...
        if (atc.Arity == 2)
        {
            Template = typeof(IChainTemplate<,>);
            Arity = 2;
            KType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out KTypeNullable);
            TType = ((INamedTypeSymbol)atc.TypeArguments[1]).UnwrapNullable(out TTypeNullable);

            KTypeName = KType.EasyName(EasyNameOptions.Full) + (KTypeNullable ? "?" : "");
            TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
            Bracket = $"<{KTypeName}, {TTypeName}>";
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override string GetHeader(SourceProductionContext context)
    {
        var head = base.GetHeader(context);
        var need = !HasHeader(out var name);
        if (need) head += $" : {TheNamespace}.{name}";
        return head;
    }

    /// <summary>
    /// Determines if this type already has a base element of the appropriate 'InvariantList' or
    /// 'IInvariantList' type, either by itself, or through inheritance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool HasHeader(out string? name)
    {
        var atr = Attributes[0].AttributeClass!.Name;
        atr = atr.RemoveTail("Attribute").ToString();
        atr += Bracket;
        name = atr;

        var args = EasyNameOptions.Full;
        var options = EasyNameOptions.Default with { TypeGenericArgumentOptions = args };

        return Symbol.Finder<bool>(false, (type, out value) =>
        {
            var temp = type.EasyName(options);
            value = string.Compare(atr, temp) == 0;
            return value;
        },
        out _, Symbol.AllBaseTypes, Symbol.AllInterfaces);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <param name="needNL"></param>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Try to emit 'Clone()' if needed...
        EmitClone(cb);

        // Iterating through the template methods...
        var existing = Symbol.GetMembers().OfType<IMethodSymbol>().ToArray();
        var methods = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);

        foreach (var method in methods)
        {
            if (existing.Any(x => SameMethod(method, x))) continue; // Already exists...

            // Documentation...
            var headdoc = Symbol.IsInterface ? IListName : ListName;
            var name = method.EasyName();
            name = $"{headdoc}{Bracket}.{name}";
            name = name.Replace('<', '{').Replace('>', '}');
            name = $"/// <inheritdoc cref=\"{name}\"/>";

            cb.AppendLine();
            cb.AppendLine(name);
            cb.AppendLine($"{InvariantGenerator.AttributeDoc}");

            // Emitting method...
            var options = EasyNameOptions.Default with
            { MemberArgumentTypeOptions = EasyNameOptions.Full, MemberUseArgumentNames = true };

            name = method.EasyName(options);
            name = name.Replace("K ", $"{KTypeName} "); // K key...
            name = name.Replace("<K", $"<{KTypeName}"); // IComparer<K> comparer...
            name = name.Replace("T ", $"{TTypeName} "); // T item...
            name = namexx.Replace("T>", $"{TTypeName}>"); // IEnumerable<T> range...

            // Host is interface...
            if (Symbol.IsInterface)
            {
                var rtype = ReturnType.EasyName(ReturnOptions);
                var rnull = ReturnNullable ? "?" : string.Empty;
            }

            // Otherwise, inheriting from an abstract class...
            else
            {


                // Explicit interfaces...
            }
        }
    }

    /// <summary>
    /// Determines if the given method is the same as the existing one.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="existing"></param>
    /// <returns></returns>
    public bool SameMethod(MethodInfo method, IMethodSymbol existing)
    {
        var mname = method.Name;
        var ename = existing.Name;
        if (mname != ename) return false; // Names differ...

        var mpars = method.GetParameters();
        var epars = existing.Parameters;
        if (mpars.Length != epars.Length) return false; // Number of pars differ...

        for (int i = 0; i < mpars.Length; i++) // Comparing their arguments, in order...
        {
            var mpar = mpars[i]; var mtype = mpar.ParameterType;
            var epar = epars[i]; var etype = (INamedTypeSymbol)epar.Type;

            var same = SameArgument(mtype, etype);
            if (!same) return false;
        }

        // Same method...
        return true;
    }

    /// <summary>
    /// Determines if the given parameter types are the same: if we only use 'Match', then any
    /// generic argument will always match, and we don't want that to happen when the argument
    /// if not the "K" or "T" given ones.
    /// </summary>
    /// <param name="mpar"></param>
    /// <param name="epar"></param>
    /// <returns></returns>
    public bool SameArgument(Type mtype, INamedTypeSymbol etype)
    {
        var comparer = SymbolEqualityComparer.Default;
        switch (mtype.Name)
        {
            case "K": if (!comparer.Equals(KType, etype)) return false; break;
            case "T": if (!comparer.Equals(TType, etype)) return false; break;
            default: if (!etype.Match(mtype)) return false; break;
        }

        var margs = mtype.GenericTypeArguments;
        var eargs = etype.TypeArguments;
        if (margs.Length != eargs.Length) return false;

        for (int i = 0; i < margs.Length; i++)
        {
            var marg = margs[i];
            var earg = eargs[i];
            var same = SameArgument(marg, (INamedTypeSymbol)earg);
            if (!same) return false;
        }
        
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to emit the 'Clone' method.
    /// </summary>
    /// <param name="cb"></param>
    public void EmitClone(CodeBuilder cb)
    {
        // If existing or requested by the host, skip...
        if (HasClone(Symbol, out _, out _)) return;

        // Documentation...
        cb.AppendLine($"/// <inheritdoc cref=\"ICloneable.Clone\"/>");
        cb.AppendLine($"{InvariantGenerator.AttributeDoc}");

        // Interface...
        if (Symbol.IsInterface)
        {
            var name = Symbol.EasyName();
            cb.AppendLine($"new {name} Clone();");
        }

        // Otherwise we are inheriting from an abstract class...
        else
        {
            var rtype = ReturnType.EasyName(ReturnOptions);
            var rnull = ReturnNullable ? "?" : string.Empty;
            var modifiers = Symbol.IsAbstract ? "public abstract override " : "public override ";

            if (Symbol.IsAbstract) cb.AppendLine($"{modifiers}{rtype}{rnull} Clone();");
            else
            {
                var hostname = Symbol.EasyName();

                cb.AppendLine($"{modifiers}{rtype}{rnull} Clone()");
                cb.AppendLine("{");
                cb.IndentLevel++;
                {
                    cb.AppendLine($"var v_host = new {hostname}(this);");
                    cb.AppendLine("return v_host;");
                }
                cb.IndentLevel--;
                cb.AppendLine("}");
            }

            // Explicit 'Clone' interfaces...
            foreach (var iface in GetCloneInterfaces())
            {
                cb.AppendLine();
                cb.AppendLine($"{iface}");
                cb.AppendLine($"{iface}.Clone() => Clone();");
            }
        }
    }

    /// <summary>
    /// Determines if a 'Clone()' method exists or has been requested for the given type.
    /// </summary>
    /// <returns></returns>
    public static bool HasClone(
        INamedTypeSymbol type, out IMethodSymbol? method, out AttributeData? atr)
    {
        method = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0 &&
            x.ReturnsVoid == false);

        atr = type.GetAttributes().FirstOrDefault(x =>
            x.AttributeClass is not null &&
            x.AttributeClass.Name == "Cloneable");

        return method is not null || atr is not null;
    }

    /// <summary>
    /// Obtains a list with the 'Clone'-alike interfaces that need explicit implementation.
    /// </summary>
    /// <returns></returns>
    public List<string> GetCloneInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        List<INamedTypeSymbol> list = [];
        foreach (var iface in Symbol.Interfaces) TryCapture(iface);

        var items = list.Select(x => x.EasyName(EasyNameOptions.Full)).ToList();
        return items;

        // Tries to capture the given interface as an explicit one.
        bool TryCapture(INamedTypeSymbol iface)
        {
            var found = false;

            // First, its childs...
            foreach (var child in iface.Interfaces) if (TryCapture(child)) found = true;

            // If no child, then maybe this interface by itself...
            if (!found)
            {
                found =
                    FindInvariantAttribute(iface, true, out _) || // Acts as 'Cloneable' !!!
                    HasClone(iface, out _, out _);
            }

            // If found, add to the list...
            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp is null) list.Add(iface);
            }

            // Finishing...
            return found;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a method with the given name and argument types, either in the given type
    /// or in the first one in the given chains. In strict mode, the argument types must match.
    /// Otherwise, it is just enough if they are assignable.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="usehost"></param>
    /// <param name="name"></param>
    /// <param name="argtypes"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FindMethod(
        INamedTypeSymbol type,
        bool usehost,
        string name,
        ITypeSymbol[] argtypes,
        bool strict,
        [NotNullWhen(true)] out IMethodSymbol? value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        name = name.NotNullNotEmpty(true);
        argtypes.ThrowWhenNull();
        chains.ThrowWhenNull();

        var found = type.Finder(usehost, (type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == name &&
                x.Parameters.Length == argtypes.Length &&
                argtypes
                    .Select((t, i) => new { T = t, I = i })
                    .All(arg => strict ?
                        SymbolEqualityComparer.Default.Equals(arg.T, x.Parameters[arg.I].Type) :
                        arg.T.IsAssignableTo(x.Parameters[arg.I].Type)));

            return value is not null;
        },
        out value, chains);
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a <see cref="InvariantListAttribute"/> in the given type, or in the first
    /// one in the given chains. If any is found, returns the first decorating attribute.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="usehost"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FindInvariantAttribute(
        INamedTypeSymbol type,
        bool usehost,
        [NotNullWhen(true)] out AttributeData? value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        chains.ThrowWhenNull();

        return type.Finder(usehost, (type, out value) =>
        {
            value = type.GetAttributes().FirstOrDefault(x =>
                x.AttributeClass is not null && (
                x.AttributeClass.Name.StartsWith("InvariantList") ||
                x.AttributeClass.Name.StartsWith("IInvariantList")));

            return value is not null;
        },
        out value, chains);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the value of the <see cref="IInvariantListAttribute.ReturnType"/>
    /// setting from the given attribute.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static bool GetReturnType(
        AttributeData at,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        var name = nameof(IInvariantListAttribute.ReturnType);

        if (at.GetNamedArgument(name, out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
            {
                value = temp.UnwrapNullable(out nullable);
                return true;
            }
        }

        value = null;
        nullable = false;
        return false;
    }
}