using static System.ConsoleColor;
using System.Net.Http.Json;

namespace Runner;

// ========================================================
public static class NuGet
{
    /// <summary>
    /// Obtains the latest published version of the NuGet package identified by its name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="includePreRelease"></param>
    /// <returns></returns>
    public static string? GetLatestVersion(
        string name,
        bool includePreRelease = false)
    {
        using var http = new HttpClient();

        string lower = name.ToLowerInvariant();
        string url = $"https://api.nuget.org/v3-flatcontainer/{lower}/index.json";

        var response =
            http.GetFromJsonAsync<NuGetVersionResponse>(url)
            .GetAwaiter()
            .GetResult();

        if (response is null ||
            response.Versions == null ||
            response.Versions.Length == 0) return null;

        var versions = includePreRelease
            ? response.Versions
            : response.Versions.Where(x => !x.Contains('-'));

        return versions.LastOrDefault();
    }

    private record NuGetVersionResponse(string[] Versions);

    // ----------------------------------------------------

    /// <summary>
    /// Gets the NuGet package files in the directory of the given project.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="regulars"></param>
    /// <param name="symbols"></param>
    public static void GetFiles(
        ProjectFile project,
        out List<FileInfo> regulars, out List<FileInfo> symbols)
    {
        var dir = new DirectoryInfo(project.Directory);
        if (dir.Exists)
        {
            regulars = [.. dir.GetFiles("*.nupkg", SearchOption.AllDirectories)];
            symbols = [.. dir.GetFiles("*.snupkg", SearchOption.AllDirectories)];
        }
        else
        {
            regulars = [];
            symbols = [];
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to delete the given collection of files.
    /// </summary>
    public static void DeleteFiles(IEnumerable<FileInfo> items)
    {
        foreach (var item in items)
        {
            try { item.Delete(); }
            catch (Exception)
            {
                Console.WriteEx(true, Red, "Cannot delete file: ");
                Console.WriteLineEx(true, item.FullName);
                throw;
            }
        }
    }
}