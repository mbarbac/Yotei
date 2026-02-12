namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyAttributeData
{
    /// <summary>
    /// If not null, the options to include the attribute class. If null, then only its name is
    /// used.
    /// </summary>
    public EasyTypeSymbol? TypeOptions { get; set; }

    /// <summary>
    /// Ends the element's name with the 'Attribute' suffix.
    /// </summary>
    public bool UseAttributeSuffix { get; set; }

    /// <summary>
    /// If not null, the options to include the generic arguments of the attribute class, if any.
    /// If null, they are ignored.
    /// </summary>
    public EasyTypeSymbol? GenericOptions { get; set; }

    /// <summary>
    /// If enabled, include parameter brackets, even if <see cref="ValueOptions"/> is null.
    /// </summary>
    public bool UseBrackets { get; set; }

    /// <summary>
    /// If not null, the options to include the parameters of the attribute, if any. If null,
    /// they are ignored.
    /// </summary>
    public EasyTypedConstant? ValueOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// A shared instance with default-alike settings.
    /// </summary>
    public static EasyAttributeData Default { get; } = new()
    {
        GenericOptions = EasyTypeSymbol.Default,
        ValueOptions = EasyTypedConstant.Default
    };

    /// <summary>
    /// A shared instance with full-alike settings.
    /// </summary>
    public static EasyAttributeData Full { get; } = new()
    {
        TypeOptions = EasyTypeSymbol.Full,
        UseAttributeSuffix = true,
        GenericOptions = EasyTypeSymbol.Full,
        UseBrackets = true,
        ValueOptions = EasyTypedConstant.Full,
    };
}

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Returns a display string for the given element using default options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(
        this AttributeData source) => source.EasyName(EasyAttributeData.Default);

    /// <summary>
    /// Returns a display string for the given element using the given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this AttributeData source, EasyAttributeData options)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        var sb = new StringBuilder();
        var type = source.AttributeClass ?? throw new ArgumentException("Attribute class is null.").WithData(source);
        var name = type.Name;

        // Type options...
        var xoptions = (options.TypeOptions ?? EasyTypeSymbol.Default).WithNoHideName();
        var head = type.EasyName(xoptions);

        if (options.UseAttributeSuffix) { } // head already carries 'Attribute' if needed...
        else
        {
            var temp = name.RemoveLast("Attribute").ToString();
            if (!name.EndsWith("Attribute")) name += "Attribute";
            head = head.Replace(name, temp);
        }

        sb.Append(head);

        // Parameters...
        var cons = source.ConstructorArguments;
        var named = source.NamedArguments;

        if ((options.UseBrackets || options.ValueOptions != null) &&
            (cons.Length > 0 || named.Length > 0))
        {
            var done = false;
            sb.Append('(');

            for (int i = 0; i < cons.Length; i++) // Only values...
            {
                var arg = cons[i];
                var str = arg.EasyName(options.ValueOptions);
                if (done) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
                done = true;
            }

            for (int i = 0; i < named.Length; i++) // Names and values...
            {
                var temp = named[i].Key;
                var arg = named[i].Value;
                var str = arg.EasyName(options.ValueOptions, temp);
                if (done) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
                done = true;
            }

            sb.Append(')');
        }

        // Finishing...
        return sb.ToString();
    }
}