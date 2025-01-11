// internal sealed class StackMachine {
//     private List<Token> Tokens { get; set; }
//     private int InstructionPointer = 0;
//     private Stack<int> Stack = new();
//     private readonly Dictionary<TokenType, Func<bool>> Registry = new();

//     public StackMachine(List<Token> tokens) {
//         Tokens = tokens;

//         Registry = new() {
//             {TokenType.Push, ExecutePush},
//             {TokenType.Pop, ExecutePop},
//             {TokenType.Print, ExecutePrint},
//             {TokenType.Read, ExecuteRead},
//             {TokenType.Jump, ExecuteJump},
//             {TokenType.Add, ExecuteAdd},
//             {TokenType.Sub, ExecuteSub},
//         };
//     }

//     internal bool Execute() {
//         while (Tokens[InstructionPointer].Type is not TokenType.Eof) {
//             var token = Tokens[InstructionPointer];

//             if (Registry.TryGetValue(token.Type, out var action)) {
//                 var result = action();
//                 if (!result) return false;
//             } 
//             else if (token.Type is not TokenType.Label) {
//                 return Error($"undefined identifier '{token.Value}'");
//             }

//             InstructionPointer++;
//             Thread.Sleep(50);
//         }

//         return true;
//     }

//     private static bool Error(string message) {
//         Console.WriteLine($"Interpreter Error: {message}");
//         return false;
//     }

//     private bool ExecutePush() {
//         if (InstructionPointer >= Tokens.Count) {
//             return Error("expected argument after 'push'");
//         }

//         var arg = Tokens[++InstructionPointer].Value;
//         if (!int.TryParse(arg, out var argInt)) {
//             return Error("expected numeric argument after 'push'");
//         }

//         Stack.Push(argInt);
//         return true;
//     }

//     private bool ExecutePop() {
//         if (Stack.Count == 0) {
//             return Error("attempted to pop value off an empty stack");
//         }

//         Stack.Pop();
//         return true;
//     }

//     private bool ExecutePrint() {
//         if (Stack.Count == 0) {
//             return Error("attempted to print value from empty stack");
//         }

//         Console.WriteLine(Stack.Peek());
//         return true;
//     }

//     private bool ExecuteRead() {
//         var input = Console.ReadLine();
//         if (!int.TryParse(input, out var argInt)) {
//             return Error("expected numeric argument for 'read'");
//         }

//         Stack.Push(argInt);
//         return true;
//     }

//     private bool ExecuteJump() {
//         if (InstructionPointer >= Tokens.Count) {
//             return Error("expected identifier argument after 'jump'");
//         }

//         InstructionPointer += 2;
//         if (InstructionPointer < Tokens.Count && Tokens[InstructionPointer].Type is TokenType.When) {
//             return ExecuteJumpWhen();
//         }

//         InstructionPointer--;
//         return JumpTo(Tokens[InstructionPointer].Value);
//     }

//     private bool ExecuteAdd() {
//         if (Stack.Count < 2) {
//             return Error("not enough values on the stack to perform 'add'");
//         }

//         var a = Stack.Pop();
//         var b = Stack.Pop();

//         Stack.Push(a + b);

//         return true;
//     }

//     private bool ExecuteSub() {
//         if (Stack.Count < 2) {
//             return Error("not enough values on the stack to perform 'sub'");
//         }

//         var a = Stack.Pop();
//         var b = Stack.Pop();

//         Stack.Push(b - a);

//         return true;
//     }

//     private bool ExecuteJumpWhen() {
//         InstructionPointer--;
//         var label = Tokens[InstructionPointer++].Value;

//         var when = Tokens[InstructionPointer++];
//         if (when.Type is not TokenType.When) {
//             return Error("jump 'when' expected");
//         }

//         var number = int.Parse(Tokens[InstructionPointer].Value);

//         if (number == Stack.Peek()) {
//             JumpTo(label);
//         }

//         return true;
//     }

//     private bool JumpTo(string label) {
//         var labelIndex = Tokens
//             .FindIndex(0, Tokens.Count, x => x.Value == label && x.Type is TokenType.Label);

//         if (labelIndex == -1) {
//             return Error($"undefined label '{label}'");
//         }

//         InstructionPointer = labelIndex;
//         return true;
//     }
// }