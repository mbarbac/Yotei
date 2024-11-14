/*
namespace Runner;

// ========================================================
internal static class Wrappers
{
    // ----------------------------------------------------

    /// <summary>
    /// Returns a header with the given number of spaces.
    /// </summary>
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

        if (!Headers.TryGetValue(size, out var header)) Headers.Add(size, header = new(' ', size));
        return header;
    }
    readonly static string Header0 = string.Empty;
    readonly static string Header1 = new(' ', 1);
    readonly static string Header2 = new(' ', 2);
    readonly static string Header3 = new(' ', 3);
    readonly static string Header4 = new(' ', 4);
    readonly static string Header5 = new(' ', 5);
    readonly static string Header6 = new(' ', 6);
    readonly static string Header7 = new(' ', 7);
    readonly static string Header8 = new(' ', 8);
    readonly static string Header9 = new(' ', 9);
    readonly static Dictionary<int, string> Headers = [];
}*/