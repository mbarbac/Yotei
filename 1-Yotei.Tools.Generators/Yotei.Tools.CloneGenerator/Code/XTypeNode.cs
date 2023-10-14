using System.Reflection.Metadata.Ecma335;
using System.Security;

namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    public XTypeNode(Node parent, TypeCandidate candidate) : base(parent, candidate) { }
    public XTypeNode(Node parent, ITypeSymbol symbol):base(parent, symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;
        if (!Symbol.ValidateNotRecord(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder cb)
    {
        // Intercepting manual declaration or implementation...
        if (HasMethod(Symbol) != null) return;

        // Initiating...
        var modifiers = GetModifiers();
        PrintDocumentation(cb);        

        // Interfaces...
        if (IsInterface)
        {
            cb.AppendLine($"{modifiers}{Symbol.Name} Clone();");
            return;
        }

        // Abstract...
        if (Symbol.IsAbstract)
        {
            modifiers = "public abstract ";
            cb.AppendLine($"{modifiers}{Symbol.Name} Clone();");
        }

        // Regular...
        else
        {
            var builder = new TypeBuilder(context, Symbol);
            var underscores = GetIncludeUnderscores();
            var specs = GetSpecs();
            var receiver = "v_temp";
            var code = builder.GetCode(receiver, specs, null, underscores);

            cb.AppendLine($"{modifiers}{Symbol.Name} Clone()");
            cb.AppendLine("{");
            cb.IndentLevel++;

            if (code == null)
            {
                context.WarningCannotGenerateCode(Symbol);
                cb.AppendLine("throw new NotImplementedException();");
            }
            else
            {
                cb.Append(code);
                cb.AppendLine($"return {receiver};");
            }

            cb.IndentLevel--;
            cb.AppendLine("}");
        }

        // Interfaces to implement...
        var ifaces = GetInterfacesToImplement();
        foreach (var iface in ifaces)
        {
            var name = iface.FullyQualifiedName(addNullable: false);
            var type = iface.Name == "ICloneable" ? "object" : name;

            cb.AppendLine();
            cb.AppendLine(type);
            cb.AppendLine($"{name}.Clone() => Clone();");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Prints the appropriate documentation.
    /// </summary>
    void PrintDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        /// <returns></returns>
        """);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string with the appropriate method modifiers, followed by a 'space' if needed.
    /// </summary>
    /// <returns></returns>
    string GetModifiers()
    {
        // Interfaces...
        if (IsInterface)
        {
            return Symbol.AllInterfaces.Any(x =>
                x.Name == "ICloneable" ||
                x.HasAttributes(CloneableAttr.LongName))
                ? "new "
                : string.Empty;
        }

        // Implementation...
        else
        {
            bool value;

            foreach (var type in Symbol.AllBaseTypes())
            {
                var method = HasMethod(type);
                if (method != null)
                {
                    return method.IsVirtual || method.IsOverride || method.IsAbstract
                        ? "public override "
                        : "public new ";
                }

                if (CloneableAttr.GetPreventVirtual(type, out value))
                {
                    return !value ? "public override " : "public new ";
                }
            }

            if (Symbol.IsSealed) return "public ";

            CloneableAttr.GetPreventVirtual(Symbol, out value);
            return !value ? "public virtual " : "public ";
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the interfaces that need implementation.
    /// This method is only called for implementation purposes.
    /// </summary>
    /// <returns></returns>
    List<ITypeSymbol> GetInterfacesToImplement()
    {
        var list = new NoDuplicatesList<ITypeSymbol>()
        {
            ThrowDuplicates = false,
            Equivalent = SymbolEqualityComparer.Default.Equals,
        };

        foreach (var iface in Symbol.Interfaces) Populate(iface);
        return list.ToList();

        // Recursive...
        bool Populate(ITypeSymbol iface)
        {
            bool done = false;

            if (iface.Name == "ICloneable") done = true;
            if (HasMethod(iface) != null) done = true;
            if (iface.HasAttributes(CloneableAttr.LongName)) done = true;

            foreach (var child in iface.Interfaces)
            {
                if (Populate(child)) done = true;
            }

            if (done) list.Add(iface);
            return done;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type has a 'Clone' method, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static IMethodSymbol? HasMethod(ITypeSymbol type)
    {
        return type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the value of the <see cref="CloneableAttr.Specs"/>.
    /// This method is only called for implementation purposes.
    /// </summary>
    /// <returns></returns>
    public string? GetSpecs()
    {
        if (CloneableAttr.GetSpecs(Symbol, out var value)) return value;

        foreach (var type in Symbol.AllBaseTypes())
            if (CloneableAttr.GetSpecs(type, out value)) return value;

        foreach (var iface in Symbol.AllInterfaces)
            if (CloneableAttr.GetSpecs(iface, out value)) return value;

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the value of the <see cref="CloneableAttr.IncludeUnderscores"/>.
    /// This method is only called for implementation purposes.
    /// </summary>
    /// <returns></returns>
    public bool GetIncludeUnderscores()
    {
        if (CloneableAttr.GetIncludeUnderscores(Symbol, out var value)) return value;

        foreach (var type in Symbol.AllBaseTypes())
            if (CloneableAttr.GetIncludeUnderscores(type, out value)) return value;

        foreach (var iface in Symbol.AllInterfaces)
            if (CloneableAttr.GetIncludeUnderscores(iface, out value)) return value;

        return false;
    }
}