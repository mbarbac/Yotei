namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class CandidateExtensions
{






    /*
    /// <summary>
    /// Returns a suitable file name, without extensions, based on the tail-most namespace.
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public static string FileNameByTailNamespace(this INodeCandidate candidate)
    {
        candidate.ThrowWhenNull();

        var nschain = candidate.Syntax.GetNamespaceSyntaxChain();
        var tpchain = candidate.Symbol.GetTypeSymbolChain();
        List<string> parts = [];

        foreach (var ns in nschain)
        {
            var name = ns.Name.LongName();
            var temps = name.Split('.');
            parts.AddRange(temps);
        }

        parts.Reverse();
        return string.Join(".", parts);
    }

    /// <summary>
    /// Returns a suitable file name, without extensions, based on the tail-most type.
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public static string FileNameByTailType(this INodeCandidate candidate)
    {
        candidate.ThrowWhenNull();

        var nschain = candidate.Syntax.GetNamespaceSyntaxChain();
        var tpchain = candidate.Symbol.GetTypeSymbolChain();
        List<string> parts = [];

        foreach (var ns in nschain)
        {
            var name = ns.Name.LongName();
            var temps = name.Split('.');
            parts.AddRange(temps);
        }

        foreach (var tp in tpchain)
        {
            var name = tp.Name;
            
            if (name.Length == 0) name = "$";
            else
            {
                var index = name.IndexOf('`');
                if (index > 0) name = name[..index];
            }
            parts.Add(name);
        }

        parts.Reverse();
        return string.Join(".", parts);
    }*/
}