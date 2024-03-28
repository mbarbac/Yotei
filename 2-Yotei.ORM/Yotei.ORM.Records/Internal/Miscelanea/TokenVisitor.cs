#pragma warning disable IDE0060

namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents the ability of parsing a token chain and return the command info extracted from it.
/// </summary>
public class TokenVisitor
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public TokenVisitor(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <summary>
    /// The connection this instance is associated with.
    /// </summary>
    public IConnection Connection { get; }
    IEngine Engine => Connection.Engine;

    // ----------------------------------------------------

    /// <summary>
    /// Parses the given token and returns the command info extracted from it.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public ICommandInfo Visit(Token token, TokenOptions options)
    {
        token.ThrowWhenNull();
        options.ThrowWhenNull();

        return token switch
        {
            TokenArgument item => Visit(item, options),
            TokenBinary item => Visit(item, options),
            TokenChain item => Visit(item, options),
            TokenCoalesce item => Visit(item, options),
            TokenCommand item => Visit(item, options),
            TokenConvert item => Visit(item, options),
            TokenIdentifier item => Visit(item, options),
            TokenIndexed item => Visit(item, options),
            TokenInvoke item => Visit(item, options),
            TokenLiteral item => Visit(item, options),
            TokenMethod item => Visit(item, options),
            TokenSetter item => Visit(item, options),
            TokenTernary item => Visit(item, options),
            TokenUnary item => Visit(item, options),
            TokenValue item => Visit(item, options),

            _ => throw new UnExpectedException("Unknown token.").WithData(token)
        };
    }

    /// <summary>
    /// Parses the given range of tokens and returns the command info extracted from it.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public ICommandInfo Visit(IEnumerable<Token> range, TokenOptions options)
    {
        range.ThrowWhenNull();
        options.ThrowWhenNull();

        var chain = new TokenChain(range);
        var info = Visit(chain, options with { Separator = ", " });

        info = new Code.CommandInfo($"({info.Text})", info.Parameters);
        return info;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> By definition, argument tokens are considered translation artifacts so this method
    /// must return an empty instance.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected ICommandInfo Visit(TokenArgument token, TokenOptions options)
    {
        return new Code.CommandInfo(Engine);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method returns the unary operation between rounded brackets.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenBinary token, TokenOptions options)
    {
        var left = Visit(token.Left, options);

        // Comparisons with NULL values...
        if (token.Right is TokenValue value && value.Value == null && options.UseNullString)
        {
            var temp = Engine.NullValueLiteral;

            switch (token.Operation)
            {
                case ExpressionType.Equal:
                    return new Code.CommandInfo($"({left.Text} IS {temp})", left.Parameters);

                case ExpressionType.NotEqual:
                    return new Code.CommandInfo($"({left.Text} IS NOT {temp})", left.Parameters);
            }
        }

        // Supported operations...
        var op = token.Operation switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "<>",

            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            ExpressionType.Modulo => "%",
            ExpressionType.Power => "^",

            ExpressionType.And => "AND",
            ExpressionType.Or => "OR",

            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",

            _ => throw new UnreachableException("Unsupported binary operation.").WithData(token)
        };

        // Returning...
        var right = Visit(token.Right, options);

        left = left.AddText($" {op} ");
        left = left.Add(right);
        left = new Code.CommandInfo($"({left.Text})", left.Parameters);
        return left;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenChain token, TokenOptions options)
    {
        ICommandInfo info = new Code.CommandInfo(Engine);
        var num = 0;

        foreach (var item in token)
        {
            if (num > 0) info = info.AddText(options.Separator);
            num++;

            var temp = Visit(token, options);
            info = info.Add(info);
        }
        return info;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenCoalesce token, TokenOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> By default, this method just wraps the command text between rounded brackets.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenCommand token, TokenOptions options)
    {
        var text = $"({token.Command.Text})";
        return new Code.CommandInfo(text, token.Command.Parameters);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method returns the convert or cast operation between brackets.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenConvert token, TokenOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method shall consider that if the host token is an invoke one, then not dot
    /// shall be added. Otherwise, adds a dot separator between the host and the identifier.
    /// <br/> In addition, it shall remove any redundant heading dots, except the first one.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenIdentifier token, TokenOptions options)
    {
        var host = Visit(token.Host, options);
        var name = options.UseTerminators ? token.Identifier.Value : token.Identifier.UnwrappedValue;
        name ??= string.Empty;

        if (token.Host is not TokenArgument)
        {
            name =
                host.Text.Length == 0 ? name :
                token.Host is TokenInvoke ? $"{host.Text}{name}" : $"{host.Text}.{name}";
        }

        while (name.StartsWith('.') && name.Length > 1) name = name[1..];

        host = new Code.CommandInfo(name, host.Parameters);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method adds to the host the indexes between squared brackets, and separated
    /// by commas.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenIndexed token, TokenOptions options)
    {
        var host = Visit(token.Host, options);
        var args = Visit(token.Indexes, options with { Separator = ", " });

        host = host.Add($"[{args.Text}]", args.Parameters);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by definition, appends to the host the parsed arguments with no
    /// separators in between.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenInvoke token, TokenOptions options)
    {
        var host = Visit(token.Host, options);
        var args = Visit(token.Arguments, options with { Separator = null });

        host = host.Add(args.Text, args.Parameters);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method, by definition, just returns the literal text, with no attemp of
    /// capturing it as a parameter.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenLiteral token, TokenOptions options)
    {
        return new Code.CommandInfo(token.Value, new ParameterList(Engine));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenMethod token, TokenOptions options)
    {
        throw null;
    }

    /// <summary>
    /// Deterines if the chain is empty or contains just a sole asterisk, or not.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    bool IsEmptyOrSoleAsterisk(TokenChain chain)
    {
        if (chain.Count == 0) return true;

        if (chain.Count == 1 &&
            chain[0] is TokenValue value && (
            (value.Value is char chr && chr == '*') ||
            (value.Value is string str && str == "*")))
            return true;

        return false;
    }

    /// <summary>
    /// Returns an alias built from the given collection of arguments.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    string VisitAlias(TokenChain chain)
    {
        var temp = Visit(chain, TokenOptions.False());
        var id = new IdentifierPart(Engine, temp.Text);

        if (id.Value is null) throw new ArgumentException(
            "Generated alias is null or empty.").WithData(chain);

        return Engine.UseTerminators
            ? id.UnwrappedValue.Wrap(Engine.LeftTerminator, Engine.RightTerminator)!
            : id.UnwrappedValue!;
    }

    /// <summary>
    /// Expands the first unique element of the given chain, provided it is an enumerable one.
    /// Otherwise, returns the chain itself.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    TokenChain TryExpand(TokenChain chain)
    {
        if (chain.Count == 1)
        {
            var token = chain[0];
            if (token is TokenValue value &&
                value.Value is not string &&
                value.Value is IEnumerable iter)
            {
                chain = chain.Clear();
                foreach (var item in iter)
                {
                    switch (item)
                    {
                        case Token temp:
                            chain = chain.Add(temp);
                            break;

                        case LambdaNode temp:
                            var darg = temp.GetArgument() ?? new LambdaNodeArgument("FAKE");
                            var other = new ExpressionParser(Engine).Parse(temp);
                            chain = chain.Add(other);
                            break;

                        default:
                            chain = chain.Add(new TokenValue(item));
                            break;
                    }
                }
            }
        }

        return chain;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method returns the setter operation between rounded brackets.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenSetter token, TokenOptions options)
    {
        var target = Visit(token.Target, options);
        var value = Visit(token.Value, options);

        target = target.AddText($" = ");
        target = target.Add(value.Text, value.Parameters);
        target = new Code.CommandInfo($"({target.Text})", target.Parameters);
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenTernary token, TokenOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method returns the unary operation between rounded brackets.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenUnary token, TokenOptions options)
    {
        var target = Visit(token.Target, options);

        return token.Operation switch
        {
            ExpressionType.Not => new Code.CommandInfo($"(NOT {target.Text})", target.Parameters),
            ExpressionType.Negate => new Code.CommandInfo($"-({target.Text})", target.Parameters),

            _ => throw new UnExpectedException("Unsupported unary operation.").WithData(token)
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ICommandInfo Visit(TokenValue token, TokenOptions options)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the appropriate string representation of the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public virtual string ToValueString(object? value, TokenOptions options)
    {
        throw null;
    }
}