namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents the file container of a captured type-alike source code generation node.
/// </summary>
public record ContainerInfo
{
    /// <summary>
    /// Creates a new instance using the given type.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="childsOnly"></param>
    /// <returns></returns>
    public static ContainerInfo Create(BaseTypeDeclarationSyntax syntax, bool childsOnly)
    {
        ArgumentNullException.ThrowIfNull(syntax);
        var container = new ContainerInfo();

        var tphost = syntax; while (true)
        {
            var str = tphost.Identifier.Text;
            if (tphost is TypeDeclarationSyntax dec && dec.TypeParameterList != null)
                str += dec.TypeParameterList.ToString();

            container.HostTypes.Add(str); // TODO...
            // TODO: parent class if any...

            break;
        }

        return container;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    /// <summary>
    /// The collection of file-level pragmas to be emitted in the generated file.
    /// </summary>
    public List<string> FilePragmas { get; } = [];

    /// <summary>
    /// The collection of file-level usings to be emitted in the generated file.
    /// </summary>
    public List<string> FileUsings { get; } = [];

    /// <summary>
    /// The collection of host namespaces.
    /// </summary>
    public List<HostNamespace> HostNamespaces { get; } = [];

    /// <summary>
    /// The collection of host types' file-name parts.
    /// </summary>
    public List<string> HostTypes { get; } = [];

    // ====================================================
    /// <summary>
    /// Represents a host namespace in a container.
    /// </summary>
    [SuppressMessage("", "IDE1006")]
    public record HostNamespace(string name)
    {
        /// <summary>
        /// The file-name part of this namespace.
        /// </summary>
        public string Name { get; } = name.NotNullNotEmpty(trim: true);

        /// <summary>
        /// The collection of namespace-level usings.
        /// </summary>
        public List<string> Usings { get; } = [];
    }
}