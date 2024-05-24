using System.Collections.Immutable;
using Enuii.Reports;
using Enuii.Symbols.Typing;
using Enuii.Syntax.AST;

namespace Enuii.Semantics;

public class Analyzer
{
    public SyntaxTree SimpleTree { get; }
    public Reporter   Reporter   { get; }

    public Analyzer(SyntaxTree tree, Reporter? reporter = null)
    {
        SimpleTree = tree;
        Reporter   = reporter ?? new();
    }

    public SemanticTree Start()
    {
        var statements = ImmutableArray.CreateBuilder<SemanticStatement>();

        foreach (var stmt in SimpleTree.Body)
            statements.Add(BindStatement(stmt));

        return new([..statements]);
    }


    /* ====================================================================== */
    /*                               Statements                               */
    /* ====================================================================== */

    private SemanticStatement BindStatement(Statement stmt)
    {
        switch (stmt.Kind)
        {
            case NodeKind.ExpressionStatement:
                return BindExpressionStatement((ExpressionStatement) stmt);

            default:
                throw new Exception($"Unrecognized statement kind: {stmt.Kind}");
        }
    }

    private SemanticExpressionStatement BindExpressionStatement(ExpressionStatement stmt)
    {
        var expr = BindExpression(stmt.Expression);
        return new(expr);
    }

    /* ====================================================================== */
    /*                               Expressions                              */
    /* ====================================================================== */

    private SemanticExpression BindExpression(Expression expr)
    {
        switch (expr.Kind)
        {
            case NodeKind.Unknown:
            case NodeKind.Null:
            case NodeKind.Boolean:
            case NodeKind.Integer:
            case NodeKind.Float:
            case NodeKind.Char:
            case NodeKind.String:
                return BindLiteral((Literal) expr);

            default:
                throw new Exception($"Unrecognized expression kind: {expr.Kind}");
        }
    }

    private SemanticLiteral BindLiteral(Literal l)
    {
        var type = TypeSymbol.GetNodeType(l.Kind);
        return new(l.Value, type, l.Span);
    }
}
