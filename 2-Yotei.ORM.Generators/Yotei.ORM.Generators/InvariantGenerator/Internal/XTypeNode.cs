
namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal partial class XTypeNode : TypeNode
{
    const string IINVARIANTBAG = "IInvariantBag";
    const string IINVARIANTLIST = "IInvariantList";
    const string INVARIANTBAG = "InvariantBag";
    const string INVARIANTLIST = "InvariantList";
    const string NAMESPACEAPI = "Yotei.ORM.Tools";
    const string NAMESPACECODE = "Yotei.ORM.Tools.Code";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol)
    {
        Attribute = default!;
        IsBag = default;
        Arity = default;
        KType = default!; KTypeName = default!; KTypeNullable = default;
        TType = default!; TTypeName = default!; TTypeNullable = default;
        Bracket = default!;
        Template = default!;
    }

    AttributeData Attribute;
    bool IsBag;
    int Arity;
    INamedTypeSymbol KType; string KTypeName; bool KTypeNullable;
    INamedTypeSymbol TType; string TTypeName; bool TTypeNullable;
    string Bracket;
    Type Template;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (!base.OnValidate(context)) return false;

        if (Symbol.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); return false; }

        // Obtaining the unique attribute...
        if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); return false; }
        else if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); return false; }
        else Attribute = Attributes[0];

        // Validating is a bag or a list...
        var atc = Attribute.AttributeClass!;
        IsBag = atc.Name.StartsWith(INVARIANTBAG) || atc.Name.StartsWith(IINVARIANTBAG);


        /// <summary>
        /// Attribute is not a generic one.
        /// </summary>
        Arity = atc.Arity;
        if (Arity == 0)
        {
            var args = Attribute.ConstructorArguments
                .Where(static x => !x.IsNull && x.Kind == TypedConstantKind.Type)
                .Select(static x => (INamedTypeSymbol)x.Value!)
                .ToArray();

            // For <T> attributes...
            if (args.Length == 1)
            {
                Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);
                TType = args[0].UnwrapNullable(out TTypeNullable);

                TTypeName = TType.EasyName(EasyTypeOptions.Full);
                if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

                Bracket = $"<{TTypeName}>";
                Arity = 1;
            }

            // For <K, T> attributes...
            else if (args.Length == 2)
            {
                Template = typeof(IListTemplate<,>);
                KType = args[0].UnwrapNullable(out KTypeNullable);
                TType = args[1].UnwrapNullable(out TTypeNullable);

                KTypeName = KType.EasyName(EasyTypeOptions.Full);
                TTypeName = KType.EasyName(EasyTypeOptions.Full);
                if (KTypeNullable && !KTypeName.EndsWith('?')) KTypeName += '?';
                if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

                Bracket = $"<{KTypeName}, {TTypeName}>";
                Arity = 2;
            }

            // Should not happen...
            else { Symbol.ReportError(TreeError.InvalidAttribute, context); return false; }
        }

        /// <summary>
        /// Attribute is a '<T>' one.
        /// </summary>
        else if (atc.Arity == 1)
        {
            Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);
            TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out TTypeNullable);

            TTypeName = TType.EasyName(EasyTypeOptions.Full);
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{TTypeName}>";
            Arity = 1;
        }

        /// <summary>
        ///Attribute is a '<K, T>' one. 
        /// </summary>
        else if (atc.Arity == 2)
        {
            Template = typeof(IListTemplate<,>);
            KType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out KTypeNullable);
            TType = ((INamedTypeSymbol)atc.TypeArguments[1]).UnwrapNullable(out TTypeNullable);

            KTypeName = KType.EasyName(EasyTypeOptions.Full);
            TTypeName = KType.EasyName(EasyTypeOptions.Full);
            if (KTypeNullable && !KTypeName.EndsWith('?')) KTypeName += '?';
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{KTypeName}, {TTypeName}>";
            Arity = 2;
        }

        // Invalid attribute...
        else { Symbol.ReportError(TreeError.InvalidAttribute, context); return false; }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override string? GetBaseList()
    {
        // Finding if the base invariant element is already present...
        var chains = new INamedTypeSymbol[][] {
            Symbol.IsInterface ? [.. Symbol.AllInterfaces] : [.. Symbol.AllBaseTypes] };

        var found = Finder.Find(chains, out INamedTypeSymbol? value, (type, out value) =>
        {
            if (type.Match(typeof(IInvariantBagAttribute)) ||
                type.Match(typeof(IInvariantBagAttribute<>)) ||
                type.Match(typeof(IInvariantListAttribute)) ||
                type.Match(typeof(IInvariantListAttribute<>)) ||
                type.Match(typeof(IInvariantListAttribute<,>)))
            {
                value = type;
                return true;
            }

            value = null;
            return false;
        });

        if (!found) // If not, let's add it...
        {
            var name = Attribute.AttributeClass!.Name;
            name = name.RemoveLast("Attribute").ToString();
            name += Bracket;
            return name;
        }

        return null; // As it was already present...
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected override bool OnEmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Get the appropriate return type...
        var rtype = Symbol.UnwrapNullable(out var rnull);
        if (HasReturnType(Attribute, out var xtype, out var xnull)) { rtype = xtype; rnull = xnull; }

        var options = ReturnOptions(rtype, Symbol);
        var stype = rtype.EasyName(options);
        var snull = rnull ? "?" : string.Empty;

        // Emit 'Clone' if needed...
        TryEmitClone(cb, stype, snull);

        // Used to emit the called methods...
        var moptions = new EasyMethodOptions
        {
            GenericOptions = new EasyTypeOptions
            {
                NamespaceStyle = EasyNamespaceStyle.Standard,
                UseHost = true,
                UseSpecialNames = true,
                NullableStyle = EasyNullableStyle.UseAnnotations,
                GenericStyle = EasyGenericStyle.UseEasyNames,
            },
            ParameterOptions = new EasyParameterOptions
            {
                UseModifiers = true,
                TypeOptions = EasyTypeOptions.Default with { NamespaceStyle = EasyNamespaceStyle.Standard },
                UseName = true,
            },
        };

        // Used to emit the args of the method...
        var argsoptions = moptions with
        {
            ParameterOptions = new EasyParameterOptions { UseName = true },
        };

        // Finding both template and existing methods...
        var methods = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);
        var existing = Symbol.GetMembers().OfType<IMethodSymbol>().ToDebugArray();

        // Iterating though the template methods...
        foreach (var method in methods)
        {
            // If already implemented nothing else to do...
            if (existing.Any(x => SameMethod(method, x))) continue;

            // Method header...
            cb.AppendLine();
            EmitDocumentation(method, cb);

            // Host is an interface...
            if (Symbol.IsInterface)
            {
                if (method.Name == "Remove") { } // DEBUG-ONLY

                var name = method.EasyName(moptions);
                name = ReplaceKT(name);

                cb.AppendLine($"new {stype}{snull} {name};");
                continue;
            }

            // Otherwise emit inheriting from the base class...
            else
            {
                var mods = Symbol.IsAbstract ? "abstract override" : "override";
                var name = method.EasyName(moptions);
                name = ReplaceKT(name);

                var args = method.EasyName(argsoptions);
                args = ReplaceKT(args);

                cb.AppendLine($"public {mods} {stype}{snull} {name}");
                cb.AppendLine($"=> ({stype}{snull})base.{args};");

                foreach (var iface in GetMethodInterfaces(method))
                {
                    cb.AppendLine();
                    cb.AppendLine(iface);
                    cb.AppendLine($"{iface}.{name} => {args};");
                }
            }
        }

        // Finishing...
        return true;
    }

    /// <summary>
    /// Invoked to obtain the interfaces that need implementation.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    List<string> GetMethodInterfaces(MethodInfo method)
    {
        List<INamedTypeSymbol> list = [];
        foreach (var iface in Symbol.Interfaces) TryCapture(iface);

        var items = list.Select(static x => x.EasyName(EasyTypeOptions.Full)).ToList();
        return items;

        /// <summary>
        /// Tries to capture the given interface as an explicit one.
        /// </summary>
        bool TryCapture(INamedTypeSymbol iface)
        {
            var found = false;

            // First, the iface child themselves...
            foreach (var child in iface.Interfaces) if (TryCapture(child)) found = true;

            // If no child, then try this interface itself...
            if (!found)
            {
                if (HasInvariantAttribute(iface, out _)) found = true;
                else
                {
                    var items = iface.GetMembers().OfType<IMethodSymbol>().ToDebugArray();
                    found = items.Any(x => SameMethod(method, x));
                }
            }

            // If found, add to the list...
            if (found)
            {
                var temp = list.Find(x => Comparer.Equals(x, iface));
                if (temp == null) list.Add(iface); 
            }

            // Finishing...
            return found;
        }
    }

    /// <summary>
    /// Used to replace generic arguments with the appropriate ones.
    /// </summary>
    string ReplaceKT(string item)
    {
        if (KType != null && KType.Name != "K")
        {
            if (item.Contains("K key")) item = item.Replace("K key", $"{KTypeName} key");
            if (item.Contains("K? key")) item = item.Replace("K? key", $"{KTypeName} key");
            if (item.Contains("<K")) item = item.Replace("<K", $"<{KTypeName}");
        }
        if (TType != null && TType.Name != "T")
        {
            if (item.Contains("T value")) item = item.Replace("T value", $"{TTypeName} value");
            if (item.Contains("T? value")) item = item.Replace("T? value", $"{TTypeName} value");
            if (item.Contains("T>")) item = item.Replace("T>", $"{TTypeName}>");
            if (item.Contains("T?>")) item = item.Replace("T?>", $"{TTypeName}?>");
        }
        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit 'Clone()' if needed.
    /// </summary>
    void TryEmitClone(CodeBuilder cb, string stype, string snull)
    {
        // If existing or requested, we're done...
        if (HasClone(Symbol, out _, out _)) return;

        // Documentation...
        cb.AppendLine($"/// <inheritdoc cref=\"ICloneable.Clone\"/>");
        cb.AppendLine($"{AttributeDoc}");

        // Interface...
        if (Symbol.IsInterface)
        {
            var name = Symbol.EasyName();
            cb.AppendLine($"new {name} Clone();");
            return;
        }

        // We know we're inheriting from a base class...
        if (Symbol.IsAbstract)
        {
            cb.AppendLine($"public abstract override {stype}{snull} Clone();");
        }
        else
        {
            var host = Symbol.EasyName();

            cb.AppendLine($"public override {stype}{snull} Clone()");
            cb.AppendLine("{");
            cb.IndentLevel++;
            {
                cb.AppendLine($"var v_host = new {host}(this);");
                cb.AppendLine($"return v_host;");
            }
            cb.IndentLevel--;
            cb.AppendLine("}");

            foreach (var iface in GetCloneInterfaces())
            {
                cb.AppendLine();
                cb.AppendLine(iface);
                cb.AppendLine($"{iface}.Clone() => Clone();");
            }
        }
    }

    /// <summary>
    /// Determines if a 'Clone' method is carried by the given type, or if it has been requested.
    /// Either case this method returns true, and in the first scenario returns the found method
    /// in the out argument and, in the second one, the requesting attribute in its own out one.
    /// If not found and not requested, returns false.
    /// </summary>
    /// 
    static bool HasClone(INamedTypeSymbol type, out IMethodSymbol? method, out AttributeData? at)
    {
        method = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(static x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0 &&
            x.ReturnsVoid == false);

        at = type.GetAttributes().FirstOrDefault(static x =>
            x.AttributeClass != null &&
            x.AttributeClass.Name == "Cloneable");

        return method is not null || at is not null;
    }

    /// <summary>
    /// Returns a list with the clone-alike interface needing explicit implementation.
    /// </summary>
    List<string> GetCloneInterfaces()
    {
        List<INamedTypeSymbol> list = [];
        foreach (var iface in Symbol.Interfaces) TryCapture(iface);

        var items = list.Select(static x => x.EasyName(EasyTypeOptions.Full)).ToList();
        return items;

        /// <summary>
        /// Tries to capture the given interface as an explicit one.
        /// </summary>
        bool TryCapture(INamedTypeSymbol iface)
        {
            var found = false;

            // First, the iface child themselves...
            foreach (var child in iface.Interfaces) if (TryCapture(child)) found = true;

            // If no child, then try this interface itself...
            if (!found)
            {
                found =
                    HasInvariantAttribute(iface, out _) ||
                    HasClone(iface, out _, out _);
            }

            // If found, add to the list...
            if (found)
            {
                var temp = list.Find(x => Comparer.Equals(x, iface));
                if (temp == null) list.Add(iface);
            }

            // Finishing...
            return found;
        }
    }
}