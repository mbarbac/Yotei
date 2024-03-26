namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents a flatten and ordered collection of unique tokens.
/// </summary>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public sealed partial class TokenChain : Token, IEnumerable<Token>
{
    readonly List<Token> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public TokenChain() { }

    /// <summary>
    /// Initializes a new instance with the given token.
    /// </summary>
    /// <param name="token"></param>
    public TokenChain(Token token) => AddInternal(token);

    /// <summary>
    /// Initializes a new instance with the tokens from the given range.
    /// </summary>
    /// <param name="range"></param>
    public TokenChain(IEnumerable<Token> range) => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    TokenChain(TokenChain source) => AddRangeInternal(source);

    /// <inheritdoc/>
    public IEnumerator<Token> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    string ToDebugString(int count) => Count <= count
        ? $"({Count})[{string.Join(", ", Items)}]"
        : $"({Count})[{string.Join(", ", Items.Take(count))}]...";

    /// <summary>
    /// Returns a string representation of this instance using the given head, tail and separator
    /// characters.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="separator"></param>
    /// <param name="tail"></param>
    /// <returns></returns>
    public string ToString(char head, string separator, char tail)
    {
        head = head >= ' ' ? head : throw new ArgumentException("Invalid head.").WithData(head);
        tail = tail >= ' ' ? tail : throw new ArgumentException("Invalid tail.").WithData(tail);
        separator = separator.ThrowWhenNull();

        return Count == 0
            ? $"{head}{tail}"
            : $"{head}{string.Join(separator, Items)}{tail}";
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override TokenArgument? GetArgument()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            var token = Items[i].GetArgument();
            if (token != null) return token;
        }
        return null;
    }

    /// <summary>
    /// Reduces this instance to a simpler form, if possible, or returns the original instance
    /// instead.
    /// </summary>
    /// <returns></returns>
    public Token Reduce() => Count switch
    {
        0 => TokenLiteral.Empty,
        1 => this[0],
        _ => this
    };

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of tokens in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets the token stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Token this[int index] => Items[index];

    /// <summary>
    /// Determines if this collection contains the given token.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public bool Contains(Token token) => IndexOf(token) >= 0;

    /// <summary>
    /// Returns the index of the the given token, or -1 if not found.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public int IndexOf(Token token)
    {
        token.ThrowWhenNull();
        return IndexOf(x => ReferenceEquals(x, token));
    }

    /// <summary>
    /// Determines if this collection contains an token that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<Token> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence of an token that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<Token> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of an token that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<Token> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the indexes of the tokens in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<Token> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var nums = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// Returns an array with the tokens in this collection.
    /// </summary>
    /// <returns></returns>
    public Token[] ToArray() => Items.ToArray();

    /// <summary>
    /// Returns a list with the tokens in this collection.
    /// </summary>
    /// <returns></returns>
    public List<Token> ToList() => new(Items);

    /// <summary>
    /// Minimizes the memory consumption of this collection.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of tokens starting from the given index.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public TokenChain GetRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.GetRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int GetRangeInternal(int index, int count)
    {
        if (count == 0 && index >= 0) return ClearInternal();
        if (index == 0 && count == Count) return 0;

        var range = Items.GetRange(index, count);
        Items.Clear();
        Items.AddRange(range);
        return count;
    }

    /// <summary>
    /// Returns a new instance where the token at the given index has been replaced by the new
    /// given one, if not equal to the existing one. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public TokenChain Replace(int index, Token token)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(index, token);
        return num > 0 ? clone : this;
    }
    int ReplaceInternal(int index, Token token)
    {
        var temp = IndexOf(token);
        if (temp == index) return 0;
        if (temp >= 0) throw new DuplicateException("Duplicated element.").WithData(token);

        RemoveAtInternal(index);
        return InsertInternal(index, token);
    }

    /// <summary>
    /// Returns a new instance where the given token has been added to the collection. If the
    /// token is an enumeration of tokens, then these elements are added instead. If no changes
    /// are detected, returns the original instance.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public TokenChain Add(Token token)
    {
        var clone = Clone();
        var num = clone.AddInternal(token);
        return num > 0 ? clone : this;
    }
    int AddInternal(Token token)
    {
        if (token is IEnumerable<Token> range) return AddRangeInternal(range);
        else
        {
            var temp = IndexOf(token);
            if (temp >= 0) throw new DuplicateException("Duplicated element.").WithData(token);

            Items.Add(token);
            return 1;
        }
    }

    /// <summary>
    /// Returns a new instance where the tokens from the given range have been added to the
    /// collection. If any  token is an enumeration of tokens, then these elements are added
    /// instead. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public TokenChain AddRange(IEnumerable<Token> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<Token> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = AddInternal(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Returns a new instance where the given token has been inserted into the collection at
    /// the given index. If the token is an enumeration of tokens, then these elements are the
    /// ones inserted instead.If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public TokenChain Insert(int index, Token token)
    {
        var clone = Clone();
        var num = clone.InsertInternal(index, token);
        return num > 0 ? clone : this;
    }
    int InsertInternal(int index, Token token)
    {
        if (token is IEnumerable<Token> range) return InsertRangeInternal(index, range);
        else
        {
            var temp = IndexOf(token);
            if (temp >= 0) throw new DuplicateException("Duplicated element.").WithData(token);

            Items.Insert(index, token);
            return 1;
        }
    }

    /// <summary>
    /// Returns a new instance the tokens from the given range have been inserted into the
    /// collection, starting at the given index. If any  token is an enumeration of tokens, then
    /// these elements are inserted instead. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public TokenChain InsertRange(int index, IEnumerable<Token> range)
    {
        var clone = Clone();
        var num = clone.InsertRangeInternal(index, range);
        return num > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<Token> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = InsertInternal(index, item);
            index += r;
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Returns a new instance where the token at the given index has been removed from the
    /// original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TokenChain RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.RemoveAtInternal(index);
        return num > 0 ? clone : this;
    }
    int RemoveAtInternal(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Returns a new instance where the given number of tokens, starting from the given index,
    /// have been removed from the collection. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public TokenChain RemoveRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.RemoveRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// Returns a new instance where the given token has been removed. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public TokenChain Remove(Token token)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(token);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(Token token)
    {
        var index = IndexOf(token);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where the first token that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public TokenChain Remove(Predicate<Token> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<Token> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where the last token that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public TokenChain RemoveLast(Predicate<Token> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<Token> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where all tokens that match the given predicate has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public TokenChain RemoveAll(Predicate<Token> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<Token> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// Returns a new instance where all the tokens have been removed. If no changes are
    /// detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    public TokenChain Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    int ClearInternal()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}