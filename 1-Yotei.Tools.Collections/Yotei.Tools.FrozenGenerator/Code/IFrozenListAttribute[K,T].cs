namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Decorates interfaces for which the 'IFrozenList[K, T]' one will be implemented, if needed,
/// and its elements upcasted to the decorated type.
/// <br/> The host type must comply will all the needed requeriments, such as either having a
/// 'Clone()' method, or be decorated with the '[Cloneable]' attribute.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IFrozenListAttribute<K, T> : Attribute { }