using System.Text;
using Enuii.Reports;
using Enuii.Runtime.Evaluation;
using Enuii.Scoping;
using Enuii.Semantics;
using Enuii.Syntax.AST;
using Enuii.Syntax.Lexing;

if (Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    Console.OutputEncoding = Encoding.Unicode;
    Console.InputEncoding  = Encoding.Unicode;
}

var reporter = new Reporter();
var semScope = new SemanticScope();
var scope    = new Scope();

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine() ?? "";


    var lexer    = new Lexer(line, reporter);
    var tokens   = lexer.Start();
    var parser   = new Parser(tokens, reporter);
    var synTree  = parser.Start();
    var analyzer = new Analyzer(synTree, semScope, reporter);
    var semTree  = analyzer.Start();

    if (reporter.Errors.Count == 0)
    {
        var evaluator = new Evaluator(semTree, scope, reporter);
        var value = evaluator.Start();

        Console.WriteLine(value);
    }

    foreach (var error in reporter.Errors)
        Console.WriteLine(error);

    reporter.Flush();
}
