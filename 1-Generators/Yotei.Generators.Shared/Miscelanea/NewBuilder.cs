using System.Reflection.Metadata;

namespace Yotei.Generators;

// ========================================================
/// <summary>
/// Represents the ability of emitting code for creating a new instance of the associated type.
/// Hence, this facility should not be called for interfaces or other types for which not a new
/// operation is valid.
/// </summary>
internal class NewBuilder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="typeSymbol"></param>
    public NewBuilder(INamedTypeSymbol typeSymbol)
    {
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
    /// The symbol of this type.
    /// </summary>
    public INamedTypeSymbol TypeSymbol { get; }

    /// <summary>
    /// The name of this type.
    /// </summary>
    public string TypeName => TypeSymbol.Name;

    // ----------------------------------------------------

    static string CloneableMemberAttribute => nameof(CloneableMemberAttribute);
    static string CloneableMemberDeep => nameof(CloneableMemberDeep);
    static string CloneableMemberIgnore => nameof(CloneableMemberIgnore);

    /// <summary>
    /// Determines if the given property is a cloneable one, or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsCloneable(IPropertySymbol symbol)
    {
        if (symbol.HasAttribute(CloneableMemberAttribute, out var data))
        {
            var arg = data.GetNamedArgument(CloneableMemberIgnore);
            if (arg != null &&
                arg.Value.Value is bool valueIgnore && valueIgnore) return false;

            data.GetNamedArgument(CloneableMemberDeep);
            if (arg != null &&
                arg.Value.Value is bool valueDeep && valueDeep) return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if the given property is a cloneable one, or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsCloneable(IFieldSymbol symbol)
    {
        if (symbol.HasAttribute(CloneableMemberAttribute, out var data))
        {
            var arg = data.GetNamedArgument(CloneableMemberIgnore);
            if (arg != null &&
                arg.Value.Value is bool valueIgnore && valueIgnore) return false;

            data.GetNamedArgument(CloneableMemberDeep);
            if (arg != null &&
                arg.Value.Value is bool valueDeep && valueDeep) return true;
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of constructors for this type, in descending order by the number
    /// of parameters each.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> GetConstructors() => TypeSymbol.GetMembers()
        .OfType<IMethodSymbol>()
        .Where(x =>
        x.MethodKind == MethodKind.Constructor &&
        x.IsStatic == false)
        .OrderByDescending(x => x.Parameters.Length)
        .ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of properties for this type.
    /// </summary>
    /// <returns></returns>
    List<IPropertySymbol> GetProperties()
    {
        var list = new NoDuplicatesList<IPropertySymbol>(new PropertyComparer());
        Populate(list, TypeSymbol);
        return list.ToList();

        void Populate(NoDuplicatesList<IPropertySymbol> list, INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().ToDebugArray();
            foreach (var member in members)
            {
                var temp = list.Any(x => list.Comparer.Equals(x, member));
                if (!temp) list.Add(member);
            }

            var parent = type.BaseType;
            if (parent != null) Populate(list, parent);
        }
    }

    /// <summary>
    /// Returns the collection of read properties in the given set.
    /// </summary>
    /// <param name="props"></param>
    /// <returns></returns>
    List<IPropertySymbol> GetReadProperties(List<IPropertySymbol> props) => props.Where(x =>
        x.CanBeReferencedByName &&
        x.GetMethod != null &&
        x.IsStatic == false && (
        x.DeclaredAccessibility == Accessibility.Public ||
        x.DeclaredAccessibility == Accessibility.Protected))
        .ToList();

    /// <summary>
    /// Returns the collection of write properties in the given set.
    /// </summary>
    /// <param name="props"></param>
    /// <returns></returns>
    List<IPropertySymbol> GetWriteProperties(List<IPropertySymbol> props) => props.Where(x =>
        x.SetMethod != null &&
        x.IsWriteOnly == false)
        .ToList();

    /// <summary>
    /// A comparer for properties.
    /// </summary>
    class PropertyComparer : IEqualityComparer<IPropertySymbol>
    {
        public bool Equals(IPropertySymbol x, IPropertySymbol y)
        {
            if (!SymbolEqualityComparer.Default.Equals(x.Type, y.Type)) return false;
            if (x.Name != y.Name) return false;
            return true;
        }

        public int GetHashCode(IPropertySymbol obj)
            => SymbolEqualityComparer.Default.GetHashCode(obj);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of fields for this type.
    /// </summary>
    /// <returns></returns>
    List<IFieldSymbol> GetFields()
    {
        var list = new NoDuplicatesList<IFieldSymbol>(new FieldComparer());
        Populate(list, TypeSymbol);
        return list.ToList();

        void Populate(NoDuplicatesList<IFieldSymbol> list, INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>();
            foreach (var member in members)
            {
                var temp = list.Any(x => list.Comparer.Equals(x, member));
                if (!temp) list.Add(member);
            }

            var parent = type.BaseType;
            if (parent != null) Populate(list, parent);
        }
    }

    /// <summary>
    /// Returns the collection of read fields in the given set.
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    List<IFieldSymbol> GetReadFields(List<IFieldSymbol> fields) => fields.Where(x =>
        x.CanBeReferencedByName &&
        x.AssociatedSymbol == null &&
        x.IsStatic == false && (
        x.DeclaredAccessibility == Accessibility.Public ||
        x.DeclaredAccessibility == Accessibility.Protected))
        .ToList();

    /// <summary>
    /// Returns the collection of write fields in the given set.
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    List<IFieldSymbol> GetWriteFields(List<IFieldSymbol> fields) => fields.Where(x =>
        x.IsConst == false &&
        x.IsReadOnly == false)
        .ToList();

    /// <summary>
    /// A comparer for fields.
    /// </summary>
    class FieldComparer : IEqualityComparer<IFieldSymbol>
    {
        public bool Equals(IFieldSymbol x, IFieldSymbol y)
        {
            if (!SymbolEqualityComparer.Default.Equals(x.Type, y.Type)) return false;
            if (x.Name != y.Name) return false;
            return true;
        }

        public int GetHashCode(IFieldSymbol obj)
            => SymbolEqualityComparer.Default.GetHashCode(obj);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit code for the given inits.
    /// This method always end with a semicolon and a new line.
    /// </summary>
    /// <param name="cb"></param>
    /// <param name="inits"></param>
    void PrintInits(CodeBuilder cb, List<NewArgument> inits)
    {
        if (inits.Count > 0)
        {
            cb.AppendLine();
            cb.AppendLine("{");
            cb.Tabs++;

            foreach (var item in inits)
            {
                var code = item.Generate();
                cb.AppendLine($"{item.MemberName} = {code},");
            }

            cb.Tabs--;
            cb.Append("}");
        }
        cb.AppendLine(";");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the code to asign to a variable whose <paramref name="varName"/> name is given a
    /// new instance of the type referred by this instance, using the optional list of arguments
    /// provided. Returns true if the code has been emitted, or false otherwise.
    /// </summary>
    /// <param name="varName"></param>
    /// <param name="cb"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public bool Print(string varName, CodeBuilder cb, params NewArgument[] args)
    {
        varName = varName.NotNullNotEmpty(nameof(varName));
        cb = cb.ThrowIfNull(nameof(cb));
        args = args.ThrowIfNull(nameof(args));

        if (RegularConstructor(varName, cb, args)) return true;
        if (CopyConstructor(false, varName, cb, args)) return true;
        if (CopyConstructor(true, varName, cb, args)) return true;
        if (EmptyConstructor(varName, cb, args)) return true;

        cb.AppendLine("throw new NotImplementedException();");
        return false;
    }

    /// <summary>
    /// Emits code for a constructor with one or more parameters.
    /// </summary>
    /// <param name="varName"></param>
    /// <param name="cb"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    bool RegularConstructor(string varName, CodeBuilder cb, params NewArgument[] args)
    {
        var cons = GetConstructors();
        foreach (var con in cons)
        {
            if (con.Parameters.Length == 0) continue;

            var rprops = GetReadProperties(GetProperties());
            var rfields = GetReadFields(GetFields());
            var xargs = args.ToList();
            var pars = GetParameters(con.Parameters, xargs, rprops, rfields);
            if (pars.Count != con.Parameters.Length) return false;

            var wprops = GetWriteProperties(rprops);
            var wfields = GetWriteFields(rfields);
            var inits = GetInits(xargs, wprops, wfields);
            if (xargs.Count > 0) return false;

            var str = string.Join(", ", pars.Select(x => x.Generate()));
            cb.Append($"var {varName} = new {TypeName}({str})");
            PrintInits(cb, inits);
            return true;
        }

        // Not found...
        return false;
    }

    /// <summary>
    /// Emits code for a copy constructor, using or not a compatible interface argument as
    /// requested.
    /// </summary>
    /// <param name="allowInterface"></param>
    /// <param name="varName"></param>
    /// <param name="cb"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    bool CopyConstructor(
        bool allowInterface,
        string varName, CodeBuilder cb, params NewArgument[] args)
    {
        var cons = GetConstructors();
        foreach (var con in cons)
        {
            if (con.Parameters.Length != 1) continue;

            var par = con.Parameters[0];
            var valid = allowInterface
                ? TypeSymbol.IsAssignableTo(par.Type)
                : SymbolEqualityComparer.Default.Equals(TypeSymbol, par.Type);

            if (!valid) continue;

            var wprops = GetWriteProperties(GetReadProperties(GetProperties()));
            var wfields = GetWriteFields(GetReadFields(GetFields()));
            var xargs = args.ToList();
            var inits = GetInits(xargs, wprops, wfields);
            if (xargs.Count > 0) return false;

            cb.Append($"var {varName} = new {TypeName}(this)");
            PrintInits(cb, inits);
            return true;
        }

        // Not found...
        return false;
    }

    /// <summary>
    /// Emits code for an empty or parameterless constructor.
    /// </summary>
    /// <param name="varName"></param>
    /// <param name="cb"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    bool EmptyConstructor(string varName, CodeBuilder cb, params NewArgument[] args)
    {
        var cons = GetConstructors();
        foreach (var con in cons)
        {
            if (con.Parameters.Length != 0) continue;

            var wprops = GetWriteProperties(GetReadProperties(GetProperties()));
            var wfields = GetWriteFields(GetReadFields(GetFields()));
            var xargs = args.ToList();
            var inits = GetInits(xargs, wprops, wfields);
            if (xargs.Count > 0) return false;

            cb.Append($"var {varName} = new {TypeName}()");
            PrintInits(cb, inits);
            return true;
        }

        // Not found...
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the list of arguments to use with the type constructor.
    /// The given lists are adjusted as needed.
    /// </summary>
    /// <param name="pars"></param>
    /// <param name="xargs"></param>
    /// <param name="rprops"></param>
    /// <param name="rfield"></param>
    /// <returns></returns>
    List<NewArgument> GetParameters(
        ImmutableArray<IParameterSymbol> pars,
        List<NewArgument> xargs,
        List<IPropertySymbol> rprops,
        List<IFieldSymbol> rfields)
    {
        var list = new List<NewArgument>();
        var comp = StringComparison.OrdinalIgnoreCase;

        foreach (var par in pars)
        {
            // There might be arguments assignable to the parameter...
            var xarg = xargs.SingleOrDefault(x =>
                x.Type.IsAssignableTo(par.Type) && (
                string.Compare(x.ArgumentName, par.Name, comp) == 0 ||
                string.Compare(x.MemberName, par.Name, comp) == 0));

            if (xarg != null)
            {
                xargs.Remove(xarg);
                list.Add(xarg);

                var xprop = rprops.Find(x =>
                    string.Compare(xarg.ArgumentName, x.Name, comp) == 0 ||
                    string.Compare(xarg.MemberName, x.Name, comp) == 0);

                if (xprop != null)
                {
                    rprops.Remove(xprop);
                    continue;
                }

                var xfield = rfields.Find(x =>
                    string.Compare(xarg.ArgumentName, x.Name, comp) == 0 ||
                    string.Compare(xarg.MemberName, x.Name, comp) == 0);
                
                if (xfield != null)
                {
                    rfields.Remove(xfield);
                    continue;
                }

                continue;
            }

            // There might be properties assignable to the parameter...
            var rprop = rprops.SingleOrDefault(x =>
                string.Compare(x.Name, par.Name, comp) == 0 &&
                x.Type.IsAssignableTo(par.Type));

            if (rprop != null)
            {
                rprops.Remove(rprop);
                list.Add(new NewArgument(rprop.Type, rprop.Name));
                continue;
            }

            // There might be fiels assignable to the parameter...
            var rfield = rfields.SingleOrDefault(x =>
                string.Compare(x.Name, par.Name, comp) == 0 &&
                x.Type.IsAssignableTo(par.Type));

            if (rfield != null)
            {
                rfields.Remove(rfield);
                list.Add(new NewArgument(rfield.Type, rfield.Name));
                continue;
            }
        }

        return list;
    }

    /// <summary>
    /// Returns the list of arguments to use for init purposes.
    /// The given lists are adjusted as needed.
    /// </summary>
    /// <param name="xargs"></param>
    /// <param name="wprops"></param>
    /// <param name="rfield"></param>
    /// <returns></returns>
    List<NewArgument> GetInits(
        List<NewArgument> xargs,
        List<IPropertySymbol> wprops,
        List<IFieldSymbol> wfields)
    {
        var list = new List<NewArgument>();
        var comp = StringComparison.OrdinalIgnoreCase;

        // Using the given arguments...
        for (int i = 0; i < xargs.Count; i++)
        {
            var xarg = xargs[i];

            var wprop = wprops.SingleOrDefault(x =>
                xarg.Type.IsAssignableTo(x.Type) && (
                string.Compare(x.Name, xarg.MemberName, comp) == 0 ||
                string.Compare(x.Name, xarg.ArgumentName, comp) == 0));

            if (wprop != null)
            {
                var isCloneable = IsCloneable(wprop);
                var item = new NewArgument(wprop.Type, xarg.ArgumentName, wprop.Name, isCloneable);
                list.Add(item);

                wprops.Remove(wprop);
                xargs.RemoveAt(i);
                i = -1;
                continue;
            }

            var wfield = wfields.SingleOrDefault(x =>
                xarg.Type.IsAssignableTo(x.Type) && (
                string.Compare(x.Name, xarg.MemberName, comp) == 0 ||
                string.Compare(x.Name, xarg.ArgumentName, comp) == 0));

            if (wfield != null)
            {
                var isCloneable = IsCloneable(wfield);
                var item = new NewArgument(wfield.Type, xarg.ArgumentName, wfield.Name, isCloneable);
                list.Add(item);

                wfields.Remove(wfield);
                xargs.RemoveAt(i);
                i = -1;
                continue;
            }
        }

        // Using the remaining members...
        foreach (var prop in wprops)
        {
            var item = new NewArgument(prop.Type, prop.Name, prop.Name);
            list.Add(item);
        }
        wprops.Clear();

        foreach (var field in wfields)
        {
            var item = new NewArgument(field.Type, field.Name, field.Name);
            list.Add(item);
        }
        wfields.Clear();

        return list;
    }
}