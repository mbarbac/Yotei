namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates classes for which the 'InvariantList{K,T}' one is used as its base one.
/// <br/> 'Clone()' capability is added automatically if needed.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute<T> : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public InvariantListAttribute()
    {
        TType = typeof(T);
    }

    /// <summary>
    /// The type of the 'T' items the reimplementation refers to.
    /// </summary>
    public Type TType { get; }
}