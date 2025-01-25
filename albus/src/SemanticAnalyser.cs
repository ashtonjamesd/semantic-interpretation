using albus.src;

public interface IExpressionVisitor {
    public void Visit(LiteralExpression expr);
    public void Visit(VariableDeclaration expr);
}

public class SemanticAnalyzer {
    private readonly Ast Ast;

    public SemanticAnalyzer(Ast ast) {
        Ast = ast;
    }

    public void Analyze() {
        foreach (var expr in Ast.Body) {
            var x = AnalyzeExpression(expr);
            Console.WriteLine(x);
        }
    }

    public object? AnalyzeExpression(Expression expr) {
        return expr switch {
            BinaryExpression binary => AnalyzeBinaryExpression(binary),
            LiteralExpression literal => AnalyzeLiteralExpression(literal),
            _ => null,
        };
    }

    public object AnalyzeBinaryExpression(BinaryExpression binary) {
        var left = AnalyzeExpression(binary.Left);
        var right = AnalyzeExpression(binary.Right);

        return binary.Operator.Type switch {
            TokenType.Plus => (int)left + (int)right,
            TokenType.Minus => (int)left - (int)right,
            TokenType.Star => (int)left * (int)right,
            TokenType.Slash => (int)left / (int)right,
            TokenType.Modulo => (int)left % (int)right,
        };

    }

    public object AnalyzeLiteralExpression(LiteralExpression literal) {
        return literal.Value;
    }
}