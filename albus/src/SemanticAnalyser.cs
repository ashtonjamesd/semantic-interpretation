using albus.src;

public interface IExpressionVisitor {
    public void Visit(LiteralExpression expr);
    public void Visit(VariableDeclaration expr);
}

public class SemanticAnalyzer : IExpressionVisitor {
    private readonly Ast Ast;
    private int Current = 0;

    public SemanticAnalyzer(Ast ast) {
        Ast = ast;
    }

    public void Visit(VariableDeclaration expr) {

    }

    public void Visit(LiteralExpression expr) {
        
    }

    public object EvaluateBinary(BinaryExpression expr) {
        

        return expr.Operator.Type switch {
        };
    }

    public void EvaluateExpression(Expression expr) {
        // return expr.Accept(this);
    }

    public TokenType MapValueToType(TokenType type) {
        return type switch {
            TokenType.String => TokenType.StringType,
            TokenType.Integer => TokenType.IntType,
            TokenType.Float => TokenType.FloatType,
            TokenType.Char => TokenType.CharType,
            _ => type
        };
    }
}