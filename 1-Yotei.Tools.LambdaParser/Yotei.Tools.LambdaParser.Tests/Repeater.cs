using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
public static class Repeater
{
    public static void Repeat(this Action action, int count = 3)
    {
        action.ThrowWhenNull();

        if (count < 1) count = 1;
        for (int i = 1; i <= count; i++)
        {
            WriteHeader(count, i);
            action();
        }

        static void WriteHeader(int count, int i)
        {
            if (count > 1)
            {
                WriteLine();
                WriteLine(DarkYellow, "----------------------------------------");
                WriteLine(DarkYellow, $"Repetition #: {i}");
            }
        }
    }
}