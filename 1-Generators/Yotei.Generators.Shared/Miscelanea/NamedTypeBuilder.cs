namespace Yotei.Generators;

// ========================================================
/// <summary>
/// Used to emit code for generating new instances of a given type.
/// </summary>
internal class NamedTypeBuilder
{
    public NamedTypeBuilder(INamedTypeSymbol type)
    {
        type = type.ThrowIfNull(nameof(type));
        if (type.IsNamespace) throw new ArgumentException($"Type cannot be a namespace: {type}");

        TypeSymbol = type;
        Name = type.Name.NotNullNotEmpty(nameof(type));
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"Builder: {Name}";

    /// <summary>
    /// The named type symbol of this instance.
    /// </summary>
    public INamedTypeSymbol TypeSymbol { get; }

    /// <summary>
    /// The name of the type this instace refers to.
    /// </summary>
    public string Name { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of constructors to be considered by this type builder, ordered
    /// in descending order by the number of arguments of constructor.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<IMethodSymbol> GetConstructors()
    {
        return TypeSymbol.GetMembers().OfType<IMethodSymbol>().Where(x =>
            x.MethodKind == MethodKind.Constructor &&
            x.IsStatic == false)
            .OrderByDescending(x => x.Parameters.Length);
    }

    /// <summary>
    /// Returns the collection of read properties to be considered by this type builder. Unless
    /// overriden, this method returns all public and protected properties that can be read
    /// from the given type and its base ones.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<IPropertySymbol> GetReadProperties()
    {
        var comp = SymbolEqualityComparer.Default;
        var list = new NoDuplicatesList<IPropertySymbol>(comp);

        GetReadProperties(list, TypeSymbol);
        return list;
    }
    static void GetReadProperties(NoDuplicatesList<IPropertySymbol> list, INamedTypeSymbol? type)
    {
        if (type == null) return;
        if (type.IsNamespace) return;

        var items = type.GetMembers().OfType<IPropertySymbol>().Where(x =>
            x.CanBeReferencedByName &&
            x.GetMethod != null &&
            x.IsStatic == false && (
            x.DeclaredAccessibility == Accessibility.Public ||
            x.DeclaredAccessibility == Accessibility.Protected));

        list.AddRange(items);
        type = type.BaseType;
        GetReadProperties(list, type);
    }

    /// <summary>
    /// Returns the collection of write properties to be considered by this type builder. This
    /// method extracts from the collection of read ones those that can be written.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<IPropertySymbol> GetWriteProperties()
    {
        return GetReadProperties().Where(IsValid);

        bool IsValid(IPropertySymbol item)
        {
            var comp = SymbolEqualityComparer.Default;

            if (item.SetMethod == null) return false;

            if (!(item.DeclaredAccessibility == Accessibility.Public ||
                item.DeclaredAccessibility == Accessibility.Protected)
                && !comp.Equals(TypeSymbol, item.Type)) return false;

            return true;
        }
    }

    /// <summary>
    /// Returns the collection of read fields to be considered by this type builder. Unless
    /// overriden, this method returns all public and protected fields that can be read, from
    /// the given type and its base ones.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<IFieldSymbol> GetReadFields()
    {
        var comp = SymbolEqualityComparer.Default;
        var list = new NoDuplicatesList<IFieldSymbol>(comp);

        GetReadFields(list, TypeSymbol);
        return list;
    }
    static void GetReadFields(NoDuplicatesList<IFieldSymbol> list, INamedTypeSymbol? type)
    {
        if (type == null) return;
        if (type.IsNamespace) return;

        var items = type.GetMembers().OfType<IFieldSymbol>().Where(x =>
            x.CanBeReferencedByName &&
            x.AssociatedSymbol == null &&
            x.IsStatic == false && (
            x.DeclaredAccessibility == Accessibility.Public ||
            x.DeclaredAccessibility == Accessibility.Protected));

        list.AddRange(items);
        type = type.BaseType;
        GetReadFields(list, type);
    }

    /// <summary>
    /// Returns the collection of write fields to be considered by this type builder. This
    /// method extracts from the collection of read ones those that can be written..
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<IFieldSymbol> GetWriteFields()
    {
        return GetReadFields().Where(IsValid);

        bool IsValid(IFieldSymbol item)
        {
            var comp = SymbolEqualityComparer.Default;

            if (item.IsConst) return false;
            if (item.IsReadOnly && !comp.Equals(TypeSymbol, item.Type)) return false;

            return true;
        }
    }

    /// <summary>
    /// Emits the appropriate code to obtain the value of the given property.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public virtual string GetCode(IPropertySymbol symbol) => $"this.{symbol.Name}";

    /// <summary>
    /// Emits the appropriate code to obtain the value of the given field.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public virtual string GetCode(IFieldSymbol symbol) => $"this.{symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// Emits the code to assign a new instance of this type to the variable whose name is
    /// given. Returns false if the code cannot be generated.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cb"></param>
    public bool Print(string name, CodeBuilder cb)
    {
        name = name.NotNullNotEmpty(nameof(name));
        cb = cb.ThrowIfNull(nameof(cb));

        var cons = GetConstructors().ToImmutableArray();
        if (ForCopy()) return true;
        if (ForMany()) return true;
        if (ForEmpty()) return true;

        return false;

        /// <summary>
        /// Emits code for a copy constructor.
        /// </summary>
        bool ForCopy()
        {
            for (int c = 0; c < cons.Length; c++)
            {
                var con = cons[c];
                if (con.Parameters.Length != 1) continue;

                var arg = con.Parameters[0];
                if (TypeSymbol.IsAssignableTo(arg.Type))
                {
                    // Found...
                    cb.AppendLine($"var {name} = new {Name}(this);");
                    return true;
                }
            }

            // Not found...
            return false;
        }

        /// <summary>
        /// Emits code for a parameterless constructor.
        /// </summary>
        bool ForEmpty()
        {
            for (int c = 0; c < cons.Length; c++)
            {
                var con = cons[c];
                if (con.Parameters.Length != 0) continue;

                // Found...
                var props = GetWriteProperties().ToArray();
                var fields = GetWriteFields().ToArray();
                var num = props.Length + fields.Length;

                cb.Append($"var {name} = new {Name}()");
                if (num != 0)
                {
                    cb.AppendLine();
                    cb.AppendLine("{");
                    cb.Tabs++;

                    foreach (var prop in props)
                    {
                        var code = GetCode(prop);
                        cb.AppendLine($"{prop.Name} = {code},");
                    }

                    foreach (var field in fields)
                    {
                        var code = GetCode(field);
                        cb.AppendLine($"{field.Name} = {code},");
                    }

                    cb.Tabs--;
                    cb.AppendLine("};");
                }
                else cb.AppendLine(";");
                return true;
            }

            // Not found...
            return false;
        }

        /// <summary>
        /// Emits code for a constructor with many arguments.
        /// </summary>
        bool ForMany()
        {
            var rprops = GetReadProperties().ToArray();
            var rfields = GetReadFields().ToArray();
            var rnum = rprops.Length + rfields.Length;

            for (int c = 0; c < cons.Length; c++)
            {
                var con = cons[c];
                if (con.Parameters.Length == 0) continue;
                if (con.Parameters.Length > rnum) continue;

                // Argument matching is case insensitive...
                var aprops = new List<IPropertySymbol>();
                var afields = new List<IFieldSymbol>();
                var comp = StringComparison.OrdinalIgnoreCase;

                var args = con.Parameters.ToList();
                for (int a = 0; a < args.Count; a++)
                {
                    var arg = args[a];
                    var argname = arg.Name.NullWhenEmpty();
                    if (argname == null) continue;

                    var prop = rprops.SingleOrDefault(x =>
                        x.Type.IsAssignableTo(arg.Type) &&
                        string.Compare(Adjust(x.Name), argname, comp) == 0);

                    if (prop != null)
                    {
                        aprops.Add(prop);
                        args.Remove(arg);
                        a = -1;
                        continue;
                    }

                    var field = rfields.SingleOrDefault(x =>
                        x.Type.IsAssignableTo(arg.Type) &&
                        string.Compare(Adjust(x.Name), argname, comp) == 0);

                    if (field != null)
                    {
                        afields.Add(field);
                        args.Remove(arg);
                        a = -1;
                        continue;
                    }
                }

                // We have found all arguments...
                if (args.Count == 0)
                {
                    var wprops = GetWriteProperties().ToList();
                    foreach (var tprop in aprops) wprops.Remove(tprop);

                    var wfields = GetWriteFields().ToList();
                    foreach (var tfield in afields) wfields.Remove(tfield);

                    // We may still have init-only properties from base types...
                    if (wprops.Any(x =>
                        x.SetMethod!.IsInitOnly &&
                        !SymbolEqualityComparer.Default.Equals(TypeSymbol, x.Type)))
                        continue;

                    // Found...
                    var wnum = wprops.Count + wfields.Count;

                    var sprops = string.Join(", ", aprops.Select(GetCode));
                    var sfields = string.Join(", ", afields.Select(GetCode));
                    var str = sprops.Length == 0
                        ? sfields
                        : sprops + (sfields.Length == 0
                        ? string.Empty
                        : (", " + sfields));

                    cb.Append($"var {name} = new {Name}({str})");
                    if (wnum != 0)
                    {
                        cb.AppendLine();
                        cb.AppendLine("{");
                        cb.Tabs++;

                        foreach (var prop in wprops)
                        {
                            var code = GetCode(prop);
                            cb.AppendLine($"{prop.Name} = {code},");
                        }

                        foreach (var field in wfields)
                        {
                            var code = GetCode(field);
                            cb.AppendLine($"{field.Name} = {code},");
                        }

                        cb.Tabs--;
                        cb.AppendLine("};");
                    }
                    else cb.AppendLine(";");
                    return true;
                }
            }

            string? Adjust(string? name)
            {
                if (name != null && name.StartsWith("_")) name = name.Substring(1);
                return name;
            }

            // Not found...
            return false;
        }
    }
}