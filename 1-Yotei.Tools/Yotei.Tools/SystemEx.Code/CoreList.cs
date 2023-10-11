namespace Yotei.Tools;

// ========================================================
public static class CoreList
{
    /// <summary>
    /// Determines the behavior when adding or inserting duplicate elements.
    /// </summary>
    public enum Behavior
    {
        /// <summary>
        /// Add or insert the duplicate element,
        /// </summary>
        Add,

        /// <summary>
        /// Throw a duplicate exception,
        /// </summary>
        Throw,

        /// <summary>
        /// Just ignore the request of adding or inserting the duplicate element.
        /// </summary>
        Ignore,
    }
}