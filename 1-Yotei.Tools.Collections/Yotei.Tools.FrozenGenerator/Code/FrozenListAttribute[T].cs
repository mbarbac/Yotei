namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Decorates classes that are meant to inherit from the 'FrozenList[T]' one, which will be
/// added to the inheritance chain if needed, and whose elements upcasted to the target type if
/// not already implemented in the decorated class.
/// <br/> The host type must comply will all the needed requeriments, such as either having a
/// 'Clone()' method, or be decorated with the '[Cloneable]' attribute and having an appropriate
/// copy constructor.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class FrozenListAttribute<T> : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    public FrozenListAttribute(Type target) => Target = target.ThrowWhenNull();

    /// <summary>
    /// The target type that the upcasted methods will use.
    /// </summary>
    public Type Target { get; }
}