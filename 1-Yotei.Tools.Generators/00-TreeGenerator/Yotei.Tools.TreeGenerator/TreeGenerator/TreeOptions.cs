#pragma warning disable IDE0028

namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Maintains options read from the consuming project's tree configuration file.
/// </summary>
public class TreeOptions : IEquatable<TreeOptions>
{
    readonly Dictionary<string, string> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public TreeOptions() => Items = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance with the 'name = value' elements obtained from parsing the
    /// given text (potentially a multi-line one). If the givent text is null, then it is just
    /// ignored.
    /// </summary>
    /// <param name="text"></param>
    public TreeOptions(string? text) : this()
    {
        if (!TryAdd(text, out var error) && error is not null) throw error;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to add to this instance the 'name = value' elements obtained from parsing the given
    /// text, potentially a multi-line one. If that text is null, then it is just ignored and this
    /// method returns false. Returns true if successfully parsed and its elements added to this
    /// instance, or false an the error detected in the out argument.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public bool TryAdd(string? text, out Exception? error)
    {
        error = null;
        if (text is null) return false;

        string? line;
        using var reader = new StringReader(text);

        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("//")) continue;
            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                var index = line.IndexOf('=');
                if (index < 0) throw new FormatException("No '=' found").WithData(line);

                var key = line[..index].NullWhenEmpty(trim: true) ??
                    throw new FormatException("No 'name' found").WithData(line);

                var value = line[(index + 1)..].Trim();
                Items[key] = value;
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }

        return true;
    }

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
}