using System.Collections.Immutable;
using System.Text;
using Enuii.General.Constants;
using Enuii.General.Positioning;
using Enuii.Reports;

namespace Enuii.Syntax.Lexing;

public class Lexer
{
    public string   Source   { get; }
    public Reporter Reporter { get; }

    private int  Index   { get; set; } = 0;
    private char Current => Index     >= Source.Length ? '\0' : Source[Index];
    private char Next    => Index + 1 >= Source.Length ? '\0' : Source[Index + 1];
    private bool EOF     => Index >= Source.Length;
    private bool EOL     => Current == '\n';

    private readonly ImmutableArray<Token>.Builder Tokens = ImmutableArray.CreateBuilder<Token>();

    public Lexer(string source, Reporter? reporter = null)
    {
        Source   = source.Replace("\r", "");
        Reporter = reporter ?? new();
    }

    private void Advance()
    {
        if (Index < Source.Length)
            Index++;
    }

    private Position GetPosition(int? index = null)
    {
        index ??= Index;
        int line = 1;
        int column = 1;
        for (int i = 0; i < index; i++)
        {
            if (Current == '\n')
            {
                line++;
                column = 0;
            }
            else
                column++;
        }
        return new (line, column, (int) index);
    }

    public Token GetToken()
    {
        // Initiating some token properties
        var value = new StringBuilder(Current.ToString());
        var span  = GetPosition().ToSpan();

        void Step()
        {
            Advance();
            value.Append(Current);
        }

        // The token maker helper function
        Token CreateToken(TokenKind kind)
            => new(value.ToString(), kind, span.SetEnd(GetPosition()));

        // if at end-of-file return said token
        if (EOF)
            return Token.EOF(GetPosition());

        /* ============================= Numbers ============================ */
        //   Peek in and if it's number char advance then add it
        //   after that return the token
        if (char.IsAsciiDigit(Current) || Current == Constants.DOT && char.IsAsciiDigit(Next))
        {
            bool isFloat = Current == Constants.DOT;
            while (char.IsAsciiDigit(Next) || Next == Constants.DOT && !isFloat)
            {
                if (Next == Constants.DOT)
                    isFloat = true;
                Step();
            }

            // if 'f' or 'F' is present after the number; it's a float
            if ("fF".Contains(Next))
            {
                Step();
                isFloat = true;
            }

            return CreateToken(isFloat ? TokenKind.Float : TokenKind.Integer);
        }

        if (Current == Constants.INF)
        {
            bool isFloat = false;
            if ("fF".Contains(Next))
            {
                Step();
                isFloat = true;
            }

            return CreateToken(isFloat ? TokenKind.Float : TokenKind.Integer);
        }

        // If none of the above; not known
        return CreateToken(TokenKind.Unknown);
    }

    public Token[] Start()
    {
        while (!EOF)
        {
            var token = GetToken();
            Tokens.Add(token);
            Advance();
        }

        return [..Tokens];
    }
}