﻿﻿using albus.src;

namespace albus;

internal class Program {
    static void Main(string[] args) {
        if (args.Length < 1) {
            Console.WriteLine("expected source file path");
            return;
        }

        if (!File.Exists(args[0])) {
            Console.WriteLine("source file path does not exist");
            return;
        }

        bool isDebug = false;
        if (args.Length == 2) {
            if (args[1] is not ("-d")) {
                Console.WriteLine($"invalid flag '{args[1]}'");
                return;
            }

            isDebug = true;
        }

        var source = File.ReadAllText(args[0]);

        var lexer = new Lexer(source, isDebug);
        var tokens = lexer.Tokenize();
        if (lexer.HasError) {
            return;
        }

        var parser = new Parser(tokens);
        var ast = parser.ParseAst();
        if (parser.HasError) {
            return;
        }

        foreach (var expr in ast.Body) {
            Console.WriteLine($"Expr: {expr}");
        }

        var validator = new Resolver(ast);
        validator.Analyze();

        Console.WriteLine("execution finished.");
    }
}