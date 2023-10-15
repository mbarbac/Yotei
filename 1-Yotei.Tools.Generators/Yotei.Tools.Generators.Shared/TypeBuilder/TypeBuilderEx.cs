namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal partial class TypeBuilder
{
    string Receiver = default!;
    SpecsParser Specs = default!;
    EnforcedMember? Enforced = null;
    bool Underscores = false;

    bool EnforcedUsed = false;
    List<IMethodSymbol> Builders = default!;
    List<IPropertySymbol> Properties = default!;
    List<IFieldSymbol> Fields = default!;

    ISymbol ErrorSymbol => Enforced?.Member ?? TypeSymbol;

    /// <summary>
    /// Returns the code that assigns to the 'receiver' variable a new instance of the associated
    /// type, or null if such code cannot be produced.
    /// <br/> If the specs resolve into "this" or into "base", then that value is returned.
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="specs"></param>
    /// <param name="enforced"></param>
    /// <param name="underscores"></param>
    /// <returns></returns>
    public string? GetCode(
        string receiver, string? specs, EnforcedMember? enforced, bool underscores)
    {
        string? code;

        Receiver = receiver.NotNullNotEmpty(nameof(receiver));
        Specs = new SpecsParser(specs);
        Enforced = enforced;
        Underscores = underscores;

        // Intercepting special cases...
        if (string.Compare(Specs.Name, "this", ignoreCase: true) == 0) return "this";
        if (string.Compare(Specs.Name, "base", ignoreCase: true) == 0) return "this";

        // Capturing type elements...
        Properties = GetTypeProperties();
        Fields = GetTypeFields();
        Builders = CaptureBuilders(); if (Builders.Count == 0)
        {
            Context.ErrorNoBuilders(TypeSymbol, Specs.Name);
            return null;
        }

        // Trying the different categories of builders, in order...
        code = TryCopyBuilders(true); if (code != null) return code;
        code = TryCopyBuilders(false); if (code != null) return code;
        code = TryRegularBuilders(); if (code != null) return code;
        code = TryEmptyBuilders(); if (code != null) return code;

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries empty builders.
    /// </summary>
    /// <returns></returns>
    string? TryEmptyBuilders()
    {
        // Looping...
        foreach (var builder in Builders)
        {
            if (builder.Parameters.Length != 0) continue;

            var ordinary = IsOrdinary(builder);
            var code = ordinary ? $"{builder.Name}()" : $"new {TypeSymbol.Name}()";

            var properties = GetWriteProperties(Properties);
            var fields = GetWriteFields(Fields);
            return CodeAndInits(code, ordinary, properties, fields);
        }

        // Not found...
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries copy builders.
    /// </summary>
    /// <param name="strict"></param>
    /// <returns></returns>
    string? TryCopyBuilders(bool strict)
    {
        // Special cases to intercept...
        if (Specs.Arguments.Count == 0) return null;
        if (Specs.Arguments.Count != 1) return null;

        // Looping...
        foreach (var builder in Builders)
        {
            if (builder.Parameters.Length != 1) continue;

            var par = builder.Parameters[0];
            if (!ValidForCopyBuilder(par)) continue;

            var ordinary = IsOrdinary(builder);
            var code = ordinary ? $"{builder.Name}(this)" : $"new {TypeSymbol.Name}(this)";

            var properties = GetWriteProperties(Properties);
            var fields = GetWriteFields(Fields);
            return CodeAndInits(code, ordinary, properties, fields);
        }

        // Not found...
        return null;

        // Determines if the parameter is valid for a copy builder...
        bool ValidForCopyBuilder(IParameterSymbol par)
        {
            return strict
                ? SymbolComparer.Equals(TypeSymbol, par.Type)
                : TypeSymbol.IsAssignableTo(par.Type);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries regular builders.
    /// </summary>
    /// <returns></returns>
    string? TryRegularBuilders()
    {
        // Looping...
        foreach (var builder in Builders)
        {
            if (builder.Parameters.Length == 0) continue;
            if (IsCopyBuilder(builder)) continue;
            if (!Specs.AllArguments &&
                (Specs.Arguments.Count != builder.Parameters.Length)) continue;

            var properties = Properties.ToList();
            var fields = Fields.ToList();
            var ordinary = IsOrdinary(builder);
            var sb = new StringBuilder(ordinary ? $"{builder.Name}(" : $"new {TypeSymbol.Name}(");

            var members = new List<ISymbol>();
            if (Specs.AllArguments) { if (!DoAllArguments()) continue; }
            else { if (!DoConcreteArguments()) continue; }

            foreach (var member in members)
            {
                if (member is IPropertySymbol p) properties.Remove(p);
                if (member is IFieldSymbol f) fields.Remove(f);
            }
            sb.Append(")");
            properties = GetWriteProperties(properties);
            fields = GetWriteFields(fields);
            return CodeAndInits(sb.ToString(), ordinary, properties, fields);

            /// <summary>
            /// Scenario: all arguments requested...
            /// </summary>
            bool DoAllArguments()
            {
                for (int i = 0; i < builder.Parameters.Length; i++)
                {
                    // Getting a member that matches with the parameter...
                    var par = builder.Parameters[i];
                    var member = MatchMember(out var error, par.Name, properties, fields);
                    if (error) Context.ErrorAmbiguousMatch(ErrorSymbol, par.Name);
                    if (member == null) return false;

                    // Getting the value to use...
                    string GetValue()
                    {
                        if (Enforced != null && Enforced.Member.Name == member.Name)
                        {
                            EnforcedUsed = true;
                            return Enforced.ValueName;
                        }
                        return member.Name;
                    }

                    // Adding value and used member...
                    var name = GetValue();
                    if (i > 0) sb.Append(", ");
                    sb.Append(name);
                    members.Add(member);
                }
                return true;
            }

            /// <summary>
            /// Scenario: concrete arguments requested...
            /// </summary>
            bool DoConcreteArguments()
            {
                for (int i = 0; i < builder.Parameters.Length; i++)
                {
                    // Special case for 'this'...
                    var arg = Specs.Arguments[i];
                    if (arg.Name == "this")
                    {
                        if (i > 0) sb.Append(", ");
                        sb.Append("this");
                        continue;
                    }

                    // Validating the parameter matches the spec argument...
                    var par = builder.Parameters[i];                    
                    var match = MatchParameter(out var error, arg.Name, par, builder);
                    if (error) Context.ErrorAmbiguousMatch(ErrorSymbol, arg.Name);
                    if (!match) return false;

                    // Getting the matching member name...
                    string GetName()
                    {
                        if (arg.IsEnforcedMember && Enforced != null) return Enforced.Member.Name;
                        if (arg.Member != null && !arg.IsEnforcedMember) return arg.Member;
                        return arg.Name;
                    }
                    var name = GetName();
                    var member = MatchMember(out error, name, properties, fields);
                    if (error) Context.ErrorAmbiguousMatch(ErrorSymbol, name);
                    if (member == null) return false;

                    // Getting the value to use...
                    string GetValue()
                    {
                        var name = member.Name;

                        if (Enforced != null && Enforced.Member.Name == name)
                        {
                            EnforcedUsed = true;
                            name = Enforced.ValueName;
                        }
                        if (arg.UseClone)
                        {
                            name = $"({name} is null) ? null : {name}.Clone()";
                        }
                        return name;
                    }

                    // Adding value and used member...
                    name = GetValue();
                    if (i > 0) sb.Append(", ");
                    sb.Append(name);
                    members.Add(member);
                }
                return true;
            }
        }

        // Not found...
        return null;

        // Determines if the builder is a copy one, or not...
        bool IsCopyBuilder(IMethodSymbol item)
        {
            return
                item.Parameters.Length == 1 && (
                SymbolComparer.Equals(TypeSymbol, item.Parameters[0].Type) ||
                TypeSymbol.IsAssignableTo(item.Parameters[0].Type));
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to add to the code produced, and from the given collections of remaining members
    /// not yet used, the init/set specifications, if any.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="ordinary"></param>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    string? CodeAndInits(
        string code, bool ordinary,
        List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        // Parsing inits...
        var items = GetInits(properties, fields);
        if (items == null) return null;

        // Generating code...
        var sb = new StringBuilder($"var {Receiver} = {code}");

        // No init/set arguments...
        if (items.Count == 0) sb.AppendLine(";");
        else
        {
            // Constructors...
            if (!ordinary)
            {
                sb.AppendLine();
                sb.AppendLine("{"); foreach (var item in items)
                {
                    var value = GetValue(item);
                    sb.AppendLine($"   {item.Member.Name} = {value},");
                }
                sb.AppendLine("};");
            }

            // Ordinary builders...
            else
            {
                sb.AppendLine(";"); foreach (var item in items)
                {
                    if (item.IsInitOnly) { Context.ErrorInitOnly(item.Member); return null; }

                    var value = GetValue(item);
                    sb.AppendLine($"{Receiver}.{item.Member.Name} = {value};");
                }
            }
        }

        // Finishing...
        if (Enforced != null && !EnforcedUsed)
        {
            Context.ErrorEnforcedNotUsed(TypeSymbol, Enforced.Member);
            return null;
        }
        return sb.ToString();

        // Gets the value to use with the given init/set element...
        string GetValue(InitElement item)
        {
            var name = item.Member.Name;

            if (Enforced != null && Enforced.Member.Name == item.Member.Name)
            {
                EnforcedUsed = true;
                name = Enforced.ValueName;
            }
            if (item.UseClone)
            {
                name = $"({name} is null) ? null : {name}.Clone()";
            }
            return name;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the parsed list of init/set optional elements. Returns null if an error is detected.
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    List<InitElement>? GetInits(List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        // Preventing members with underscores if such is requested...
        if (!Underscores)
        {
            var ps = properties.Where(x => x.Name.StartsWith("_")).ToList();
            foreach (var p in ps) properties.Remove(p);

            var fs = fields.Where(x => x.Name.StartsWith("_")).ToList();
            foreach (var f in fs) fields.Remove(f);
        }

        // Parsing the specifications...
        var list = new List<InitElement>();
        foreach (var optional in Specs.Optionals)
        {
            if (optional.IsExcludeAll)
            {
                list.Clear();
            }
            else if (optional.IsIncludeAll)
            {
                list.Clear();
                list.AddRange(properties.Select(x => new InitElement(x)));
                list.AddRange(fields.Select(x => new InitElement(x)));
            }
            else if (optional.IsExclude)
            {
                var member = MatchMember(out var error, optional.Member, properties, fields);
                if (error) Context.WarningAmbiguousMatch(ErrorSymbol, optional.Member);
                if (member != null)
                {
                    while (true)
                    {
                        var item = list.Find(x => x.Member.Name == member.Name);
                        if (item != null) list.Remove(item);
                        else break;
                    }
                }
            }
            else if (optional.IsInclude)
            {
                if (optional.Member == "@") // Match a member with the enforced name...
                {
                    if (Enforced == null) { Context.ErrorNoMatch(ErrorSymbol, optional.Member); return null; }
                    var item = new InitElement(Enforced.Member);
                    list.Add(item);
                }
                else // Standard specification...
                {
                    var member = MatchMember(out var error, optional.Member, properties, fields);
                    if (error) { Context.ErrorAmbiguousMatch(ErrorSymbol, optional.Member); return null; }
                    if (member == null) { Context.ErrorNoMatch(ErrorSymbol, optional.Member); return null; }

                    var item = new InitElement(member);
                    if (optional.UseClone) item.UseClone = true;
                    if (optional.IsMemberEnforced) item.UseEnforced = true;
                    list.Add(item);
                }
            }
        }

        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the appropriate builders (constructors or methods) from the given
    /// specifications, in decreasing order by their number of arguments.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> CaptureBuilders()
    {
        var items = Specs.Name == null ? GetTypeConstructors() : GetTypeMethods();

        if (Specs.Name != null)
        {
            var temps = items.Where(x => x.Name == Specs.Name).ToList();
            if (temps.Count == 0)
                temps = items.Where(x => string.Compare(x.Name, Specs.Name, ignoreCase: true) == 0).ToList();

            items = temps;
        }

        if (!Specs.AllArguments)
        {
            var temps = items.Where(x => x.Parameters.Length != Specs.Arguments.Count).ToList();
            foreach (var temp in temps) items.Remove(temp);
        }

        items = items.OrderByDescending(x => x.Parameters.Length).ToList();
        return items;
    }

    /// <summary>
    /// Tries to find a member whose name matches the given one. Returns the symbol of the found
    /// one, or null if any. If several matches are found, the error out argument is set.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="error"></param>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    ISymbol? MatchMember(
        out bool error,
        string name,
        List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        error = false;
        name = name.NotNullNotEmpty(nameof(name), trim: false);

        // Case sensitive...
        var prop = properties.Find(x => x.Name == name); if (prop != null) return prop;
        var field = fields.Find(x => x.Name == name); if (field != null) return field;

        // Relaxed...
        var tp = properties.Where(x => string.Compare(x.Name, name, ignoreCase: true) == 0).ToList();
        var tf = fields.Where(x => string.Compare(x.Name, name, ignoreCase: true) == 0).ToList();

        // Ambiguous match...
        if ((tp.Count + tf.Count) > 1) { error = true; return null; }

        // Match or null...
        if (tp.Count == 1) return tp[0];
        if (tf.Count == 1) return tf[0];
        return null;
    }

    /// <summary>
    /// Determines if the given name is a match of the parameter one, in the context of the
    /// parameters of the given method. If it is an ambiguous match, then the error argument
    /// is set.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="error"></param>
    /// <param name="par"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    bool MatchParameter(
        out bool error,
        string name,
        IParameterSymbol par, IMethodSymbol builder)
    {
        error = false;
        name = name.NotNullNotEmpty(nameof(name), trim: false);

        // Case sensitive...
        if (name == par.Name) return true;

        // Relaxed...
        if (string.Compare(name, par.Name, ignoreCase: true) == 0)
        {
            var pars = builder.Parameters.Where(
                x => string.Compare(name, x.Name, ignoreCase: true) == 0).ToList();

            if (pars.Count == 1) return true;
            error = true;
        }

        return false;
    }
}