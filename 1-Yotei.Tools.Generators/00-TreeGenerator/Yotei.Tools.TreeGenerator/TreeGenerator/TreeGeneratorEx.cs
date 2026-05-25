namespace Yotei.Tools.Generators;

// ========================================================
partial class TreeGenerator
{
    /// <summary>
    /// INFRASTRUCTURE ONLY. Not intended for inheritors' usage.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Registering post-initialization actions...
        context.RegisterPostInitializationOutput(OnInitialize);

        // Reading config options from consuming project...
        var options = context.AdditionalTextsProvider
            .Where(x => ConfigurationFileName != null && x.Path.EndsWith(ConfigurationFileName))
            .Select((x, token) => new TreeOptions(x.GetText(token)?.ToString(), out _))
            .Collect();
    }

    // ----------------------------------------------------

    /*
    protected virtual void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        var asm = Assembly.GetExecutingAssembly();
        var name = $"{GetType().Namespace}.Public.WithAttribute.cs";

        using var stream = asm.GetManifestResourceStream(name);
        if (stream == null)
        {
            var names = asm.GetManifestResourceNames();
            throw new FileNotFoundException($"Not found: {name}").WithData(names);
        }

        using var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();
    }*/
}