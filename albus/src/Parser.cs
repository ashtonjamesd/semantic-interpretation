using albus.src;

public class Parser {
    private readonly List<Token> Tokens;
    private int Current;
    public bool HasError { get; private set; }
    private readonly Ast Ast = new();

    public Parser(List<Token> tokens) {
        Tokens = tokens;
    }

    public Ast ParseAst() {
        while (!IsLastToken() && Tokens[Current].Type is not TokenType.Eof) {
            var expr = ParseStatement();
            Ast.Body.Add(expr);

            if (expr is BadExpression) {
                return Ast;
            }

            Current++;
        }

        return Ast;
    }

    private Expression ParseStatement() {
        return Tokens[Current].Type switch {
            TokenType.Let => ParseVariableDeclaration(),
            _ => ParseExpression()
        };
    }

    private Expression ParseExpression() {
        return ParsePrimary();
    }

    private Expression ParseVariableDeclaration() {
        Current++;

        var identifier = Tokens[Current].Lexeme;
        if (!Expect(TokenType.Identifier, "identifier after 'let'")) {
            return ExpressionError();
        }

        if (!Expect(TokenType.SingleEquals, "'=' after identifier")) {
            return ExpressionError();
        }

        var value = ParseExpression();
        if (HasError) {
            return ExpressionError("expected expression in variable declaration");
        }

        if (!Expect(TokenType.SemiColon, "';' after expression")) {
            return ExpressionError();
        }

        return new VariableDeclaration(identifier, value);
    }

    private Expression ParsePrimary() {
        var token = Tokens[Current++];

        return token.Type switch {
            TokenType.String => new LiteralExpression(token.Lexeme),
            TokenType.Integer => new LiteralExpression(int.Parse(token.Lexeme)),
            TokenType.Char => new LiteralExpression(char.Parse(token.Lexeme)),
            TokenType.True or TokenType.False => new LiteralExpression(bool.Parse(token.Lexeme)),
            TokenType.Identifier => new LiteralExpression(token.Lexeme),
            _ => ExpressionError()
        };
    }

    private bool Expect(TokenType type, string value) {
        if (IsLastToken() || Tokens[Current].Type != type) {
            return ParseError($"expected {value}");
        }

        Current++;
        return true;
    }

    private BadExpression ExpressionError(string? message = null) {
        if (message is not null) {
            ParseError(message);
        }

        HasError = true;
        return new BadExpression();
    }

    private bool ParseError(string message) {
        HasError = true;

        var currentToken = Tokens[IsLastToken() ? Current - 1 : Current];
        Console.WriteLine($"parsing error: {message} on line {currentToken.Line}");

        return false;
    }

    private bool IsLastToken() {
        return Current >= Tokens.Count;
    }
}