namespace Yotei.ORM.Entities;

// ========================================================
/// <summary>
/// Represents the collection of entity maps known to a given connection.
/// </summary>
public interface IEntityMapCollection : IEnumerable<IEntityMap>
{
    /// <summary>
    /// The number of maps registered into this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the collection of maps registered into this instance for the given entity type. The
    /// first element in the returned collection, if any, is considered the default map for that
    /// entity type.
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    List<IEntityMap> Find(Type entityType);

    /// <summary>
    /// Tries to find the default map for the given entity type. If so, returns <c>true</c> and
    /// that out argument is set to this map. Otherwise, returns <c>false</c> and that argument
    /// is set to null.
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    bool Find(Type entityType, out IEntityMap? map);

    /// <summary>
    /// Tries to find the default map for the given entity type. If so, returns <c>true</c> and
    /// that out argument is set to this map. Otherwise, returns <c>false</c> and that argument
    /// is set to null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map"></param>
    /// <returns></returns>
    bool Find<T>(out IEntityMap<T>? map);

    /// <summary>
    /// Registers the given map into this collection, if it was not registered yet. By default,
    /// the map being added becomes the default map for its entity type.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="asDefault"></param>
    /// <returns></returns>
    bool Add(IEntityMap map, bool asDefault = true);

    /// <summary>
    /// Removes the given map from this collection. Removed maps are still functional, but are
    /// not discoverable by the associated connection.
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    bool Remove(IEntityMap map);

    /// <summary>
    /// Clears this collection.
    /// </summary>
    void Clear();
}