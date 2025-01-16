## Control Flow

The stack machine is not very useful at this point as it lacks the core concepts when dealing with written logic: conditionals, and loops. We won’t be adding conditionals and loops in the conventional way you are familiar with, but rather with ‘jump’ statements and labels.

Conditional jump statements are a type of control flow that will execute only when a certain condition is true.

The syntax for this instructions will be as follows:

```
Jump <label_name> when <numeric>
```

The interpreter will only jump to the label specified if the numeric value is present at the top of the stack. The interpreter will also support a default, non-conditional jump statement that is just ‘jump’ followed by the label_name.

We can start by adding the ‘Jump’ statement, and then work on the conditionals afterwards. Add a new value in the TokenType enum and in the Lexer Keywords dictionary.

Since the label is not a keyword, it does not get added to the Keywords dictionary. In the lexer, we instead alter the ParseIdentifier method to have a special case for when the keyword for a token is not present. We check if the token after the identifier is a colon character. If so, we tokenize it as a valid label definition.

```
if (Current < Source.Length && Source[Current] is ':') {
    return new(value, TokenType.Label);
}
```

Next we need to add the implementation for the ‘Jump’ instruction in the ‘Machine’ class. First we need to find the label the code is trying to jump to. We do this by taking the lexeme of the token after the ‘Jump’ keyword and attempting to find a matching label token in the ‘Tokens’ List. We then set the index of the instruction pointer to the index of the matching label.

```
private bool ExecuteJump() {
    if (InstructionPointer >= Tokens.Count) {
        return Error("expected identifier argument after 'jump'");
    }

    InstructionPointer++;
    var label = Tokens[InstructionPointer].Value;

    for (int i = 0; i < Tokens.Count; i++) {
        if (Tokens[i].Type is TokenType.Label && Tokens[i].Value == label) {
            InstructionPointer = i;
            return true;
        }
    }

    return Error($"undefined label '{label}'");
}
```

If a token’s type is not found in the registry of executable keywords, we then check whether it is a label definition. Labels are valid but are not executable instructions, so we skip them. If the token is neither a recognised keyword nor a label, we treat it as an unknown identifier and return an error.

```
else if (token.Type is not TokenType.Label) {
    return Error($"unknown identifier '{token.Value}'");
}
```

Let’s try creating a loop in our program to test if it works.

```
push 1

main:
  print
  jump main
```

```
1
1
1
...
```

As expected, the interpreter enters an infinite loop of outputting the number '1'.

Implementing the conditional jump will be the exact same but with an additional comparison. It will help to separate the jump logic from the ExecuteJump method as we are likely to reuse this when implementing the new instructions.

Our refactored implementation now looks like this.

```
private bool ExecuteJump() {
    if (InstructionPointer >= Tokens.Count) {
        return Error("expected identifier argument after 'jump'");
    }

    InstructionPointer++;
    return JumpTo(Tokens[InstructionPointer].Value);
}
```

```
private bool JumpTo(string label) {
    for (int i = 0; i < Tokens.Count; i++) {
        if (Tokens[i].Type is TokenType.Label && Tokens[i].Value == label) {
            InstructionPointer = i;
            return true;
        }
    }

    return Error($"undefined label '{label}'");
}
```

In our language, conditional jumps check the value currently at the top of the stack and jump if the condition involving the value evaluates to true. Our implementation will allow the user to compare any integer in the condition.

First, we add a new 'When' keyword to the lexer. Since the stack machine will encounter the 'Jump' token first, it will still call the ExecuteJump method. We can define another method, 'ExecuteJumpWhen', that will handle the conditional logic.

Next, we need to update the ExecuteJump method to handle the case where a 'When' token appears after the label. To do this, we look ahead two tokens and check if a When token is present.

```
private bool ExecuteJump() {
    // ...

    InstructionPointer += 2;
    if (InstructionPointer < Tokens.Count && Tokens[InstructionPointer].Type is TokenType.When) {
        return ExecuteJumpWhen();
    }

    InstructionPointer--;
    return JumpTo(Tokens[InstructionPointer].Value);
}
```

The label token is retrieved from the value of the instruction pointer minus one. Then we increment the pointer so that it is pointing at the numeric argument passed to the conditional.

```
private bool ExecuteJumpWhen() {
    var label = Tokens[InstructionPointer++ - 1].Value;
    
    var number = int.Parse(Tokens[InstructionPointer].Value);
    if (number == Stack.Peek()) {
        return JumpTo(label);
    }

    return true;
}
```

Let's test it is working with a basic conditional jump program.

```
push 1

main:
  print
  jump main when 2

print
```

As expected, the jump statement is skipped and both print statements are executed and then the program terminates.

```
1
1
```

The output of the above program but with 'when 1' instead would output the number '1' infinitely.

```
1
1
1
...
```

The interpreter is working pretty nicely, however the code has been left in a purposefully vulnerable state and attention needs to be drawn to how it handles exceptions. In the next section, we will see what amendments we can make.