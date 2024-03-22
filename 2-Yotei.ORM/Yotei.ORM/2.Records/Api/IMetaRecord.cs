namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a record that is described by associated metadata.
/// </summary>
public interface IMetaRecord : IEquatable<IMetaRecord>
{
    /// <summary>
    /// The actual record this instance refers to.
    /// </summary>
    IRecord Record { get; }

    /// <summary>
    /// The schema that describes the record in this instance.
    /// </summary>
    ISchema Schema { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Compares this instance against the given target one and returns the changes detected, or
    /// null if any. Orphan sources entries and target ones are taken into consideration only if
    /// explicitly requested.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    IMetaRecord? CompareTo(
        IMetaRecord target,
        bool orphanSources = false, bool orphanTargets = false);

    /// <summary>
    /// Compares this instance against the given target one and returns the changes detected, or
    /// null if any. Orphan sources entries and target ones are taken into consideration only if
    /// explicitly requested.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="comparer"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    IMetaRecord? CompareTo(
        IMetaRecord target,
        IEqualityComparer comparer,
        bool orphanSources = false, bool orphanTargets = false);
}