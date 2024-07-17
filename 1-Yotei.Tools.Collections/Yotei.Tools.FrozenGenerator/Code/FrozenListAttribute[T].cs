namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Decorates classes for which the 'FrozenList[T]' one will be used as their base type,
/// and its related elements upcasted to the decorated type.
/// <br/><br/>
/// By convention, the first interface listed in the type declaration, if any, will be used to
/// implement explicitly the 'FrozenList[T]' methods.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class FrozenListAttribute<T> : Attribute { }