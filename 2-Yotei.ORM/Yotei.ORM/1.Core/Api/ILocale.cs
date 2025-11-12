namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Provides support for culture-sensitive environments.
/// </summary>
public interface ILocale
{
    /// <summary>
    /// Represents read-only support for a given culture.
    /// </summary>
    CultureInfo CultureInfo { get; }

    /// <summary>
    /// Defines string comparison options to use with <see cref="CompareInfo"/>.
    /// </summary>
    CompareOptions CompareOptions { get; }
}