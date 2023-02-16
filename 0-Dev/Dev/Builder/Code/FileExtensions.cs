namespace Dev.Builder;

// ========================================================
public static class FileExtensions
{
    /// <summary>
    /// Determines if this file is a debug one, or not.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool IsDebug(this File file)
    {
        file = file.ThrowIfNull();

        return
            file.Path.EndsWith("\\debug", Program.Comparison) ||
            file.Path.Contains("\\debug\\", Program.Comparison);
    }

    /// <summary>
    /// Determines if this file is a release one, or not.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool IsRelease(this File file)
    {
        file = file.ThrowIfNull();

        return
            file.Path.EndsWith("\\release", Program.Comparison) ||
            file.Path.Contains("\\release\\", Program.Comparison);
    }
}