namespace Yotei.Generators;

// ========================================================
/// <summary>
/// Represents the ability of emitting code for building a new instance of the given type,
/// using an optional list of parameters if such is needed.
/// </summary>
internal class NewItemBuilder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="typeSymbol"></param>
    public NewItemBuilder(TypeDeclarationSyntax typeSyntax, INamedTypeSymbol typeSymbol)
    {
        TypeSyntax = typeSyntax.ThrowIfNull(nameof(typeSyntax));
        TypeSymbol = typeSymbol.ThrowIfNull(nameof(typeSymbol));

        if (typeSymbol.IsNamespace) throw new ArgumentException(
            $"Named symbol is a namespace and not a type: {typeSymbol}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Builder: {TypeName}";

    /// <summary>
    /// The syntax of this type.
    /// </summary>
    public TypeDeclarationSyntax TypeSyntax { get; }

    /// <summary>
    /// The symbol of this type.
    /// </summary>
    public INamedTypeSymbol TypeSymbol { get; }

    /// <summary>
    /// The name of this type.
    /// </summary>
    public string TypeName => TypeSymbol.Name;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of suitable constructors of this type.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<IMethodSymbol> GetConstructors() =>
        TypeSymbol.GetMembers()
        .OfType<IMethodSymbol>()
        .Where(x => x.MethodKind == MethodKind.Constructor);

    public IEnumerable<IMethodSymbol> GetValidConstructors(IEnumerable<IMethodSymbol> items) =>
        items
        .Where(x => x.IsStatic == false)
        .OrderByDescending(x => x.Parameters.Length);

    /// <summary>
    /// Returns the collection of suitable properties of this type.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<IPropertySymbol> GetProperties()
    {
        var list = new NoDuplicatesList<IPropertySymbol>(new PropertyComparer());
        Populate(list, TypeSymbol);
        return list;

        void Populate(NoDuplicatesList<IPropertySymbol> list, INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>();
            list.AddRange(members);

            type = type.BaseType!; if (type == null) return;
            Populate(list, type);
        }
    }
    internal class PropertyComparer : IEqualityComparer<IPropertySymbol>
    {
        public bool Equals(IPropertySymbol x, IPropertySymbol y)
        {
            var typecomp = SymbolEqualityComparer.Default;
            if (!typecomp.Equals(x.Type, y.Type)) return false;
            if (x.Name != y.Name) return false;
            return true;
        }
        public int GetHashCode(IPropertySymbol obj)
            => SymbolEqualityComparer.Default.GetHashCode(obj);
    }

    public IEnumerable<IPropertySymbol> GetReadProperties(IEnumerable<IPropertySymbol> items) =>
        items.Where(x =>
        x.CanBeReferencedByName &&
        x.GetMethod != null &&
        x.IsStatic == false && (
        x.DeclaredAccessibility == Accessibility.Public ||
        x.DeclaredAccessibility == Accessibility.Protected));

    public IEnumerable<IPropertySymbol> GetWriteProperties(IEnumerable<IPropertySymbol> items) =>
        items.Where(x =>
        x.SetMethod != null);

    /// <summary>
    /// Gets the collection of suitable fields of this type.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<IFieldSymbol> GetFields()
    {
        var list = new NoDuplicatesList<IFieldSymbol>(new FieldComparer());
        Populate(list, TypeSymbol);
        return list;

        void Populate(NoDuplicatesList<IFieldSymbol> list, INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>();
            list.AddRange(members);

            type = type.BaseType!; if (type == null) return;
            Populate(list, type);
        }
    }
    internal class FieldComparer : IEqualityComparer<IFieldSymbol>
    {
        public bool Equals(IFieldSymbol x, IFieldSymbol y)
        {
            var typecomp = SymbolEqualityComparer.Default;
            if (!typecomp.Equals(x.Type, y.Type)) return false;
            if (x.Name != y.Name) return false;
            return true;
        }
        public int GetHashCode(IFieldSymbol obj)
            => SymbolEqualityComparer.Default.GetHashCode(obj);
    }

    public IEnumerable<IFieldSymbol> GetReadFields(IEnumerable<IFieldSymbol> items) =>
        items.Where(x =>
        x.CanBeReferencedByName &&
        x.AssociatedSymbol == null &&
        x.IsStatic == false && (
        x.DeclaredAccessibility == Accessibility.Public ||
        x.DeclaredAccessibility == Accessibility.Protected));

    public IEnumerable<IFieldSymbol> GetWriteFields(IEnumerable<IFieldSymbol> items) =>
        items.Where(x =>
        x.IsConst == false &&
        x.IsReadOnly == false);

    // ----------------------------------------------------

    /// <summary>
    /// Appends to the given code builder the code to assign to the variable whose name is given
    /// a new instance of this type, using the given optional list of mandatory arguments.
    /// Returns true if a suitable constructor was found, and the code was so emitted, or false
    /// otherwise.
    /// </summary>
    /// <param name="varName"></param>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public bool Print(
        string varName, SourceProductionContext context, CodeBuilder cb,
        params NewItemArgument[] args)
    {
        varName = varName.NotNullNotEmpty(nameof(varName));
        cb = cb.ThrowIfNull(nameof(cb));
        args = args.ThrowIfNull(nameof(args));

        if (RegularConstructor(varName, cb, args)) return true;
        if (CopyConstructor(false, varName, cb, args)) return true;
        if (CopyConstructor(true, varName, cb, args)) return true;
        if (EmptyConstructor(varName, cb, args)) return true;

        var str = args.Length == 0
            ? "<none>"
            : string.Join(", ", args.Select(x => x.ToString()));

        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            "Kerosene",
            "Cannot generate a new instance.",
            "Cannot generate a new instance of type '{0}' with arguments '{1}'.",
            "Build",
            DiagnosticSeverity.Error,
            true),
            TypeSyntax.GetLocation(),
            new object[] { TypeSymbol.Name, str }));

        cb.AppendLine("throw new System.NotImplementedException();");
        return false;
    }

    /// <summary>
    /// Emits code for a constructor with many arguments.
    /// </summary>
    /// <returns></returns>
    bool RegularConstructor(string varName, CodeBuilder cb, params NewItemArgument[] args)
    {
        var cons = GetValidConstructors(GetConstructors()).ToArray();
        foreach (var con in cons)
        {
            if (con.Parameters.Length == 0) continue;

            var props = GetReadProperties(GetProperties()).ToList();
            var fields = GetReadFields(GetFields()).ToList();
            var xargs = args.ToList();
            var pars = GetParameters(con.Parameters, xargs, props, fields);
            if (pars == null) continue;

            props = GetWriteProperties(props).ToList();
            fields = GetWriteFields(fields).ToList();
            var inits = GetInits(true, xargs, props, fields);
            if (xargs.Count > 0) continue;

            var str = string.Join(", ", pars.Select(x => x.Generate()));
            cb.Append($"var {varName} = new {TypeName}({str})");
            if (inits.Count > 0)
            {
                cb.AppendLine();
                cb.AppendLine("{");
                cb.Tabs++;

                foreach (var item in inits)
                {
                    var code = item.Generate();
                    cb.AppendLine($"{item.MatchName} = {code},");
                }

                cb.Tabs--;
                cb.Append("}");
            }
            cb.AppendLine(";");
            return true;
        }

        // Not found...
        return false;
    }

    /// <summary>
    /// Emits code for a copy constructor.
    /// </summary>
    /// <returns></returns>
    bool CopyConstructor(
        bool allowInterface, string varName, CodeBuilder cb, params NewItemArgument[] args)
    {
        var cons = GetValidConstructors(GetConstructors()).ToArray();
        foreach (var con in cons)
        {
            if (con.Parameters.Length != 1) continue;

            var par = con.Parameters[0];
            var valid = allowInterface
                ? TypeSymbol.IsAssignableTo(par.Type)
                : SymbolEqualityComparer.Default.Equals(TypeSymbol, par.Type);

            if (!valid) continue;

            var props = GetWriteProperties(GetReadProperties(GetProperties())).ToList();
            var fields = GetWriteFields(GetReadFields(GetFields())).ToList();
            var xargs = args.ToList();
            var inits = GetInits(false, xargs, props, fields);
            if (xargs.Count > 0) continue;

            cb.Append($"var {varName} = new {TypeName}(this)");
            if (inits.Count > 0)
            {
                cb.AppendLine();
                cb.AppendLine("{");
                cb.Tabs++;

                foreach (var item in inits)
                {
                    var code = item.Generate();
                    cb.AppendLine($"{item.MatchName} = {code},");
                }

                cb.Tabs--;
                cb.Append("}");
            }
            cb.AppendLine(";");
            return true;
        }

        // Not found...
        return false;
    }

    /// <summary>
    /// Emits code for a parameterless constructor.
    /// </summary>
    /// <returns></returns>
    bool EmptyConstructor(string varName, CodeBuilder cb, params NewItemArgument[] args)
    {
        var cons = GetValidConstructors(GetConstructors()).ToArray();
        foreach (var con in cons)
        {
            if (con.Parameters.Length != 0) continue;

            var props = GetWriteProperties(GetReadProperties(GetProperties())).ToList();
            var fields = GetWriteFields(GetReadFields(GetFields())).ToList();
            var xargs = args.ToList();
            var inits = GetInits(true, xargs, props, fields);
            if (xargs.Count > 0) continue;

            cb.Append($"var {varName} = new {TypeName}()");
            if (inits.Count > 0)
            {
                cb.AppendLine();
                cb.AppendLine("{");
                cb.Tabs++;

                foreach (var item in inits)
                {
                    var code = item.Generate();
                    cb.AppendLine($"{item.MatchName} = {code},");
                }

                cb.Tabs--;
                cb.Append("}");
            }
            cb.AppendLine(";");
            return true;
        }

        // Not found...
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the constructor parameters, adjusting the given lists, or null if there is no
    /// match for all required parameters. This method assumes the properties and fiels can
    /// be read.
    /// </summary>
    /// <returns></returns>
    List<NewItemArgument>? GetParameters(
        ImmutableArray<IParameterSymbol> pars,
        List<NewItemArgument> xargs,
        List<IPropertySymbol> props,
        List<IFieldSymbol> fields)
    {
        var list = new List<NewItemArgument>();
        foreach (var par in pars)
        {
            var xarg = xargs.SingleOrDefault(x =>
                x.Type.IsAssignableTo(par.Type) &&
                Compare(par.Name, x.MatchName));

            if (xarg != null)
            {
                xargs.Remove(xarg);
                list.Add(xarg);

                var tprop = props.SingleOrDefault(x =>
                    x.Type.IsAssignableTo(par.Type) &&
                    Compare(par.Name, x.Name));

                if (tprop != null) props.Remove(tprop);

                var tfield = fields.SingleOrDefault(x =>
                    x.Type.IsAssignableTo(par.Type) &&
                    Compare(par.Name, x.Name));

                if (tfield != null) fields.Remove(tfield);

                continue;
            }

            var prop = props.SingleOrDefault(x =>
                x.Type.IsAssignableTo(par.Type) &&
                Compare(par.Name, x.Name));

            if (prop != null)
            {
                props.Remove(prop);
                list.Add(new NewItemArgument(prop));
                continue;
            }

            var field = fields.SingleOrDefault(x =>
                x.Type.IsAssignableTo(par.Type) &&
                Compare(par.Name, x.Name));

            if (field != null)
            {
                fields.Remove(field);
                list.Add(new NewItemArgument(field));
                continue;
            }

            return null;
        }

        return list;
    }

    /// <summary>
    /// Gets the init parameters, adjusting the given lists. The <paramref name="addMembers"/>
    /// governs whether to add the remaining properties and fields, or not. This method assumes
    /// the properties and fields are writtable/init ones.
    /// </summary>
    /// <returns></returns>
    List<NewItemArgument> GetInits(
        bool addMembers,
        List<NewItemArgument> xargs,
        List<IPropertySymbol> props, List<IFieldSymbol> fields)
    {
        var list = new List<NewItemArgument>();

        var targs = xargs.ToList();
        foreach (var targ in targs)
        {
            var prop = props.SingleOrDefault(x =>
                targ.Type.IsAssignableTo(x.Type) &&
                Compare(x.Name, targ.MatchName));

            if (prop != null)
            {
                props.Remove(prop);
                xargs.Remove(targ);
                list.Add(targ);
                continue;
            }

            var field = fields.SingleOrDefault(x =>
                targ.Type.IsAssignableTo(x.Type) &&
                Compare(x.Name, targ.MatchName));

            if (field != null)
            {
                fields.Remove(field);
                xargs.Remove(targ);
                list.Add(targ);
                continue;
            }
        }

        if (addMembers)
        {
            foreach (var prop in props) list.Add(new NewItemArgument(prop)); props.Clear();
            foreach (var field in fields) list.Add(new NewItemArgument(field)); fields.Clear();
        }
        return list;
    }

    /// <summary>
    /// Determines if the given names match or not.
    /// </summary>
    static bool Compare(string source, string target)
    {
        var comp = StringComparison.OrdinalIgnoreCase;
        if (string.Compare(source, target, comp) == 0) return true;

        if (source.StartsWith("_")) return Compare(source.Substring(1), target);
        if (target.StartsWith("_")) return Compare(source, target.Substring(1));
        return false;
    }
}