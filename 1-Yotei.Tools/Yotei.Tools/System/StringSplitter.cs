namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Splits source strings according to the rules in this instance.
/// </summary>
public sealed class StringSplitter : IEnumerable<ReadOnlyMemory<char>>, IEnumerator<ReadOnlyMemory<char>>
{
    readonly string? _Source;
    readonly IEnumerable<string> _Separators;

    string _Separator = default!;
    bool _SeparatorFound = false;
    int _Index = 0;
    int _Ini = 0;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    public StringSplitter(string? source, params IEnumerable<string> separators)
    {
        _Source = source;
        _Separators = Validate(separators);

        Reset();
    }

    static IEnumerable<string> Validate(IEnumerable<string> separators)
    {
        separators.ThrowWhenNull();

        foreach (var separator in separators) separator.NotNullNotEmpty(trim: false);
        return separators;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separators"></param>
    public StringSplitter(string? source, params char[] separators)
        : this(source, separators.ThrowWhenNull().Select(x => x.ToString())) { }

    /// <summary>
    /// Specifies the rules for finding the set of given separators.
    /// </summary>
    public StringComparison Comparison { get; set; }

    // ----------------------------------------------------

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    public StringSplitter GetEnumerator() => this;
    IEnumerator<ReadOnlyMemory<char>> IEnumerable<ReadOnlyMemory<char>>.GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;

    /// <inheritdoc cref="IEnumerator.Reset"/>
    public void Reset()
    {
        Current = default;
        CurrentIsSeparator = false;

        _SeparatorFound = false;
        _Separator = default!;
        _Index = 0;
        _Ini = 0;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => GC.SuppressFinalize(this);

    /// <inheritdoc cref="IEnumerator.Current"/>
    public ReadOnlyMemory<char> Current { get; private set; }
    object IEnumerator.Current => Current;

    /// <summary>
    /// Determines if the current value refers to one of the given separators, or not.
    /// </summary>
    public bool CurrentIsSeparator { get; private set; }

    /// <inheritdoc cref="IEnumerator.MoveNext"/>
    public bool MoveNext()
    {
        // Separator from previous iteration...
        if (_SeparatorFound)
        {
            Current = _Separator.AsMemory();
            CurrentIsSeparator = true;

            _SeparatorFound = false;
            _Separator = default!;
            return true;
        }

        // Looping...
        if (_Source is not null)
        {
            while (_Index < _Source.Length)
            {
                var span = _Source[_Index..];

                // First found separator wins...
                foreach (var separator in _Separators)
                {
                    if (span.StartsWith(separator, Comparison))
                    {
                        Current = _Source.AsMemory(_Ini, _Index - _Ini);
                        CurrentIsSeparator = false;

                        _SeparatorFound = true;
                        _Separator = separator;
                        _Index += separator.Length;
                        _Ini = _Index;
                        return true;
                    }
                }

                // Otherwise, try from next character...
                _Index++;
            }
        }

        // Remaining...
        if (_Source is not null && _Index >= _Ini)
        {
            Current = _Source.AsMemory(_Ini, _Index - _Ini);
            CurrentIsSeparator = false;

            _Ini = int.MaxValue;
            return true;
        }

        // Finishing...
        Reset();
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Filters the results of this splitter using the given predicate over the found element.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IEnumerable<ReadOnlyMemory<char>> Where(Predicate<ReadOnlyMemory<char>> predicate)
    {
        predicate.ThrowWhenNull();

        var iter = new StringSplitter(_Source, _Separators);
        while (iter.MoveNext())
            if (predicate(iter.Current)) yield return iter.Current;
    }

    /// <summary>
    /// Filters the results of this splitter using the given predicate over the found element,
    /// and whose second argument determines if that element is a separator or not.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IEnumerable<ReadOnlyMemory<char>> Where(Func<ReadOnlyMemory<char>, bool, bool> predicate)
    {
        predicate.ThrowWhenNull();

        var iter = new StringSplitter(_Source, _Separators);
        while (iter.MoveNext())
            if (predicate(iter.Current, iter.CurrentIsSeparator)) yield return iter.Current;
    }
}