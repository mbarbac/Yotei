#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Extends the <see cref="IAsyncDisposable"/> interface.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
interface IAsyncDisposableEx : IAsyncDisposable, IDisposableEx
{ }