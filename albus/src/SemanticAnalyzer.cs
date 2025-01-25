using albus.src;

public class SemanticAnalyzer {
    private readonly Ast Ast;
    private readonly Stack<Dictionary<string, object>> SymbolTable = new();

    public SemanticAnalyzer(Ast ast) {
        Ast = ast;
        SymbolTable.Push(new Dictionary<string, object>());
    }

    public void Analyze() {
        foreach (var expr in Ast.Body) {
            var x = EvaluateExpression(expr);
            Console.WriteLine(x);
        }
    }

    public object EvaluateExpression(Expression expr) {
        var result = expr switch {
            BinaryExpression binary => EvaluateBinaryExpression(binary),
            LiteralExpression literal => EvaluateLiteralExpression(literal),
            VariableDeclaration variableDeclaration => EvaluateVariableDeclaration(variableDeclaration),
            _ => null
        };

        return result;
    }

    private object EvaluateBinaryExpression(BinaryExpression binary) {
        var left = EvaluateExpression(binary.Left);
        var right = EvaluateExpression(binary.Right);

        return binary.Operator.Type switch {
            TokenType.Plus => (int)left + (int)right,
            TokenType.Minus => (int)left - (int)right,
            TokenType.Star => (int)left * (int)right,
            TokenType.Slash => (int)left / (int)right,
            TokenType.Modulo => (int)left % (int)right,
        };
    }

    private object EvaluateVariableDeclaration(VariableDeclaration declaration) {
        if (SymbolTable.Peek().ContainsKey(declaration.Identifier)) {
            Console.WriteLine($"variable '{declaration.Identifier}' already defined in this scope.");
        }

        var value = EvaluateExpression(declaration.Value);
        SymbolTable.Peek()[declaration.Identifier] = value;

        return value;
    }

    private object EvaluateLiteralExpression(LiteralExpression literal) {
        return literal.Value;
    }
}