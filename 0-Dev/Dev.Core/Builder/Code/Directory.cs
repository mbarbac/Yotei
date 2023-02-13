namespace Dev.Builder;

// ========================================================
public static class DirectoryExtensions
{
    /// <summary>
    /// Find all projects in the given directory and subdirectories, provided the path do not
    /// beguin with the exclude, if it is not empty.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static ImmutableArray<Project> FindProjects(this Directory directory, Directory exclude)
    {
        directory = directory.ThrowIfNull();
        exclude = exclude.ThrowIfNull();

        var list = new List<Project>(); PopulateFrom(directory);
        return list.ToImmutableArray();

        /// <summary> Invoked to populate the list.
        /// </summary>
        void PopulateFrom(Directory directory)
        {
            if (exclude.Path.Length > 0 &&
                directory.Path.StartsWith(exclude.Path, Program.Comparison)) return;

            var files = directory.GetFiles("*.csproj");
            foreach (var file in files) list.Add(new Project(file));

            var dirs = directory.GetDirectories();
            foreach (var dir in dirs) PopulateFrom(dir);
        }
    }
}