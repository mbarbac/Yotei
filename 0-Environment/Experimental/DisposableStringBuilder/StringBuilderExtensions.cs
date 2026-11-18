using System.Collections.Concurrent;

namespace Experimental;

// ========================================================
public static class StringBuilderExtensions
{
    /// <summary>
    /// Extension block for static methods on <see cref="StringBuilder"/>
    /// </summary>
    extension(StringBuilder)
    {
        /// <summary>
        /// Obtains a new empty disposable string builder.
        /// </summary>
        /// <returns></returns>
        public static DisposableStringBuilder Rent() => new();

        /// <summary>
        /// Obtains a new disposable string builder with the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DisposableStringBuilder Rent(string? value) => new(value);

        /// <summary>
        /// Clears the string builder pool.
        /// </summary>
        public static void ClearPool() => Pool.Clear();
    }

    // ----------------------------------------------------

    static readonly ConcurrentBag<StringBuilder> Pool = [];
    private const int MAXPOOLSIZE = 64;
    private const int MAXCAPACITY = 8192;

    /// <summary>
    /// Represents a disposable string builder that, when disposed, returns its underlying builder
    /// to a shared pool.
    /// </summary>
    public class DisposableStringBuilder
        : IDisposable
        , IEquatable<ReadOnlySpan<char>>
        , IEquatable<StringBuilder>
        , IEquatable<DisposableStringBuilder>
    {
        /// <summary>
        /// Obtains an instance from the pool, if available, or returns a new one.
        /// </summary>
        /// <returns></returns>
        static StringBuilder Rent()
            => Pool.TryTake(out var sb) ? sb : new StringBuilder();

        /// <summary>
        /// Returns the given instance to the pool after clearing it, if the conditions are met.
        /// </summary>
        /// <param name="sb"></param>
        static void Return(StringBuilder sb)
        {
            sb.Clear();
            if (sb.Capacity < MAXCAPACITY && Pool.Count < MAXPOOLSIZE) Pool.Add(sb);
        }

        // ------------------------------------------------

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public DisposableStringBuilder() => _Builder = Rent();

        /// <summary>
        /// Initializes a new instance with the given value.
        /// </summary>
        /// <param name="value"></param>
        public DisposableStringBuilder(string? value) { _Builder = Rent(); _Builder.Append(value); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            Return(_Builder!); _Builder = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Determines if this instance has been disposed.
        /// </summary>
        public bool IsDisposed => _Builder == null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _Builder?.ToString() ?? string.Empty;

        /// <summary>
        /// <inheritdoc cref="StringBuilder.ToString(int, int)"/>
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string ToString(int startIndex, int length) => Builder.ToString(startIndex, length);

        /// <summary>
        /// The actual builder carried by this instance.
        /// </summary>
        /// Notes: not intercepting null values on purpose to fail after disposal.
        public StringBuilder Builder => _Builder!;
        StringBuilder? _Builder;

        /// <summary>
        /// Implicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator StringBuilder(
            DisposableStringBuilder value)
            => value.Builder;

        // ------------------------------------------------

        /// <summary>
        /// Gets or sets the character at the specified character position in this instance.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char this[int index] { get => Builder[index]; set => Builder[index] = value; }

        /// <summary>
        /// Gets or sets the length of the builder.
        /// </summary>
        public int Length { get => Builder.Length; set => Builder.Length = value; }

        /// <summary>
        /// Gets the maximum capacity of this instance.
        /// </summary>
        public int MaxCapacity => Builder.MaxCapacity;

        /// <summary>
        /// Gets or sets the maximum number of characters that can be contained in the builder.
        /// </summary>
        public int Capacity { get => Builder.Capacity; set => Builder.Capacity = value; }

        /// <summary>
        /// <inheritdoc cref="StringBuilder.CopyTo(int, Span{char}, int)"/>
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="destination"></param>
        /// <param name="count"></param>
        public void CopyTo(
            int sourceIndex, Span<char> destination, int count)
            => Builder.CopyTo(sourceIndex, destination, count);

        /// <summary>
        /// <inheritdoc cref="StringBuilder.CopyTo(int, char[], int, int)"/>
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="destination"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="count"></param>
        public void CopyTo(
            int sourceIndex, char[] destination, int destinationIndex, int count)
            => Builder.CopyTo(sourceIndex, destination, destinationIndex, count);

        /// <summary>
        /// <inheritdoc cref="StringBuilder.EnsureCapacity(int)"/>
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public int EnsureCapacity(int capacity) => Builder.EnsureCapacity(capacity);

        /// <summary>
        /// Determines if the characters in this instance are the same as the ones in the given
        /// span.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public bool Equals(ReadOnlySpan<char> span) => Builder.Equals(span);

        /// <summary>
        /// Determines if the characters in this instance are the same as the ones in the given
        /// string builder.
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        public bool Equals([NotNullWhen(true)] StringBuilder? sb) => Builder.Equals(sb);

        /// <summary>
        /// Determines if the characters in this instance are the same as the ones in the given
        /// instance.
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        public bool Equals([NotNullWhen(true)] DisposableStringBuilder? sb)
            => sb is not null
            && Builder.Equals(sb.Builder);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => Equals(obj as DisposableStringBuilder);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Builder?.GetHashCode() ?? 0;

        // ------------------------------------------------

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(sbyte value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(ulong value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(uint value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(ushort value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends a copy of a substring within a specified string builder to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(StringBuilder? value, int startIndex, int count)
        {
            Builder.Append(value, startIndex, count);
            return this;
        }

        /// <summary>
        /// Appends a copy of a specified substring to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(string? value, int startIndex, int count)
        {
            Builder.Append(value, startIndex, count);
            return this;
        }

        /// <summary>
        /// Appends a string value to the end of the builder.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(string? value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(float value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(ReadOnlySpan<char> value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(StringBuilder? value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(object? value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(ReadOnlyMemory<char> value)
        {
            Builder.Append(value);
            return this;
        }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(char value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(byte value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends a specified number of copies of the string representation of the given unicode
        /// character to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="repeatCount"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(char value, int repeatCount)
        {
            Builder.Append(value, repeatCount);
            return this;
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to
        /// this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="charCount"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(char[]? value, int startIndex, int charCount)
        {
            Builder.Append(value, startIndex, charCount);
            return this;
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified array to
        /// this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(char[]? value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(double value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(bool value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(short value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(int value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(long value) { Builder.Append(value); return this; }

        /// <summary>
        /// Appends the string representation of the specified value to this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Append(decimal value) { Builder.Append(value); return this; }

        // ------------------------------------------------

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of a corresponding argument in a parameter span.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            [StringSyntax("CompositeFormat")] string format,
            params scoped ReadOnlySpan<object?> args)
        {
            Builder.AppendFormat(format, args);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of any of the arguments using a specified format provider.
        /// </summary>
        /// <typeparam name="TArg0"></typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat<TArg0, TArg1, TArg2>(
            IFormatProvider? provider,
            CompositeFormat format, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            Builder.AppendFormat(provider, format, arg0, arg1, arg2);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of any of the arguments using a specified format provider.
        /// </summary>
        /// <typeparam name="TArg0"></typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat<TArg0, TArg1>(
            IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1)
        {
            Builder.AppendFormat(provider, format, arg0, arg1);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of any of the arguments using a specified format provider.
        /// </summary>
        /// <typeparam name="TArg0"></typeparam>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat<TArg0>(
            IFormatProvider? provider, CompositeFormat format, TArg0 arg0)
        {
            Builder.AppendFormat(provider, format, arg0);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of a corresponding argument in a parameter array.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            [StringSyntax("CompositeFormat")] string format, params object?[] args)
        {
            Builder.AppendFormat(format, args);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of any of the arguments using a specified format provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            IFormatProvider? provider, CompositeFormat format, params object?[] args)
        {
            Builder.AppendFormat(provider, format, args);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of either of two arguments.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            [StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1)
        {
            Builder.AppendFormat(format, arg0, arg1);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of a single argument.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            [StringSyntax("CompositeFormat")] string format, object? arg0)
        {
            Builder.AppendFormat(format, arg0);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of any of the arguments using a specified format provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            IFormatProvider? provider, CompositeFormat format, params scoped ReadOnlySpan<object?> args)
        {
            Builder.AppendFormat(provider, format, args);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of a corresponding argument in a parameter span using a specified format
        /// provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            IFormatProvider? provider,
            [StringSyntax("CompositeFormat")] string format, params scoped ReadOnlySpan<object?> args)
        {
            Builder.AppendFormat(provider, format, args);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of a corresponding argument in a parameter array using a specified format
        /// provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            IFormatProvider? provider,
            [StringSyntax("CompositeFormat")] string format, params object?[] args)
        {
            Builder.AppendFormat(provider, format, args);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of either of three arguments using a specified format provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            IFormatProvider? provider,
            [StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2)
        {
            Builder.AppendFormat(provider, format, arg0, arg1, arg2);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of either of two arguments using a specified format provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            IFormatProvider? provider,
            [StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1)
        {
            Builder.AppendFormat(provider, format, arg0, arg1);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of a single argument using a specified format provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            IFormatProvider? provider,
            [StringSyntax("CompositeFormat")] string format, object? arg0)
        {
            Builder.AppendFormat(provider, format, arg0);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by the string
        /// representation of either of three arguments.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendFormat(
            [StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2)
        {
            Builder.AppendFormat(format, arg0, arg1, arg2);
            return this;
        }

        // ------------------------------------------------

        /// <summary>
        /// Concatenates and appends the members of a collection, using the specified char separator
        /// between each member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendJoin<T>(char separator, IEnumerable<T> values)
        {
            Builder.AppendJoin(separator, values);
            return this;
        }

        /// <summary>
        /// Concatenates and appends the members of a collection, using the specified separator
        /// between each member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendJoin<T>(string? separator, IEnumerable<T> values)
        {
            Builder.AppendJoin(separator, values);
            return this;
        }

        /// <summary>
        /// Concatenates the strings of the provided span, using the specified separator between
        /// each string, then appends the result to the current instance of the string builder.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendJoin(
            string? separator, params scoped ReadOnlySpan<string?> values)
        {
            Builder.AppendJoin(separator, values);
            return this;
        }

        /// <summary>
        /// Concatenates the strings of the provided span, using the specified char separator
        /// between each string, then appends the result to the current instance of the string
        /// builder.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendJoin(
            char separator, params scoped ReadOnlySpan<string?> values)
        {
            Builder.AppendJoin(separator, values);
            return this;
        }

        /// <summary>
        /// Concatenates the strings of the provided array, using the specified separator between
        /// each string, then appends the result to the current instance of the string builder.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendJoin(string? separator, params string?[] values)
        {
            Builder.AppendJoin(separator, values);
            return this;
        }

        /// <summary>
        /// Concatenates the string representations of the elements in the provided span of objects,
        /// using the specified char separator between each member, then appends the result to the
        /// current instance of the string builder.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendJoin(
            char separator, params scoped ReadOnlySpan<object?> values)
        {
            Builder.AppendJoin(separator, values);
            return this;
        }

        /// <summary>
        /// Concatenates the strings of the provided array, using the specified char separator
        /// between each string, then appends the result to the current instance of the string
        /// builder.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendJoin(char separator, params string?[] values)
        {
            Builder.AppendJoin(separator, values);
            return this;
        }

        /// <summary>
        /// Concatenates the string representations of the elements in the provided array of objects,
        /// using the specified char separator between each member, then appends the result to the
        /// current instance of the string builder.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendJoin(char separator, params object?[] values)
        {
            Builder.AppendJoin(separator, values);
            return this;
        }

        /// <summary>
        /// Concatenates the string representations of the elements in the provided span of objects,
        /// using the specified separator between each member, then appends the result to the current
        /// instance of the string builder.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendJoin(
            string? separator, params scoped ReadOnlySpan<object?> values)
        {
            Builder.AppendJoin(separator, values);
            return this;
        }

        /// <summary>
        /// Concatenates the string representations of the elements in the provided array of objects,
        /// using the specified separator between each member, then appends the result to the current
        /// instance of the string builder.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendJoin(string? separator, params object?[] values)
        {
            Builder.AppendJoin(separator, values);
            return this;
        }

        // ------------------------------------------------

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to the
        /// end of the current builder.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder AppendLine(string? value) { Builder.AppendLine(value); return this; }

        /// <summary>
        /// Appends the default line terminator to the end of the current builder.
        /// </summary>
        /// <returns></returns>
        public DisposableStringBuilder AppendLine() { Builder.AppendLine(); return this; }

        // ------------------------------------------------

        /// <summary>
        /// Removes all characters from the current instance.
        /// </summary>
        /// <returns></returns>
        public DisposableStringBuilder Clear() { Builder.Clear(); return this; }

        /// <summary>
        /// Removes the specified range of characters from this instance.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public DisposableStringBuilder Remove(int startIndex, int length)
        {
            Builder.Remove(startIndex, length);
            return this;
        }

        // ------------------------------------------------

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, sbyte value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, ulong value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, uint value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, ushort value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts one or more copies of a specified string into this instance at the specified
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, string? value, int count)
        {
            Builder.Insert(index, value, count);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, string? value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, float value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, object? value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, long value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, int value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, short value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, double value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, decimal value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into
        /// this instance at the specified character position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="charCount"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, char[]? value, int startIndex, int charCount)
        {
            Builder.Insert(index, value, startIndex, charCount);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, char[]? value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, bool value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, byte value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, ReadOnlySpan<char> value)
        {
            Builder.Insert(index, value);
            return this;
        }

        /// <summary>
        /// Inserts the string representation of the given value into this instance at the given
        /// position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DisposableStringBuilder Insert(int index, char value)
        {
            Builder.Insert(index, value);
            return this;
        }

        // ------------------------------------------------

        /// <summary>
        /// Replaces, within a substring of this instance, all occurrences of a specified string
        /// with another specified string.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DisposableStringBuilder Replace(
            string oldValue, string? newValue, int startIndex, int count)
        {
            Builder.Replace(oldValue, newValue, startIndex, count);
            return this;
        }

        /// <summary>
        /// Replaces all instances of one read-only character span with another in this builder.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public DisposableStringBuilder Replace(
            ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
        {
            Builder.Replace(oldValue, newValue);
            return this;
        }

        /// <summary>
        /// Replaces all instances of one read-only character span with another in part of this
        /// builder.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DisposableStringBuilder Replace(
            ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
        {
            Builder.Replace(oldValue, newValue, startIndex, count);
            return this;
        }

        /// <summary>
        /// Replaces, within a substring of this instance, all occurrences of a specified character
        /// with another specified character.
        /// </summary>
        /// <param name="oldChar"></param>
        /// <param name="newChar"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DisposableStringBuilder Replace(char oldChar, char newChar, int startIndex, int count)
        {
            Builder.Replace(oldChar, newChar, startIndex, count);
            return this;
        }

        /// <summary>
        /// Replaces all occurrences of a specified character in this instance with another
        /// specified character.
        /// </summary>
        /// <param name="oldChar"></param>
        /// <param name="newChar"></param>
        /// <returns></returns>
        public DisposableStringBuilder Replace(char oldChar, char newChar)
        {
            Builder.Replace(oldChar, newChar);
            return this;
        }

        /// <summary>
        /// Replaces all occurrences of a specified string in this instance with another
        /// specified string.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public StringBuilder Replace(string oldValue, string? newValue)
        {
            Builder.Replace(oldValue, newValue);
            return this;
        }
    }
}