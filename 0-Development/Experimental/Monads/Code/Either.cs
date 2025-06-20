namespace Experimental.Monads;

// ========================================================
/// <summary>
/// Helpers for the <see cref="Either{L, R}"/> monad.
/// </summary>
public static class Either
{
    /// <summary>
    /// Returns a new L-instance.
    /// </summary>
    /// <typeparam name="L"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Either<L, R> Left<L, R>(L value) => new(value);

    /// <summary>
    /// Returns a new R-instance.
    /// </summary>
    /// <typeparam name="L"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Either<L, R> Right<L, R>(R value) => new(value);
}