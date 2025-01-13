public class Tokenizer {
    private readonly string Source;
    private readonly List<Token> Tokens = new();
    private int Current = 0;
    private int CurrentLine = 1;

    public Tokenizer(string source) {
        Source = source;
    }

    public List<Token> Tokenize() {
        while (!IsEnd()) {
            var token = ParseToken();
            Tokens.Add(token);

            if (token.Kind == TokenKind.Bad) 
                break;

            Current++;
        }

        foreach (var t in Tokens) 
            Console.WriteLine($"{t.Kind}: {t.Lexeme}");

        return Tokens;
    }

    private Token ParseToken() {
        var token = Source[Current];

        return  token switch {
            _ when char.IsDigit(token) => ParseNumeric(),
            _ when char.IsLetter(token) => ParseIdentifier(),
            '\"' => ParseString(),
            '\'' => ParseChar(),
            _ => Error("invalid token")
        };
    }

    private Token ParseChar() {
        Current++;
        if (IsEnd() || Source[Current] is '\'') {
            return Error("empty char literal");
        }

        var lexeme = Source[Current++].ToString();
        
        if (IsEnd() || Source[Current++] is not '\'') {
            return Error("invalid char literal");
        }

        return NewToken(lexeme, TokenKind.Char);
    }

    private Token ParseIdentifier() {
        int start = Current;

        while (!IsEnd() && char.IsLetter(Source[Current]))
            Current++;

        var lexeme = Source[start..Current];
        return NewToken(lexeme, TokenKind.Identifier);
    }

    private Token ParseNumeric() {
        int start = Current;

        bool hasDecimal = false;
        while (!IsEnd() && (char.IsDigit(Source[Current]) || Source[Current] is '.')) {
            if (Source[Current] is '.') {
                if (hasDecimal) {
                    return Error("invalid format for numeric literal");
                }

                hasDecimal = true;
            }

            Current++;
        }

        var lexeme = Source[start..Current];

        return NewToken(lexeme, TokenKind.Integer);
    }

    private Token ParseString() {
        int start = Current;

        Current++;
        while (!IsEnd() && Source[Current] is not '\"')
            Current++;

        var lexeme = Source[start..(Current + (IsEnd() ? 0 : 1))];

        if (lexeme[^1] is not '\"') {
            return Error("unterminated string literal");
        }

        return NewToken(lexeme, TokenKind.String);
    }

    private Token NewToken(string lexeme, TokenKind kind) {
        return new(lexeme, kind, CurrentLine);
    }

    private Token Error(string message) {
        Console.WriteLine(message);
        return NewToken("", TokenKind.Bad);
    }

    private bool IsEnd() {
        return Current >= Source.Length;
    }
}