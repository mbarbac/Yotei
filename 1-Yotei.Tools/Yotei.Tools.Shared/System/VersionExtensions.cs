namespace Yotei.Tools;

// ========================================================
public static class VersionExtensions
{
    extension(System.Version source)
    {
        /// <summary>
        /// Returns a 'major.minor.revision' string representation of the given version.
        /// </summary>
        /// <returns></returns>
        public string To3String()
        {
            var major = source.Major;
            var minor = source.Minor;
            var revision = source.Revision < 0 ? 0 : source.Revision;

            return $"{major}.{minor}.{revision}";
        }
    }
}