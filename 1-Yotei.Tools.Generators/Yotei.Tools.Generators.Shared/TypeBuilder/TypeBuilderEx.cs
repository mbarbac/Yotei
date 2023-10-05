using System.Net.Mail;

namespace Yotei.Tools.Generators;

// ========================================================
internal partial class TypeBuilder
{
    SymbolEqualityComparer SymbolComparer = SymbolEqualityComparer.Default;

    string Receiver = default!;
    BuilderSpecs Specs = default!;
    EnforcedMember? EnforcedMember = default;
    bool EnforcedUsed = default;
    bool IncludeUnderscores = false;
    List<IMethodSymbol> TypeMethods = default!;
    List<IPropertySymbol> TypeProperties = default!;
    List<IFieldSymbol> TypeFields = default!;

    /// <summary>
    /// Returns the code that assigns to the 'rceiver' variable a new instance of the associated
    /// type, taking into consideration the given specifications if the optional enforced member.
    /// If the code cannot be produced, return null.
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="specs"></param>
    /// <param name="enforced"></param>
    /// <param name="underscores"></param>
    /// <returns></returns>
    public string? GetCode(
        string receiver,
        string? specs, EnforcedMember? enforced, bool underscores)
    {
        string? code;

        Receiver = receiver.NotNullNotEmpty(nameof(receiver));
        Specs = new BuilderSpecs(specs);
        EnforcedMember = enforced;
        EnforcedUsed = false;
        IncludeUnderscores = underscores;

        if (string.Compare(Specs.Name, "this", ignoreCase: true) == 0) return "this";
        if (string.Compare(Specs.Name, "base", ignoreCase: true) == 0) return "base";

        TypeMethods = CaptureBuilders(); if (TypeMethods.Count == 0)
        {
            Context.ErrorNoMatch(TypeSymbol, Specs.Name ?? "<constructor>");
            return null;
        }
        TypeProperties = GetTypeProperties();
        TypeFields = GetTypeFields();

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
    public string? TryEmptyBuilders()
    {
        // Only if specs allow empty ones...
        if (Specs.Arguments.Count != 0) return null;

        // Looping...
        foreach (var method in TypeMethods)
        {
            if (method.Parameters.Length != 0) continue;

            var regular = method.MethodKind == MethodKind.Ordinary;
            var code = regular ? $"{method.Name}()" : $"new {TypeSymbol.Name}()";

            var properties = GetWriteProperties(TypeProperties);
            var fields = GetWriteFields(TypeFields);
            return CodeAndInits(code, regular, properties, fields);
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
    public string? TryCopyBuilders(bool strict)
    {
        // Only if specs allow single argument ones...
        if (Specs.Arguments.Count != 1) return null;

        // Looping...
        foreach (var method in TypeMethods)
        {
            if (method.Parameters.Length != 1) continue;

            var par = method.Parameters[0];
            if (!ValidCopyBuilder(par)) continue;

            var regular = method.MethodKind == MethodKind.Ordinary;
            var code = regular ? $"{method.Name}(this)" : $"new {TypeSymbol.Name}(this)";

            var properties = GetWriteProperties(TypeProperties);
            var fields = GetWriteFields(TypeFields);
            return CodeAndInits(code, regular, properties, fields);
        }

        // Not found...
        return null;

        // Determines if the parameter is valid for a copy builder...
        bool ValidCopyBuilder(IParameterSymbol par) => strict
            ? SymbolComparer.Equals(TypeSymbol, par.Type)
            : TypeSymbol.IsAssignableTo(par.Type);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries regular builders.
    /// </summary>
    /// <returns></returns>
    public string? TryRegularBuilders()
    {
        // Looping...
        foreach (var method in TypeMethods)
        {
            if (method.Parameters.Length == 0) continue;
            if (!Specs.AllArguments && (Specs.Arguments.Count != method.Parameters.Length)) continue;
            if (IsCopyBuilder(method)) continue;

            var properties = TypeProperties.ToList();
            var fields = TypeFields.ToList();
            var regular = method.MethodKind == MethodKind.Ordinary;
            var code = regular ? $"{method.Name}(" : $"new {TypeSymbol.Name}(";

            var sb = new StringBuilder(code);
            var members = new List<ISymbol>();

            // Scenario: all arguments...
            if (Specs.AllArguments)
            {
                var valid = true;
                for (int i = 0; i < method.Parameters.Length; i++)
                {
                    var par = method.Parameters[i];
                    var member = MatchMember(par.Name, out var error, properties, fields);
                    if (error) Context.ErrorAmbiguousMatch(EnforcedMember?.Member ?? TypeSymbol, par.Name);
                    if (member == null) { valid = false; break; }

                    members.Add(member);
                    if (i > 0) sb.Append(", ");
                    sb.Append(member.Name);
                }
                if (!valid) continue;
            }

            // Scenario: concrete arguments...
            else
            {
                var valid = true;
                for (int i = 0; i < method.Parameters.Length; i++)
                {
                    var par = method.Parameters[i];
                    var arg = Specs.Arguments[i];
                    var match = MatchArgument(arg, par, method, out var error);
                    if (error) Context.ErrorAmbiguousMatch(EnforcedMember?.Member ?? TypeSymbol, arg.Name);
                    if (!match) { valid = false; break; }

                    var name = arg.GetMember(EnforcedMember);
                    var member = MatchMember(name, out error, properties, fields);
                    if (error) Context.ErrorAmbiguousMatch(EnforcedMember?.Member ?? TypeSymbol, name);
                    if (member == null) { valid = false; break; }

                    name = EnforcedMember != null && EnforcedMember.Name == member.Name
                        ? EnforcedMember.ValueName
                        : member.Name;

                    if (arg.UseClone) name = $"({name} is null) ? null : {name}.Clone()";

                    members.Add(member);
                    if (i > 0) sb.Append(", ");
                    sb.Append(name);
                }
                if (!valid) continue;
            }

            // Producing code...
            foreach (var member in members)
            {
                if (EnforcedMember != null &&
                    EnforcedMember.Name == member.Name) EnforcedUsed = true;

                if (member is IPropertySymbol p) properties.Remove(p);
                if (member is IFieldSymbol f) fields.Remove(f);
            }

            sb.Append(')');
            code = sb.ToString();
            properties = GetWriteProperties(properties);
            fields = GetWriteFields(fields);
            return CodeAndInits(code, regular, properties, fields);
        }

        // Not found...
        return null;

        // Determines if we are in a copy builder situation...
        bool IsCopyBuilder(IMethodSymbol method) =>
            method.Parameters.Length == 1 && (
            SymbolComparer.Equals(TypeSymbol, method.Parameters[0].Type) ||
            TypeSymbol.IsAssignableTo(method.Parameters[0].Type));

    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to add to the code produced the init/set elements from the given specifications and
    /// the given set of remaining properties and fields not yet used in the affected builder.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="regular"></param>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public string? CodeAndInits(
        string code, bool regular,
        List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        // Parsing the list of optionals...
        var items = GetInits(properties, fields);
        if (items == null) return null;

        // Generating code...
        var sb = new StringBuilder($"var {Receiver} = {code}");

        if (!regular) // Constructors...
        {
            var inits = items.Where(x => x.IsInit).ToList();
            if (inits.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("{"); foreach (var init in inits)
                {
                    code = init.GetValue(EnforcedMember, out var used);
                    if (used) EnforcedUsed = true;

                    code = $"  {init.Member.Name} = {code},";
                    sb.AppendLine(code);

                    items.Remove(init);
                }
                sb.AppendLine("};");
            }
            else sb.AppendLine(";");

            foreach (var item in items)
            {
                code = item.GetValue(EnforcedMember, out var used);
                if (used) EnforcedUsed = true;

                code = $"{Receiver}.{item.Member.Name} = {code};";
                sb.AppendLine(code);
            }
        }
        else // Regular methods...
        {
            sb.AppendLine(";");
            foreach (var item in items)
            {
                if (item.Property != null && item.IsInit)
                {
                    Context.ErrorInitOnly(item.Member);
                    return null;
                }

                code = item.GetValue(EnforcedMember, out var used);
                if (used) EnforcedUsed = true;

                code = $"{Receiver}.{item.Member.Name} = {code};";
                sb.AppendLine(code);
            }
        }

        // Finishing...
        if (EnforcedMember != null && !EnforcedUsed)
        {
            Context.ErrorEnforcedNotUsed(TypeSymbol, EnforcedMember.Member);
            return null;
        }
        return sb.ToString();
    }

    /// <summary>
    /// Gets the parsed list of init/set arguments, using the given lists of remaining members,
    /// and the set of init/set specifications.
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    List<InitElement>? GetInits(List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        // Preventing members with underscores, if requested...
        if (!IncludeUnderscores)
        {
            var ps = properties.Where(x => x.Name.StartsWith("_")).ToList();
            foreach (var prop in ps) properties.Remove(prop);

            var fs = fields.Where(x => x.Name.StartsWith("_")).ToList();
            foreach (var field in fs) fields.Remove(field);
        }

        // Parsing the list of inits...
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
                list.AddRange(properties.Select(x => new InitElement(x, false)));
                list.AddRange(fields.Select(x => new InitElement(x, false)));
            }
            else if (optional.IsExclude)
            {
                var name = optional.GetMemberName();
                var member = MatchMember(name, out var error, properties, fields);
                if (error) Context.ErrorAmbiguousMatch(EnforcedMember?.Member ?? TypeSymbol, name);
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
                var name = optional.GetMemberName();
                var member = MatchMember(name, out var error, properties, fields);
                if (error) Context.ErrorAmbiguousMatch(EnforcedMember?.Member ?? TypeSymbol, name);
                if (member == null)
                {
                    Context.ErrorNoMatch(EnforcedMember?.Member ?? TypeSymbol, name);
                    return null;
                }
                var item = new InitElement(member, optional.UseClone);
                list.Add(item);
            }
        }
        return list;
    }
}