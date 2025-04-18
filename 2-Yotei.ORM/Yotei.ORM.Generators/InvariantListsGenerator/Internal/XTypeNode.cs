namespace Yotei.ORM.Generators;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    const string IInvariantListNamespace = "Yotei.ORM.Tools";
    const string InvariantListNamespace = "Yotei.ORM.Tools.Code";

    const string IInvariantListName = "IInvariantList";
    const string InvariantListName = "InvariantList";

    AttributeData AttributeData = null!;
    INamedTypeSymbol AttributeClass = null!;
    int Arity = 0;
    INamedTypeSymbol KType = null!; bool KTypeNullable = false; // Arity == 2
    INamedTypeSymbol TType = null!; bool TTypeNullable = false; // Arity == 1 or 2

    Type Template = null!;
    IMethodSymbol? CloneMethod = null;
    string KTypeName = null!;
    string TTypeName = null!;
    string AttributeName = null!;
    string BracketName = null!;
    string InvariantNamespace = null!;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        // Base validations...
        if (!base.Validate(context)) return false;

        // Only one attribute is allowed...
        if (Candidate != null)
        {
            if (Candidate.Attributes.Length == 0) { InvariantListDiagnostics.NoAttributes(Symbol).Report(context); return false; }
            if (Candidate.Attributes.Length > 1) { InvariantListDiagnostics.TooManyAttributes(Symbol).Report(context); return false; }
            AttributeData = Candidate.Attributes[0];
        }
        else
        {
            var ats = Symbol.GetAttributes().Where(x =>
                x.AttributeClass != null &&
                x.AttributeClass.Name.Contains(InvariantListName))
                .ToArray();

            if (ats.Length == 0) { InvariantListDiagnostics.NoAttributes(Symbol).Report(context); return false; }
            if (ats.Length > 1) { InvariantListDiagnostics.TooManyAttributes(Symbol).Report(context); return false; }
            AttributeData = ats[0];
        }

        if ((AttributeClass = AttributeData.AttributeClass!) == null)
        {
            InvariantListDiagnostics.InvalidAttribute(Symbol).Report(context);
            return false;
        }

        // Capturing arity and types...
        Arity = AttributeClass.Arity;

        if (Arity == 0) // No generic parameters...
        {
            var args = AttributeData.ConstructorArguments;

            if (args.Length == 1)
            {
                if ((TType = GetType(args[0], out TTypeNullable, context)!) == null) return false;
                Arity = 1;
            }
            else if (args.Length == 2)
            {
                if ((KType = GetType(args[0], out KTypeNullable, context)!) == null) return false;
                if ((TType = GetType(args[1], out TTypeNullable, context)!) == null) return false;
                Arity = 2;
            }
            else
            {
                InvariantListDiagnostics.InvalidAttribute(Symbol).Report(context);
                return false;
            }
        }
        else if (Arity == 1) // One generic <T> parameter...
        {
            if ((TType = GetType((INamedTypeSymbol)AttributeClass.TypeArguments[0], out TTypeNullable, context)!) == null) return false;
        }
        else if (Arity == 2) // Two generic <K,T> parameters...
        {
            if ((KType = GetType((INamedTypeSymbol)AttributeClass.TypeArguments[0], out KTypeNullable, context)!) == null) return false;
            if ((TType = GetType((INamedTypeSymbol)AttributeClass.TypeArguments[1], out TTypeNullable, context)!) == null) return false;
        }
        else
        {
            InvariantListDiagnostics.InvalidAttribute(Symbol).Report(context);
            return false;
        }

        // Capturing the template to use...
        Template = Arity == 1
            ? typeof(IChainTemplate<>)
            : typeof(IChainTemplate<,>);

        // Determining if a clone method already exists in this type...
        CloneMethod = FindCloneMethod(Symbol);

        if (CloneMethod == null && !Symbol.IsInterface()) // We need a copy constructor...
        {
            var cons = Symbol.GetCopyConstructor();
            if (cons == null)
            {
                TreeDiagnostics.NoCopyConstructor(Symbol).Report(context);
                return false;
            }
        }

        // Finishing...
        KTypeName = KType?.EasyName(RoslynNameOptions.Full)!; if (KTypeNullable) KTypeName += "?";
        TTypeName = TType?.EasyName(RoslynNameOptions.Full)!; if (TTypeNullable) TTypeName += "?";

        AttributeName = AttributeClass.Name.RemoveEnd("Attribute");
        BracketName = Arity == 1 ? $"<{TTypeName}>" : $"<{KTypeName}, {TTypeName}>";

        InvariantNamespace = Symbol.IsInterface() ? IInvariantListNamespace : InvariantListNamespace;

        return true;
    }

    /// <summary>
    /// Invoked to get the actual type and if it is a nullable one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="isNullable"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    INamedTypeSymbol? GetType(TypedConstant source, out bool isNullable, SourceProductionContext context)
    {
        if (source.IsNull || source.Kind != TypedConstantKind.Type)
        {
            InvariantListDiagnostics.InvalidAttribute(Symbol).Report(context);
            isNullable = false;
            return null;
        }

        var type = (INamedTypeSymbol)source.Value!;

        if (type.Name is "Nullable" or "AsNullable")
        {
            if (type.TypeArguments.Length != 1)
            {
                InvariantListDiagnostics.InvalidAttribute(Symbol).Report(context);
                isNullable = false;
                return null;
            }

            type = (INamedTypeSymbol)type.TypeArguments[0];
            isNullable = true;
            return type;
        }
        else
        {
            isNullable = false;
            return type;
        }
    }

    /// <summary>
    /// Invoked to get the actual type and if it is a nullable one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="isNullable"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    INamedTypeSymbol? GetType(INamedTypeSymbol source, out bool isNullable, SourceProductionContext context)
    {
        if (source.Name is "Nullable" or "AsNullable")
        {
            if (source.TypeArguments.Length != 1)
            {
                InvariantListDiagnostics.InvalidAttribute(Symbol).Report(context);
                isNullable = false;
                return null;
            }

            source = (INamedTypeSymbol)source.TypeArguments[0];
            isNullable = true;
            return source;
        }
        else
        {
            isNullable = false;
            return source;
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override string? GetHeader(SourceProductionContext context)
    {
        var name = $"{InvariantNamespace}.{AttributeName}{BracketName}";
        var head = base.GetHeader(context) + $" : {name}";

        return head;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        var symbolname = Symbol.EasyName();
        var headdoc = Symbol.IsInterface() ? IInvariantListName : InvariantListName;

        // Emitting 'Clone()' if needed...
        if (CloneMethod == null)
        {
            cb.AppendLine("/// <inheritdoc cref=\"ICloneable.Clone\"/>");
            cb.AppendLine(AttributeDoc);

            cb.AppendLine(Symbol.IsInterface()
                ? $"new {symbolname} Clone();"
                : $"public override {symbolname} Clone() => new {symbolname}(this);");

            if (!Symbol.IsInterface())
            {
                foreach (var iface in FindCloneInterfaces(Symbol))
                {
                    var name = iface.EasyName(RoslynNameOptions.Full);

                    cb.AppendLine();
                    cb.AppendLine($"{name}");
                    cb.AppendLine($"{name}.Clone() => Clone();");
                }
            }
        }

        // The interface to use for base methods' reimplementation...
        var invface = FindInvariantListIface();
        var invfaceFull = invface?.EasyName(RoslynNameOptions.Full);
        var invfaceShort = invface?.EasyName();

        // Name options...
        var ioptions = EasyNameOptions.Default with // For iface method names...
        {
            UseMemberArgumentsTypes = EasyNameOptions.Full,
            UseMemberArgumentsNames = true,
        };
        var coptions = ioptions with // For base methods' invocations...
        {
            UseMemberArgumentsTypes = null,
        };

        // Iterating through the methods that may need implementation...
        var implemented = Symbol.GetMembers().OfType<IMethodSymbol>().ToArray();
        var methods = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);

        foreach (var method in methods)
        {
            if (!CanEmit(method, implemented)) continue;

            var mname = method.EasyName(ioptions);
            mname = mname.Replace("K ", $"{KTypeName} "); // K key...
            mname = mname.Replace("<K", $"<{KTypeName}"); // IComparer<K> comparer...
            mname = mname.Replace("T ", $"{TTypeName} "); // T item...
            mname = mname.Replace("T>", $"{TTypeName}>"); // IEnumerable<T> range...

            // Interfaces...
            if (Symbol.IsInterface())
            {
                var core = method.EasyName();
                core = $"{headdoc}.{core}";
                core = core.Replace('<', '{').Replace('>', '}');
                core = $"/// <inheritdoc cref=\"{core}\"/>";

                cb.AppendLine();
                cb.AppendLine(core);
                cb.AppendLine(AttributeDoc);

                cb.AppendLine($"new {symbolname} {mname};");
            }

            // Classes...
            else
            {
                if (Symbol.Name == "IdentifierChain") { } // DEBUG-ONLY

                var core = method.EasyName();
                var temp = invfaceShort ?? headdoc;
                core = $"{temp}.{core}";
                core = core.Replace('<', '{').Replace('>', '}');
                core = $"/// <inheritdoc cref=\"{core}\"/>";

                cb.AppendLine();
                cb.AppendLine(core);
                cb.AppendLine(AttributeDoc);

                var xname = symbolname;
                var args = method.EasyName(coptions);
                cb.AppendLine($"public override {xname} {mname}");
                cb.AppendLine($"=> ({xname})base.{args};");

                if (invface != null)
                {
                    cb.AppendLine();
                    cb.AppendLine($"{invfaceShort}");
                    cb.AppendLine($"{invfaceShort}.{mname} => {args};");
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given method can be emitted or not.
    /// </summary>
    bool CanEmit(MethodInfo method, IMethodSymbol[] implemented)
    {
        foreach (var declared in implemented)
        {
            var mname = method.Name;
            var dname = declared.Name;
            if (mname != dname) continue; // Names differ...

            var mpars = method.GetParameters();
            var dpars = declared.Parameters;
            if (mpars.Length != dpars.Length) continue; // Number of parameters differ...

            var count = mpars.Length;
            for (int i = 0; i < mpars.Length; i++)
            {
                var mpar = mpars[i];
                var dpar = dpars[i];

                if (!mpar.ParameterType.IsGenericParameter) // Template parameter not generic...
                {
                    var mtype = mpar.ParameterType;
                    var dtype = dpar.Type;

                    if (dtype.Match(mtype)) count--; // Found...
                }

                else // Template parameter being a generic one...
                {
                    var match = false;
                    var dtype = dpar.Type;
                    var comparer = SymbolComparer.Default;

                    if (KType != null && comparer.Equals(dtype, KType)) match = true;
                    if (TType != null && comparer.Equals(dtype, TType)) match = true;

                    if (match) count--; // Found...
                }
            }

            // All parameters matched, it is implemented...
            if (count == 0) return false;
        }

        // No impediments, we can emit code for the given method...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds the interfaces where 'Clone()' is declared, if any, staring from the interfaces
    /// of the given host.
    /// </summary>
    IEnumerable<INamedTypeSymbol> FindCloneInterfaces(INamedTypeSymbol host)
    {
        var comparer = SymbolComparer.Default;
        List<INamedTypeSymbol> items = [];

        foreach (var iface in host.Interfaces) Capture(iface);
        return items;

        // Tries to capture the given interface...
        void Capture(INamedTypeSymbol iface)
        {
            var found = false;

            var method = FindCloneMethod(iface);
            if (method != null) found = true;
            else
            {
                var ats = iface.GetAttributes().FirstOrDefault(x =>
                    x.AttributeClass != null &&
                    x.AttributeClass.Name.Contains(InvariantListName));

                if (ats != null) found = true;
            }

            if (found)
            {
                var temp = items.Find(x => comparer.Equals(iface, x));
                if (temp == null) items.Add(iface);
            }

            foreach (var child in iface.Interfaces) Capture(child);
        }
    }

    /// <summary>
    /// Returns the 'IInvariantList' -alike interface to reimplement, if any is used explicitly
    /// in the class declaration, or null if any is found. This method should only be called for
    /// class' hosts.
    /// </summary>
    INamedTypeSymbol? FindInvariantListIface()
    {
        if (Symbol.IsInterface()) return null; // We are not interested in interfaces...

        var iinvariantName = IInvariantListName + "Attribute";

        foreach (var iface in Symbol.Interfaces)
            if (IsInvariantAlike(iface)) return iface;

        return null;

        // Determines if the interface is an 'IInvariantList' -alike one.
        bool IsInvariantAlike(INamedTypeSymbol iface)
        {
            // Migth implement 'IInvariantList' directly...
            if (iface.Name.StartsWith(iinvariantName)) return true;

            // Or might be decorated with an 'Invariant' attribute...
            var ats = iface.GetAttributes();
            foreach (var at in ats)
            {
                if (at.AttributeClass != null &&
                    at.AttributeClass.Name.Contains(iinvariantName)) return true;
            }

            // Childs...
            foreach (var child in iface.Interfaces)
                if (IsInvariantAlike(child)) return true;

            // Not found...
            return false;
        }
    }

    // ----------------------------------------------------

    string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(InvariantListGenerator)}}", "{{VersionDoc}}")]
        """;

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a '<c>Clone()</c>' method in the given type, including also its base types
    /// and interfaces if requested. Returns null if not found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="chain"></param>
    /// <param name="ifaces"></param>
    /// <returns></returns>
    static IMethodSymbol? FindCloneMethod(ITypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var item = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0 &&
            x.ReturnsVoid == false);

        if (item == null && chain)
        {
            foreach (var temp in type.AllBaseTypes())
            {
                item = FindCloneMethod(temp);
                if (item != null) break;
            }
        }

        if (item == null && ifaces)
        {
            foreach (var temp in type.AllInterfaces)
            {
                item = FindCloneMethod(temp);
                if (item != null) break;
            }
        }

        return item;
    }
}