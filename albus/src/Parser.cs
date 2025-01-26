using System.Data;
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
            TokenType.If => ParseIfStatement(),
            TokenType.Def => ParseFunctionDeclaration(),
            TokenType.Return => ParseReturnStatement(), 
            TokenType.While => ParseWhileStatement(),
            TokenType.Break => ParseBreakStatement(),
            TokenType.Next => ParseNextStatement(),
            TokenType.Identifier => ParseAssignmentStatement(),
            _ => ParseExpression()
        };
    }

    private Expression ParseExpression() {
        return ParseTernary();
    }

    private Expression ParseAssignmentStatement() {
        var identifier = Tokens[Current++];

        if (!Expect(TokenType.SingleEquals, "'=' in assignment expression")) {
            return ExpressionError();
        }

        var value = ParseExpression();
        if (HasError) {
            return ExpressionError("expected expression in assignment expression");
        }

        if (!Expect(TokenType.SemiColon, "';' after statement")) {
            return ExpressionError();
        }

        Current--;
        return new AssignmentExpression(identifier.Lexeme, value);
    }

    private Expression ParseReturnStatement() {
        Current++;

        var expression = ParseExpression();
        if (HasError) {
            return ExpressionError("expected expression in return statement");
        }

        if (!Expect(TokenType.SemiColon, "';' after return statement")) {
            return ExpressionError();
        }

        Current--;
        return new ReturnStatement(expression);
    }

    private Expression ParseFunctionDeclaration() {
        Current++;

        var identifier = Tokens[Current].Lexeme;
        if (!Expect(TokenType.Identifier, "function name after 'def'")) {
            return ExpressionError();
        }
        
        if (!Expect(TokenType.LeftParen, "'(' after function name")) {
            return ExpressionError();
        }

        List<FunctionParameter> parameters = [];
        while (!IsLastToken() && !Match(TokenType.RightParen)) {
            var name = Tokens[Current++];

            if (IsLastToken()) {
                return ExpressionError("invalid function signature");
            }

            if (!Expect(TokenType.Colon, "':' after parameter")) {
                return ExpressionError();
            }

            var type = Tokens[Current++];
            parameters.Add(new(name.Lexeme, type));

            if (!Match(TokenType.RightParen) && !Expect(TokenType.Comma, "',' or ')' after parameter")) {
                return ExpressionError();
            }
        }

        Current++;
        if (!Expect(TokenType.Colon, "':' to specify function return type")) {
            return ExpressionError();
        }

        if (IsLastToken()) {
            return ExpressionError("expected function return type in signature");
        }
        var returnType = Tokens[Current++];

        var statements = new List<Expression>();
        while (!IsLastToken() && !Match(TokenType.End)) {
            var statement = ParseStatement();
            statements.Add(statement);

            Current++;
        }

        if (IsLastToken()) {
            return ExpressionError("function declaration started but missing 'End'");
        }

        Current++;

        return new FunctionDeclaration(identifier, parameters, returnType, statements);
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

        Current--;
        return new VariableDeclaration(identifier, value);
    }

    private Expression ParseIfStatement() {
        Current++;

        var condition = ParseLogicalOr();
        if (HasError) {
            return ExpressionError("invalid condition in if statement");
        }

        if (!Expect(TokenType.Then, "'then' after if statement condition")) {
            return ExpressionError();
        }

        var ifBody = new List<Expression>();
        while (!IsLastToken() && CurrentToken().Type is not (TokenType.Endif or TokenType.Elseif or TokenType.Else)) {
            var statement = ParseStatement();
        Console.WriteLine(statement.GetType());
            Current++;

            ifBody.Add(statement);
        }

        IfStatement? alternate = null;
        if (!IsLastToken()) {
            if (Match(TokenType.Elseif)) {
                alternate = (IfStatement)ParseIfStatement();
            } 
            else if (Match(TokenType.Else)) {
                Current++;

                if (!Expect(TokenType.Then, "'then' after if statement condition")) {
                    return ExpressionError();
                }

                var elseBody = new List<Expression>();
                while (!IsLastToken() && CurrentToken().Type is not TokenType.Endif) {
                    var statement = ParseStatement();
                    Current++;

                    elseBody.Add(statement);
                }

                alternate = new IfStatement(null, elseBody, null);
            }
        }

        return new IfStatement(condition, ifBody, alternate);
    }

    private Expression ParseWhileStatement() {
        Current++;

        var condition = ParseExpression();
        if (HasError) {
            return ExpressionError("invalid condition in while statement");
        }

        List<Expression> body = [];
        while (!IsLastToken() && !Match(TokenType.End)) {
            var statement = ParseStatement();
            body.Add(statement);

            Current++;
        }

        return new WhileStatement(condition, body);
    }

    private Expression ParseBreakStatement() {
        Current++;

        if (!Expect(TokenType.SemiColon, "';' after 'break'")) {
            return ExpressionError();
        }

        Current--;
        return new BreakStatement();
    }

    private Expression ParseNextStatement() {
        Current++;

        if (!Expect(TokenType.SemiColon, "';' after 'next'")) {
            return ExpressionError();
        }

        Current--;
        return new NextStatement();
    }

    private Expression ParseTernary() {
        var condition = ParseLogicalOr();

        if (Match(TokenType.Then)) {
            Current++;
            var trueBranch = ParseExpression();
            if (HasError) {
                return ExpressionError("expected expression in ternary operator");
            }

            if (!Expect(TokenType.Else, "'else' after ternary condition")) {
                return ExpressionError();
            }

            var falseBranch = ParseExpression();
            if (HasError) {
                return ExpressionError("expected expression in ternary operator");
            }

            return new TernaryExpression(condition, trueBranch, falseBranch);
        }

        return condition;
    }

    private Expression ParseLogicalOr() {
        var left = ParseLogicalAnd();

        while (Match(TokenType.Or)) {
            Token op = Tokens[Current++];
            var right = ParseLogicalAnd();
            left = new BinaryExpression(left, op, right);
        }

        return left;
    }

    private Expression ParseLogicalAnd() {
        var left = ParseEquality();

        while (Match(TokenType.And)) {
            Token op = Tokens[Current++];
            var right = ParseEquality();
            left = new BinaryExpression(left, op, right);
        }

        return left;
    }

    private Expression ParseEquality() {
        var left = ParseComparison();

        while (Match(TokenType.DoubleEquals) || Match(TokenType.NotEquals)) {
            Token op = Tokens[Current++];
            var right = ParseComparison();
            left = new BinaryExpression(left, op, right);
        }

        return left;
    }

    private Expression ParseComparison() {
        var left = ParseTerm();

        while (Match(TokenType.GreaterThan) || Match(TokenType.LessThan) || 
                Match(TokenType.GreaterThanEquals) || Match(TokenType.LessThanEquals)) {
            Token op = Tokens[Current++];
            var right = ParseTerm();
            left = new BinaryExpression(left, op, right);
        }

        return left;
    }

    private Expression ParseTerm() {
        var left = ParseFactor();

        while (Match(TokenType.Plus) || Match(TokenType.Minus)) {
            Token op = Tokens[Current++];
            var right = ParseFactor();
            left = new BinaryExpression(left, op, right);
        }

        return left;
    }

    private Expression ParseFactor() {
        var left = ParseUnary();

        while (Match(TokenType.Star) || Match(TokenType.Slash) || Match(TokenType.Modulo)) {
            Token op = Tokens[Current++];
            var right = ParseUnary();
            left = new BinaryExpression(left, op, right);
        }

        return left;
    }

    private Expression ParseUnary() {
        if (Match(TokenType.Minus) || Match(TokenType.Not)) {
            Token op = Tokens[Current++];
            var operand = ParseUnary();
            return new UnaryExpression(operand, op);
        }

        return ParsePrimary();
    }


    private Expression ParsePrimary() {
        var token = Tokens[Current++];

        return token.Type switch {
            TokenType.String => new LiteralExpression(token.Lexeme[1..(token.Lexeme.Length - 1)]),
            TokenType.Integer => new LiteralExpression(int.Parse(token.Lexeme)),
            TokenType.Char => new LiteralExpression(char.Parse(token.Lexeme)),
            TokenType.True or TokenType.False => new LiteralExpression(bool.Parse(token.Lexeme)),
            TokenType.Identifier => new IdentifierExpression(token.Lexeme),
            _ => ExpressionError()
        };
    }

    private bool Expect(TokenType type, string value) {
        if (IsLastToken() || CurrentToken().Type != type) {
            return ParseError($"expected {value}");
        }

        Current++;
        return true;
    }

    private bool Match(TokenType type) {
        if (IsLastToken()) return false;
        return Tokens[Current].Type == type;
    }

    private Token CurrentToken() {
        return Tokens[Current];
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
        return Current >= Tokens.Count || Tokens[Current].Type is TokenType.Eof;
    }
}