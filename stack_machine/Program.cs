using System.Diagnostics;

internal abstract class Program {
    static void Main(string[] args) {
        if (args.Length != 1) {
            Console.WriteLine("expected source file path as a command line argument.");
            return;
        }

        if (!File.Exists(args[0])) {
            Console.WriteLine("unable to open source file.");
            return;
        }

        var source = File.ReadAllText(args[0]);

        var lexer = new Lexer(source, isDebug: true);
        var tokens = lexer.Tokenize();
        if (tokens.Last().Type is TokenType.Bad) {
            return;
        }

        var machine = new StackMachine(tokens);
        var result = machine.Execute();

        if (!result) {
            Console.WriteLine("Program execution finished with errors");
            return;
        }

        Console.WriteLine("Program execution finished");
    }
}