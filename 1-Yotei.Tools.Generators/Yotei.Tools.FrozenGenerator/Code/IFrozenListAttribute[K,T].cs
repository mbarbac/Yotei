namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Implements the 'IFrozenList{K,T}' on the decorated interface.
/// The inherited members are upcasted so that their new return type is the decorated interface.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IFrozenListAttribute<K, T> : Attribute { }