using System.Linq.Expressions;

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
    public readonly string Identifier;
    public readonly Expression Value;

    public VariableDeclaration(string identifier, Expression value) {
        Identifier = identifier;
        Value = value;
    }

    public override string ToString() {
        return $"{Identifier} = {Value}";
    }
}

public class BinaryExpression : Expression {
    public readonly Expression Left;
    public readonly Token Operator;
    public readonly Expression Right;

    public BinaryExpression(Expression left, Token op, Expression right) {
        Left = left;
        Operator = op;
        Right = right;
    }

    public override string ToString() {
        return $"({Left} {Operator.Lexeme} {Right})";
    }
}

public class IfStatement : Expression {
    public readonly Expression? Condition;
    public readonly List<Expression> Body;
    public readonly IfStatement? Alternate;

    public IfStatement(Expression? condition, List<Expression> body, IfStatement? alternate) {
        Condition = condition;
        Body = body;
        Alternate = alternate;
    }

    public override string ToString() {
        string bodyStr = string.Join("\n", Body.Select(b => "  " + b.ToString()));

        var ifWord = "if";
        if (Condition is null) {
            ifWord = "else";
        } 

        return $"{ifWord} {Condition} then \n{bodyStr} \n{Alternate}  endif";
    }
}

public class WhileStatement : Expression {
    public readonly Expression Condition;
    public readonly List<Expression> Body;

    public WhileStatement(Expression condition, List<Expression> body) {
        Condition = condition;
        Body = body;
    }

    public override string ToString() {
        string bodyStr = string.Join("\n", Body.Select(b => "  " + b.ToString()));

        return $"while {Condition} \n{bodyStr} \nend";
    }
}

public class BreakStatement : Expression {
    public BreakStatement() {

    }

    public override string ToString() {
        return "break";
    }
}

public class ContinueStatement : Expression {
    public ContinueStatement() {
        
    }

    public override string ToString() {
        return "continue";
    }
}