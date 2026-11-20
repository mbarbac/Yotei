#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

using StringSpan = System.ReadOnlySpan<char>;
namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class StringExtensions
{
    extension(string source)
    {
        public string RemoveLast(StringSpan value) => throw null;
    }
}