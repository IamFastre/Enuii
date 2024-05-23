using Enuii.Syntax.Lexing;

var lexer = new Lexer("123.1.1ffT");
var tokens = lexer.Start();

foreach (var tk in tokens)
    Console.WriteLine(tk);
