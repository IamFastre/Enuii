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

    private char Peek(int i = 1) => Index + i < Source.Length ? Source[Index + i] : '\0';

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

        // Increment one step and append current tp value
        void Step(int steps = 1)
        {
            for (int i = 0; i < steps; i++)
            {
                Advance();
                value.Append(Current);
            }
        }

        // Check if a sequence of chars is coming
        // if so step, if not don't
        bool IsUpcoming(IEnumerable<char> sequence)
        {
            var length = sequence.Count();
            for (int i = 0; i < length; i++)
                if (!(sequence.ElementAt(i) == Peek(i)))
                    return false;
            Step(length - 1);
            return true;
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

        // Character-sequence characters
        if (IsUpcoming("**"))
            return CreateToken(TokenKind.Power);

        // Single character tokens
        switch (Current)
        {
            case '=':
                return CreateToken(TokenKind.Equal);
            case '+':
                return CreateToken(TokenKind.Plus);
            case '-':
                return CreateToken(TokenKind.Minus);
            case '*':
                return CreateToken(TokenKind.Asterisk);
            case '/':
                return CreateToken(TokenKind.ForwardSlash);
            case '%':
                return CreateToken(TokenKind.Percent);
            case '!':
                return CreateToken(TokenKind.BangMark);
            case '~':
                return CreateToken(TokenKind.Tilde);
            case '&':
                return CreateToken(TokenKind.Ampersand);
            case '|':
                return CreateToken(TokenKind.Pipe);
            case '^':
                return CreateToken(TokenKind.Caret);
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