internal class Program {
    static void Main(string[] args) {
        var source = File.ReadAllText(args[0]);

        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();

        var machine = new StackMachine(tokens);
        machine.Execute();
    }
}