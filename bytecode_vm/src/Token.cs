public class Token {
    public readonly string Lexeme;
    public readonly TokenKind Kind;
    public readonly int Line;

    public Token(string lexeme, TokenKind kind, int line) {
        Lexeme = lexeme;
        Kind = kind;
        Line = line;
    }
}