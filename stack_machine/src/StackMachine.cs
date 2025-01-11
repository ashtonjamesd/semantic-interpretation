internal sealed class StackMachine {
    private List<Token> Tokens { get; set; }
    private int InstructionPointer = 0;
    private Stack<int> Stack = new();

    public StackMachine(List<Token> tokens) {
        Tokens = tokens;

        Registry = new() {
            [TokenType.Push] = ExecutePush,
            [TokenType.Pop] = ExecutePop,
            [TokenType.Print] = ExecutePrint,
            [TokenType.Add] = ExecuteAdd,
            [TokenType.Sub] = ExecuteSub,
            [TokenType.Jump] = ExecuteJump
        };
    }

    private readonly Dictionary<TokenType, Func<bool>> Registry;

    internal bool Execute() {
        while (Tokens[InstructionPointer].Type is not TokenType.Eof) {
            var token = Tokens[InstructionPointer];

            var isMethod = Registry.TryGetValue(token.Type, out var method);
            if (isMethod) {
                var result = method!();
                if (!result) return false;
            }
            else if (token.Type is not TokenType.Label) {
                return Error($"unknown identifier '{token.Value}'");
            }

            InstructionPointer++;
        }

        return true;
    }

    private bool Error(string message) {
        var line = Tokens[InstructionPointer].Line;
        
        Console.WriteLine($"Interpreter Error: {message} on line {line}");
        return false;
    }

    private bool ExecutePush() {
        if (InstructionPointer >= Tokens.Count) {
            return Error("expected argument after 'push'");
        }

        InstructionPointer++; // skip past the 'push'
        var arg = Tokens[InstructionPointer].Value;
        if (!int.TryParse(arg, out var argInt)) {
            return Error("expected numeric argument after 'push'");
        }

        Stack.Push(argInt);
        return true;
    }

    private bool ExecutePop() {
        if (Stack.Count == 0) {
            return Error("attempted to pop value off an empty stack");
        }

        Stack.Pop();
        return true;
    }

    private bool ExecutePrint() {
        if (Stack.Count == 0) {
            return Error("attempted to print value from an empty stack");
        }

        Console.WriteLine(Stack.Peek());

        return true;
    }

    private bool ExecuteAdd() {
        if (Stack.Count < 2) {
            return Error("not enough values on the stack to perform 'add'");
        }

        var a = Stack.Pop();
        var b = Stack.Pop();

        Stack.Push(a + b);

        return true;
    }

    private bool ExecuteSub() {
        if (Stack.Count < 2) {
            return Error("not enough values on the stack to perform 'sub'");
        }

        var a = Stack.Pop();
        var b = Stack.Pop();

        Stack.Push(b - a);

        return true;
    }

    private bool ExecuteJump() {
        if (InstructionPointer >= Tokens.Count) {
            return Error("expected identifier argument after 'jump'");
        }

        InstructionPointer += 2;
        if (InstructionPointer < Tokens.Count && Tokens[InstructionPointer].Type is TokenType.When) {
            return ExecuteJumpWhen();
        }

        InstructionPointer--;
        return JumpTo(Tokens[InstructionPointer].Value);
    }

    private bool ExecuteJumpWhen() {
        var label = Tokens[InstructionPointer++ - 1].Value;

        var number = int.Parse(Tokens[InstructionPointer].Value);
        if (number == Stack.Peek()) {
            return JumpTo(label);
        }

        return true;
    }

    private bool JumpTo(string label) {
        for (int i = 0; i < Tokens.Count; i++) {
            if (Tokens[i].Type is TokenType.Label && Tokens[i].Value == label) {
                InstructionPointer = i;
                return true;
            }
        }

        return Error($"undefined label '{label}'");
    }
}