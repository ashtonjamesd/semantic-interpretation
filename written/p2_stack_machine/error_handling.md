## Error Handling

Let’s start by error proofing the code in the Lexer class. We are currently not handling the case where the Lexer class returns a ‘Bad’ token, in which it will continue to parse the rest of the tokens.

We can add a simple check after the ‘ParseToken’ method that examines the last token parsed. If the type of the token is ‘Bad’, then it will terminate the tokenization of the source and return. 

```
if (token.Type is TokenType.BadToken) {
    return Tokens;
}
```

The ‘is’ keyword, in this case, is fundamentally equivalent to using ‘==’. It's just a little bit more naturally expressive. The C-Sharp compiler will optimise this particular if block to the statement below anyway.

```
if (token.Type == TokenType.BadToken) {
    return Tokens;
}
```

To properly handle this in the Main method, we can check if any of the tokens have the BadToken type. If this is the case, we do not pass the tokens to the interpreter.

```
if (tokens.Any(t => t.Type is TokenType.BadToken)) {
    return;
}
```

Let's also add some validation to the handling of the source code file. We can check if the path was even provided, then if it exists, and then we can start parsing.

```
if (args.Length is not 1) {
    Console.WriteLine("expected source file path as a single command line argument.");
    return;
}

if (!File.Exists(args[0])) {
    Console.WriteLine("unable to open source file.");
    return;
}

var source = File.ReadAllText(args[0]);
```

When we encounter errors we often get helpful information such as an error message, what file the error occurred in, the line number, and sometimes even a snippet of the offending code or stack trace. This helps the programmer debug the code and make amendments to the source. We do not need all of that information as the code is currently limited to single file execution, however, we will implement the bare minimum of the line count, and an error message.

We can start by adding a new field in the lexer class.

```
private int CurrentLine = 1;
```

Every time we encounter a newline character, we can increment the CurrentLine field. We will also move the skip whitespace logic into a separate method.


```
private void SkipWhitespace() {
    while (Current < Source.Length && char.IsWhiteSpace(Source[Current])) {

        if (Source[Current] is '\n') {
            CurrentLine++;
        }
        
        Current++;
        continue;
    }
}
```

Now when we run the code with a bad character in the source we get the following:

```
Lexer Error: invalid character ';' on line 8
```

We still have some error handling to take care of in the StackMachine class, and we are going to handle it in a similar way. Currently, the interpreter has no way of knowing what line an instruction is on unless it is in the lexer class, therefore we can't inform the user where the error occurred. To solve this, we can attach the line number to a token whenever we create a new one. We can simply add a new field to the Token class.

```
public readonly int Line;
```

The Error method in the StackMachine class can be updated as follows:

```
private bool Error(string message) {
    var line = Tokens[InstructionPointer].Line;
    
    Console.WriteLine($"Interpreter Error: {message} on line {line}");
    return false;
}
```

Now when an error occurs in the interpreter, we see:

```
Interpreter Error: expected numeric argument after 'push' on line 1
```