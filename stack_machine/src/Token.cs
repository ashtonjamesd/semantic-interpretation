public sealed class Token {
    public string Lexeme { get; set; }
    public TokenType Type { get; set; }

    public Token(string lexeme, TokenType type) {
        Lexeme = lexeme;
        Type = type;
    }
}