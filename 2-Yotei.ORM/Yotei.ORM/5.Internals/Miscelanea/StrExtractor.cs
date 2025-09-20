using Pair = (int Ini, int End);

namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Provides methods to extract literals from a given source.
/// </summary>
public record StrExtractor
{
    /// <summary>
    /// Tries to remove the left-most and right-most rounded brackets from the given parts, when
    /// considering them both as as single unit. By default, if brackets are found, heading and
    /// tailing spaces are removed.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <param name="removeoutside"></param>
    /// <returns></returns>
    public static bool RemoveBrackets(ref string left, ref string right, bool removeoutside = true)
    {
        throw null;

        

        
    }

    // ----------------------------------------------------

    const char RINI = '(';
    const char REND = ')';

    /// <summary>
    /// Gets the unmatched rounded brackets pairs from the start of the span. ')(()' => ')('...
    /// </summary>
    static List<Pair> PairsFromIni(ReadOnlySpan<char> span)
    {
        List<Pair> list = []; for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == RINI) { list.Add(new(i, -1)); continue; }
            if (span[i] == REND)
            {
            }
        }
        return list;
    }


    /*
    /// <summary>
    /// Gets the possible rounded brackets pairs from the start of the span. ')(()' => ')('...
    /// </summary>
    static List<Pair> PairsFromIni(ReadOnlySpan<char> span, bool removematched)
    {
        List<Pair> list = []; for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == RINI) { list.Add(new(i, -1)); continue; }
            if (span[i] == REND)
            {
                for (int k = 0; k < list.Count; k++)
                    if (list[k].End < 0) { list[k] = new(list[k].Ini, i); continue; }

                list.Add(new(-1, i));
                continue;
            }
        }
        if (removematched) RemoveMatchedPairs(list);
        return list;
    }

    /// <summary>
    /// Gets the possible rounded brackets pairs from the end of the span. ')(()' => ')('...
    /// </summary>
    static List<Pair> PairsFromEnd(ReadOnlySpan<char> span, bool removematched)
    {
        List<Pair> list = []; for (int i = span.Length - 1; i >= 0; i--)
        {
            if (span[i] == REND) { list.Insert(0, new(-1, i)); continue; }
            if (span[i] == RINI)
            {
                for (int k = 0; k < list.Count; k++)
                    if (list[k].Ini < 0) { list[k] = new(i, list[k].Ini); continue; }

                list.Insert(0, new(i, -1));
                continue;
            }
        }
        if (removematched) RemoveMatchedPairs(list);
        return list;
    }
    */
}
/*

    // ----------------------------------------------------

    readonly bool UseEngine;
    readonly IEngine Engine;
    readonly bool Sensitive;

    /// <summary>
    /// Initializes a new instance that finds literals using the given comparison mode, and does
    /// not use any engine name terminators.
    /// </summary>
    /// <param name="sensitive"></param>
    public StrExtractor(bool sensitive)
    {
        UseEngine = false;
        Engine = null!;
        Sensitive = sensitive;
    }

    /// <summary>
    /// Initializes a new instance that finds literals using the comparison mode of the given
    /// engine, provided those literals are not protected by the engine terminators, if any.
    /// </summary>
    /// <param name="engine"></param>
    public StrExtractor(IEngine engine)
    {
        UseEngine = engine.ThrowWhenNull().UseTerminators;
        Engine = engine;
        Sensitive = engine.CaseSensitiveNames;
    }

    /// <summary>
    /// Determines if the specifications must be isolated, or not.
    /// <br/> The default value of this property is '<c>true</c>'.
    /// </summary>
    public bool IsolatedSpecs { get; init; }

    /// <summary>
    /// Determines if head or tail extra spaces shall be ignored or not.
    /// <br/> The default value of this property is '<c>true</c>'.
    /// </summary>
    public bool IgnoreExtraSpaces { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract from the head of the given source any of the given specifications.
    /// The first specification found, if any, wins.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="main"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public bool ExtractHead(
        string source, out string main, out string found,
        params string[] specs)
    {
        return UseEngine
            ? ExtractHeadEngine(source, out main, out found, specs)
            : ExtractHeadSensitive(source, out main, out found, specs);
    }

    /// <summary>
    /// Performs the extraction using the given comparison mode.
    /// </summary>
    bool ExtractHeadSensitive(
        string source, out string main, out string found,
        params string[] specs)
    {
        throw null;
    }

    /// <summary>
    /// Performs the extraction using the given engine.
    /// </summary>
    bool ExtractHeadEngine(
        string source, out string main, out string found,
        params string[] specs)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract from the tail of the given source any of the given specifications.
    /// The first specification found, if any, wins.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="main"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public bool ExtractTail(
        string source, out string main, out string found,
        params string[] specs)
    {
        return UseEngine
            ? ExtractTailEngine(source, out main, out found, specs)
            : ExtractTailSensitive(source, out main, out found, specs);
    }

    /// <summary>
    /// Performs the extraction using the given comparison mode.
    /// </summary>
    bool ExtractTailSensitive(
        string source, out string main, out string found,
        params string[] specs)
    {
        throw null;
    }

    /// <summary>
    /// Performs the extraction using the given engine.
    /// </summary>
    bool ExtractTailEngine(
        string source, out string main, out string found,
        params string[] specs)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract from the source the first ocurrence of any of the given specifications.
    /// The first specification found, if any, wins.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="main"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public bool ExtractFirst(
        string source, out string main, out string found,
        params string[] specs)
    {
        return UseEngine
            ? ExtractFirstEngine(source, out main, out found, specs)
            : ExtractFirstSensitive(source, out main, out found, specs);
    }

    /// <summary>
    /// Performs the extraction using the given comparison mode.
    /// </summary>
    bool ExtractFirstSensitive(
        string source, out string main, out string found,
        params string[] specs)
    {
        throw null;
    }

    /// <summary>
    /// Performs the extraction using the given engine.
    /// </summary>
    bool ExtractFirstEngine(
        string source, out string main, out string found,
        params string[] specs)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract from the source the last ocurrence of any of the given specifications.
    /// The first specification found, if any, wins.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="main"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public bool ExtractLast(
        string source, out string main, out string found,
        params string[] specs)
    {
        return UseEngine
            ? ExtractLastEngine(source, out main, out found, specs)
            : ExtractLastSensitive(source, out main, out found, specs);
    }

    /// <summary>
    /// Performs the extraction using the given comparison mode.
    /// </summary>
    bool ExtractLastSensitive(
        string source, out string main, out string found,
        params string[] specs)
    {
        throw null;
    }

    /// <summary>
    /// Performs the extraction using the given engine.
    /// </summary>
    bool ExtractLastEngine(
        string source, out string main, out string found,
        params string[] specs)
    {
        throw null;
    }
 */