#pragma warning disable IDE0042

namespace Yotei.ORM.Internals;

// ========================================================
public static partial class Extractor
{
    /// <summary>
    /// Extracts from the head of the given source the first matching specification, which must
    /// be isolated or not as requested, provided it is not protected by the engine terminators,
    /// if they are used. If found, the returned spec is trimmed, and all spaces are kept in main.
    /// <br/> If '<paramref name="isolated"/>' is <c>true</c>, then only isolated specifications
    /// are candidates for matching.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="engine"></param>
    /// <param name="engine"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static (string Main, string Spec) ExtractHead(
        string source, IEngine engine, bool isolated, out bool found, params string[] specs)
    {
        source.ThrowWhenNull();
        engine.ThrowWhenNull();

        var sensitive = engine.CaseSensitiveNames;
        if (!engine.UseTerminators ||
            (!source.Contains(engine.LeftTerminator) && !source.Contains(engine.RightTerminator)))
            return ExtractHead(source, sensitive, isolated, out found, specs);

        // We can assume tokenizer only renders chains and text tokens...
        var tokenizer = new StrWrappedTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var item = tokenizer.Tokenize(source);
        if (item is StrTokenText text)
            return ExtractHead(text.Payload, sensitive, isolated, out found, specs);

        if (item is StrTokenChain chain && chain.Count > 0 && chain[0] is StrTokenText token)
        {
            var parts = ExtractHead(token.Payload, sensitive, isolated, out found, specs);
            if (found)
            {
                var builder = new StrTokenChain.Builder();
                builder.Add(new StrTokenText(parts.Main));
                for (int i = 1; i < chain.Count; i++) builder.Add(chain[i]);

                return (builder.ToString(), parts.Spec);
            }
        }

        found = false;
        return (string.Empty, string.Empty);
    }

    /// <summary>
    /// Extracts from the tail of the given source the first matching specification, which must
    /// be isolated or not as requested, provided it is not protected by the engine terminators,
    /// if they are used. If found, the returned spec is trimmed, and all spaces are kept in main.
    /// <br/> If '<paramref name="isolated"/>' is <c>true</c>, then only isolated specifications
    /// are candidates for matching.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="engine"></param>
    /// <param name="engine"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static (string Main, string Spec) ExtractTail(
        string source, IEngine engine, bool isolated, out bool found, params string[] specs)
    {
        source.ThrowWhenNull();
        engine.ThrowWhenNull();

        var sensitive = engine.CaseSensitiveNames;
        if (!engine.UseTerminators ||
            (!source.Contains(engine.LeftTerminator) && !source.Contains(engine.RightTerminator)))
            return ExtractTail(source, sensitive, isolated, out found, specs);

        // We can assume tokenizer only renders chains and text tokens...
        var tokenizer = new StrWrappedTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var item = tokenizer.Tokenize(source);
        if (item is StrTokenText text)
            return ExtractTail(text.Payload, sensitive, isolated, out found, specs);

        if (item is StrTokenChain chain && chain.Count > 0 && chain[^1] is StrTokenText token)
        {
            var parts = ExtractTail(token.Payload, sensitive, isolated, out found, specs);
            if (found)
            {
                var builder = new StrTokenChain.Builder();
                for (int i = 0; i < chain.Count - 1; i++) builder.Add(chain[i]);
                builder.Add(new StrTokenText(parts.Main));

                return (builder.ToString(), parts.Spec);
            }
        }

        found = false;
        return (string.Empty, string.Empty);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given source the left and right parts that are separated by the first
    /// separator that matches any of the given specifications, in order, which must be isolated
    /// or not as requested, and provided they are not protected by the engine's terminators, if
    /// they are used. If a separator is found, it is trimmed before returning, and all spaces
    /// are kept in both the left and right parts.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sensitive"></param>
    /// <param name="engine"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static (string Left, string Spec, string Right) ExtractFirstSeparator(
        string source, IEngine engine, bool isolated, out bool found, params string[] specs)
    {
        source.ThrowWhenNull();
        engine.ThrowWhenNull();

        var sensitive = engine.CaseSensitiveNames;
        if (!engine.UseTerminators ||
            (!source.Contains(engine.LeftTerminator) && !source.Contains(engine.RightTerminator)))
            return ExtractFirstSeparator(source, sensitive, isolated, out found, specs);

        var tokenizer = new StrWrappedTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var item = tokenizer.Tokenize(source);
        var items = item is StrTokenChain temp ? temp : [item];

        for (int i = 0; i < items.Count; i++)
        {
            item = items[i];
            if (item is not StrTokenText text) continue;

            var parts = ExtractFirstSeparator(text.Payload, sensitive, isolated, out found, specs);
            if (found)
            {
                var left = new StrTokenChain.Builder();
                for (int k = 0; k < i; k++) left.Add(items[k]);
                left.Add(new StrTokenText(parts.Left));

                var right = new StrTokenChain.Builder();
                right.Add(new StrTokenText(parts.Right));
                for (int k = i + 1; k < items.Count; k++) right.Add(items[k]);

                return (left.ToString(), parts.Spec, right.ToString());
            }
        }

        found = false;
        return (string.Empty, string.Empty, string.Empty);
    }

    /// <summary>
    /// Extracts from the given source the left and right parts that are separated by the last
    /// separator that matches any of the given specifications, in order, which must be isolated
    /// or not as requested, and provided they are not protected by the engine's terminators, if
    /// they are used. If a separator is found, it is trimmed before returning, and all spaces
    /// are kept in both the left and right parts.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sensitive"></param>
    /// <param name="engine"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static (string Left, string Spec, string Right) ExtractLastSeparator(
        string source, IEngine engine, bool isolated, out bool found, params string[] specs)
    {
        source.ThrowWhenNull();
        engine.ThrowWhenNull();

        var sensitive = engine.CaseSensitiveNames;
        if (!engine.UseTerminators ||
            (!source.Contains(engine.LeftTerminator) && !source.Contains(engine.RightTerminator)))
            return ExtractLastSeparator(source, sensitive, isolated, out found, specs);

        var tokenizer = new StrWrappedTokenizer(engine.LeftTerminator, engine.RightTerminator);
        var item = tokenizer.Tokenize(source);
        var items = item is StrTokenChain temp ? temp : [item];

        for (int i = items.Count - 1; i >= 0; i--)
        {
            item = items[i];
            if (item is not StrTokenText text) continue;

            var parts = ExtractLastSeparator(text.Payload, sensitive, isolated, out found, specs);
            if (found)
            {
                var left = new StrTokenChain.Builder();
                for (int k = 0; k < i; k++) left.Add(items[k]);
                left.Add(new StrTokenText(parts.Left));

                var right = new StrTokenChain.Builder();
                right.Add(new StrTokenText(parts.Right));
                for (int k = i + 1; k < items.Count; k++) right.Add(items[k]);

                return (left.ToString(), parts.Spec, right.ToString());
            }
        }

        found = false;
        return (string.Empty, string.Empty, string.Empty);
    }
}