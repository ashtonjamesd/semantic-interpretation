public sealed class Token {
    public readonly string Value;
    public readonly TokenType Type;
    public readonly int Line;

    public Token(string value, TokenType type, int line) {
        Value = value;
        Type = type;
        Line = line;
    }
}