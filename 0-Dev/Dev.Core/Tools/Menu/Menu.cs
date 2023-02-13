namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a CLI menu.
/// </summary>
public static class Menu
{
    public const string SeparatorLine = "******************************";

    /// <summary>
    /// Shows a menu with the given items where the user can select one among them to execute.
    /// A default <see cref="MenuExit"/> one is always injected as the first one.
    /// </summary>
    /// <param name="items"></param>
    public static void Run(params MenuItem[] items)
    {
        items = items.ThrowIfNull();

        var list = new List<MenuItem> { new MenuExit() };
        foreach (var action in items) list.Add(action);

        while (true)
        {
            WriteLine();
            WriteLine(Color.Green, SeparatorLine);
            for (int i = 0; i < list.Count; i++)
            {
                Write(Color.Green, $"-[{i,2}]: ");
                list[i].PrintHead();
            }

            while (true)
            {
                WriteLine();
                Write(Color.Green, "Please enter value of selection: ");

                var str = ReadLine();
                if (str != null && str.Trim().Length > 0)
                {
                    var num = int.TryParse(str, out var temp) ? temp : -1;
                    if (num < 0) continue;
                    if (num == 0) return;

                    if (num > 0 && num < list.Count)
                    {
                        list[num].Execute();
                        break;
                    }
                }
            }
        }
    }
}