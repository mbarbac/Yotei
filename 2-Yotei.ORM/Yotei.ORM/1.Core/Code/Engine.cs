﻿namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEngine"/>
/// </summary>
[InheritWiths<IEngine>]
public partial class Engine : IEngine
{
    public const bool IGNORECASE = true;
    public const string NULLVALUELITERAL = "NULL";
    public const bool POSITIONALPARAMETERS = false;
    public const string PARAMETERPREFIX = "#";
    public const bool NATIVEPAGING = false;
    public const bool USETERMINATORS = true;
    public const char LEFTTERMINATOR = '[';
    public const char RIGHTTERMINATOR = ']';

    static char ValidateTerminator(char value)
    {
        if (value <= ' ') throw new ArgumentException("Invalid terminator.").WithData(value);
        if (value == '.') throw new ArgumentException("Invalid terminator.").WithData(value);
        return value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public Engine()
    {
        IgnoreCase = IGNORECASE;
        NullValueLiteral = NULLVALUELITERAL;
        PositionalParameters = POSITIONALPARAMETERS;
        ParameterPrefix = PARAMETERPREFIX;
        NativePaging = NATIVEPAGING;
        UseTerminators = USETERMINATORS;
        LeftTerminator = LEFTTERMINATOR;
        RightTerminator = RIGHTTERMINATOR;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source)
    {
        source.ThrowWhenNull();

        IgnoreCase = source.IgnoreCase;
        NullValueLiteral = source.NullValueLiteral;
        PositionalParameters = source.PositionalParameters;
        ParameterPrefix = source.ParameterPrefix;
        NativePaging = source.NativePaging;
        UseTerminators = source.UseTerminators;
        LeftTerminator = source.LeftTerminator;
        RightTerminator = source.RightTerminator;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => "ORM.Engine";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns><inheritdoc/></returns>
    public virtual bool Equals(IEngine? other)
    {
        if (other is null) return false;

        return
            IgnoreCase == other.IgnoreCase &&
            string.Compare(NullValueLiteral, other.NullValueLiteral, IgnoreCase) == 0 &&
            PositionalParameters == other.PositionalParameters &&
            string.Compare(ParameterPrefix, other.ParameterPrefix, IgnoreCase) == 0 &&
            NativePaging == other.NativePaging &&
            UseTerminators == other.UseTerminators &&
            LeftTerminator == other.LeftTerminator &&
            RightTerminator == other.RightTerminator;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns><inheritdoc/></returns>
    public override bool Equals(object? obj) => Equals(obj as IEngine);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, IgnoreCase);
        code = HashCode.Combine(code, NullValueLiteral);
        code = HashCode.Combine(code, PositionalParameters);
        code = HashCode.Combine(code, ParameterPrefix);
        code = HashCode.Combine(code, NativePaging);
        code = HashCode.Combine(code, UseTerminators);
        code = HashCode.Combine(code, LeftTerminator);
        code = HashCode.Combine(code, RightTerminator);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IgnoreCase { get; init; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string NullValueLiteral { get; init => field = value.NotNullNotEmpty(true); }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool PositionalParameters { get; init; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string ParameterPrefix { get; init => field = value.NotNullNotEmpty(true); }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool NativePaging { get; init; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool UseTerminators { get; init; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public char LeftTerminator { get; init => field = ValidateTerminator(value); }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public char RightTerminator { get; init => field = ValidateTerminator(value); }
}