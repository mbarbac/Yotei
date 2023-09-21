namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Determines the behavior of a <see cref="ICoreList{T}"/> when adding or inserting duplicate
/// elements.
/// </summary>
public enum CoreListBehavior
{
    /// <summary>
    /// Adds or inserts the duplicate element. This is the default behavior that mimics the one
    /// of the standard list.
    /// </summary>
    Add,

    /// <summary>
    /// Throw an exception if a duplicate element is added or inserted to the list. This setting
    /// permits defining list with no duplicates.
    /// </summary>
    Throw,

    /// <summary>
    /// Ignores the request to add or insert the duplicate element. This setting is used in niche
    /// scenarios where defining a list with no duplicates, but it is just fine to discard the
    /// new elements.
    /// </summary>
    Ignore,
}