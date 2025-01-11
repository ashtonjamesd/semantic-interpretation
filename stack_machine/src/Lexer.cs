public class Lexer {
    private readonly List<Token> Tokens = new();
    private readonly string Source;
    private int Current = 0;
    private int CurrentLine = 1;

    private readonly Dictionary<string, TokenType> Keywords = new() {
        ["push"] = TokenType.Push,
        ["pop"] = TokenType.Pop,
        ["print"] = TokenType.Print,
        ["add"] = TokenType.Add,
        ["sub"] = TokenType.Sub,
        ["jump"] = TokenType.Jump,
        ["when"] = TokenType.When,
    };

    public Lexer(string source) {
        Source = source;
    }

    public List<Token> Tokenize() {
        while (Current < Source.Length) {
            SkipWhitespace();
            if (Current >= Source.Length) 
                break;

            var token = ParseToken();
            Tokens.Add(token);

            if (token.Type is TokenType.BadToken) {
                return Tokens;
            }

            Current++;
        }

        Tokens.Add(new("", TokenType.Eof, CurrentLine));

        PrintLexer();
        return Tokens;
    }

    private void SkipWhitespace() {
        while (Current < Source.Length && char.IsWhiteSpace(Source[Current])) {

            if (Source[Current] is '\n') {
                CurrentLine++;
            }

            Current++;
            continue;
        }
    }

    private Token Error(string message) {
        Console.WriteLine($"Lexer Error: {message} on line {CurrentLine}");
        return new("", TokenType.BadToken, CurrentLine);
    }

    private Token ParseToken() {
        var token = Source[Current];

        if (char.IsLetter(token)) {
            return ParseIdentifier();
        }
        else if (char.IsDigit(token)) {
            return ParseNumeric();
        }

        return Error($"invalid character '{token}'");
    }

    private Token ParseIdentifier() {
        int start = Current;
        while (Current < Source.Length && char.IsLetter(Source[Current]))
            Current++;
        var value = Source[start..Current];

        if (Keywords.TryGetValue(value, out var keywordType)) {
            return new(value, keywordType, CurrentLine);
        }

        if (Current < Source.Length && Source[Current] is ':') {
            return new(value, TokenType.Label, CurrentLine);
        }

        return new(value, TokenType.Identifier, CurrentLine);
    }

    private Token ParseNumeric() {
        int start = Current;
        while (Current < Source.Length && char.IsDigit(Source[Current]))
            Current++;

        var value = Source[start..Current];
        return new(value, TokenType.Numeric, CurrentLine);
    }

    private void PrintLexer() {
        Console.WriteLine($"Source: {Source}");
        Console.WriteLine("\nTokens:");
        foreach (var token in Tokens) {
            Console.WriteLine($"\t{token.Type}: {token.Value}");
        }
    }
}