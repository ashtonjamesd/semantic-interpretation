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
    };

    internal List<Token> Tokenize() {
        while (Current < Source.Length) {
            if (char.IsWhiteSpace(Source[Current])) {
                Current++;
                continue;
            }

            var token = ParseToken();
            Tokens.Add(token);

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
            Console.WriteLine($"Invalid token: '{token}'");
            return new(token.ToString(), TokenType.Bad);
        }
    }

    private Token ParseIdentifier() {
        int start = Current;

        while (Current < Source.Length && char.IsLetter(Source[Current]))
            Current++;

        var lexeme = Source.Substring(start, Current - start);
        return new(lexeme, Keywords[lexeme]);
    }

    private Token ParseNumeric() {
        int start = Current;

        while (Current < Source.Length && char.IsDigit(Source[Current]))
            Current++;

        var lexeme = Source.Substring(start, Current - start);
        return new(lexeme, TokenType.Numeric);
    }
}