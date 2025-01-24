using albus.src;

public interface IExpressionVisitor {
    public void Visit(VariableDeclaration expr);
}

public class SemanticAnalyser : IExpressionVisitor {
    private readonly Ast Ast;
    private int Current = 0;

    public SemanticAnalyser(Ast ast) {
        Ast = ast;
    }

    public void Visit(VariableDeclaration expr) {
        
    }
}