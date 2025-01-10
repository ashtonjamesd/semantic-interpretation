public sealed class Lexer {
    private List<Token> Tokens = new();
    private int Current = 0;
    private string Source;
    private readonly bool IsDebug;

    public Lexer(string source, bool isDebug = false) {
        Source = source;
        IsDebug = isDebug;
    }

    private readonly Dictionary<string, TokenType> Keywords = new() {
        {"push", TokenType.Push},
        {"pop", TokenType.Pop},
        {"print", TokenType.Print},
        {"read", TokenType.Read},
        {"jump", TokenType.Jump},
        {"add", TokenType.Add},
        {"sub", TokenType.Sub},
        {"when", TokenType.When},
    };

    public List<Token> Tokenize() {
        while (!IsEnd()) {
            if (char.IsWhiteSpace(Source[Current])) {
                Current++;
                continue;
            }

            var token = ParseToken();
            Tokens.Add(token);

            if (token.Type is TokenType.Bad) {
                return Tokens;
            }

            Current++;
        }

        Tokens.Add(new("", TokenType.Eof));

        if (IsDebug) {
            PrintLexer();
        }

        return Tokens;
    }

    private Token ParseToken() {
        var token = Source[Current];

        return Source[Current] switch {
            _ when char.IsLetter(token) => ParseIdentifier(),
            _ when char.IsDigit(token) => ParseNumeric(),
            _ => Error($"invalid token: '{token}'")
        };
    }

    private Token ParseIdentifier() {
        int start = Current;
        while (!IsEnd() && (Source[Current] is '_' || char.IsLetter(Source[Current])))
            Current++;

        var lexeme = Source[start..Current];
        Console.WriteLine(lexeme);

        if (Keywords.TryGetValue(lexeme, out var type)) {
            return new(lexeme, type);
        }

        if (!IsEnd() && Source[Current] is ':') {
            return new(lexeme, TokenType.Label);
        }

        return new(lexeme, TokenType.Identifier);
    }

    private Token ParseNumeric() {
        int start = Current;
        while (!IsEnd() && char.IsDigit(Source[Current]))
            Current++;

        var lexeme = Source[start..Current];
        return new(lexeme, TokenType.Numeric);
    }

    private static Token Error(string message) {
        Console.WriteLine(message);
        return new("", TokenType.Bad);
    }

    private void PrintLexer() {
        Console.WriteLine("\nTokens:");
        foreach (var token in Tokens) {
            Console.WriteLine($"  {token.Type}: {token.Lexeme}");
        }
    }

    private bool IsEnd() {
        return Current >= Source.Length;
    }
}