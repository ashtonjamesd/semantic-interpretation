## Simple Lexical Analysis

Lexical analysis, or tokenization is the process of transforming a less meaningful piece of source code into a more meaningful higher representation. This representation is often comprised of a collection of 'tokens' which hold the transformed data of the source code. Each token will have a 'type' or 'category' they fall into, such as an operator, identifier, or keyword, along with the value of the token itself.

We can start by creating a class to represent a token in our language. Each token will hold two pieces of data; the 'Type' of the token and the 'Value'.

```
public sealed class Token {
    public string Value { get; set; }
    public TokenType Type { get; set; }

    public Token(string value, TokenType type) {
        Value = value;
        Type = type;
    }
}
```

A good practice is to minimise reliance on string comparison as it is easy to make typographical errors when working with strings. Instead, we have used an enumerated type to represent a fixed set of values. Note that the value of the token will always be parsed as a string, however the 'Type' field helps the interpreter to correctly parse the token's value into the appropriate data type.

```
public enum TokenType {
    Push,
    Pop,
    Numeric,
    Identifier,
    BadToken
}
```

A numeric token for the number '42' would look like this:

```
Token
  String Value: "42"
  TokenType Type: Numeric
```

Next, we can create a class to encapsulate all of the logic for lexical analysis, aptly named the ‘Lexer’ class. This will convert the raw source string into a higher representation, making them much easier to work with.

The Lexer class contains three private fields. One for storing the list of parsed tokens, the source string itself, and a pointer to the current character in the source string.

```
public class Lexer {
    private readonly List<Token> Tokens = new();
    private readonly string Source;
    private int Current = 0;

    public Lexer(string source) {
        Source = source;
    }

    public List<Token> Tokenize() {
        
        return Tokens;
    }
}
```

The main method can be updated to accept the source code path as a command-line argument, as shown below.

```
internal class Program {
    static void Main(string[] args) {
        var source = File.ReadAllText(args[0]);

        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();
    }
}
```

The lexer will take in a string of source code, loop over each character in the string, and decide which token to create based on what it encounters. For instance, if the character ‘4’ is encountered it will attempt to parse a number. If a 'd' is encountered, it will attempt to parse an identifier or keyword token.

Let’s say we have the program: ‘push 10’. This will be parsed into two separate tokens: ‘push’, and ‘10’. The lexer will encounter the ‘p’ first and traverse through the string until it encounters a non-alphabetic character, in other words: the space after the ‘h’. Once it has finished parsing the identifier, it will create a new token with a section of the source string cut out.

We can use a while loop to traverse the input source and increment after each character. The 'ParseToken' method will decide what type of token to create.

```
public List<Token> Tokenize() {
    while (Current < Source.Length) {
        var token = ParseToken();
        Tokens.Add(token);

        Current++;
    }

    return Tokens;
}
```

We also must remember to account for the whitespace the lexer will encounter. Another loop at the beginning of the top-level one can take care of this.

```
while (Current < Source.Length && char.IsWhiteSpace(Source[Current])) {
    Current++;
    continue;
}
```

Since the incrementing of the character pointer can lead us up to the end of the source, we need an additional check to break early.

```
if (Current >= Source.Length) 
    break;
```

Currently, we are only parsing identifiers so we will assume every other token encountered is a 'BadToken'.

```
private Token ParseToken() {
    if (char.IsLetter(Source[Current])) {
        return ParseIdentifier();
    }

    return new("", TokenType.BadToken);
}
```

To implement the tokenization of identifiers, we begin by creating a temporary variable to store the start of the identifier, set to the value of Current.

```
private Token ParseIdentifier() {
    int start = Current;

}
```

Next, we loop while we are still in the bounds of the source and the current character is alphabetic. You could also check for underscores if that is part of your syntax.

```
while (Current < Source.Length && char.IsLetter(Source[Current]))
    Current++;
```

Now we can slice a section of the string out using the start variable and the ‘Current’ value.

```
var value = Source[start..Current];
```

Finally, we can return a new token with the extracted value:

```
return new(value, TokenType.Identifier);
```

Let’s check this is parsing the source code correctly with a simple method to output the token list.

```
private void PrintLexer() {
    Console.WriteLine($"Source: {Source}");
    Console.WriteLine("\nTokens:");
    foreach (var token in Tokens) {
        Console.WriteLine($"\t{token.Type}: {token.Value}");
    }
}
```

```
  // at the end of the 'Tokenize' method

PrintLexer();
return Tokens;
```

Here we can see it has successfully parsed the raw string into more meaningful tokens.

```
Source: this is a test

Tokens:
    Identifier: this
    Identifier: is
    Identifier: a
    Identifier: test
```

Currently, the lexer is parsing the tokens with the Identifier type, even if it is a keyword such as ‘push’ or ‘pop’. To amend this, we can add a new value to the TokenType enum for both of those instructions. The Lexer then needs some way of figuring out whether or not an identifier is a keyword. We can use a dictionary with the key as the name of the token, and the value as the type associated with it.

```
private readonly Dictionary<string, TokenType> Keywords = new() {
    ["push"] = TokenType.Push,
    ["pop"] = TokenType.Pop,
};
```

In the ‘ParseIdentifier’ method, we can now check if the dictionary contains the identifier string, and if so, return a new token with that type.

```
if (Keywords.TryGetValue(value, out var keywordType)) {
    return new(value, keywordType);
}
```

Parsing a numeric token is the exact same as parsing an identifier, except we loop while the current character is a digit, rather than a letter.

```
private Token ParseNumeric() {
    int start = Current;
    while (Current < Source.Length && char.IsDigit(Source[Current]))
        Current++;

    var value = Source[start..Current];
    return new(value, TokenType.Numeric);
}
```

We also need to update the ParseToken method and add the following so the lexer knows when to parse a numeric token:

```
else if (char.IsDigit(Source[Current])) {
    return ParseNumeric();
}
```

Let's run our program with the following code and review the output:

```
push 10
pop
test
```

As you can see below, it correctly parses the 'push' and 'pop' as keywords, the 'test' as an identifier, and the 10 as a numeric.

```
Tokens:
    Push: push
    Numeric: 10
    Pop: pop
    Identifier: test
```

One last thing we need to do to aid the execution of the tokens is to provide a terminator token so the interpreter knows when to stop executing. We can add a new 'Eof' value to the TokenType enum and then push a token at the end of the Tokenize method.

```
Tokens.Add(new("", TokenType.Eof));
```

We have successfully completed the first step of translating the source code into a high-level representation suitable for use by the program's execution component. With this foundation in place, we can now proceed to develop the core of the interpreter.