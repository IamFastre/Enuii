using Enuii.Syntax.Lexing;

var lexer = new Lexer("123T ");
var tokens = lexer.Start();

foreach (var tk in tokens)
    Console.WriteLine(tk);
