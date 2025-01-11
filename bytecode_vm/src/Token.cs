public class Token {
    private readonly string Lexeme;
    private readonly TokenKind kind;
    private readonly int Line;

    public Token(string lexeme, TokenKind kind, int line) {
        Lexeme = lexeme;
        kind = kind;
        Line = line;
    }
}