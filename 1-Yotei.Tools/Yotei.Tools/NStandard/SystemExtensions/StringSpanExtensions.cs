using StringSpan = System.ReadOnlySpan<char>;

#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class StringSpanExtensions
{
    extension(StringSpan source)
    {
    }
}