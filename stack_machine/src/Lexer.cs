public sealed class Token {
    public string Value { get; set; }
    public TokenType Type { get; set; }

    public Token(string value, TokenType type) {
        Value = value;
        Type = type;
    }
}

public enum TokenType {
    Push,
    Pop,
    Numeric,
    Identifier,
    BadToken,
    Print,
    Eof
}

public class Lexer {
    private readonly List<Token> Tokens = new();
    private readonly string Source;
    private int Current = 0;

    private readonly Dictionary<string, TokenType> Keywords = new() {
        ["push"] = TokenType.Push,
        ["pop"] = TokenType.Pop,
        ["print"] = TokenType.Print,
    };

    public Lexer(string source) {
        Source = source;
    }

    public List<Token> Tokenize() {
        while (Current < Source.Length) {
            while (char.IsWhiteSpace(Source[Current])) {
                Current++;
                continue;
            }

            var token = ParseToken();
            Tokens.Add(token);

            Current++;
        }

        Tokens.Add(new("", TokenType.Eof));

        PrintLexer();
        return Tokens;
    }

    private Token ParseToken() {
        if (char.IsLetter(Source[Current])) {
            return ParseIdentifier();
        }
else if (char.IsDigit(Source[Current])) {
    return ParseNumeric();
}
        return new("", TokenType.BadToken);
    }


    private Token ParseIdentifier() {
        int start = Current;
        while (Current < Source.Length && char.IsLetter(Source[Current]))
            Current++;
        var value = Source[start..Current];

            if (Keywords.TryGetValue(value, out var keywordType)) {
    return new(value, keywordType);
}
        return new(value, TokenType.Identifier);
    }

    private Token ParseNumeric() {
    int start = Current;
    while (Current < Source.Length && char.IsDigit(Source[Current]))
        Current++;

    var value = Source[start..Current];
    return new(value, TokenType.Numeric);
}

    private void PrintLexer() {
        Console.WriteLine($"Source: {Source}");
        Console.WriteLine("\nTokens:");
        foreach (var token in Tokens) {
            Console.WriteLine($"\t{token.Type}: {token.Value}");
        }
    }
}