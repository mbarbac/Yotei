﻿using System.Diagnostics.Contracts;

namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Provides options for the <see cref="TokenVisitor"/> methods.
/// </summary>
public record TokenOptions
{
    public const bool USENULLSTRING = true;
    public const bool USEQUOTES = true;
    public const bool USETERMINATORS = true;
    public const bool CAPTUREVALUES = true;
    public const bool CONVERTVALUES = true;
    public const string? SEPARATOR = null;

    // ----------------------------------------------------

    /// <summary>
    /// Gets an instance with the default options.
    /// </summary>
    public static TokenOptions Default() => new();

    /// <summary>
    /// Gets an instance with all options set to false or null.
    /// <br/> Note that the locale is set to the culture from which this method is invoked.
    /// </summary>
    /// <returns></returns>
    public static TokenOptions False() => new(
        locale: null,
        useNullString: false,
        useQuotes: false,
        useTerminators: false,
        captureValues: false,
        convertValues: false,
        separator: null);

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="locale"></param>
    /// <param name="useNullString"></param>
    /// <param name="useQuotes"></param>
    /// <param name="useTerminators"></param>
    /// <param name="captureValues"></param>
    /// <param name="convertValues"></param>
    /// <param name="separator"></param>
    public TokenOptions(
        Locale? locale = null,
        bool useNullString = USENULLSTRING,
        bool useQuotes = USEQUOTES,
        bool useTerminators = USETERMINATORS,
        bool captureValues = CAPTUREVALUES,
        bool convertValues = CONVERTVALUES,
        string? separator = SEPARATOR)
    {
        Locale = locale ?? new();
        UseNullString = useNullString;
        UseQuotes = useQuotes;
        UseTerminators = useTerminators;
        CaptureValues = captureValues;
        ConvertValues = convertValues;
        Separator = separator;
    }

    /// <summary>
    /// The locales to use with culture-sensitive elements.
    /// </summary>
    public Locale Locale { get; init; }

    /// <summary>
    /// Whether to use the <see cref="ORM.IEngine.NullValueLiteral"/> value when translating
    /// null values, or rather treat them as regular ones.
    /// </summary>
    public bool UseNullString { get; init; }

    /// <summary>
    /// Whether wrap values that are not captured between quotes, or rather to emit their raw
    /// string representation.
    /// </summary>
    public bool UseQuotes { get; init; }

    /// <summary>
    /// Whether to use the engine terminators when emitting identifiers, if the engine use them,
    /// or rather to use their raw values.
    /// </summary>
    public bool UseTerminators { get; init; }

    /// <summary>
    /// Whether to capture the values found into parameters, or not.
    /// </summary>
    public bool CaptureValues { get; init; }

    /// <summary>
    /// Whether to use the engine converter to convert the captured values into the database
    /// understood ones, or not.
    /// </summary>
    public bool ConvertValues { get; init; }

    /// <summary>
    /// The separator to use elements in a range, or null if it is not used.
    /// </summary>
    public string? Separator { get; init; }
}