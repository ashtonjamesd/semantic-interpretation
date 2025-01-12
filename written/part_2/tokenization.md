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

For parsing strings we keep most of the identifier logic except we account for the double quote and end double quote 