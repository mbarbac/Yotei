#if NETSTANDARD2_0
namespace Yotei.Tools.Std20;
#pragma warning disable IDE0079
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static class IEnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>
        /// Returns an array based in this enumeration, if the project was compile under a DEBUG
        /// environment, or the original enumeration otherwise.
        /// </summary>
        /// <returns></returns>
#if DEBUG
        public T[] ToDebugArray() => [.. source];
#else
        IEnumerable<T> ToDebugArray() => source;
#endif
    }
}