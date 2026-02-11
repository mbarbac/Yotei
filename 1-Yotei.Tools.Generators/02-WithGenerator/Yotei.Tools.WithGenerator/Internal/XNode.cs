namespace Yotei.Tools.WithGenerator;

// ========================================================
internal static class XNode
{
    /// <summary>
    /// Tries to find the 'UseVirtual' named argument on the given attribute.
    /// Is found, returns its value in the out argument.
    /// </summary>
    public static bool FindUseVirtual(AttributeData at, out bool value)
    {
        if (at.FindNamedArgument("UseVirtual", out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }
        value = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find either the 'ReturnType' named argument on the given attribute, or its unique
    /// generic type, as appropriate.
    /// Is found, returns its value in the out argument.
    /// </summary>
    public static bool FindReturnType(
        AttributeData at, [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        ArgumentNullException.ThrowIfNull(at.AttributeClass);

        // Generic attribute...
        if (at.AttributeClass.Arity == 1)
        {
            value = (INamedTypeSymbol)at.AttributeClass.TypeArguments[0];
            value = value.UnwrapNullable(out nullable);
            return true;
        }

        // Not-generic attribute...
        else if (at.AttributeClass.Arity == 0)
        {
            if (at.FindNamedArgument("ReturnType", out var arg))
            {
                if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
                {
                    value = temp.UnwrapNullable(out nullable);
                    return true;
                }
            }
        }

        // Defaul...
        value = null;
        nullable = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is decorated with a <see cref="InheritsWithAttribute"/> or
    /// with a <see cref="InheritsWithAttribute{T}"/> attribute, in that order and, if so, returns
    /// the found one in the out argument.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool FindInheritsWithAttribute(
        INamedTypeSymbol type, [NotNullWhen(true)] out AttributeData? value)
    {
        value = type.GetAttributes(typeof(InheritsWithAttribute)).FirstOrDefault();
        if (value is not null) return true;

        value = type.GetAttributes(typeof(InheritsWithAttribute<>)).FirstOrDefault();
        if (value is not null) return true;

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given member is decorated with a <see cref="WithAttribute"/> or with
    /// a <see cref="WithAttribute{T}"/> attribute and, if so, returns the found one in the out
    /// argument.
    /// </summary>
    public static bool FindWithAttribute(
        IPropertySymbol member, [NotNullWhen(true)] out AttributeData? value)
    {
        value = member.GetAttributes(typeof(WithAttribute)).FirstOrDefault();
        if (value is not null) return true;

        value = member.GetAttributes(typeof(WithAttribute<>)).FirstOrDefault();
        if (value is not null) return true;

        return false;
    }

    /// <summary>
    /// Determines if the given member is decorated with a <see cref="WithAttribute"/> or with
    /// a <see cref="WithAttribute{T}"/> attribute and, if so, returns the found one in the out
    /// argument.
    /// </summary>
    public static bool FindWithAttribute(
        IFieldSymbol member, [NotNullWhen(true)] out AttributeData? value)
    {
        value = member.GetAttributes(typeof(WithAttribute)).FirstOrDefault();
        if (value is not null) return true;

        value = member.GetAttributes(typeof(WithAttribute<>)).FirstOrDefault();
        if (value is not null) return true;

        return false;
    }
    
    // ----------------------------------------------------

    /// <summary>
    /// The version of the generator for documentation purposes.
    /// </summary>
    static string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    /// <summary>
    /// A string with the 'GeneratedCode' attribute of the generator for documentation purposes.
    /// </summary>
    static string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{VersionDoc}}")]
        """;

    /// <summary>
    /// Emits documentation.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="cb"></param>
    public static void EmitDocumentation(ISymbol symbol, CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Emulates the 'with' keyword for the '{{symbol.Name}}' member.
        /// </summary>
        {{AttributeDoc}}
        """);
}