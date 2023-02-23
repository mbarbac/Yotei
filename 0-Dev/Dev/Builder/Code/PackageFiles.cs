namespace Dev.Builder;

// ========================================================
/// <summary>
/// Gets the collection of existing package files for a given packable project.
/// </summary>
public class PackageFiles
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="packable"></param>
    public PackageFiles(Packable packable, PushMode mode)
    {
        packable = packable.ThrowIfNull();
        RegularFiles = GetFiles($"{packable.Name}.{packable.Version}.nupkg");
        SymbolFiles = GetFiles($"{packable.Name}.{packable.Version}.snupkg");

        /// <summary> Invoked to get the list of files for the given pattern.
        /// </summary>
        ImmutableArray<File> GetFiles(string pattern)
        {
            var list = new List<File>(); Populate(packable.Path);
            return list.ToImmutableArray();

            /// <summary> Invoked to populate the list of files from the given directory.
            /// </summary>
            void Populate(Directory directory)
            {
                if ((mode == PushMode.Local && directory.IsDebug()) ||
                    (mode == PushMode.Remote && directory.IsRelease()))
                {
                    var files = directory.GetFiles(pattern);
                    foreach (var file in files) list.Add(file);
                }
                foreach (var dir in directory.GetDirectories()) Populate(dir);
            }
        }
    }

    /// <summary>
    /// The collection of regular package files.
    /// </summary>
    public ImmutableArray<File> RegularFiles { get; }

    /// <summary>
    /// The collection of symbol package files.
    /// </summary>
    public ImmutableArray<File> SymbolFiles { get; }
}