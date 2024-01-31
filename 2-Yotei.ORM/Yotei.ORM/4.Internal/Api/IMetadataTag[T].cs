namespace Yotei.ORM.Internal;

// ========================================================
/// <inheritdoc cref="IMetadataTag"/>
/// <typeparam name="T"></typeparam>
public interface IMetadataTag<T> : IMetadataTag
{
    /// <inheritdoc cref="IMetadataTag.DefaultValue"/>
    new T DefaultValue { get; }

    // ----------------------------------------------------

    /// <inheritdoc cref="IMetadataTag.Replace(string, string)"/>
    new IMetadataTag<T> Replace(string oldname, string newname);

    /// <inheritdoc cref="IMetadataTag.Add(string)"/>
    new IMetadataTag<T> Add(string name);

    /// <inheritdoc cref="IMetadataTag.AddRange(IEnumerable{string})"/>
    new IMetadataTag<T> AddRange(IEnumerable<string> range);

    /// <inheritdoc cref="IMetadataTag.Remove(string)"/>
    new IMetadataTag<T> Remove(string name);

    /// <inheritdoc cref="IMetadataTag.Clear"/>
    new IMetadataTag<T> Clear();
}