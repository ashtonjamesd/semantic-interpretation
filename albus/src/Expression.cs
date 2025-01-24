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

public class UnaryExpression : Expression {
    public readonly Expression Left;
    public readonly Token Operator;

        public UnaryExpression(Expression left, Token op) {
        Left = left;
        Operator = op;
    }

    public override string ToString() {
        return $"({Operator.Lexeme} {Left})";
    }
}

public class FunctionDeclaration: Expression {
    public readonly string Identifier;
    public readonly List<FunctionParameter> Parameters;
    public readonly Token ReturnType;
    public readonly List<Expression> Body;

    public FunctionDeclaration(string identifier, List<FunctionParameter> parameters, Token returnType, List<Expression> body) {
        Identifier = identifier;
        Parameters = parameters;
        ReturnType = returnType;
        Body = body;
    }

    public override string ToString() {
        var parameters = string.Join(",", Parameters.Select(x => x.Identifier));
        string bodyStr = string.Join("\n", Body.Select(b => "  " + b.ToString()));

        return $"def {Identifier} ({parameters}): {ReturnType.Lexeme}\n{bodyStr}";
    }
}

public class ReturnStatement : Expression {
    public readonly Expression Value;

    public ReturnStatement(Expression value) {
        Value = value;
    }

    public override string ToString() {
        return $"{Value}";
    }
}

public class FunctionParameter : Expression
{
    public readonly string Identifier;
    public readonly Token Type;

    public FunctionParameter(string identifier, Token type)
    {
        Identifier = identifier;
        Type = type;
    }

    public override string ToString() {
        return $"{Identifier}: {Type},";
    }
}

public class TernaryExpression : Expression {
    public readonly Expression Condition;
    public readonly Expression TrueBranch;
    public readonly Expression FalseBranch;

    public TernaryExpression(Expression condition, Expression trueBranch, Expression falseBranch) {
        Condition = condition;
        TrueBranch = trueBranch;
        FalseBranch = falseBranch;
    }

    public override string ToString() {
        return $"{Condition} then {TrueBranch} else {FalseBranch}";
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

public class NextStatement : Expression {
    public NextStatement() {
        
    }

    public override string ToString() {
        return "next";
    }
}