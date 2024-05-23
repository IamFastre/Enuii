using Enuii.Syntax.Lexing;

var lexer = new Lexer("∞ ∞f");
var tokens = lexer.Start();

foreach (var tk in tokens)
    Console.WriteLine(tk);
