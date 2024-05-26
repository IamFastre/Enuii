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
    private char Current => Peek(0);
    private char Next    => Peek(1);
    private bool EOF     => Index >= Source.Length;
    private bool EOL     => Current == '\n';
    private bool PEOF    => Index + 1 >= Source.Length;
    private bool PEOL    => Next == '\n';

    public Lexer(string source, Reporter? reporter = null)
    {
        Source   = source.Replace("\r", "");
        Reporter = reporter ?? new();
    }

    /* =========================== Helper Methods =========================== */

    // Look for character beyond index
    private char Peek(int i = 1)
        => Index + i < Source.Length ? Source[Index + i] : '\0';

    // Advances the index without overshoot over the length
    private void Advance()
    {
        if (Index < Source.Length)
            Index++;
    }

    // Convert current index (or given index if given) to a position
    private Position GetPosition(int? index = null)
    {
        index ??= Index;
        int line = 1, column = 1;

        for (int i = 0; i < index; i++)
            if (Current == '\n')
            {
                line++;
                column = 0;
            }
            else
                column++;

        return new(line, column, (int) index);
    }

    /* ====================================================================== */

    public Token[] Start()
    {
        var tokens = ImmutableArray.CreateBuilder<Token>();
        while (true)
        {
            var token = GetToken();
            tokens.Add(token);
            if (token.Kind == TokenKind.EOF)
                break;
            Advance();
        }

        return [..tokens];
    }

    /* ====================================================================== */

    // Get the current token starting with `Current`
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


        /* ============================ Specials ============================ */

        // Check for end-of-file
        if (EOF)
            return Token.EOF(GetPosition());

        // Check for new line
        if (EOL)
            return Token.NL(GetPosition());

        // Check for whitespaces
        if (char.IsWhiteSpace(Current))
        {
            while (Next == Current && char.IsWhiteSpace(Next))
                Step();
            return CreateToken(value.Length > 1 ? TokenKind.BigWhiteSpace : TokenKind.WhiteSpace);
        }

        /* ============================= Numbers ============================ */
        //    Peek in and if it's number char advance then add it
        //    after that return the token
        if (char.IsAsciiDigit(Current) || Current == CONSTS.DOT && char.IsAsciiDigit(Next))
        {
            bool isFloat = Current == CONSTS.DOT;
            while (char.IsAsciiDigit(Next) || Next == CONSTS.DOT && !isFloat)
            {
                if (Next == CONSTS.DOT)
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

        // Look for for ∞ and ∞f
        if (Current == CONSTS.INF)
        {
            bool isFloat = false;
            if ("fF".Contains(Next))
            {
                Step();
                isFloat = true;
            }

            return CreateToken(isFloat ? TokenKind.Float : TokenKind.Integer);
        }

        /* ============================= Quotes ============================= */
        //     Lexes strings and chars by known their closing pair
        //     and appending everything in between to value
        if (CONSTS.StrOpen.Contains(Current) || CONSTS.CharOpen.Contains(Current))
        {
            var (close, kind) = CONSTS.GetQuotePair(Current);
            while (Next != close && !(PEOF || PEOL))
            {
                if (Next == '\\')
                    Step();
                Step();
            }

            if (Next == close)
                Step();
            else
                Reporter.ReportUnterminatedQuote(kind, span);
            
            return CreateToken(kind);
        }

        
        /* =========================== Identifiers ========================== */
        //    Lexes identifiers, keywords, and constant literal values,
        //    such as false, maybe, true and null
        if (char.IsLetter(Current) || Current == '_')
        {
            while (char.IsLetterOrDigit(Next) || Next == '_')
                Step();

            return CreateToken(CONSTS.GetIdentifierKind(value.ToString()));
        }

        /* ======================= Character-sequences ====================== */
        //    Use `IsUpcoming` helper function to test
        // Operators:
        if (IsUpcoming("**"))
            return CreateToken(TokenKind.Power);

        if (IsUpcoming("&&"))
            return CreateToken(TokenKind.DoubleAmpersand);

        if (IsUpcoming("||"))
            return CreateToken(TokenKind.DoublePipe);

        // Others:
        if (IsUpcoming("->"))
            return CreateToken(TokenKind.SingleArrow);

        /* ======================== Single-character ======================== */
        //    Matching every single character token
        switch (Current)
        {
            // Operators:
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
            // Brackets:
            case '(':
                return CreateToken(TokenKind.OpenParenthesis);
            case ')':
                return CreateToken(TokenKind.CloseParenthesis);
            // Others
            case ':':
                return CreateToken(TokenKind.Colon);
        }

        // If none of the above; not known
        Reporter.ReportUnrecognizedChar(value, span);
        return CreateToken(TokenKind.Unknown);
    }
}
