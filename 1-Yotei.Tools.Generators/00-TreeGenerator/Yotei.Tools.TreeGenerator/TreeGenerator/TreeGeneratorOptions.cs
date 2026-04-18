namespace Yotei.Tools.Generators;

/* NOTES:
 * - The configuration options MUST appear in the CONSUMING project, within a 'PropertyGroup'
 *   section, and then right after they MUST be made visible to the compiler in am 'ItemGroup'
 *   section, as follows:
 *      
 *      <PropertyGroup>
 *          <MySettingName>value</MySettingName>
 *      </PropertyGroup>
 *      <ItemGroup>
 *          <CompilerVisibleProperty Include="MySettingName" />
 *      </ItemGroup>
 * 
 * - The setting name MUST NOT contain dots or special characters; you can use the underscore
 *   character as a separator if needed: 'MyGenerator_MySettingName'
 */

// ========================================================
/// <summary>
/// Maintains options for tree oriented incremental source code generators.
/// </summary>
public record TreeGeneratorOptions
{
    /// <summary>
    /// Determines if the <see cref="IsNullable{T}"/> and the <see cref="IsNullableAttribute"/>
    /// nullability helpers are added to the generated code.
    /// <br/> The default value of this property is <see langword="true"/>.
    /// </summary>
    public bool EmitNullabilityHelpers { get; set; } = true;

    /// <summary>
    /// Determins if the names of the generated files shall be organized in folders, or rather
    /// they shall be flat file names. When true, folder names are built as all the first-level
    /// dot-separated parts, except the last one.
    /// <br/> The default value of this property is <see langword="true"/>.
    /// </summary>
    public bool UseFileFolders { get; set; } = true;

    /// <summary>
    /// Determines if the names of the generated files shall be reversed, or not. When true, their
    /// first-level dot-separated parts are reversed.
    /// <br/> The default value of this property is <see langword="true"/>.
    /// </summary>
    public bool ReverseFileNames { get; set; } = true;
}