namespace Yotei.Tools.Generators;

/* NOTE:
 * The configuration options a project want to use must appear in the CONSUMING project within
 * a 'PropertyGroup' section, and then right after, they must be made visible to the compiler
 * in an 'ItemGroup' section, as follows:
 * 
 *      <PropertyGroup>
 *          <MySettingName>value</MySettingName>
 *      </PropertyGroup>
 *      <ItemGroup>
 *          <CompilerVisibleProperty Include="MySettingName" />
 *      </ItemGroup>
 * 
 * The setting name MUST NOT contain dots or special characters. The underscore character is
 * typically used to provide some structure - for instance, the name of the derived class does
 * preceed the setting name, as in: 'MyGenerator_MySettingName'.
 */

// ========================================================
/// <summary>
/// Maintains options for the tree-oriented incremental source code generators.
/// </summary>
public record TreeGeneratorOptions
{
    /// <summary>
    /// Determines if the names of the generated files shall be organized in folders, or rather
    /// they shall be flat file names. When true, then the folder names are considered to be all
    /// the first-level dot-separated name parts, except the last one.
    /// </summary>
    public bool UseFileFolders{ get; set; }

    /// <summary>
    /// Determines if the names of the generated files shall be reversed, or not. When true, then
    /// their first-level dot-separated parts are reversed.
    /// </summary>
    public bool ReverseFileNames { get; set; }

    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and the <see cref="IsNullableAttribute"/>
    /// nullability helpers are added to the generated code.
    /// </summary>
    public bool EmitNullabilityHelpers { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Populates this instance with the values read from the csproj file.
    /// <br/> The supported property types are: <see langword="bool"/>, <see langword="int"/>,
    /// and <see langword="string"/>.
    /// </summary>
    public void ReadElements(AnalyzerConfigOptions options)
    {
        var type = GetType();
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        foreach (var prop in props)
        {
            var name = $"build_property.{prop.DeclaringType.Name}_{prop.Name}";
            if (options.TryGetValue(name, out string? value))
            {
                switch (prop.PropertyType)
                {
                    case Type t when t == typeof(bool):
                        if (bool.TryParse(value, out var vbool)) prop.SetValue(this, vbool);
                        break;

                    case Type t when t == typeof(int):
                        if (int.TryParse(value, out var vint)) prop.SetValue(this, vint);
                        break;

                    case Type t when t == typeof(string):
                        if (value is not null) prop.SetValue(this, value);
                        break;
                }
            }
        }
    }
}