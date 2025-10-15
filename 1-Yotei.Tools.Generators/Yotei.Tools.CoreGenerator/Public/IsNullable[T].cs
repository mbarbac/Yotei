namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Used to identifiy types that shall be treated as nullable ones by source code generators.
/// <br/> This type is typically used in scenarios where neither 'T?' nor 'Nullable{T}' are
/// accepted by the compiler but, conceptually, that was the application code would have liked
/// to do.
/// <br/> If several tree-based generators are imported by the same assembly, this type may
/// appear as duplicate. The best ways to avoid this are either that each imported generator
/// defines this type, or instruct the consuming application to do so.S
/// </summary>
/// <typeparam name="T"></typeparam>
public class IsNullable<T> { }