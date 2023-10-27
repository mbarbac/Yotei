namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal partial class TypeBuilder
{
    ISymbol ErrorSymbol => Enforced?.Member ?? TypeSymbol;
    string Receiver = default!;
    BuilderSpecs Specs = default!;
    EnforcedMember? Enforced = null;
    bool Underscores = false;

    bool EnforcedUsed = false;
    List<IPropertySymbol> Properties = default!;
    List<IFieldSymbol> Fields = default!;
    List<IMethodSymbol> Builders = default!;

    /// <summary>
    /// Returns the code that assigns to the 'receiver' variable a new instance of the associated
    /// type, or null if such code cannot be produced.
    /// <br/> If the specs resolve into "this" or into "base", then these literals are returned
    /// for the caller to react accordinly. In these cases, it is considered an error to have any
    /// further specifications.
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

        if ((Receiver = receiver.NullWhenEmpty()!) == null) return null;
        try { Specs = new BuilderSpecs(specs); } catch { return null; }
        Enforced = enforced;
        Underscores = underscores;

        // Special cases...
        if (string.Compare(Specs.Name, "this", ignoreCase: true) == 0 ||
            string.Compare(Specs.Name, "base", ignoreCase: true) == 0)
        {
            if (!Specs.AllArguments || Specs.IncludeAll) return null;
            return Specs.Name!.ToLower();
        }

        // Capturing type elements...
        Properties = GetTypeProperties();
        Fields = GetTypeFields();
        if ((Builders = CaptureBuilders()).Count == 0)
        {
            Context.NoBuildersFound(ErrorSymbol, DiagnosticSeverity.Warning);
            return null;
        }

        // Trying the different categories of builders, in order...
        code = TryCopyBuilders(true); if (code != null) return code;
        code = TryCopyBuilders(false); if (code != null) return code;
        code = TryRegularBuilders(); if (code != null) return code;
        code = TryEmptyBuilders(); if (code != null) return code;

        // Cannot generate code...
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

            EnforcedUsed = false;
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
        // We can use a shortcut here...
        if (Specs.Arguments.Count != 1) return null;

        // Looping...
        foreach (var builder in Builders)
        {
            if (builder.Parameters.Length != 1) continue;
            var par = builder.Parameters[0];
            if (!ValidForCopyBuilder(par)) continue;

            EnforcedUsed = false;
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

            EnforcedUsed = false;
            var properties = Properties.ToList();
            var fields = Fields.ToList();
            var ordinary = IsOrdinary(builder);
            var sb = new StringBuilder(ordinary ? $"{builder.Name}(" : $"new {TypeSymbol.Name}(");
            var members = new List<ISymbol>();

            var done = Specs.AllArguments ? TryAllArguments() : TryConcreteArguments();
            if (!done) continue;

            // Removing used members...
            foreach (var member in members)
            {
                if (member is IPropertySymbol p) properties.Remove(p);
                if (member is IFieldSymbol f) fields.Remove(f);
            }
            properties = GetWriteProperties(properties);
            fields = GetWriteFields(fields);

            // Returning...
            sb.Append(')');
            return CodeAndInits(sb.ToString(), ordinary, properties, fields);

            /// <summary>
            /// Invoked to try the builder in an 'all-arguments' context...
            /// </summary>
            bool TryAllArguments()
            {
                for (int i = 0; i < builder.Parameters.Length; i++)
                {
                    // Getting a member that matches the parameter...
                    var par = builder.Parameters[i];
                    var member = MatchMember(par.Name, out _, properties, fields);
                    if (member == null) return false;
                    members.Add(member);

                    // Getting the value to use...
                    var value = member.Name;
                    if (Enforced != null && Enforced.Member.Name == member.Name)
                    {
                        EnforcedUsed = true;
                        value = Enforced.ValueName;
                    }

                    // Printing...
                    if (i > 0) sb.Append(", ");
                    sb.Append(value);
                }
                return true;
            }

            /// <summary>
            /// Invoked to try the builder in an 'concrete-arguments-only' context...
            /// </summary>
            bool TryConcreteArguments()
            {
                if (Specs.Arguments.Count != builder.Parameters.Length) return false;

                for (int i = 0; i < builder.Parameters.Length; i++)
                {
                    // Validating match between the parameter and the given spec argument...
                    var arg = Specs.Arguments[i];
                    var par = builder.Parameters[i];
                    var match = MatchArgument(arg.Name, out _, par, builder);
                    if (!match) return false;

                    // Getting the value to use...
                    string value;

                    if (arg.IsValueThis)
                    {
                        value = TryClone("this", arg.UseClone);
                    }
                    else if (arg.IsValueEnforced)
                    {
                        if (Enforced == null)
                        {
                            Context.NoEnforcedElement(TypeSymbol, DiagnosticSeverity.Warning);
                            return false;
                        }
                        value = TryClone(Enforced.ValueName, arg.UseClone);
                        members.Add(Enforced.Member);
                        EnforcedUsed = true;
                    }
                    else
                    {
                        var name = arg.Value ?? arg.Name;
                        var member = MatchMember(name, out _, properties, fields);
                        if (member == null) return false;

                        members.Add(member);
                        value = TryClone(member.Name, arg.UseClone);

                        if (Enforced != null && Enforced.Member.Name == member.Name)
                        {
                            // Need to use the enforced value as the member will be removed...
                            value = TryClone(Enforced.ValueName, arg.UseClone);
                            EnforcedUsed = true;
                        }
                    }

                    // Printing...
                    if (i > 0) sb.Append(", ");
                    sb.Append(value);
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
    /// Once the builder code is produced, let's try to add the init/set mebers from the parsed
    /// specifications, taking only into account the given lists of remaining members.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="ordinary"></param>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    string? CodeAndInits(
        string code,
        bool ordinary,
        List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        var items = GetInits(properties, fields);
        if (items == null) return null;

        if (Enforced != null && !EnforcedUsed)
        {
            Context.EnforcedNotUsed(TypeSymbol, Enforced.Member, DiagnosticSeverity.Error);
            return null;
        }

        var sb = new StringBuilder($"var {Receiver} = {code}");
        if (items.Count == 0)
        {
            sb.AppendLine(";");
        }
        else
        {
            // Ordinary builders...
            if (ordinary)
            {
                sb.AppendLine(";");
                foreach (var item in items)
                {
                    var value = TryClone(item.Value, item.UseClone);
                    sb.AppendLine($"{Receiver}.{item.Member.Name} = {value};");
                }
            }

            // Constructors...
            else
            {
                sb.AppendLine();
                sb.AppendLine("{");
                foreach (var item in items)
                {
                    var value = TryClone(item.Value, item.UseClone);
                    sb.AppendLine($"   {item.Member.Name} = {value},");
                }
                sb.AppendLine("};");
            }
        }

        sb.AppendLine($"return {Receiver};");
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the list of parsed init/set specifications, or null if an error is detected.
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    List<InitElement>? GetInits(List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        var list = new List<InitElement>();
        foreach (var optional in Specs.Optionals)
        {
            if (optional.IsExcludeAll) // Remove all previous contents...
            {
                list.Clear();
                continue;
            }
            else if (optional.IsIncludeAll) // Include all remaining members...
            {
                foreach (var p in properties)
                {
                    if (!Underscores && p.Name.StartsWith("_")) continue;

                    var value = p.Name;
                    if (Enforced != null && Enforced.Member.Name == p.Name)
                    {
                        EnforcedUsed = true;
                        value = Enforced.ValueName;
                    }
                    var item = new InitElement(p, value, false);
                    list.Add(item);
                }
                foreach (var f in fields)
                {
                    if (!Underscores && f.Name.StartsWith("_")) continue;

                    var value = f.Name;
                    if (Enforced != null && Enforced.Member.Name == f.Name)
                    {
                        EnforcedUsed = true;
                        value = Enforced.ValueName;
                    }
                    var item = new InitElement(f, value, false);
                    list.Add(item);
                }
                continue;
            }
            if (optional.IsExclude) // Remove any previous matching members...
            {
                var member = MatchMember(optional.Name, out _, properties, fields);
                if (member == null) continue;

                while (true)
                {
                    var item = list.Find(x => x.Member.Name == member.Name);
                    if (item != null)
                    {
                        // Not erasing EnforcedUsed: can be used to flag its usage, and then
                        // remove the member...
                        list.Remove(item);
                    }
                    else break;
                }
                continue;
            }
            if (optional.IsInclude) // Include a remaining member, if found...
            {
                if (optional.IsNameEnforced)
                {
                    if (Enforced == null)
                    {
                        Context.NoEnforcedElement(ErrorSymbol, DiagnosticSeverity.Error);
                        return null;
                    }
                    var item = new InitElement(Enforced.Member, Enforced.ValueName, optional.UseClone);
                    EnforcedUsed = true;
                    list.Add(item);
                }
                else
                {
                    var member = MatchMember(optional.Name, out var error, properties, fields);
                    if (error)
                    {
                        Context.AmbiguousMatch(ErrorSymbol, optional.Name, DiagnosticSeverity.Error);
                        return null;
                    }
                    if (member == null)
                    {
                        Context.NoMatch(ErrorSymbol, optional.Name, DiagnosticSeverity.Error);
                        return null;
                    }
                    var value = optional.Value ?? member.Name;
                    var item = new InitElement(member, value, optional.UseClone);
                    list.Add(item);
                }
                continue;
            }
        }
        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// After the specs are parsed, returns a list with the builders to try, in decreasing order
    /// by their number of arguments.
    /// </summary>
    /// <returns></returns>
    List<IMethodSymbol> CaptureBuilders()
    {
        var items = Specs.Name == null ? GetTypeConstructors() : GetTypeMethods();

        // We may need to consider several regular methods...
        if (Specs.Name != null)
        {
            var temps = items.Where(x => x.Name == Specs.Name).ToList();
            if (temps.Count == 0)
                temps = items.Where(x => string.Compare(x.Name, Specs.Name, ignoreCase: true) == 0).ToList();

            items = temps;
        }

        // If concrete arguments are requested, we can pre-filter by their number...
        if (!Specs.AllArguments)
        {
            var temps = items.Where(x => x.Parameters.Length != Specs.Arguments.Count).ToList();
            foreach (var temp in temps) items.Remove(temp);
        }

        // Finishing,,,
        items = items.OrderByDescending(x => x.Parameters.Length).ToList();
        return items;
    }
}