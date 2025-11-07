namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Used to identify types that shall be treated as nullable ones by source code generators. This
/// type is mostly used when neither '<typeparamref name="T"/>' nor '<see cref="IsNullable{T}"/>'
/// are accepted by the compiler, as for instance in generic arguments.
/// </summary>
/// <typeparam name="T"></typeparam>
/// This type is intentionally internal to prevent duplication: if several generators are used
/// importing this shared project, this type will appear as duplicated. So, if those generators
/// need this capability, they need to declare a type with this name and a single generatic
/// argument in their own namespaces.
internal class IsNullable<T> { }