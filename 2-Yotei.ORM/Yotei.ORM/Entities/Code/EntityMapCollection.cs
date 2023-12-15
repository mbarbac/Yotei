namespace Yotei.ORM.Entities.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEntityMapCollection"/>
/// </summary>
public class EntityMapCollection : IEntityMapCollection
{
    readonly List<IEntityMap> Items = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public EntityMapCollection() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IEntityMap> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public List<IEntityMap> Find(Type entityType)
    {
        entityType.ThrowWhenNull();
        return Items.Where(x => x.EntityType == entityType).ToList();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    public bool Find(Type entityType, out IEntityMap? map)
    {
        entityType.ThrowWhenNull();

        map = Items.Count == 0 ? null : Items[0];
        return map is not null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map"></param>
    /// <returns></returns>
    public bool Find<T>(out IEntityMap<T>? map)
    {
        var r = Find(typeof(T), out var temp);
        map = r ? (IEntityMap<T>?)temp : null;
        return r;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="map"></param>
    /// <param name="asDefault"></param>
    /// <returns></returns>
    public bool Add(IEntityMap map, bool asDefault = true)
    {
        map.ThrowWhenNull();

        if (!Items.Contains(map))
        {
            if (asDefault) Items.Insert(0, map); else Items.Add(map);
            return true;
        }
        else return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    public bool Remove(IEntityMap map) => Items.Remove(map.ThrowWhenNull());

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Clear() => Items.Clear();
}