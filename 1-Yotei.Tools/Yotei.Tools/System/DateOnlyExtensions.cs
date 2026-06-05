namespace Yotei.Tools;

// ========================================================
public static class DateOnlyExtensions
{
    extension(DateOnly source)
    {
        /// <summary>
        /// Returns an ISO-compliant representation.
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToIsoString(char separator = '/')
            => $"{source.Year:D4}{separator}{source.Month:D2}{separator}{source.Day:D2}";
    }
}