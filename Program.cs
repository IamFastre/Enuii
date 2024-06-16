using System.Text;
using Enuii.Main.REPL;

if (Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    Console.OutputEncoding = Encoding.Unicode;
    Console.InputEncoding  = Encoding.Unicode;
}

var repl = new REPL(args);
repl.Start();
