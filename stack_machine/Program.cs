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

        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();
        if (tokens is [] || tokens.Last().Type is TokenType.Bad) {
            return;
        }

        foreach (var t in tokens) 
            Console.WriteLine($"{t.Type}: '{t.Lexeme}'");

        var machine = new StackMachine(tokens);
        machine.Execute();
    }
}