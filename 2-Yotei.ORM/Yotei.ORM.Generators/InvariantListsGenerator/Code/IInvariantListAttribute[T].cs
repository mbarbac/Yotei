namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates interfaces for which the 'IInvariantList{T}' one is to be reimplemented.
/// <br/> 'Clone()' capability is added automatically if needed.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute<T> : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public IInvariantListAttribute()
    {
        TType = typeof(T);
    }

    /// <summary>
    /// The type of the 'T' items the reimplementation refers to.
    /// </summary>
    public Type TType { get; }
}