using System.Collections.Immutable;
using Enuii.General.Constants;
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
        }
    }

    private ExpressionStatement GetExpressionStatement()
    {
        var expr = GetExpression();
        return new(expr);
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
            left = GetPrimary();
        else
        {
            var uOp = Eat();
            left = GetSecondary(unaryPrecedence);

            if (left is null)
            {
                Reporter.ReportExpressionExpectedAfter(uOp.Value, uOp.Span);
                return Literal.Fabricate(uOp.Span);
            }

            left = new UnaryExpression(uOp, left);
        }

        /* ============================= Binary ============================= */
        while (true)
        {
            var binaryPrecedence = Current.Kind.BinaryPrecedence();
            if (binaryPrecedence == 0 || binaryPrecedence <= parentPrecedence)
                break;

            var binOp = Eat();
            var right = GetSecondary(binaryPrecedence);

            if (right is null)
            {
                Reporter.ReportExpressionExpectedAfter(binOp.Value, binOp.Span);
                return Literal.Fabricate(left.Span.To(binOp.Span));
            }

            left = new BinaryExpression(left, binOp, right);
        }

        return left;
    }

    private Expression GetPrimary()
    {
        switch (Current.Kind)
        {
            case TokenKind.Null:
                return new Literal(Eat(), NodeKind.Null);

            case TokenKind.Boolean:
                return new Literal(Eat(), NodeKind.Boolean);

            case TokenKind.Integer:
                return new Literal(Eat(), NodeKind.Integer);

            case TokenKind.Float:
                return new Literal(Eat(), NodeKind.Float);

            case TokenKind.Char:
                return new Literal(Eat(), NodeKind.Char);

            case TokenKind.String:
                return new Literal(Eat(), NodeKind.String);

            case TokenKind.Identifier:
                return new NameLiteral(Eat());

            case TokenKind.OpenParenthesis:
                return GetParenthesized();

            default:
                if (EOF)
                    Reporter.ReportEndOfFile(Current.Span);
                else
                    Reporter.ReportInvalidSyntax(Current.Value, Current.Span);

                return Literal.Fabricate(Eat().Span);
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
            return Literal.Fabricate(open.Span.To(cls?.Span ?? expr.Span));

        return new ParenthesizedExpression(open, expr, cls);
    }
}
