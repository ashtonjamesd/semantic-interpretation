## Tokenization

We will have a similar, albeit slightly more complex, implementation of the Lexer from the stack machine.

```
public class Tokenizer {
    private readonly string Source;
    private readonly List<Token> Tokens = new();
    private int Current = 0;

    public Tokenizer(string source) {
        Source = source;
    }

    public List<Token> Tokenize() {
        while (!IsEnd()) {
            

            Current++;
        }

        return Tokens;
    }

    private bool IsEnd() {
        return Current >= Source.Length;
    }
}
```

The identifiers will be parsed the same way, however, we want to allow for some additional data types in our interpreter such as floating point numbers and strings.

For parsing strings we keep most of the identifier logic except we account for the double quote at the beginning and end of the string. We advance past the initial double quote before we start the loop, then loop until we find the terminating double quote.

We then substring from the start to the current pointer. If we have reached the end then we add an offset of '0', otherwise we add '1' to account for the final double quote.

```
private Token ParseString() {
    int start = Current;

    Current++;
    while (!IsEnd() && Source[Current] is not '\"')
        Current++;

    var lexeme = Source[start..(Current + (IsEnd() ? 0 : 1))];

    if (lexeme[^1] is not '\"') {
        return Error("unterminated string literal");
    }

    return new(lexeme, TokenKind.String, CurrentLine);
}
```

Next, we want to add support for floating point numbers. This is easy enough as we just need to account for an single optional dot in the token.

We continue to loop as long as a digit or '.' is encountered. Inside the loop, we handle the logic for validating the format of the number.

```
bool hasDecimal = false;

while (!IsEnd() && (char.IsDigit(Source[Current]) || Source[Current] is '.')) {
    if (Source[Current] is '.') {
        if (hasDecimal) {
            return Error("invalid format for numeric literal");
        }

        hasDecimal = true;
    }

    Current++;
}
```

Our language will also support the char data type. To tokenize a char literal, we check if the literal is empty, which is not allowed. Lastly, we verify the char has a closing single quote all while checking the bounds of the current pointer.

```
private Token ParseChar() {
    Current++;
    if (IsEnd() || Source[Current] is '\'') {
        return Error("empty char literal");
    }

    var lexeme = Source[Current++].ToString();
    
    if (IsEnd() || Source[Current++] is not '\'') {
        return Error("invalid char literal");
    }

    return NewToken(lexeme, TokenKind.Char);
}
```

As you can see, the tokenization process works very similarly to the one in the stack machine. The only difference is the increase in token types, such as char and string which each require special methods to parse.