#if YOTEI_TOOLS_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// <inheritdoc cref="IsNullable{T}"/>
/// </summary>
[AttributeUsage(AttributeTargets.All)]
#if YOTEI_TOOLS_COREGENERATOR
internal
#else
public
#endif
class IsNullableAttribute : Attribute
{ }