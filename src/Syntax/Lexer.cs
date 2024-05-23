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

        // The token maker helper function
        Token CreateToken(TokenKind kind)
            => new(value.ToString(), kind, span.SetEnd(GetPosition()));

        // if at end-of-file return said token
        if (EOF)
            return Token.EOF(GetPosition());

        // Numbers
        //   Peek in and if it's number char advance then add it
        //   after that return the token
        if (char.IsAsciiDigit(Current))
        {
            bool isFloat = false;
            while (char.IsAsciiDigit(Next) || Next == Constants.DOT && !isFloat)
            {
                if (Next == Constants.DOT)
                    isFloat = true;
                Advance();
                value.Append(Current);
            }

            return CreateToken(isFloat ? TokenKind.Float : TokenKind.Int);
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