namespace albus.src;

public abstract class Expression { }

public class Ast {
    public List<Expression> Body = [];
}

public class BadExpression : Expression { }

public class LiteralExpression : Expression {
    public readonly object Value;

    public LiteralExpression(object value) {
        Value = value;
    }

    public override string ToString() {
        return Value.ToString() ?? "null";
    }
}

public class VariableDeclaration : Expression {
    private readonly string Identifier;
    private readonly Expression Value;

    public VariableDeclaration(string identifier, Expression value) {
        Identifier = identifier;
        Value = value;
    }

    public override string ToString() {
        return $"{Identifier} = {Value}";
    }
}

public class BinaryExpression : Expression {
    private readonly Expression Left;
    private readonly Token Operator;
    private readonly Expression Right;

    public BinaryExpression(Expression left, Token op, Expression right) {
        Left = left;
        Operator = op;
        Right = right;
    }

    public override string ToString() {
        return $"{Left} {Operator.Lexeme} {Right}";
    }
}

