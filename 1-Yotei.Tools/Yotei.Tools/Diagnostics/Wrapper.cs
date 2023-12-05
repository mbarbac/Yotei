namespace Yotei.Tools.Diagnostics;

// ========================================================
internal static class Wrappers
{
    /// <summary>
    /// An array that contains just one null element.
    /// </summary>
    internal static object?[] NullArray { get; } = [];

    /// <summary>
    /// Determines if the DEBUG environment is at the start of a new line, or not.
    /// </summary>
    internal static bool DebugAtOrigin { get; set; } = true;

    /// <summary>
    /// Splits the given value into its regular and NL parts, which are included by default.
    /// If the value is null or empty, then an empty array is returned.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="addNL"></param>
    /// <returns></returns>
    [SuppressMessage("Style", "IDE0305:Simplify collection initialization")]
    internal static string[] TokenizeNL(string? value, bool addNL = true)
    {
        if (value == null) return [];
        if (value.Length == 0) return [];

        var list = new List<string>();
        var span = value.AsSpan();

        while (true)
        {
            var token = "\r\n";
            var index = span.IndexOf(token);
            var found = index >= 0;

            if (!found)
            {
                token = "\n";
                index = span.IndexOf(token);
                found = index >= 0;
            }

            if (found)
            {
                if (addNL)
                {
                    list.Add(span[..index].ToString());
                    list.Add(token);
                }
                span = span[(index + token.Length)..];
            }

            else
            {
                list.Add(span.ToString());
                break;
            }
        }

        return list.ToArray();
    }

    /// <summary>
    /// Returns a header with the given number of spaces.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    internal static string Header(int size)
    {
        switch (size)
        {
            case 0: return Header0;
            case 1: return Header1;
            case 2: return Header2;
            case 3: return Header3;
            case 4: return Header4;
            case 5: return Header5;
            case 6: return Header6;
            case 7: return Header7;
            case 8: return Header8;
            case 9: return Header9;
        }

        if (!Headers.TryGetValue(size, out var header))
            Headers.Add(size, header = new string(' ', size));

        return header;
    }
    readonly static string Header0 = string.Empty;
    readonly static string Header1 = " ";
    readonly static string Header2 = "  ";
    readonly static string Header3 = "   ";
    readonly static string Header4 = "    ";
    readonly static string Header5 = "     ";
    readonly static string Header6 = "      ";
    readonly static string Header7 = "       ";
    readonly static string Header8 = "        ";
    readonly static string Header9 = "         ";
    readonly static Dictionary<int, string> Headers = [];
}