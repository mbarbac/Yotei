namespace Yotei.ORM.Generators.InvariantGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
public partial class XTypeNode : TypeNode
{
    const string IINVARIANTBAG = "IInvariantBag", INVARIANTBAG = "InvariantBag";
    const string IINVARIANTLIST = "IInvariantList", INVARIANTLIST = "InvariantList";
    const string NAMESPACEAPI = "Yotei.ORM.Tools", NAMESPACECODE = "Yotei.ORM.Tools";

    AttributeData Attribute;
    bool IsBag;
    int Arity;
    INamedTypeSymbol KType; string KTypeName; bool KTypeNullable;
    INamedTypeSymbol TType; string TTypeName; bool TTypeNullable;
    string Bracket;
    Type Template;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol)
    {
        Attribute = default!;
        IsBag = default!;
        Arity = default!;
        KType = default!; KTypeName = default!; KTypeNullable = default;
        TType = default!; TTypeName = default!; TTypeNullable = default;
        Bracket = default!;
        Template = default!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        // Host type's kind validations...
        if (Symbol.IsRecord) { TreeError.RecordsNotSupported.Report(Symbol, context); r = false; }
        if (Symbol.TypeKind == TypeKind.Struct) { TreeError.KindNotSupported.Report(Symbol, context); r = false; }

        // Just one attribute allowed...
        if (Attributes.Count == 0) { TreeError.NoAttributes.Report(Symbol, context); r = false; }
        else if (Attributes.Count > 1) { TreeError.TooManyAttributes.Report(Symbol, context); r = false; }
        else Attribute = Attributes[0];

        // Capturing whether the attribute is for a bag or a list...
        var atc = Attribute.AttributeClass!;
        IsBag = atc.Name.StartsWith(IINVARIANTBAG) || atc.Name.StartsWith(INVARIANTBAG);

        // Capturing arity and related settings...
        Arity = atc.Arity;

        var options = EasyTypeOptions.Full with
        { UseSpecialNames = true, NullableStyle = EasyNullableStyle.UseAnnotations  };

        // Attribute is a not-generic one...
        if (Arity == 0)
        {
            var args = Attribute.ConstructorArguments
                .Where(static x => !x.IsNull && x.Kind == TypedConstantKind.Type)
                .Select(static x => (INamedTypeSymbol)x.Value!)
                .ToArray();

            if (args.Length == 1) // One [T] argument (can be a bag or a list)...
            {
                Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);

                TType = args[0].UnwrapNullable(out TTypeNullable);
                TTypeName = TType.EasyName(options);
                if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

                Bracket = $"<{TTypeName}>";
                Arity = 1;
            }

            else if (args.Length == 2) // Two [K,T] arguments (must be a list)...
            {
                Template = typeof(IListTemplate<,>);
                KType = args[0].UnwrapNullable(out KTypeNullable);
                TType = args[1].UnwrapNullable(out TTypeNullable);

                KTypeName = KType.EasyName(options);
                TTypeName = TType.EasyName(options);
                if (KTypeNullable && !KTypeName.EndsWith('?')) KTypeName += '?';
                if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

                Bracket = $"<{KTypeName}, {TTypeName}>";
                Arity = 2;
            }

            else { TreeError.InvalidAttribute.Report(Symbol, context); r = false; } // Invalid
        }

        // Attribute is a '[T]' one (can be a bag or a list)...
        else if (Arity == 1)
        {
            Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);

            TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out TTypeNullable);
            TTypeName = TType.EasyName(options);
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{TTypeName}>";
            Arity = 1;
        }

        // Attribute is a '[K,T]' one (must be a list)...
        else if (Arity == 2)
        {
            Template = typeof(IListTemplate<,>);
            KType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out KTypeNullable);
            TType = ((INamedTypeSymbol)atc.TypeArguments[1]).UnwrapNullable(out TTypeNullable);

            KTypeName = KType.EasyName(options);
            TTypeName = TType.EasyName(options);
            if (KTypeNullable && !KTypeName.EndsWith('?')) KTypeName += '?';
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{KTypeName}, {TTypeName}>";
            Arity = 2;
        }

        // Otherwise, invalid arity...
        else { TreeError.InvalidAttribute.Report(Symbol, context); r = false; }

        // Finishing...
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override string? GetBaseList()
    {
        IEnumerable<INamedTypeSymbol>[] chains =
            [Symbol.IsInterface ? [.. Symbol.AllInterfaces] : [.. Symbol.AllBaseTypes]];

        var found = Finder.Find(chains, out INamedTypeSymbol? value, (type, out value) =>
        {
            if (type.MatchAny([
                typeof(IInvariantBagAttribute), typeof(IInvariantBagAttribute<>),
                typeof(IInvariantListAttribute),
                typeof(IInvariantListAttribute<,>),typeof(IInvariantListAttribute<>),]))
            {
                value = type;
                return true;
            }

            value = null;
            return false;
        });

        if (!found) // Lets add to the base list...
        {
            var name = Attribute.AttributeClass!.Name;
            name = name.RemoveLast("Attribute").ToString();
            name += Bracket;
            return name;
        }

        return null; // If found we need not to add...
    }
    
    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected override bool OnEmitCore(ref TreeContext context, CodeBuilder cb)
    {
        // Obtain the return type of the generated methods...
        var hasrtype = HasReturnType(Attribute, out var rtype, out var rnull);
        if (!hasrtype) { rtype = Symbol; rnull = false; }
        var options = GetReturnOptions(rtype!, Symbol);
        var strtype = rtype!.EasyName(options);
        var strnull = rnull ? "?" : string.Empty;

        // Emit 'Clone' if needed...
        TryEmitClone(cb, strtype, strnull);

        // Finding both template and existing methods...
        var methods = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);
        var existing = Symbol.GetMembers().OfType<IMethodSymbol>().ToDebugArray();

        var toptions = new EasyTypeOptions() // Factorizes easy type options...
        {
            NamespaceStyle = EasyNamespaceStyle.Default,
            UseHost = true,
            UseSpecialNames = true,
            NullableStyle = EasyNullableStyle.UseAnnotations,
            GenericListStyle = EasyGenericListStyle.UseNames,
        };
        var moptions = new EasyMethodOptions() // Easy method options...
        {
            GenericArgumentOptions = toptions,
            ParameterOptions = new EasyParameterOptions()
            {
                UseModifiers = true,
                TypeOptions = toptions,
                UseName = true
            }
        };
        var argoptions = new EasyMethodOptions() // Method options with argument names only...
        {
            GenericArgumentOptions = toptions,
            ParameterOptions = new EasyParameterOptions() { UseName = true }
        };

        // Iterating through the template methods...
        var first = true;
        foreach (var method in methods)
        {
            // If already implemented, do the next method...
            if (existing.Any(x => SameMethod(method, x))) continue;

            // Method header...
            if (!first) cb.AppendLine(); first = false;
            EmitDocumentation(method, cb);

            // Host is an interface...
            if (Symbol.IsInterface)
            {
                var name = method.EasyName(moptions);
                name = ReplaceKT(name);

                cb.AppendLine($"new {strtype}{strnull} {name};");
                continue;
            }

            // Host is a class...
            else
            {
                var mods = Symbol.IsAbstract ? "abstract override" : "override";
                var name = method.EasyName(moptions);
                name = ReplaceKT(name);

                var args = method.EasyName(argoptions);
                args = ReplaceKT(args);

                cb.AppendLine($"public {mods} {strtype}{strnull} {name}");
                cb.AppendLine($"=> ({strtype}{strnull})base.{args};");

                var ifaces = GetMethodInterfaces(method);
                foreach (var iface in ifaces)
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
    /// Obtains the list of interfaces that need explicit implementation, if any.
    /// </summary>
    List<string> GetMethodInterfaces(MethodInfo method)
    {
        var comparer = SymbolEqualityComparer.Default;

        List<INamedTypeSymbol> list = [];
        foreach (var iface in Symbol.Interfaces) TryCapture(iface);

        var items = list.Select(static x => x.EasyName(EasyTypeOptions.Full)).ToList();
        return items;

        /// <summary>
        /// Tries to capture the given interface.
        /// </summary>
        void TryCapture(INamedTypeSymbol iface)
        {
            // Childs first...
            foreach (var child in iface.Interfaces) TryCapture(child);

            // If already captured, we're done...
            var temp = list.Find(x => comparer.Equals(x, iface));
            if (temp is not null) return;

            // Method already exist...
            var found = HasInvariantAttributes(iface, out _);
            if (!found)
            {
                var items = iface.GetMembers().OfType<IMethodSymbol>();
                found = items.Any(x => SameMethod(method, x));
            }

            // Add to the list...
            if (found)
            {
                var other = list.Find(x => comparer.Equals(x, iface));
                if (other is null) list.Add(iface);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit 'Clone()' if such is needed.
    /// </summary>
    void TryEmitClone(CodeBuilder cb, string strtype, string strnull)
    {
        // If existing or requested, we're done...
        if (HasClone(Symbol, out _, out _)) return;

        // Documentation...
        cb.AppendLine($"/// <inheritdoc cref=\"ICloneable.Clone\"/>");
        cb.AppendLine($"{DocAttribute}");

        // Host is interface...
        if (Symbol.IsInterface)
        {
            var name = Symbol.EasyName();
            cb.AppendLine($"new {name} Clone();");
            return;
        }

        // Host is abstract class...
        else if (Symbol.IsAbstract)
        {
            cb.AppendLine($"public abstract override {strtype}{strnull} Clone();");
        }

        // Host is a regular type...
        else
        {
            var host = Symbol.EasyName();

            cb.AppendLine($"public override {strtype}{strnull} Clone()");
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
    /// Determines if the given type has a 'Clone()' method, or if it has been requested. In the
    /// first case, returns true and the found method in the out argument. In the second one, also
    /// returns true but the requesting attribute in its out argument. Otherwise, returns false and
    /// the out arguments are undefined.
    /// </summary>
    static bool HasClone(INamedTypeSymbol type, out IMethodSymbol? method, out AttributeData? attr)
    {
        method = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(static x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0 &&
            x.ReturnsVoid == false);

        attr = type.GetAttributes().FirstOrDefault(static x =>
            x.AttributeClass != null &&
            x.AttributeClass.Name == "Cloneable");

        return method != null && attr != null;
    }

    /// <summary>
    /// Obtains the list of interfaces that need explicit implementation, if any.
    /// </summary>
    List<string> GetCloneInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;

        List<INamedTypeSymbol> list = [];
        foreach (var iface in Symbol.Interfaces) TryCapture(iface);

        var items = list.Select(static x => x.EasyName(EasyTypeOptions.Full)).ToList();
        return items;

        /// <summary>
        /// Tries to capture the given interface.
        /// </summary>
        void TryCapture(INamedTypeSymbol iface)
        {
            // Childs first...
            foreach (var child in iface.Interfaces) TryCapture(child);

            // If already captured, we're done...
            var temp = list.Find(x => comparer.Equals(x, iface));
            if (temp is not null) return;

            // Method already exist...
            var found =
                HasInvariantAttributes(iface, out _) ||
                HasClone(iface, out _, out _);

            // Add to the list...
            if (found)
            {
                var other = list.Find(x => comparer.Equals(x, iface));
                if (other is null) list.Add(iface);
            }
        }
    }
}