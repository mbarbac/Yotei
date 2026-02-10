namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal record EasyAttributeData
{
    /// <summary>
    /// If not null, the options to include the attribute class. If null, then only its name is
    /// used.
    /// </summary>
    public EasyTypeSymbol? ClassOptions { get; set; }

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
    /// If enabled, include parameter brackets, even if <see cref="ParameterOptions"/> is null.
    /// </summary>
    public bool UseBrackets { get; set; }

    /// <summary>
    /// If not null, the options to include the parameters of the attribute, if any. If null,
    /// they are ignored.
    /// </summary>
    public EasyTypedConstant? ParameterOptions { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with a set of default code generation settings.
    /// </summary>
    public static EasyAttributeData Default => new()
    {
        GenericOptions = EasyTypeSymbol.Default,
        ParameterOptions = EasyTypedConstant.Default
    };

    /// <summary>
    /// Returns a new instance with full settings.
    /// </summary>
    public static EasyAttributeData Full => new()
    {
        ClassOptions = EasyTypeSymbol.Full,
        UseAttributeSuffix = true,
        GenericOptions = EasyTypeSymbol.Full,
        UseBrackets = true,
        ParameterOptions = EasyTypedConstant.Full,
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
        var host = source.AttributeClass ?? throw new ArgumentException("Attribute class is null.").WithData(source);

        // Ahora mismo, host.Name == "InheritsWithAttribute"
        // Tema: conservar (si procede) su host y ns, y remover/añadir Attribute si procede.
        // Luego, añadir generics

        // Name...
        /*
        var xoptions = options.ClassOptions ?? EasyTypeSymbol.Default;
        if (xoptions.HideName) xoptions = xoptions with { HideName = false };
        var name = host.EasyName(xoptions);

        if (options.UseAttributeSuffix && !name.EndsWith("Attribute")) name += "Attribute";
        if (!options.UseAttributeSuffix && name.EndsWith("Attribute")) name = name.RemoveLast("Attribute").ToString(); ;
        sb.Append(name);
        */

        // Parameters...
        var cons = source.ConstructorArguments;
        var named = source.NamedArguments;

        if ((options.UseBrackets || options.ParameterOptions != null) &&
            (cons.Length > 0 || named.Length > 0))
        {
            var done = false;
            sb.Append('(');

            for (int i = 0; i < cons.Length; i++) // Only values...
            {
                var arg = cons[i];
                var str = arg.EasyName(options.ParameterOptions);
                if (done) sb.Append(str.Length > 0 ? ", " : ",");
                sb.Append(str);
                done = true;
            }

            for (int i = 0; i < named.Length; i++) // Names and values...
            {
                var temp = named[i].Key;
                var arg = named[i].Value;
                var str = arg.EasyName(options.ParameterOptions, temp);
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