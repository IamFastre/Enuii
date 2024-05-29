using System.Text;
using Enuii.Reports;
using Enuii.Runtime.Evaluation;
using Enuii.Semantics;
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
    var analyzer = new Analyzer(synTree, reporter);
    var semTree  = analyzer.Start();

    foreach (var error in reporter.Errors)
        Console.WriteLine(error);

    if (reporter.Errors.Count == 0)
    {
        var evaluator = new Evaluator(semTree, reporter);
        var value = evaluator.Start();
        Console.WriteLine(value);
    }
}
