internal sealed class Lexer {
    private List<Token> Tokens = new();
    private int Current = 0;
    private string Source;

    public Lexer(string source) {
        Source = source;
    }

    private readonly Dictionary<string, TokenType> Keywords = new() {
        {"push", TokenType.Push},
        {"pop", TokenType.Pop},
        {"print", TokenType.Print},
        {"read", TokenType.Read},
        {"jump", TokenType.Jump},
        {"add", TokenType.Add},
        {"sub", TokenType.Sub},
    };

    internal List<Token> Tokenize() {
        while (Current < Source.Length) {
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

        return Tokens;
    }

    private Token ParseToken() {
        var token = Source[Current];

        if (char.IsLetter(token)) {
            return ParseIdentifier();
        } 
        else if (char.IsDigit(token)) {
            return ParseNumeric();
        }
        else {
            return Error($"invalid token: '{token}'");
        }
    }

    private Token ParseIdentifier() {
        int start = Current;
        while (Current < Source.Length && char.IsLetter(Source[Current]))
            Current++;

        var lexeme = Source.Substring(start, Current - start);

        if (Keywords.TryGetValue(lexeme, out var type)) {
            return new(lexeme, type);
        }

        if (Current < Source.Length && Source[Current] == ':') {
            return new(lexeme, TokenType.Label);
        }

        return new(lexeme, TokenType.Identifier);
    }

    private Token ParseNumeric() {
        int start = Current;
        while (Current < Source.Length && char.IsDigit(Source[Current]))
            Current++;

        var lexeme = Source.Substring(start, Current - start);
        return new(lexeme, TokenType.Numeric);
    }

    private static Token Error(string message) {
        Console.WriteLine(message);
        return new("", TokenType.Bad);
    }
}