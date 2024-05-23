using System.Text;
using Enuii.Reports;
using Enuii.Syntax.AST;
using Enuii.Syntax.Lexing;

if (Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    Console.OutputEncoding = Encoding.Unicode;
    Console.InputEncoding  = Encoding.Unicode;
}

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine() ?? "";

    var reporter = new Reporter(); 
    var lexer    = new Lexer(line, reporter);
    var tokens   = lexer.Start();
    var parser   = new Parser(tokens, reporter);
    var synTree  = parser.Start();

    foreach (var nd in synTree.Body)
        Console.WriteLine(nd);
    Console.WriteLine();

    foreach (var tk in tokens)
        if (!tk.Kind.IsParserIgnorable())
            Console.WriteLine(tk);
    Console.WriteLine();
    
    foreach (var err in reporter.Errors)
        Console.WriteLine(err);
    Console.WriteLine();
}
