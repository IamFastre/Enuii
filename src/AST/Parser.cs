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

    private Token Eat()
    {
        var current = Current;
        Index++;
        return current;
    }

    private Expression FabricateExpression(Span? span = null)
        => Literal.Unknown(span ?? Current.Span);

    public SyntaxTree Start()
    {
        var statements = ImmutableArray.CreateBuilder<Statement>();

        while (!EOF)
            statements.Add(GetStatement());

        return new(statements);
    }

    /* ====================================================================== */
    /*                                Statement                               */
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
        => GetLiteral();

    private Expression GetLiteral()
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

            default:
                Reporter.ReportInvalidToken(Current.Value, Current.Span);
                return FabricateExpression(Eat().Span);
        }
    }
}
