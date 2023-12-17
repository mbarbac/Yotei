namespace Yotei.ORM;

// ========================================================
public static class EngineExtensions
{
    /// <summary>
    /// Returns a list with the indexes of the unwrapped ocurrences of the given character in
    /// the given source value. Unwrapped ocurrences are those not protected by the terminators
    /// of the engine, if any are used.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <param name="ch"></param>
    /// <returns></returns>
    public static List<int> UnwrappedIndexes(this IEngine engine, string? value, char ch)
    {
        engine.ThrowWhenNull();

        var nums = new List<int>();
        var deep = 0;

        // Obvious case...
        if (value is null) return nums;

        // No terminators...
        if (!engine.UseTerminators)
        {
            for (int i = 0; i < value.Length; i++) if (value[i] == ch) nums.Add(i);
            return nums;
        }

        // Left and Right terminators are the same...
        if (engine.LeftTerminator == engine.RightTerminator)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == engine.LeftTerminator) { deep = deep == 0 ? 1 : 0; continue; }
                if (value[i] == ch && deep == 0) nums.Add(i);
            }
            return nums;
        }

        // Different terminators...
        else
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == engine.LeftTerminator) { deep++; continue; }
                if (value[i] == engine.RightTerminator) { deep--; if (deep < 0) deep = 0; continue; }
                if (value[i] == ch && deep == 0) nums.Add(i);
            }
            return nums;
        }
    }
}