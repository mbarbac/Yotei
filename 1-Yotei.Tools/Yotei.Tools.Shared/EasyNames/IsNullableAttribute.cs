#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// <inheritdoc cref="IsNullable{T}"/>
/// </summary>
[AttributeUsage(AttributeTargets.All)]
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
class IsNullableAttribute : Attribute
{ }