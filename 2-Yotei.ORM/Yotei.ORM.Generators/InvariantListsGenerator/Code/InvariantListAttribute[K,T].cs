namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates classes for which the 'InvariantList{K,T}' one is used as its base one.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute<K, T> : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public InvariantListAttribute()
    {
        KType = typeof(K);
        TType = typeof(T);
    }

    /// <summary>
    /// The type of the key for the 'InvariantList{K,T}' reimplementation.
    /// </summary>
    public Type KType { get; }

    /// <summary>
    /// The type of the 'T' items the reimplementation refers to.
    /// </summary>
    public Type TType { get; }
}