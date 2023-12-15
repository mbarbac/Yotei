namespace Yotei.ORM.Entities.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEntityMap{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[SuppressMessage("", "IDE0290")]
public class EntityMap<T> : IEntityMap<T>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public EntityMap(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance and registers it into the associated connection, becoming or
    /// not the default map as requested.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="asDefault"></param>
    public EntityMap(IConnection connection, bool asDefault)
        : this(connection)
        => Connection.Maps.Add(this, asDefault);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var type = GetType();
        var str = type.EasyName();
        if (!type.IsGenericType) str += $"<{EntityType.EasyName()}>";

        return str;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Type EntityType => typeof(T);
}