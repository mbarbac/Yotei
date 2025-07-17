namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates interfaces for which the 'IInvariantList{K,T}' one is to be reimplemented.
/// <br/> 'Clone()' capability is added automatically if needed.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute<K, T> : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public IInvariantListAttribute()
    {
        KType = typeof(K);
        TType = typeof(T);
    }

    /// <summary>
    /// The type of the key for the 'IInvariantList{K,T}' interface reimplementation.
    /// </summary>
    public Type KType { get; }

    /// <summary>
    /// The type of the 'T' items the reimplementation refers to.
    /// </summary>
    public Type TType { get; }
}