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
static class ExceptionExtensions
{
    extension<T>(T source) where T : Exception
    {
        /// <summary>
        /// Adds or replaces the given [name=value] pair in the exception's dictionary.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public T WithData(object? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (string.IsNullOrWhiteSpace(name)) name = nameof(value);
            source.Data[name] = value;
            return source;
        }
    }
}