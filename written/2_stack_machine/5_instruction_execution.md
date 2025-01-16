## Instruction Execution

Our program instructions are relatively simple, and can be executed without generating a higher or intermediary representation. However, with more complicated instructions involving more than just two parts, parsing the code into an abstract syntax tree (AST) would be suitable. More on that in part 2.

We will create another class, 'StackMachine', to represent the execution part of the stack machine. This separates the two responsibilities of the program and keeps their implementation details hidden from each other. All the StackMachine class needs to know is that it takes in a list of tokens and executes them accordingly.

The StackMachine class will also return a boolean to signify whether the execution was successful or not.

```
internal class StackMachine {
    private readonly List<Token> Tokens;
    private int InstructionPointer = 0;

    public StackMachine(List<Token> tokens) {
        Tokens = tokens;

    }

    public bool Execute() {
        while (Tokens[InstructionPointer].Type is not TokenType.Eof) {
            var token = Tokens[InstructionPointer];

            InstructionPointer++;
        }

        return true;
    }
}
```

Next, we need a stack data structure for the machine to operate around. Conveniently, C# has a built-in stack class that we can use for our interpreter operations. We can add a new stack field to our engine like this:

```
private readonly Stack<int> Stack = new();
```

Next, we need to add the implementation details for the ‘push’ and ‘pop’ instructions. Each of those will be a separate function inside the Machine class.

```
private bool ExecutePush() {
    // in the event of 'push ' with no argument
    if (InstructionPointer >= Tokens.Count) {
        return Error("expected argument after 'push'");
    }

    InstructionPointer++; // skip past the 'push'

    var arg = Tokens[InstructionPointer].Value;
    
    // in the event of an invalid push 'push f'
    if (!int.TryParse(arg, out var argInt)) {
        return Error("expected numeric argument after 'push'");
    }

    Stack.Push(argInt);
    return true;
}
```

We can also create an error handling method to make life easier:

```
private static bool Error(string message) {
    Console.WriteLine($"Interpreter Error: {message}");
    return false;
}
```

Initially, we check if an argument as even be supplied to the push instruction, and if not we return an error with an appropriate message. Next, as we now know an argument has been given to the push instruction, we need to increment the instruction pointer to point at the next token. Since the user can provide anything as the argument, we validate that it is a valid integer and then push the value on to the stack.

Similarly, the implementation of ‘pop’ looks like this.

```
private bool ExecutePop() {
    if (Stack.Count == 0) {
        return Error("attempted to pop value off an empty stack");
    }

    Stack.Pop();
    return true;
}
```

Additionally, we need to add the following code in the Execute method, so the interpreter is aware of these methods and can execute them.

```
bool result = true;
if (token.Type == TokenType.Push) {
    result = ExecutePush();
}
else if (token.Type == TokenType.Pop) {
    result = ExecutePop();
}

if (!result) {
    return result;
}
```

Now we need a way of outputting the values on the stack so we can verify the operations have been successful. To achieve this, we can implement the ‘print’ function that will get the value at the top of the stack and output it to the console.

First, we add a new value to the TokenType enum.

```
public enum TokenType {
    // ...

    Print
}
```

Next, we add a new entry into our Keywords dictionary so that lexer recognises it as a valid keyword token.

```
["print"] = TokenType.Print,
```

And that’s it for the lexer. How easy was that? Now we implement the instruction in the Machine class.

We start by defining a new method called ‘ExecutePrint’. Since we do not want to affect the stack when outputting the values, rather than using pop, we use peek to view the top item. Then we can simply print it to the output.

```
private bool ExecutePrint() {
    if (Stack.Count == 0) {
        return Error("attempted to print value from an empty stack");
    }

    Console.WriteLine(Stack.Peek());

    return true;
}
```

Finally, we add an additional ‘else if’ block in the Execute method.

```
else if (token.Type == TokenType.Print) {
    result = ExecutePrint();
}
```

Now let’s create a simple program to verify it is working as expected.

```
push 3
push 2
push 1

print pop
print pop
print pop
```

Which outputs:

```
1
2
3
```

We can see it is working as expected. The program counts up from 1 to 3 and outputs each number to the console. You may notice that the ‘print’ and ‘pop’ instructions are on the same line. This still works as the program does not parse the source code based on new line characters, but rather through each individual character and on expecting spaces between instructions. This means that if you wanted to, you could put all the code on one line.

You may have noticed that every time we want to add a new instruction implementation to the StackMachine, we have to write a new 'else if' block. This is not the best way of doing this and ideally we would like to edit the code as little as possible when extending it. We can take a similar approach as in the Lexer class and use a dictionary mapping the type of the token to an execution method.

```
private readonly Dictionary<TokenType, Func<bool>> Registry;
```

Now we initialise the registry through the StackMachine constructor.

```
Registry = new() {
    [TokenType.Push] = ExecutePush,
    [TokenType.Pop] = ExecutePop,
    [TokenType.Print] = ExecutePrint,
};
```

The updated Execution snippet now looks like this:

```
var isMethod = Registry.TryGetValue(token.Type, out var method);
if (isMethod) {
    var result = method!();
    if (!result) return false;
}
else {
    return Error($"unknown identifier '{token.Value}'");
}
```

We attempt to find a method corresponding with the token type. If a match is found, it is executed, otherwise it is treated as an error and the program terminates. This makes our code a lot easier to extend as all we need to do is define a new method and create a new registry entry.

Now let’s make the stack machine a bit more interesting and implement some basic mathematical operations to the stack machine: addition and subtraction. They both work by taking the top two values on the stack and applying the operation.

In the Lexer, we first add the Add and Sub values to the TokenType enum as well as a Keyword entry for each. Next we provide the StackMachine with the implementation for the addition and subtraction methods.

```
private bool ExecuteAdd() {
    if (Stack.Count < 2) {
        return Error("not enough values on the stack to perform 'add'");
    }

    var a = Stack.Pop();
    var b = Stack.Pop();

    Stack.Push(a + b);

    return true;
}
```

For addition, we must check if the stack has the required two values on the stack. If so, we simply pop both and push the result of their addition.

```
private bool ExecuteSub() {
    if (Stack.Count < 2) {
        return Error("not enough values on the stack to perform 'sub'");
    }

    var a = Stack.Pop();
    var b = Stack.Pop();

    Stack.Push(b - a);

    return true;
}
```

The implementation for Sub is similar but instead we subtract the two numbers.

Let's test the new implementations:

```
push 5
push 7
add
print // 12
```


```
push 18
push 5
sub
print // 13
```

The next part of our stack interpreter involves creating control flow for our programs. Control flow will allow us to loop and reuse code, as well as terminate the execution when necessary.