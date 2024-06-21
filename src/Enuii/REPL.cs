using Enuii.General.Colors;
using Enuii.General.Exceptions;
using Enuii.Reports;
using Enuii.Runtime.Evaluation;
using Enuii.Scoping;
using Enuii.Semantics;
using Enuii.Symbols.Types;
using Enuii.Syntax.AST;
using Enuii.Syntax.Lexing;

namespace Enuii.Main.REPL;

public class REPL
{
    public  Reporter      Reporter      { get; } = new();
    public  SemanticScope SemanticScope { get; } = new();
    public  Scope         Scope         { get; } = new();

    public REPL(string[] args)
        => ParseArguments(args);


    /* =========================== Helper Methods =========================== */

    public void ParseArguments(string[] args) { }

    private void ReportErrors()
    {
        foreach (var error in Reporter.Errors)
            Console.WriteLine(error);

        Reporter.Flush();
    }

    private static void HandleInternalError(Exception exception)
    {
        var c = C.END + C.ITALIC + C.WHITE;
        var e = new Error(ErrorKind.InternalError, "An internal error has occurred");
        Console.WriteLine(e);
        Console.WriteLine($"  {c}Press press '{C.UNDERLINE}esc{c}', continue or '{C.UNDERLINE}enter{c}' "
                        + $"to print error or '{C.UNDERLINE}alt + enter{c}' for full error.{C.END}");

        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key is ConsoleKey.Enter)
            {
                string message;
                if (key.Modifiers.HasFlag(ConsoleModifiers.Alt))
                    message = $"{C.DIM}{exception}{C.END}";
                else
                    message = $"{C.DIM}MESSAGE: {exception.Message}{C.END}";

                Console.WriteLine("  " + message.ReplaceLineEndings("\n  "));
                break;
            }
            else if (key.Key is ConsoleKey.Escape)
                break;
        }
    }


    /* ====================================================================== */

    public void Start(string? chevron = null)
    {
        while (true)
        {
            try
            {
                Console.Write(chevron ?? Enuii.Defaults.Chevron);
                var line = Console.ReadLine() ?? "";
                Console.Write(C.END);

                var lexer    = new Lexer(line, Reporter);
                var tokens   = lexer.Start();

                var parser   = new Parser(tokens, Reporter);
                var synTree  = parser.Start();

                var analyzer = new Analyzer(synTree, SemanticScope, Reporter);
                var semTree  = analyzer.Start();

                if (Reporter.Errors.Count == 0)
                {
                    var evaluator = new Evaluator(semTree, Scope, Reporter);
                    var value     = evaluator.Start();

                    if (value.Type.ID is not TypeID.Void)
                        Console.WriteLine(value.Repr());
                }
            }
            catch (EnuiiRuntimeException) { }
            catch (Exception e)
            {
                HandleInternalError(e);
            }
            finally
            {
                ReportErrors();
            }
        }
    }
}
