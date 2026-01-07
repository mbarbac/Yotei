namespace Yotei.Tools.CoreGenerator;

// ========================================================
public partial record EasyNames
{
    public EasyTypeOptions? TypeOptions { get; init; }
    public record EasyTypeOptions
    {
        public EasyTypeOptions? UseHost { get; init; }
        public bool UseName { get; init; }

    }
    

    static void XX()
    {
        var options = new EasyNames()
        { TypeOptions = new EasyTypeOptions { UseName = false } };

        options = options with
        { TypeOptions = options.TypeOptions with { UseName = false } };
    }
}