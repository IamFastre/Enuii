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
    public  Token Current => Index     < Tokens.Count ? Tokens[Index]     : Tokens[^1];
    public  Token Next    => Index + 1 < Tokens.Count ? Tokens[Index + 1] : Tokens[^1];
    public  bool  EOF     => Current.Kind == TokenKind.EOF;

    public Parser(IEnumerable<Token> tokens, Reporter? reporter = null)
    {
        Tokens   = tokens.Where(e => !e.Kind.IsParserIgnorable()).ToList();
        Reporter = reporter ?? new();

        Reporter.InRuntime = false;
    }

    /* =========================== Helper Methods =========================== */

    private Token Eat(int amount = 1)
    {
        Token current = Current;

        for (int i = 0; i < amount; i++)
        {
            current = Current;
            Index++;
        }

        return current;
    }

    private bool IsNextKind(TokenKind kind)
    {
        if (Current.Kind == kind)
        {
            Eat();
            return true;
        }
        return false;
    }

    private Token? Optional(TokenKind kind)
    {
        if (Current.Kind == kind)
            return Eat();
        return null;
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

            case TokenKind.Hash:
                return GetDeclarationStatement();

            case TokenKind.OpenCurlyBracket:
                return GetBlockStatement();

            case TokenKind.If:
                return GetIfStatement();

            case TokenKind.While:
                return GetWhileStatement();

            case TokenKind.For:
                return GetForStatement();

            case TokenKind.GreaterGreater:
                return GetFunctionStatement();
        }
    }

    private ExpressionStatement GetExpressionStatement()
    {
        var expr = GetExpression();
        return new(expr);
    }

    private Statement GetDeclarationStatement()
    {
        // TODO: allow valueless declarations
        TypeClause? type = null;
        var hash    = Eat();
        var isConst = IsNextKind(TokenKind.Asterisk);
        var name    = Expect(TokenKind.Identifier);

        if (IsNextKind(TokenKind.Colon))
            type = GetTypeClause();

        Expect(TokenKind.Equal);
        var expr = GetExpression();

        return new DeclarationStatement(hash, isConst, name, type, expr);
    }

    private BlockStatement GetBlockStatement()
    {
        var body = ImmutableArray.CreateBuilder<Statement>();
        var open = Eat();

        // Keep on looking for statements unless it's a close bracket
        // or it's end of file
        while (Current.Kind is not TokenKind.CloseCurlyBracket && !EOF)
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
            Reporter.PopError();
            Reporter.ReportExpressionExpectedAfter(ifKeyword.Value, ifKeyword.Span);
        }

        Expect(TokenKind.Colon);
        var thenStmt = GetStatement();

        if (Current.Kind == TokenKind.Else)
            elseClause = GetElseClause();

        return new(ifKeyword, condition, thenStmt, elseClause);
    }

    private WhileStatement GetWhileStatement()
    {
        ElseClause? elseClause = null;

        var whileKeyword = Eat();
        var condition    = GetExpression();
        
        if (condition.Kind is NodeKind.Unknown)
        {
            Reporter.PopError();
            Reporter.ReportExpressionExpectedAfter(whileKeyword.Value, whileKeyword.Span);
        }

        Expect(TokenKind.Colon);
        var thenStmt = GetStatement();

        if (Current.Kind == TokenKind.Else)
            elseClause = GetElseClause();

        return new(whileKeyword, condition, thenStmt, elseClause);
    }

    private ForStatement GetForStatement()
    {
        ElseClause? elseClause = null;

        var forKeyword = Eat();

        var variable   = Expect(TokenKind.Identifier);
        Expect(TokenKind.In);
        var iterable   = GetExpression();

        Expect(TokenKind.Colon, forKeyword.Span);
        var statement  = GetStatement();

        if (Current.Kind == TokenKind.Else)
            elseClause = GetElseClause();

        return new(forKeyword, variable, iterable, statement, elseClause);
    }

    private FunctionStatement GetFunctionStatement()
    {
        TypeClause? returnType = null;

        var funcSym = Eat();
        var isConst = IsNextKind(TokenKind.Asterisk);
        var name    = Expect(TokenKind.Identifier);

        Expect(TokenKind.OpenParenthesis);
        var parameters = GetSeparated(GetParameterClause, TokenKind.CloseParenthesis);
        Expect(TokenKind.CloseParenthesis);

        if (IsNextKind(TokenKind.DashArrow))
            returnType = GetTypeClause();

        Expect(TokenKind.Colon);
        var statement = GetFunctionBodyStatement();


        return new(funcSym, isConst, name, parameters, returnType, statement);
    }

    private Statement GetFunctionBodyStatement()
    {
        if (Current.Kind == TokenKind.OpenCurlyBracket)
            return GetBlockStatement();

        return GetExpressionStatement();
    }


    /* ====================================================================== */
    /*                                 Clauses                                */
    /* ====================================================================== */

    private TypeClause GetTypeClause()
    {
        ImmutableArray<TypeClause>.Builder? parameters = null;
        var listDim = 0u;

        var nullable = false;
        var type     = Expect(TokenKind.Type);
        var span     = type.Span.Copy();

        // if `<` is met start to think generic until you meet `>`
        if (Current.Kind == TokenKind.Less)
        {
            do
            {
                Eat();
                parameters ??= ImmutableArray.CreateBuilder<TypeClause>();
                parameters.Add(GetTypeClause());
            }
            while (Current.Kind == TokenKind.Comma);

            span.SetEnd(Expect(TokenKind.Greater).Span);
        }

        if (IsNextKind(TokenKind.QuestionMark))
            nullable = true;

        // if the type is followed by `[]` then eat both tokens and add to the array dimension
        while (Current.Kind == TokenKind.OpenSquareBracket && Next.Kind == TokenKind.CloseSquareBracket)
        {
            listDim++;
            span.SetEnd(Eat(2).Span);
        }

        if (IsNextKind(TokenKind.QuestionMark))
            nullable = true;

        return new(type, parameters, listDim, nullable, span);
    }

    private ElseClause GetElseClause()
    {
        var elseKeyword = Eat();

        if (Current.Kind is not (TokenKind.If or TokenKind.While))
            Expect(TokenKind.Colon);

        var statement   = GetStatement();

        return new(elseKeyword, statement);
    }

    private SeparatedClause<E> GetSeparated<E>(Func<E> getElement, TokenKind endToken) where E : Node
    {
        if (Current.Kind == endToken)
            return SeparatedClause<E>.Empty;

        var elements   = ImmutableArray.CreateBuilder<E>();
        var separators = ImmutableArray.CreateBuilder<Token>();

        do
        {
            var elem = getElement();

            if (elem.Kind is NodeKind.Unknown)
                break;

            elements.Add(elem);

            if (Current.Kind == TokenKind.Comma)
                separators.Add(Eat());
            else
                break;
        }
        while (Current.Kind != endToken);

        return new(elements, separators);
    }

    private ParameterClause GetParameterClause()
    {
        Expression? value = null;
        var name = Expect(TokenKind.Identifier);
        Expect(TokenKind.Colon);
        var type = GetTypeClause();

        if (IsNextKind(TokenKind.Equal))
            value = GetExpression();

        return new(name, type, value);
    }


    /* ====================================================================== */
    /*                               Expressions                              */
    /* ====================================================================== */

    private Expression GetExpression()
        => GetAssignment();

    private Expression GetAssignment()
    {
        var assignee = GetSecondary();

        /* =========================== Assignments ========================== */
        if (IsNextKind(TokenKind.Equal))
        {
            var expr = GetExpression();

            if (assignee.Kind.IsAssignableTo())
                return new AssignmentExpression((NameLiteral) assignee, expr);

            Reporter.ReportInvalidAssignee(assignee.Span);
            return assignee;
        }
        /* ====================== Compound Assignments ====================== */
        else if (Current.Kind.IsAssignment())
        {
            var op = Eat();
            var expr = GetExpression();

            if (assignee.Kind.IsAssignableTo())
                return new CompoundAssignmentExpression((NameLiteral) assignee, op, expr);

            Reporter.ReportInvalidAssignee(assignee.Span);
            return assignee;
        }

        return assignee;
    }

    private Expression GetSecondary(int parentPrecedence = 0)
    {
        Expression? left;
        var unaryPrecedence = Current.Kind.UnaryPrecedence();

        /* ============================== Unary ============================= */
        if (unaryPrecedence == 0 || unaryPrecedence < parentPrecedence)
            left = GetConversion();
        else
        {
            var uOp = Eat();
            left    = GetSecondary(unaryPrecedence);

            if (left.Kind is NodeKind.Unknown)
            {
                Reporter.PopError();
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

            if (right.Kind is NodeKind.Unknown)
            {
                Reporter.PopError();
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

            if (trueExpr.Kind is NodeKind.Unknown)
            {
                Reporter.PopError();
                Reporter.ReportExpressionExpectedAfter(qMark.Value, qMark.Span);
                return ConstantLiteral.Fabricate(left.Span.To(qMark.Span));
            }

            var colon     = Expect(TokenKind.Colon);

            var falseExpr = GetSecondary();
            if (falseExpr.Kind is NodeKind.Unknown)
            {
                Reporter.PopError();
                Reporter.ReportExpressionExpectedAfter(colon.Value, colon.Span);
                return trueExpr;
            }

            return new TernaryExpression(left, trueExpr, falseExpr);
        }

        return left;
    }

    private Expression GetConversion()
    {
        var expr = GetCounting();

        while (Current.Kind is TokenKind.DashArrow)
        {
            Eat();
            expr = new ConversionExpression(expr, GetTypeClause());
        }

        return expr;
    }

    private Expression GetCounting()
    {
        Token? op;
        void Check()
            => op = Current.Kind is TokenKind.PlusPlus or TokenKind.MinusMinus ? Eat() : null;

        Check();
        var operand = GetCall();

        if (op is not null)
        {
            if (operand.Kind.IsAssignableTo())
                return new CountingExpression(op, (NameLiteral) operand, true);

            Reporter.ReportInvalidCount(op.Kind, operand.Span);
        }

        Check();
        if (op is not null)
        {
            if (operand.Kind.IsAssignableTo())
                return new CountingExpression(op, (NameLiteral) operand, false);

            Reporter.ReportInvalidCount(op.Kind, operand.Span);
        }

        return operand;
    }

    private Expression GetCall()
    {
        var callee = GetIntermediate();

        if (IsNextKind(TokenKind.OpenParenthesis))
        {
            var args = GetSeparated(GetExpression, TokenKind.CloseParenthesis);
            var end  = Expect(TokenKind.CloseParenthesis).Span;
            return new CallExpression(callee, args, end);
        }

        return callee;
    }

    private Expression GetIntermediate()
    {
        switch (Current.Kind)
        {
            case TokenKind.Pipe:
                return GetRange();

            case TokenKind.OpenSquareBracket:
                return GetList();

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

        if (Current.Kind is not TokenKind.Colon)
            start = GetPrimary();

        Expect(TokenKind.Colon);

        if (Current.Kind == TokenKind.Pipe)
            return newRange();

        if (Current.Kind is not TokenKind.Colon)
            end = GetPrimary();

        if (Current.Kind == TokenKind.Pipe)
            return newRange();

        Expect(TokenKind.Colon);
        step = GetPrimary();

        return newRange();
    }

    private ListLiteral GetList()
    {
        var open  = Eat();
        var exprs = GetSeparated(GetExpression, TokenKind.CloseSquareBracket);
        var cls   = Expect(TokenKind.CloseSquareBracket);

        return new(open, exprs, cls);
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
        var cls  = expr.Kind is not NodeKind.Unknown
                 ? Expect(TokenKind.CloseParenthesis)
                 : null;

        if (cls is null || cls.IsFabricated)
            return ConstantLiteral.Fabricate(open.Span.To(cls?.Span ?? expr.Span));

        return new ParenthesizedExpression(open, expr, cls);
    }
}
