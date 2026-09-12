using Microsoft.VisualBasic;

namespace Yotei.ORM.Internals;

// ========================================================
public partial record DbTokenVisitor
{
    /// <summary>
    /// Translates the given dynamic lambda expression into a chain of tokens, and returns its
    /// appropriate database representation.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder Visit(Func<dynamic, object> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var token = new DbLambdaParser(Engine).Parse(expression);
        var info = Visit(token);
        return info;
    }

    /// <summary>
    /// Visits the chains of tokens represented by the given last-most one, and returns the
    /// appropriate database representation.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder Visit(IDbToken token)
    {
        ArgumentNullException.ThrowIfNull(token);

        return token switch
        {
            DbTokenArgument item => VisitToken(item),
            DbTokenBinary item => VisitToken(item),
            DbTokenChain item => VisitToken(item),
            DbTokenCoalesce item => VisitToken(item),
            DbTokenCommandInfo item => VisitToken(item),
            DbTokenConvert item => VisitToken(item),
            DbTokenIdentifier item => VisitToken(item),
            DbTokenIndexed item => VisitToken(item),
            DbTokenInvoke item => VisitToken(item),
            DbTokenLiteral item => VisitToken(item),
            DbTokenMethod item => VisitToken(item),
            DbTokenSetter item => VisitToken(item),
            DbTokenTernary item => VisitToken(item),
            DbTokenUnary item => VisitToken(item),
            DbTokenValue item => VisitToken(item),

            IEnumerable<IDbToken> item => VisitRange(item),

            _ => throw new ArgumentException("Unknown token.").WithData(token)
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Visits the given collection of tokens and returns the appropriate database representation
    /// of them all joined with the current range separator. No brackets are used to wrapped that
    /// representation.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ICommandInfo.IBuilder VisitRange(IEnumerable<IDbToken> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var builder = new CommandInfo.Builder(Engine);
        var needed = false;

        foreach (var token in range)
        {
            if (token is null) throw new ArgumentException(
                "Range of tokens contains null elements.")
                .WithData(range);

            if (needed && RangeSeparator is not null) builder.AddText(RangeSeparator);
            needed = true;

            if (token is IEnumerable<IDbToken> chain)
            {
                var temp = VisitRange(chain);
                builder.Add(temp);
            }
            else
            {
                var temp = Visit(token);
                builder.Add(temp);
            }
        }

        return builder;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> Argument tokens are translation artifacts with no database representation.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenArgument token)
    {
        return new CommandInfo.Builder(Engine);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method intercepts comparisons against null right values, provided that
    /// <see cref="UseNullString"/> is enabled, returning in these cases either "IS NULL" or 
    /// a "IS NOT NULL" construction.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenBinary token)
    {
        // Always needed...
        var left = Visit(token.Left);

        // Intercepting...
        if (token.Right is DbTokenValue value && value.Value is null && UseNullString)
        {
            var temp = ToValueString(null);

            switch (token.Operation)
            {
                case ExpressionType.Equal:
                    left.ReplaceText($"({left.Text} IS {temp})");
                    return left;

                case ExpressionType.NotEqual:
                    left.ReplaceText($"({left.Text} IS NOT {temp})");
                    return left;
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

        // Finishing...
        var right = Visit(token.Right);

        left.ReplaceText($"({left.Text} {op} ");
        left.Add(right);
        left.AddText(")");
        return left;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method visits the chain elements of the chain and then joins them using no
    /// separators, to be consistent with the [...] translation syntax.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenChain token)
    {
        var visitor = ToNullSeparatorVisitor();
        return visitor.VisitRange(token);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method produces a 'COALESCE(left, right)' construction.
    /// Override as needed to adapt to the specific database engine.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenCoalesce token)
    {
        var left = Visit(token.Left);
        var right = Visit(token.Right);

        left.ReplaceText($"COALESCE({left.Text}, ");
        left.Add(right);
        left.AddText(")");
        return left;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method replaces the NULL valued parameters with the engine's NULL literal,
    /// and removes the associated parameters, provided that <see cref="UseNullString"/> is
    /// enabled. Then, wraps the result between rounded brackets.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenCommandInfo token)
    {
        var builder = token.CommandInfo.ToBuilder();
        var text = builder.Text;
        var pars = builder.Parameters.ToList();
        var parschanged = false;

        ReplaceNulls();
        RemoveBrackets();

        builder.ReplaceText($"({text})");
        if (parschanged) builder.ReplaceValues(pars);
        return builder;

        // Invoked to replace the null-valued arguments with the engine's null value literal...
        void ReplaceNulls()
        {
            var finder = new IsolatedFinder();
            var nstr = Engine.NullValueLiteral;

            for (int i = 0; i < pars.Count; i++)
            {
                var par = pars[i];
                if (par.Value is not null) continue;

                var found = false;
                var index = 0;
                while ((index = finder.Find(text, index, par.Name, Engine.IgnoreCase)) >= 0)
                {
                    text = text.Remove(index, par.Name.Length);
                    text = text.Insert(index, nstr);
                    index += nstr.Length;
                    found = true;
                }
                if (found)
                {
                    parschanged = true;
                    pars.RemoveAt(i);
                    i--;
                }
            }
        }

        // Invoked to remove any previous brackets...
        void RemoveBrackets()
        {
            if (text.Length == 0) return;
            var ini = FindHead();
            var end = FindTail();

            if (ini >= 0 && end >= 0 && end > ini)
                text = text.Unwrap('(', ')', trim: true, recursive: true)!;
        }
        int FindHead()
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ') continue;
                if (text[i] == '(') return i;
            }
            return -1;
        }
        int FindTail()
        {
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if (text[i] == ' ') continue;
                if (text[i] == ')') return i;
            }
            return -1;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method produces a 'CAST(target AS type)' construction.
    /// Override as needed to adapt to the specific database engine.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenConvert token)
    {
        var target = Visit(token.Target);

        // Actual type given...
        if (token is DbTokenConvert.ToType totype)
        {
            var name = totype.Type.EasyName();
            var builder = new CommandInfo.Builder(Engine);

            builder.Add($"CAST({target.Text} AS {name})", target.Parameters);
            return builder;
        }

        // Type specification given...
        else if (token is DbTokenConvert.ToSpec tospec)
        {
            var builder = new CommandInfo.Builder(Engine);

            builder.Add($"CAST({target.Text} AS {tospec.Type})", target.Parameters);
            return builder;
        }

        // Unknown..
        else throw new ArgumentException("Unknown token.").WithData(token);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method intercepts identifiers that are strictly the same as dynamic argument
    /// names, and substitutes them by an empty string. It also removes redundant dots from its
    /// head.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenIdentifier token)
    {
        var host = Visit(token.Host);

        var darg = token.GetArgument();
        var names = token.Identifier.Enumerate(useTerminators: false).ToList();
        for (int i = 0; i < names.Count; i++)
        {
            var temp = names[i].NullWhenDynamicName(darg, Engine.IgnoreCase);
            if (Engine.UseTerminators)
                temp = temp.Wrap(Engine.LeftTerminator, Engine.RightTerminator, trim: true);

            names[i] = temp.NullWhenEmpty(trim: true);
        }
        var name = string.Join('.', names);

        name = host.Text.Length == 0 || token.Host is DbTokenInvoke ? name : $".{name}";
        name = host.Text + name;
        while (name.StartsWith('.') && name.Length > 1) name = name[1..];

        host.ReplaceText(name);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method joins its arguments with the current range separator, and wraps the
    /// result with square brackets.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenIndexed token)
    {
        var host = Visit(token.Host);
        var args = VisitRange(token.Indexes);

        args.ReplaceText($"[{args.Text}]");
        host.Add(args);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method joins its arguments without any separator, and then adds the result to
    /// any previous content. This provides a way to inject arbitrary contents into the returned
    /// result.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenInvoke token)
    {
        var host = Visit(token.Host);
        var temp = ToNullSeparatorVisitor();
        var args = temp.VisitRange(token.Arguments);

        host.Add(args);
        return host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> By convention, the values of literal tokens are not captured as arguments, and
    /// just injected as text elements.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenLiteral token)
    {
        return new CommandInfo.Builder(Engine, token.Value);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/>- If the method name is the same as the name of the dynamic argument, then the method
    /// is treated as an invoke operation: 'x => x.Any.x(...)'.
    /// <br/>- This method intercepts 'virtual' method invocations, and translaties them into the
    /// appropriate database constructions:
    /// <br/>··· Argument level: NOT, COUNT, CAST, CONVERT.
    /// <br/>··· Member level: AS, IN, NOTIN, BETWEEN, LIKE, NOTLIKE.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenMethod token)
    {
        // Intercepting invoke-alike tokens...
        var name = token.Name;
        var darg = token.GetArgument();

        if (darg != null && darg.Name == name)
        {
            if (token.TypeArguments.Length != 0) throw new ArgumentException(
                "Invoke-alike methods do not support generic type arguments.")
                .WithData(token);

            return Visit(new DbTokenInvoke(token.Host, token.Arguments));
        }

        // Others...
        var upper = name.ToUpper();
        ICommandInfo.IBuilder temp;
        ICommandInfo.IBuilder other;
        DbTokenChain chain;

        // Argument-level methods...
        if (token.Host is DbTokenArgument)
        {
            switch (upper)
            {
                case "NOT":
                    if (token.Arguments.IsEmpty) Throw(token, $"NOT(expr) requieres 1 argument.");
                    if (token.Arguments.Length > 1) Throw(token, $"Too many NOT(expr) arguments.");
                    temp = Visit(token.Arguments[0]);
                    temp.ReplaceText($"(NOT {temp.Text})");
                    return temp;

                case "COUNT":
                    if (token.Arguments.IsEmpty || IsSoleAsterisk(token.Arguments))
                        return new CommandInfo.Builder(Engine, "COUNT(*)");

                    temp = VisitRange(token.Arguments);
                    temp.ReplaceText($"COUNT({temp.Text})");
                    return temp;

                case "CONVERT":
                case "CAST":
                    if (token.TypeArguments.Length == 1) // Cast<type>(expre) ...
                    {
                        if (token.Arguments.Length != 1) Throw(token, $"CAST<type>(expr) requieres 1 argument.");
                        temp = Visit(token.Arguments[0]);
                        name = token.TypeArguments[0].EasyName();
                        temp.ReplaceText($"CAST({name} AS {temp.Text})");
                        return temp;
                    }
                    if (token.TypeArguments.Length == 0) // Cast(expr, type) ...
                    {
                        if (token.Arguments.Length != 2) Throw(token, $"CAST(expr, type) requieres 2 arguments.");
                        temp = Visit(token.Arguments[0]);
                        other = ToRawVisitor().Visit(token.Arguments[1]);
                        temp.ReplaceText($"CAST({temp.Text} AS ");
                        temp.Add(other);
                        temp.AddText(")");
                        return temp;
                    }
                    Throw(token, "Too many CAST<...>(...) arguments.");
                    break;
            }
        }

        // Member-level methods...
        var host = Visit(token.Host);
        if (token.Host is not DbTokenArgument)
        {
            switch (upper)
            {
                case "AS":
                    if (token.Arguments.IsEmpty) Throw(token, $"AS(...) needs at least 1 argument.");
                    name = ChainToAlias(token.Arguments);
                    host.AddText($" AS {name}");
                    return host;

                case "IN":
                    if (token.Arguments.IsEmpty) Throw(token, $"IN(...) needs at least 1 argument.");
                    chain = TryExpandFirst(token.Arguments);
                    temp = VisitRange(chain);
                    host.AddText(" IN (");
                    host.Add(temp);
                    host.AddText(")");
                    return host;

                case "NOTIN":
                    if (token.Arguments.IsEmpty) Throw(token, $"NOTIN(...) needs at least 1 argument.");
                    chain = TryExpandFirst(token.Arguments);
                    temp = VisitRange(chain);
                    host.AddText(" NOT IN (");
                    host.Add(temp);
                    host.AddText(")");
                    return host;

                case "BETWEEN":
                    if (token.Arguments.Length != 2) Throw(token, $"BETWEEN(expr, expr) needs 2 arguments.");
                    temp = Visit(token.Arguments[0]);
                    other = Visit(token.Arguments[1]);
                    host.Add($" BETWEEN ({temp.Text} AND ", temp.Parameters);
                    host.Add(other);
                    host.AddText(")");
                    return host;

                case "LIKE":
                    if (token.Arguments.Length != 1) Throw(token, $"LIKE(expr) needs 1 argument.");
                    temp = Visit(token.Arguments[0]);
                    host.Add($" LIKE {temp.Text}", temp.Parameters);
                    return host;

                case "NOTLIKE":
                    if (token.Arguments.Length != 1) Throw(token, $"NOTLIKE(expr) needs 1 argument.");
                    temp = Visit(token.Arguments[0]);
                    host.Add($" NOT LIKE {temp.Text}", temp.Parameters);
                    return host;
            }
        }

        // Default method invocations...
        if (token.Host is not DbTokenArgument and not DbTokenInvoke) host.AddText(".");
        host.Add(name);

        if (token.TypeArguments.Length > 0)
        {
            host.AddText("<");
            host.AddText(string.Join(", ", token.TypeArguments.Select(x => x.EasyName())));
            host.AddText(">");
        }

        temp = VisitRange(token.Arguments);
        host.AddText("(");
        host.Add(temp);
        host.AddText(")");
        return host;

        // Exception helper...
        [DoesNotReturn]
        static void Throw(
            IDbToken token, string str) => throw new ArgumentException(str).WithData(token);
    }

    /// <summary>
    /// Determines if the given collection of tokens consist in just one element with an
    /// asterisk-alike value.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static bool IsSoleAsterisk(ImmutableArray<IDbToken> chain)
    {
        return
            chain.Length == 1 &&
            chain[0] is DbTokenValue value && (
            (value.Value is char c && c == '*') ||
            (value.Value is string s && s == "*"));
    }

    /// <summary>
    /// If the chain's first and unique element is an enumeration, returns its expansion.
    /// Otherwise, returns the original chain.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public DbTokenChain TryExpandFirst(ImmutableArray<IDbToken> chain)
    {
        if (chain.Length == 1 &&
            chain[0] is DbTokenValue value &&
            value.Value is not string &&
            value.Value is IEnumerable iter)
        {
            var builder = new DbTokenChain.Builder();
            foreach (var item in iter)
            {
                switch (item)
                {
                    case IDbToken token:
                        builder.Add(token);
                        break;

                    case LambdaNode node:
                        var other = new DbLambdaParser(Engine).Parse(node);
                        builder.Add(other);
                        break;

                    default:
                        builder.Add(new DbTokenValue(item));
                        break;
                }
            }
            return builder.ToInstance();
        }

        return [.. chain];
    }

    /// <summary>
    /// Invoked to build an alias from the contents of the given chain, by joining them without
    /// any separators. Throws an exception if the alias resolves into a null or empty literal.
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    public string ChainToAlias(ImmutableArray<IDbToken> chain)
    {
        var visitor = ToRawVisitor();
        var builder = visitor.VisitRange(chain);

        var id = new Identifier(Engine, builder.Text);
        if (id.Count == 1)
        {
            var name = id.Value;
            if (name is not null) return name;
        }
        throw new ArgumentException("Invalid alias.").WithData(chain);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> ... 
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenSetter token)
    {
        var target = Visit(token.Target);
        var value = Visit(token.Value);

        target.ReplaceText($"({target.Text} = ");
        target.Add($"{value.Text})", value.Parameters);
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method produces a 'IF (left) THEN (middle) ELSE (right)' construction.
    /// Override as needed to adapt to the specific database engine.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenTernary token)
    {
        var left = Visit(token.Left);
        var middle = Visit(token.Middle);
        var right = Visit(token.Right);

        left.ReplaceText($"IF (({left.Text}) ");
        left.Add($"THEN ({middle.Text}) ", middle.Parameters);
        left.Add($"ELSE ({right.Text}))", right.Parameters);
        return left;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method wraps the unary operation between rounded brackets.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenUnary token)
    {
        var target = Visit(token.Target);

        switch (token.Operation)
        {
            case ExpressionType.Not: target.ReplaceText($"(NOT {target.Text})"); break;
            case ExpressionType.Negate: target.ReplaceText($"-({target.Text})"); break;
            default:
                throw new ArgumentException("Unsupported unary operation.").WithData(token);
        }

        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to visit the given token.
    /// <br/> This method captures the token value as an argument, if <see cref="CaptureValues"/>
    /// is enabled. Otherwise, they text representation are just injected. Null values are treated
    /// according to the <see cref="UseNullString"/> setting.
    /// </summary>
    /// <param name="token"></param>
    protected virtual ICommandInfo.IBuilder VisitToken(DbTokenValue token)
    {
        // Command-alike...
        if (token.Value is ICommand command)
        {
            var temp = command.GetCommandInfo(iterable: false);
            var item = new DbTokenCommandInfo(temp);
            return Visit(item);
        }
        if (token.Value is ICommandInfo info)
        {
            var item = new DbTokenCommandInfo(info);
            return Visit(item);
        }

        // Null-alike...
        if (token.Value is null && UseNullString)
        {
            var nstr = ToValueString(null);
            var builder = new CommandInfo.Builder(Engine, nstr);
            return builder;
        }

        // Capturing values...
        if (CaptureValues)
        {
            var name = $"{Engine.ParameterPrefix}0";
            var par = new Parameter(name, token.Value);
            var buider = new CommandInfo.Builder(Engine, "{0}", [par]);
            return buider;
        }

        // Injecting value representation...
        else
        {
            var temp = ToValueString(token.Value);
            var builder = new CommandInfo.Builder(Engine, temp);
            return builder;
        }
    }
}