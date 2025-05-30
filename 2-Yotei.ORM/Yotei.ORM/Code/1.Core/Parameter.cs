﻿using THost = Yotei.ORM.Code.Parameter;
using IHost = Yotei.ORM.IParameter;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InheritWiths]
public partial class Parameter : IHost
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public Parameter(string name, object? value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Parameter(THost source)
    {
        source.ThrowWhenNull();

        Name = source.Name;
        Value = source.Value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Name}='{Value.Sketch()}'";

    /// <inheritdoc/>
    public string Name
    {
        get => _Name;
        init => _Name = value.NotNullNotEmpty();
    }
    string _Name = default!;

    /// <inheritdoc/>
    public object? Value { get; init; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IHost? other) => Equals(other, caseSensitiveNames: true);

    /// <summary>
    /// Indicates whether this object is equal to another object of the same type, using the
    /// given comparison.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitiveNames"></param>
    /// <returns></returns>
    public virtual bool Equals(IHost? other, bool caseSensitiveNames)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;
        if (other is not IHost valid) return false;

        if (string.Compare(Name, valid.Name, !caseSensitiveNames) != 0) return false;
        if (!CompareValues()) return false;

        return true;

        bool CompareValues() // Use 'is' instead of '=='...
        {
            if (Value is null && valid.Value is null) return true;
            if (Value is null || valid.Value is null) return false;

            return Value.Equals(valid.Value);
        }
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(THost? host, IHost? item) // Use 'is' instead of '=='...
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Name);
        code = HashCode.Combine(code, Value);
        return code;
    }
}