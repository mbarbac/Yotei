﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

/// <summary>
/// Represent a range has start and end indexes.
/// </summary>
/// <remarks>
/// Range is used by the C# compiler to support the range syntax.
/// <code>
/// int[] someArray = new int[5] { 1, 2, 3, 4, 5 };
/// int[] subArray1 = someArray[0..2]; // { 1, 2 }
/// int[] subArray2 = someArray[1..^0]; // { 2, 3, 4, 5 }
/// </code>
/// </remarks>
[ExcludeFromCodeCoverage]
internal readonly struct Range : IEquatable<Range>
{
    /// <summary>
    /// Represent the inclusive start index of the Range.
    /// </summary>
    public Index Start { get; }

    /// <summary>
    /// Represent the exclusive end index of the Range.
    /// </summary>
    public Index End { get; }

    /// <summary>
    /// Construct a Range object using the start and end indexes.
    /// </summary>
    /// <param name="start">Represent the inclusive start index of the range.</param>
    /// <param name="end">Represent the exclusive end index of the range.</param>
    [SuppressMessage("", "IDE0290")]
    public Range(Index start, Index end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Indicates whether the current Range object is equal to another object of the same
    /// type.
    /// </summary>
    /// <param name="value">An object to compare with this object</param>
    public override bool Equals([NotNullWhen(true)] object? value) =>
        value is Range r &&
        r.Start.Equals(Start) &&
        r.End.Equals(End);

    /// <summary>
    /// Indicates whether the current Range object is equal to another Range object.
    /// </summary>
    /// <param name="other">An object to compare with this object</param>
    public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        return HashHelpers.Combine(Start.GetHashCode(), End.GetHashCode());
    }

    /// <summary>
    /// Converts the value of the current Range object to its equivalent string representation.
    /// </summary>
    public override string ToString()
    {
        return Start.ToString() + ".." + End.ToString();
    }

    /// <summary>
    /// Create a Range object starting from start index to the end of the collection.
    /// </summary>
    public static Range StartAt(Index start) => new(start, Index.End);

    /// <summary>
    /// Create a Range object starting from first element in the collection to the end Index.
    /// </summary>
    public static Range EndAt(Index end) => new(Index.Start, end);

    /// <summary>
    /// Create a Range object starting from first element to the end.
    /// </summary>
    public static Range All => new(Index.Start, Index.End);

    /// <summary>
    /// Calculate the start offset and length of range object using a collection length.
    /// </summary>
    /// <param name="length">The length of the collection that the range will be used with.
    /// length has to be a positive value.</param>
    /// <remarks>
    /// For performance reason, we don't validate the input length parameter against negative
    /// values. It is expected Range will be used with collections which always have non
    /// negative length/count. We validate the range is inside the length scope though.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Offset, int Length) GetOffsetAndLength(int length)
    {
        int start;
        Index startIndex = Start;
        if (startIndex.IsFromEnd)
            start = length - startIndex.Value;
        else
            start = startIndex.Value;

        int end;
        Index endIndex = End;
        if (endIndex.IsFromEnd)
            end = length - endIndex.Value;
        else
            end = endIndex.Value;

        if ((uint)end > (uint)length || (uint)start > (uint)end)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        return (start, end - start);
    }

    private static class HashHelpers
    {
        public static int Combine(int h1, int h2)
        {
            uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)rol5 + h1) ^ h2;
        }
    }

    private static class ThrowHelper
    {
        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException("length");
        }
    }
}