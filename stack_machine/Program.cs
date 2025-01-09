internal abstract class Program {
    static void Main(string[] args) {
        if (args.Length != 1) {
            Console.WriteLine("Expected source file path.");
            return;
        }

        if (!File.Exists(args[0])) {
            Console.WriteLine("Unable to open source file.");
            return;
        }

        var source = File.ReadAllText(args[0]);
        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();

        var machine = new StackMachine(tokens);
        machine.Execute();
    }
}