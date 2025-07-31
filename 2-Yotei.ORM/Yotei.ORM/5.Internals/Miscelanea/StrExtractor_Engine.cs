namespace Yotei.ORM.Internals;

// ========================================================
public static partial class StrExtractor
{
    /// <summary>
    /// Tries to extract at the head of the given source any of the given specifications, provided
    /// it is not protected by the engine terminators, if used. If so, returns true and sets the
    /// out main argument to the remaining source without the found specification, and the out
    /// found argument to that specification. Otherwise returns false and the out arguments are
    /// set to arbitrary values.
    /// </summary>
    public static bool ExtractHead(
        this string source,
        IEngine engine, bool isolated, out string main, out string found,
        params string[] specs)
    {
        specs.ThrowWhenNull();
        engine.ThrowWhenNull();

        var sensitive = engine.CaseSensitiveNames;
        found = string.Empty;
        main = source.ThrowWhenNull();
        
        if (!engine.UseTerminators || (
            !main.Contains(engine.LeftTerminator, sensitive) &&
            !main.Contains(engine.RightTerminator, sensitive)))
            return ExtractHead(main, sensitive, isolated, out main, out found, specs);

        var tokenizer = new StrWrapTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var token = tokenizer.Tokenize(main);

        if (token is StrTokenText text)
            return ExtractHead(text.Payload, sensitive, isolated, out main, out found, specs);

        if (token is StrTokenChain chain && chain.Count > 0 && chain[0] is StrTokenText item)
        {
            var done = ExtractHead(item.Payload, sensitive, isolated, out main, out found, specs);
            if (done)
            {
                var builder = new StrTokenChain.Builder();
                builder.Add(new StrTokenText(main));
                for (int i = 1; i < chain.Count; i++) builder.Add(chain[i]);

                main = builder.ToString();
                return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract at the tail of the given source any of the given specifications, provided
    /// it is not protected by the engine terminators, if used. If so, returns true and sets the
    /// out main argument to the remaining source without the found specification, and the out
    /// found argument to that specification. Otherwise returns false and the out arguments are
    /// set to arbitrary values.
    /// </summary>
    public static bool ExtractTail(
        this string source,
        IEngine engine, bool isolated, out string main, out string found,
        params string[] specs)
    {
        specs.ThrowWhenNull();
        engine.ThrowWhenNull();

        var sensitive = engine.CaseSensitiveNames;
        found = string.Empty;
        main = source.ThrowWhenNull();

        if (!engine.UseTerminators || (
            !main.Contains(engine.LeftTerminator, sensitive) &&
            !main.Contains(engine.RightTerminator, sensitive)))
            return ExtractTail(main, sensitive, isolated, out main, out found, specs);

        var tokenizer = new StrWrapTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var token = tokenizer.Tokenize(main);

        if (token is StrTokenText text)
            return ExtractTail(text.Payload, sensitive, isolated, out main, out found, specs);

        if (token is StrTokenChain chain && chain.Count > 0 && chain[^1] is StrTokenText item)
        {
            var done = ExtractTail(item.Payload, sensitive, isolated, out main, out found, specs);
            if (done)
            {
                var builder = new StrTokenChain.Builder();
                for (int i = 0; i < chain.Count - 1; i++) builder.Add(chain[i]);
                builder.Add(new StrTokenText(main));

                main = builder.ToString();
                return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the first ocurrence of any of the given specifications, provided it is
    /// not protected by the engine terminators, if used.. If so, returns true and sets the head
    /// and tail out arguments to the remainings without the found spec, and the found argument
    /// to that specification. Otherwise, returns false and the out arguments are set to
    /// arbitrary values.
    /// </summary>
    public static bool ExtractFirst(
        this string source,
        IEngine engine, bool isolated, out string head, out string found, out string tail,
        params string[] specs)
    {
        specs.ThrowWhenNull();
        engine.ThrowWhenNull();

        var sensitive = engine.CaseSensitiveNames;
        found = string.Empty;
        tail = string.Empty;
        head = source.ThrowWhenNull();

        if (!engine.UseTerminators || (
            !head.Contains(engine.LeftTerminator, sensitive) &&
            !head.Contains(engine.RightTerminator, sensitive)))
            return ExtractFirst(head, sensitive, isolated, out head, out found, out tail, specs);

        var tokenizer = new StrWrapTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var token = tokenizer.Tokenize(head);
        var tokens = token is StrTokenChain temp ? temp : [token];

        for (int i = 0; i < tokens.Count; i++)
        {
            token = tokens[i];
            if (token is not StrTokenText text) continue;

            var done = ExtractFirst(text.Payload, sensitive, isolated, out head, out found, out tail, specs);
            if (done)
            {
                var builder = new StrTokenChain.Builder();
                for (int k = 0; k < i; k++) builder.Add(tokens[k]);
                builder.Add(new StrTokenText(head));
                head = builder.ToString();

                builder.Clear();
                builder.Add(new StrTokenText(tail));
                for (int k = i + 1; k < tokens.Count; k++) builder.Add(tokens[k]);
                tail = builder.ToString();

                return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the last ocurrence of any of the given specifications, provided it is
    /// not protected by the engine terminators, if used.. If so, returns true and sets the head
    /// and tail out arguments to the remainings without the found spec, and the found argument
    /// to that specification. Otherwise, returns false and the out arguments are set to
    /// arbitrary values.
    /// </summary>
    public static bool ExtractLast(
        this string source,
        IEngine engine, bool isolated, out string head, out string found, out string tail,
        params string[] specs)
    {
        specs.ThrowWhenNull();
        engine.ThrowWhenNull();

        var sensitive = engine.CaseSensitiveNames;
        found = string.Empty;
        tail = string.Empty;
        head = source.ThrowWhenNull();

        if (!engine.UseTerminators || (
            !head.Contains(engine.LeftTerminator, sensitive) &&
            !head.Contains(engine.RightTerminator, sensitive)))
            return ExtractLast(head, sensitive, isolated, out head, out found, out tail, specs);

        var tokenizer = new StrWrapTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var token = tokenizer.Tokenize(head);
        var tokens = token is StrTokenChain temp ? temp : [token];

        for (int i = tokens.Count - 1; i >= 0; i--)
        {
            token = tokens[i];
            if (token is not StrTokenText text) continue;

            var done = ExtractLast(text.Payload, sensitive, isolated, out head, out found, out tail, specs);
            if (done)
            {
                var builder = new StrTokenChain.Builder();
                for (int k = 0; k < i; k++) builder.Add(tokens[k]);
                builder.Add(new StrTokenText(head));
                head = builder.ToString();

                builder.Clear();
                builder.Add(new StrTokenText(tail));
                for (int k = i + 1; k < tokens.Count; k++) builder.Add(tokens[k]);
                tail = builder.ToString();

                return true;
            }
        }

        return false;
    }
}