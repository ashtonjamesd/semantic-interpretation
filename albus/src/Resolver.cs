namespace albus.src;

public class Resolver {
    private readonly Ast Ast;
    private readonly Stack<Dictionary<string, object>> SymbolTable = new();

    public Resolver(Ast ast) {
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
            AssignmentExpression assignment => EvaluateAssignment(assignment),
            IdentifierExpression identifier => EvaluateIdentifier(identifier),
            IfStatement ifStatement => EvaluateIfStatement(ifStatement),
            _ => null
        };

        return result;
    }

    private object EvaluateIfStatement(IfStatement ifStmt) {
        if (ifStmt.Condition is not null) {
            var condition = EvaluateExpression(ifStmt.Condition!);

            if (condition is not bool) {
                Console.WriteLine("expected boolean expression in if statement");
                return false;
            }

            if ((bool)condition) {
                return EvaluateBlock(ifStmt.Body);
            }
        } else {
            return EvaluateBlock(ifStmt.Body);
        }

        if (ifStmt.Alternate is not null) {
            return EvaluateIfStatement(ifStmt.Alternate);
        }

        return true;
    }

    private Dictionary<string, object>? ResolveVariable(string identifier) {
        foreach (var scope in SymbolTable) {
            if (scope.ContainsKey(identifier)) {
                return scope;
            }
        }

        return null;
    }

    private object EvaluateBinaryExpression(BinaryExpression binary) {
        var left = EvaluateExpression(binary.Left);
        var right = EvaluateExpression(binary.Right);

        var result = CheckTypes(left, right, binary.Operator.Type);
        if (!result) {
            return false;
        }

        return binary.Operator.Type switch {
            TokenType.Plus   => (left is string v && right is string v1) ? v + v1 : (int)left + (int)right,
            TokenType.Minus  => (int)left - (int)right,
            TokenType.Star   => (int)left * (int)right,
            TokenType.Slash  => (int)left / (int)right,
            TokenType.Modulo => (int)left % (int)right,
        };
    }

    private bool CheckTypes(object? a, object? b, TokenType op) {
        if (a is int && b is int) {
            return true;
        } else if (a is string && b is string && op == TokenType.Plus) {
            return true;
        } else if (a is bool || b is bool) {
            Console.WriteLine($"Error: Cannot apply operator '{op}' to boolean values.");
            return false;
        } else {
            Console.WriteLine($"Error: Cannot apply operator '{op}' to values of type '{a?.GetType().Name}' and '{b?.GetType().Name}'.");
            return false;
        }
    }

    private object EvaluateVariableDeclaration(VariableDeclaration declaration) {
        if (SymbolTable.Peek().ContainsKey(declaration.Identifier)) {
            Console.WriteLine($"variable '{declaration.Identifier}' already defined in this scope.");
            return false;
        }

        var value = EvaluateExpression(declaration.Value);
        SymbolTable.Peek()[declaration.Identifier] = value;

        return value;
    }

    private object EvaluateAssignment(AssignmentExpression assignment) {
        var found = ResolveVariable(assignment.Identifier);
        if (found is null) {
            Console.WriteLine($"variable '{assignment.Identifier}' not defined in this scope");
            return false;
        }

        if (assignment.Value is IdentifierExpression expr) {
            var variable = ResolveVariable(expr.Name);
            
        }

        var value = EvaluateExpression(assignment.Value);
        found[assignment.Identifier] = value;

        return value;
    }

    private object EvaluateBlock(List<Expression> block) {
        SymbolTable.Push(new Dictionary<string, object>());

        object? result = null;

        foreach (var stmt in block) {
            result = EvaluateExpression(stmt);
        }

        SymbolTable.Pop();
        return result;
    }

    private object EvaluateIdentifier(IdentifierExpression identifier) {
        var found = ResolveVariable(identifier.Name);
        if (found is null || !found.TryGetValue(identifier.Name, out var value)) {
            return null;
        }

        return value;
    }

    private object EvaluateLiteralExpression(LiteralExpression literal) {
        return literal.Value;
    }
}