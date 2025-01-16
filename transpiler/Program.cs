internal class Program {
    static void Main(string[] args) {
        if (args.Length < 1) {
            Console.WriteLine("expected source file path as a single command line argument.");
            return;
        }

        if (!File.Exists(args[0])) {
            Console.WriteLine("unable to open source file.");
            return;
        }

        var source = File.ReadAllText(args[0]);

        var tokenizer = new Tokenizer(source);
        var tokens = tokenizer.Tokenize();

    }
}