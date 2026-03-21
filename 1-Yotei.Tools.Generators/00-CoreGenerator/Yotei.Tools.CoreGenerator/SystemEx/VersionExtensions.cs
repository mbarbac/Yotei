namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class VersionExtensions
{
    extension(System.Version source)
    {
        /// <summary>
        /// Gets a 'major.minor.revision' string representation.
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