using System.Collections.Specialized;

namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Maintains options read from the consuming project's tree configuration file.
/// </summary>
public class TreeOptions
{
    readonly Dictionary<string, string> Items;

    public const string EmitNullabilityHelpers = nameof(EmitNullabilityHelpers);
    public const string GeneratedFilesInFolders = nameof(GeneratedFilesInFolders);
    public const string ReverseGeneratedFileNames = nameof(ReverseGeneratedFileNames);

    /// <summary>
    /// Initializes a new instance with the given contents.
    /// </summary>
    /// <param name="text"></param>
    [SuppressMessage("", "IDE0028")]
    public TreeOptions(string? text = null)
    {
        Items = new(StringComparer.OrdinalIgnoreCase);
        Parse(text);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to obtain the value associated to the given case insensitive key, casted to the
    /// given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGet<T>(string name, out T value)
    {
        name = name.NotNullNotEmpty(trim: true);

        if (Items.TryGetValue(name, out var temp))
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    if (bool.TryParse(temp, out var vbool))
                    { value = (T)(object)vbool; return true; }
                    break;

                case TypeCode.Int32:
                    if (int.TryParse(temp, out var vint))
                    { value = (T)(object)vint; return true; }
                    break;

                case TypeCode.String:
                    value = (T)(object)temp;
                    return true;
            }
        }
        value = default!;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given contents capturing its name-value pairs.
    /// </summary>
    /// <param name="text"></param>
    void Parse(string? text)
    {
        if (text is null) return;

        string? line;
        int error = 0;

        using var reader = new StringReader(text);
        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("//")) continue;
            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                var index = line.IndexOf('='); if (index < 0) throw new Exception();
                var key = line[..index].NullWhenEmpty(trim: true) ?? throw new Exception();
                var value = line[(index + 1)..].NullWhenEmpty(trim: true);

                if (value != null) Items.Add(key, value);
            }
            catch
            {
                error++;
                Items.Add($"Error#{error}", line);
            }
        }
    }
}