namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Maintains a collection of key-value option pairs read from a generator project configuration
/// file, where the keys are case insensitive, along with some standard entries as properties.
/// </summary>
public class TreeOptions : IEquatable<TreeOptions>
{
    readonly Dictionary<string, string> Items;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    [SuppressMessage("", "IDE0028")]
    [SuppressMessage("", "IDE0290")]
    public TreeOptions() => Items = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance and loads the 'name = value' pairs contained in the given text,
    /// aggregating any errors produced.
    /// </summary>
    /// NOTE: Yes, I know, using an 'out' argument in a constructor is not considered a best
    /// practice, but in this case is quite convenient.
    /// <param name="text"></param>
    /// <param name="errors"></param>
    public TreeOptions(
        string? text, out IEnumerable<Exception> errors) : this() => Load(text, out errors);

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
    /// Loads into this instance the 'name = value' pairs contained in the given text. Each pair
    /// must appear in its own line. Null or empty (spaces only) lines are discarded, as well as
    /// those that start with a '//' sequence. Returns true if all lines were parsed without any
    /// errors, of false otherwise. In the later case, the out argument contains the collection
    /// of found errors.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="errors"></param>
    /// <returns></returns>
    public bool Load(string? text, out IEnumerable<Exception> errors)
    {
        var items = new List<Exception>();
        errors = items;

        if (text is null) return false;

        string line;
        using var reader = new StringReader(text);

        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            if (line.Length == 0) continue;
            if (line.StartsWith("//")) continue;

            var index = line.IndexOf('=');
            if (index < 0)
            {
                items.Add(new ArgumentException("No '=' found in line.").WithData(line));
                continue;
            }

            var key = line[..index].NullWhenEmpty(true);
            if (key is null)
            {
                items.Add(new ArgumentException("No key/name element found in line.").WithData(line));
                continue;
            }

            var value = line[(index + 1)..].Trim();
            Items[key] = value;
        }

        return true;
    }

    /// <summary>
    /// Tries to obtain the value associated with the given case insensitive name/key. If so, the
    /// value is casted to the given type (supported ones are 'bool', 'int' and 'string').
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
        if (other is null) return false;
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
}