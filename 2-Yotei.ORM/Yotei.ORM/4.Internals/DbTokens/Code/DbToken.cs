namespace Yotei.ORM.Internals;

// ========================================================
public static class DbToken
{
    /// <summary>
    /// Returns a validated token name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ValidateTokenName(string name)
    {
        name = name.NotNullNotEmpty();

        if (!ValidFirstChar(name[0])) throw new ArgumentException(
            "Name contains invalid first character.")
            .WithData(name);

        if (name.Any(x => !ValidOtherChar(x))) throw new ArgumentException(
            "Name contains invalid character(s).")
            .WithData(name);

        return name;

        static bool ValidFirstChar(char c) =>
            VALID_FIRST.Contains(c) ||
            (c >= '0' && c <= '9') ||
            (c >= 'A' && c <= 'Z') ||
            (c >= 'a' && c <= 'z');

        static bool ValidOtherChar(char c) => ValidFirstChar(c);
    }

    static readonly string VALID_FIRST = "_$@#";
}