internal sealed class StackMachine {
    private List<Token> Tokens { get; set; }
    private int InstructionPointer = 0;

    private Stack<int> Stack = new();
    public StackMachine(List<Token> tokens) {
        Tokens = tokens;

        Registry = new() {
            {TokenType.Push, ExecutePush},
            {TokenType.Pop, ExecutePop},
            {TokenType.Print, ExecutePrint},
            {TokenType.Read, ExecuteRead},
            {TokenType.Jump, ExecuteJump},
        };
    }

    private readonly Dictionary<TokenType, Action> Registry = new();

    internal bool Execute() {
        while (InstructionPointer < Tokens.Count) {
            var token = Tokens[InstructionPointer];

            if (Registry.TryGetValue(token.Type, out var action)) {
                action();
            } 
            else if (token.Type is not TokenType.Label) {
                return Error($"undefined identifier '{token.Lexeme}'");
            }

            InstructionPointer++;
            Thread.Sleep(200);
        }

        return false;
    }

    private static bool Error(string message) {
        Console.WriteLine($"Interpreter Error: {message}");
        return false;
    }

    private void ExecutePush() {
        var arg = Tokens[++InstructionPointer].Lexeme;
        Stack.Push(int.Parse(arg));
    }

    private void ExecutePop() {
        Stack.Pop();
    }

    private void ExecutePrint() {
        var item = Stack.Peek();
        Console.WriteLine(item);
    }

    private void ExecuteRead() {
        var input = Console.ReadLine();
        int arg = int.Parse(input);

        Stack.Push(arg);
    }

    private void ExecuteJump() {
        InstructionPointer++; // advance past the 'jump'

        var labelName = Tokens[InstructionPointer].Lexeme;
        var labelIndex = Tokens
            .FindIndex(0, Tokens.Count, x => x.Lexeme == labelName && x.Type is TokenType.Label);

        InstructionPointer = labelIndex;
    }
}