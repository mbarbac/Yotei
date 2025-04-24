namespace Experimental.WeakObjects;

// ========================================================
/// <summary>
/// Represents an object that can be weakened so that it can be garbage collected as needed.
/// <br/> Collections manage their own contents accordingly.
/// </summary>
public interface IWeakObject
{
    /// <summary>
    /// Determines whether this instance is still a valid one or not.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Gets when it was the last time this instance was created or its contents obtained or
    /// updated, in UTC.
    /// </summary>
    DateTime LastUsed { get; }

    /// <summary>
    /// Rehydrates this instance.
    /// </summary>
    void Hydrate();

    /// <summary>
    /// Inconditionally weakens this instance.
    /// </summary>
    void Weaken();

    /// <summary>
    /// Weakens this instance if it has not been used for the given amount of time.
    /// </summary>
    /// <param name="timeout"></param>
    void Weaken(TimeSpan timeout);
}