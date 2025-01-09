internal sealed class StackMachine {
    private List<Token> Tokens { get; set; }
    private int Current = 0;

    private Stack<int> Stack = new();
    public StackMachine(List<Token> tokens) {
        Tokens = tokens;
    }

    internal void Execute() {
        while (Current < Tokens.Count) {
            var token = Tokens[Current];

            if (token.Type == TokenType.Push) {
                ExecutePush();
            }
            else if (token.Type == TokenType.Pop) {
                ExecutePop();
            }
            else if (token.Type == TokenType.Print) {
                ExecutePrint();
            }
            else if (token.Type == TokenType.Read) {
                ExecuteRead();
            }

            Current++;
        }
    }

    private void ExecutePush() {
        var arg = Tokens[++Current].Lexeme;
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
}