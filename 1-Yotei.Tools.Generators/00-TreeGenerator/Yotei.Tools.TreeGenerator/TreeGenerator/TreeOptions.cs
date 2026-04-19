namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Maintains options read from the consuming project's tree configuration file.
/// </summary>
public class TreeOptions : IEquatable<TreeOptions>
{
    readonly Dictionary<string, string> Items;

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
    /// Determines if the <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/>
    /// nullability helpers are to be emitted.
    /// <br/> The default value of this property is <see cref="true"/>.
    /// </summary>
    public bool EmitNullabilityHelpers
        => !TryGet<bool>(nameof(EmitNullabilityHelpers), out var temp) || temp;

    /// <summary>
    /// Determines if the generated files shall be emitted using all its first-level dot-separated
    /// parts, but the last one, as the folder specification. If the value of this property is not
    /// true, then flat file names are used instead.
    /// <br/> The default value of this property is <see cref="false"/>.
    /// </summary>
    public bool GenerateFilesInFolders
        => TryGet<bool>(nameof(GenerateFilesInFolders), out var temp) && temp;

    /// <summary>
    /// Determines if the first-level dot-separated parts of the names of the generated files
    /// shall be reversed or not.
    /// <br/> The default value of this property is <see cref="false"/>.
    /// </summary>
    public bool ReverseGeneratedFileNames
        => TryGet<bool>(nameof(ReverseGeneratedFileNames), out var temp) && temp;

    // ----------------------------------------------------

    /// <summary>
    /// Tries to obtain the value associated to the given case insensitive key, casted to the
    /// given type. The only supported ones are 'bool', 'int' and 'string'.
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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(TreeOptions other)
    {
        if (Items.Count != other.Items.Count) return false;

        foreach (var key in Items.Keys)
        {
            if (!other.Items.TryGetValue(key, out var value)) return false;
            if (Items[key] != value) return false;
        }
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        int code = 0;
        foreach (var kvp in Items)
        {
            code = HashCode.Combine(code, kvp.Key.ToLower());
            code = HashCode.Combine(code, kvp.Value);
        }
        return code;
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