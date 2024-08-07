﻿namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IEngine"/>
[InheritWiths]
public partial class Engine : IEngine
{
    public const bool CASESENSITIVENAMES = false;
    public const string NULLVALUELITERAL = "NULL";
    public const bool POSITIONALPARAMETERS = false;
    public const string PARAMETERSPREFIX = "#";
    public const bool USETERMINATORS = true;
    public const char LEFTTERMINATOR = '[';
    public const char RIGHTTERMINATOR = ']';

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public Engine() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Engine(IEngine source)
    {
        CaseSensitiveNames = source.CaseSensitiveNames;
        NullValueLiteral = source.NullValueLiteral;
        PositionalParameters = source.PositionalParameters;
        ParametersPrefix = source.ParametersPrefix;
        UseTerminators = source.UseTerminators;
        LeftTerminator = source.LeftTerminator;
        RightTerminator = source.RightTerminator;
    }

    /// <inheritdoc/>
    public override string ToString() => "ORM.Engine";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveNames { get; init; } = CASESENSITIVENAMES;

    /// <inheritdoc/>
    public string NullValueLiteral
    {
        get => _NullValueLiteral;
        init => _NullValueLiteral = value.NotNullNotEmpty();
    }
    string _NullValueLiteral = NULLVALUELITERAL;

    /// <inheritdoc/>
    public bool PositionalParameters { get; init; } = POSITIONALPARAMETERS;

    /// <inheritdoc/>
    public string ParametersPrefix
    {
        get => _ParametersPrefix;
        init => _ParametersPrefix = value.NotNullNotEmpty();
    }
    string _ParametersPrefix = PARAMETERSPREFIX;

    /// <inheritdoc/>
    public bool UseTerminators { get; init; } = USETERMINATORS;

    static char ValidateTerminator(char value)
    {
        if (value < 32) throw new ArgumentException("Invalid terminator.").WithData(value);
        if (value == '.') throw new ArgumentException("Dots cannot be terminators.").WithData(value);
        if (value == ' ') throw new ArgumentException("Spaces cannot be terminators.").WithData(value);
        return value;
    }

    /// <inheritdoc/>
    public char LeftTerminator
    {
        get => _LeftTerminator;
        init => _LeftTerminator = ValidateTerminator(value);
    }
    char _LeftTerminator = LEFTTERMINATOR;

    /// <inheritdoc/>
    public char RightTerminator
    {
        get => _RightTerminator;
        init => _RightTerminator = ValidateTerminator(value);
    }
    char _RightTerminator = RIGHTTERMINATOR;
}