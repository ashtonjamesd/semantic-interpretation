## Lexical Analysis

In this section, we'll explore more advanced error-handling capabilities and more efficient tokenization mechanisms for our lexer. The most notable difference between this and the stack machine lexer that we made is that it will handle errors with quite a bit more style. For instance, if the user has typed an unterminated string in their program, the following message should appear.

```
12 | "Hello, World!
                  ^
Syntax Error: unterminated string literal on line 12
```

This level of debugging detail helps the programmer to quickly understand what went wrong and where the error occurred. In future iterations, we will revisit this error handling and the mechanism will be expanded to include the originating file of the error and provide stack traces.

The way we will implement this is with a method that simply takes in a string error message and generates some output. Before that, we need to somehow grab all tokens on a specific line, in order to output them as one.

```
private string GetCurrentLine() {
    int lineIdx = Current - 1;
    

    // ...
}
```

We start by saving the index of the current source character. We negate one to prevent index-out-of-range errors if the lexer has errored at the very end of the source code.

```
while (lineIdx > 0 && Source[lineIdx] != '\n') {
    lineIdx--;
}
```

We know that each line in the source code is separated by a newline character. Given this, we can decrement the line index until we encounter a newline character, in which case we will know that we have reached the start of the current line. 

If we are on the first line in the program, then we will not encounter a newline character when looping back, which is why we check if index is larger than 0.

```
var nextNewLineIdx = Source.IndexOf('\n', lineIdx + 1);
if (nextNewLineIdx == -1) {
    return Source[lineIdx..].Replace("\n", "");
}

return Source[lineIdx..nextNewLineIdx].Replace("\n", "");
```

Now that we are at the start of the line, we find the index of the next newline character. If it does not exist, then the source code is simply one line long and we substring all of it. Otherwise, we substring from the start to the next newline character. We also remove any newline characters from the string to help with formatting the error. This will extract a string that contains the characters on the current line.

We now have a way of extracting the current line of source code, we can start to create the error reporting method. The method will also take in a string message to be outputted. First, we extract the current line with the method we created earlier. We then write the line to the console with its line number.

```
private Token SyntaxError(string message) {
    var line = GetCurrentLine();
    Console.WriteLine($"{CurrentLine} | {line}");  // 1 | "Hello, World!

    // ...
}
```

Next, find the start index of the line in the source code. This is used to calculate the exact position of the error within the line. We also factor in the offset of the `{CurrentLine} | ` in the first `Console.WriteLine` as we need to consider this when positioning the arrows underneath the line.

Now we construct a new string that will be pointing at the error in the line. The `Current - ++idx` determines how far into the line the error occurred. The `idx` is incremented to account for the zero-based indexing. We also factor in the offset of writing the line number with the line. Then, we draw the arrow marker underneath the line, pointing at the character causing the error.

```
var idx = Source.IndexOf(line);
var lineOffset = CurrentLine.ToString().Length + 2;

Console.WriteLine(new string(' ', Current - ++idx + lineOffset) + "^^^");
```

Finally, we write the message passed into the error method and the current line it occurred on. We then set the error flag to ensure the lexer terminates early.

```
Console.WriteLine($"{message} on line {CurrentLine}");

HasError = true;
return NewToken(TokenType.BadToken, "");
```

<br/>

Additionally, to make life easier, we can make the lexer operate with a collection of token type maps to the string representation for that token. This essentially means that the lexer will handle a lot of the token creation seamlessly. These maps will be entirely comprised of symbols rather than alphabetic or numeric tokens. 

Every time the lexer encounters a symbol character, it will attempt to find the key in the symbol maps we have defined. The reason we are using a map is because it removes the need to add an additional `if` statement in the code every time we want to add a new symbol token. We instead add a new map entry.

We are going to create each map based on the length of the token. We will start with single length tokens that are part of our language grammar.

```
private static readonly Dictionary<char, TokenType> SingleTokens = new() {
    [';'] = TokenType.SemiColon,
    [':'] = TokenType.Colon,
    ['='] = TokenType.SingleEquals,
}
```

Next, we create the map for symbol tokens that have a length of 2.

```
private static readonly Dictionary<string, TokenType> DoubleTokens = new() {
    ["=="] = TokenType.DoubleEquals,
    ["!="] = TokenType.NotEquals,
};
```

To ensure that the longer tokens take precedence over the shorter ones, we have to create a special method that will parse a symbol into one of the tokens in these maps. The reason we have to enforce precedence on longer tokens over short can be explained with the following snippet:

```
x == 2;
```

The above should be parsed as:

```
[x] [==] [2] [;]
```

However, it will only result in correctly parsed tokens if the double equals token takes precedence over the single equals token. If it was to parse the single equal token first, we would incorrectly get this:

```
[x] [=] [=] [2] [;]
```

To implement the symbol parsing method, we can start by iterating through the single tokens map to check if the current character matches any of the single-character tokens. If a match is found, we return a new token with the corresponding type. If no match is found, we return a bad token to indicate the symbol is not recognised.

```
private Token ParseShortToken() {
    char c = Source[Current];

    // ...

    if (SingleTokens.TryGetValue(c, out var type)) {
        return NewToken(type, c.ToString());
    }

    return NewToken(TokenType.BadToken, "");
}
```

However, our lexer also needs to be able to handle symbols that consist of two characters. To do this, we first check if the current character matches the beginning of any two-character tokens. If there is a match, we move the pointer forward to check the second character in the symbol. We also ensure that we do not go beyond the end of the input, in which case we break and attempt to return a single-character token or bad token.

```
foreach (var token in DoubleTokens) {
    if (!token.Key.StartsWith(c)) continue;

    Current++;
    if (IsEnd()) break;
    
    // ...
}
```

Next, we capture the second character in the symbol and combine it with the first character to form a new string representing the two-character symbol. If the double token map contains the new string, we return a new token with the corresponding type. If there is no match, we decrement to go back to the initial character so that it can check for a single-character token.

```
var nextChar = Source[Current];

var doubleToken = new string([c, nextChar]);
if (DoubleTokens.TryGetValue(doubleToken, out var doubleType)) {
    Current++;
    return NewToken(doubleType, doubleToken);
}

Current--;
```

This approach allows us to cleanly handle and extend out lexer with new tokens. The only limitation is that if we want to add tokens of length three or more, we either manually add checks for them with an `if` statement, or create additional token maps for three-character tokens.

<br/>

To add a new token symbol in our language grammar we first have to add a new value into the TokenType enum. Next, we add the new token into the correlating map:

```
[">="] = TokenType.GreaterThanEquals,
```

And that's it. Now the lexer will parse the new token when it encounters it in the source code. 

Apart from these additional features we added to our lexer, it mostly works the same as our other one.