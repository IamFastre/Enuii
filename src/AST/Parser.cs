using System.Collections.Immutable;
using Enuii.General.Positioning;
using Enuii.Reports;
using Enuii.Syntax.Lexing;

namespace Enuii.Syntax.AST;

public class Parser
{
    public List<Token> Tokens   { get; }
    public Reporter    Reporter { get; }

    private int   Index   { get; set; } = 0;
    public  Token Current => Index < Tokens.Count ? Tokens[Index] : Tokens[^1];
    public  bool  EOF     => Current.Kind == TokenKind.EOF;

    public Parser(IEnumerable<Token> tokens, Reporter? reporter = null)
    {
        Tokens   = tokens.Where(e => !e.Kind.IsParserIgnorable()).ToList();
        Reporter = reporter ?? new();
    }

    /* =========================== Helper Methods =========================== */

    private Token Eat()
    {
        var current = Current;
        Index++;
        return current;
    }

    private Token Expect(TokenKind kind, Span? span = null)
    {
        if (Current.Kind == kind)
            return Eat();

        if (EOF)
            Reporter.ReportEndOfFile(Current.Span);
        else
            Reporter.ReportExpectedToken(kind.ToString(), Current.Value, span ?? Current.Span);

        return Token.Fabricate(Current.Span);
    }

    /* ====================================================================== */

    public SyntaxTree Start()
    {
        var statements = ImmutableArray.CreateBuilder<Statement>();

        while (!EOF)
            statements.Add(GetStatement());

        return new(statements);
    }


    /* ====================================================================== */
    /*                               Statements                               */
    /* ====================================================================== */

    private Statement GetStatement()
    {
        switch (Current.Kind)
        {
            default:
                return GetExpressionStatement();

            case TokenKind.OpenCurlyBracket:
                return GetBlockStatement();

            case TokenKind.If:
                return GetIfStatement();
        }
    }

    private ExpressionStatement GetExpressionStatement()
    {
        var expr = GetExpression();
        return new(expr);
    }

    private BlockStatement GetBlockStatement()
    {
        var body = ImmutableArray.CreateBuilder<Statement>();
        var open  = Eat();

        // Keep on looking for statements unless it's a close bracket
        // or it's end of file
        while (Current.Kind != TokenKind.CloseCurlyBracket && !EOF)
            body.Add(GetStatement());

        var cls = Expect(TokenKind.CloseCurlyBracket);

        return new(open, body, cls);
    }

    private IfStatement GetIfStatement()
    {
        ElseClause? elseClause = null;

        var ifKeyword = Eat();
        var condition = GetExpression();
        
        if (condition.Kind is NodeKind.Unknown)
        {
            Reporter.Pop();
            Reporter.ReportExpressionExpectedAfter(ifKeyword.Value, ifKeyword.Span);
        }
        Expect(TokenKind.Colon);

        var thenStmt = GetStatement();

        if (Current.Kind == TokenKind.Else)
            elseClause = GetElseClause();

        return new(ifKeyword, condition, thenStmt, elseClause);
    }

    private ElseClause GetElseClause()
    {
        var elseKeyword = Eat();

        if (Current.Kind is not TokenKind.If)
            Expect(TokenKind.Colon);

        var statement   = GetStatement();

        return new(elseKeyword, statement);
    }

    /* ====================================================================== */
    /*                               Expressions                              */
    /* ====================================================================== */

    private Expression GetExpression()
        => GetSecondary();

    private Expression GetSecondary(int parentPrecedence = 0)
    {
        Expression? left;
        var unaryPrecedence = Current.Kind.UnaryPrecedence();

        /* ============================== Unary ============================= */
        if (unaryPrecedence == 0 || unaryPrecedence < parentPrecedence)
            left = GetIntermediate();
        else
        {
            var uOp = Eat();
            left    = GetSecondary(unaryPrecedence);

            if (left.Kind == NodeKind.Unknown)
            {
                Reporter.Pop();
                Reporter.ReportExpressionExpectedAfter(uOp.Value, uOp.Span);
                return ConstantLiteral.Fabricate(uOp.Span);
            }

            left = new UnaryExpression(uOp, left);
        }

        /* ============================= Binary ============================= */
        while (true)
        {
            var binaryPrecedence = Current.Kind.BinaryPrecedence();
            if (binaryPrecedence == 0 || binaryPrecedence <= parentPrecedence)
                break;

            var biOp  = Eat();
            var right = GetSecondary(binaryPrecedence);

            if (right.Kind == NodeKind.Unknown)
            {
                Reporter.Pop();
                Reporter.ReportExpressionExpectedAfter(biOp.Value, biOp.Span);
                return ConstantLiteral.Fabricate(left.Span.To(biOp.Span));
            }

            left = new BinaryExpression(left, biOp, right);
        }

        /* ============================= Ternary ============================ */
        if (Current.Kind == TokenKind.QuestionMark && parentPrecedence == 0)
        {
            var qMark     = Eat();
            var trueExpr  = GetSecondary();

            if (trueExpr.Kind == NodeKind.Unknown)
            {
                Reporter.Pop();
                Reporter.ReportExpressionExpectedAfter(qMark.Value, qMark.Span);
                return ConstantLiteral.Fabricate(left.Span.To(qMark.Span));
            }

            var colon     = Expect(TokenKind.Colon);

            var falseExpr = GetSecondary();
            if (falseExpr.Kind == NodeKind.Unknown)
            {
                Reporter.Pop();
                Reporter.ReportExpressionExpectedAfter(colon.Value, colon.Span);
                return trueExpr;
            }

            return new TernaryExpression(left, trueExpr, falseExpr);
        }

        return left;
    }

    private Expression GetIntermediate()
    {
        switch (Current.Kind)
        {
            case TokenKind.Pipe:
                return GetRange();
            default:
                return GetPrimary();
        }
    }

    private RangeLiteral GetRange()
    {
        Expression? start = null, end = null, step = null;
        var open = Eat();

        RangeLiteral newRange()
            => new(open, start, end, step, Eat());

        if (Current.Kind != TokenKind.Colon)
            start = GetPrimary();

        Expect(TokenKind.Colon);

        if (Current.Kind == TokenKind.Pipe)
            return newRange();

        end = GetPrimary();

        if (Current.Kind == TokenKind.Pipe)
            return newRange();

        Expect(TokenKind.Colon);
        step = GetPrimary();

        return newRange();
    }

    private Expression GetPrimary()
    {
        switch (Current.Kind)
        {
            case TokenKind.Null:
                return new ConstantLiteral(Eat(), NodeKind.Null);

            case TokenKind.Boolean:
                return new ConstantLiteral(Eat(), NodeKind.Boolean);

            case TokenKind.Integer:
                return new ConstantLiteral(Eat(), NodeKind.Integer);

            case TokenKind.Float:
                return new ConstantLiteral(Eat(), NodeKind.Float);

            case TokenKind.Char:
                return new ConstantLiteral(Eat(), NodeKind.Char);

            case TokenKind.String:
                return new ConstantLiteral(Eat(), NodeKind.String);

            case TokenKind.Identifier:
                return new NameLiteral(Eat());

            case TokenKind.OpenParenthesis:
                return GetParenthesized();

            default:
                if (EOF)
                    Reporter.ReportEndOfFile(Current.Span);
                else
                    Reporter.ReportInvalidSyntax(Current.Value, Current.Span);

                return ConstantLiteral.Fabricate(Eat().Span);
        }
    }

    private Expression GetParenthesized()
    {
        var open = Eat();
        var expr = GetExpression();
        var cls  = expr.Kind != NodeKind.Unknown
                 ? Expect(TokenKind.CloseParenthesis)
                 : null;

        if (cls is null || cls.IsFabricated)
            return ConstantLiteral.Fabricate(open.Span.To(cls?.Span ?? expr.Span));

        return new ParenthesizedExpression(open, expr, cls);
    }
}
