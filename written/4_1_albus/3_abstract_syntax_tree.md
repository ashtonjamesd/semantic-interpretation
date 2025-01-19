## Abstract Syntax Tree

After completing the lexer, the next step in building an interpreter is constructing the Abstract Syntax Tree (AST). An AST is a tree-like data structure that represents the source code in a way that is easy to traverse and  manipulate.

It abstracts away the raw tokens and syntax and instead creates a more meaningful representation of your program with more expressive components such as statements and declarations.

Let's look at an example. Below is a simple variable declaration.

```
let x = 42;
```

The generated AST statement structure may look like this:

```
VariableDeclaration
  |- Identifier: x
  |- Literal: 42
```

This representation strips away irrelevant syntax such as the equals sign and semicolon, keeping only the meaningful components of the expression. The result is a concise, abstract structure that captures the essence of the variable declaration.

With this simplified structure, the interpreter can easily work with the core components of the declaration - namely, the variable identifier and the assigned value - without dealing with extraneous symbols.

Each expression in the AST will typically be represented by a class or struct, depending on the programming language or design preferences. Let's start by defining a literal expression class for our language. A literal expression is something that represents a fixed value in our code such as a string or number.

```
public class LiteralExpression {
    public readonly object Value;

    public LiteralExpression(object value) {
        Value = value;
    }

    public override string ToString() {
        return Value.ToString() ?? "null";
    }
}
```

We also provide a `ToString` implementation for debugging purposes.

The literal expression constructor takes in a value of type `object` as it could be a multitude of types, such as a char literal or a numeric literal.

We are also going to create a base class that serves as a parent for all types of expressions in our language. The reason we are structuring our classes this way is to allow a single list to be able to hold different types of derived expressions. This makes it much easier to manage the statements and expressions in our parser.

Rather than returning a list of expressions directly, we are going to wrap it inside of a class which will represent our program as a single cohesive entity. This code is much more flexible this way as if we wanted to include additional information from the parser in the future - such as metadata or debugging details - we can simply add new properties to the `Ast` class without disrupting existing code in the parser.

```
public abstract class Expression {}

public class Ast {
    public List<Expression> Body = [];
}
```

The parser will follow a similar structure to the lexer, consisting of a current index variable that tracks the position of the current token, a list of tokens to iterate through, and an AST object that will be constructed and returned once parsing is complete.

```
public class Parser {
    private readonly List<Token> Tokens;
    private int Current;
    private readonly Ast Ast = new();

    public Parser(List<Token> tokens) {
        Tokens = tokens;
    }

    public Ast ParseAst() {
        // ...

        return Ast;
    }
}
```

In our parser, each expression will have a dedicated method to parse and return its corresponding type of expression. For instance, `ParseVariableDeclaration`, and `ParseLiteral`, etc.

We can add the following code in our main method for the parser.

```
var parser = new Parser(tokens);
var ast = parser.ParseAst();
if (parser.HasError) {
    return;
}

foreach (var expr in ast.Body) {
    Console.WriteLine($"Expr: {expr}");
}
```

The method below demonstrates how we parse primary expressions - the most basic building block of expressions and statements. Primary expressions include literals such as strings, integers, characters, and booleans. They are what you would call 'leaf nodes' as they are at the bottom of the AST and cannot be decomposed further.

We simply check the token type, parse it into the appropriate C# data type, and return a new literal expression with the value of the token.

```
private Expression ParsePrimary() {
    var expr = Tokens[Current];

    if (expr.Type is TokenType.String) {
        return new LiteralExpression(expr.Lexeme);
    }
    else if (expr.Type is TokenType.Integer) {
        return new LiteralExpression(int.Parse(expr.Lexeme));
    }
    else if (expr.Type is TokenType.Char) {
        return new LiteralExpression(char.Parse(expr.Lexeme));
    }
    else if (expr.Type is TokenType.False or TokenType.True) {
        return new LiteralExpression(bool.Parse(expr.Lexeme));
    }
    else if (expr.Type is TokenType.Identifier) {
        return new LiteralExpression(expr.Lexeme);
    }
    else {
        return new BadExpression(expr.Lexeme);
    }
}
```

The above code is a common pattern you may see in programming where a chain of `if-else` statements each return a value. However, this pattern can be simplified and made more declarative using a `switch` expression, as shown below. This generally makes the code look a little more expressive and is easier to extend. 

I would argue that pattern matching is one of the best features that modern programming offers as it aligns closely with how we naturally process language. Allowing code to express logic in ways that resemble the structure of everyday language reduces the complexity of traditional syntax and reads much easier.

We also introduce a `BadExpression` which will be returned if the parser encounters an error at any point.

```
return expr.Type switch {
    TokenType.String => new LiteralExpression(expr.Lexeme),
    TokenType.Integer => new LiteralExpression(int.Parse(expr.Lexeme)),
    TokenType.Char => new LiteralExpression(char.Parse(expr.Lexeme)),
    TokenType.True or TokenType.False => new LiteralExpression(bool.Parse(expr.Lexeme)),
    TokenType.Identifier => new LiteralExpression(expr.Lexeme),
    _ => new BadExpression(expr.Lexeme)
};
```

Since the primary expression is not the only expression our parser will construct, we define a `ParseExpression` method which will decide which expression it will attempt to parse. For now, it will just call the primary expression method.

```
private Expression ParseExpression() {
    return ParsePrimary();
}
```

Our main parser method will be updated with the following code.

```
while (!IsLastToken() && Tokens[Current].Type is not TokenType.Eof) {
    var expr = ParseExpression();
    Ast.Body.Add(expr);

    if (expr is BadExpression) {
        return Ast;
    }

    Current++;
}

return Ast;
```

We loop until we encounter the end-of-file token, in which case the program will return the Ast parsed thus far. After each iteration, we check if the expression parsed is 'bad' and if so we terminate the parsing early.

We also make use of the following helper method to ensure that we do not attempt to access a token outside the bounds of the token list.

```
private bool IsLastToken() {
    return Current >= Tokens.Count;
}
```

<br/>

Next, we're going to parse the first statement in our language: the variable declaration. 

In Albus, variables are declared using the 'let' keyword, followed by an identifier, an equals sign to assign a value, and finally, the value itself. The statement ends with a semicolon. Here’s an example of a variable declaration in Albus:

```
let x = 10;
```

We will start by creating a new method for parsing variable declarations.

```
private Expression ParseVariableDeclaration() {
    // this statement will be replaced later as we know it is already a 'let' token
    if (!Expect(TokenType.Let)) {
        return new BadExpression();
    }

    // ...
}
```

We are again going to employ another helper method that will check the type of the current token. This method offers a graceful way to enforce the syntax of our language and alert the programmer of any syntactical errors. We will call this method `Expect` as we are expecting a token with a particular type to appear at that point.

Before accessing the current token, we check if we are currently within the bounds of the token list. If so, we perform the comparison, otherwise we return the error (false). If it is a match, we increment `Current` as we no longer need this token as it is there to simply enforce the syntax of our language. We are also going to pass in a string to be displayed in the expected error output.

```
private bool Expect(TokenType type, String value) {
    if (IsLastToken() || Tokens[Current].Type != type) {
        return ParseError($"expected {value}");
    }

    Current++;
    return true;
}
```

Now we are going to implement the `ParseError` method to provide a nice way of handling parser errors.

The method will retrieve the current token, or the previous token if the parser is at the end of the token list. It will then output the error message along with the line it occurred on.

```
private bool ParseError(string message) {
    HasError = true;

    var currentToken = Tokens[IsLastToken() ? Current - 1 : Current];
    Console.WriteLine($"parsing error: {message} on line {currentToken.Line}");

    return false;
}
```

To track any errors that have occurred in the parser, we introduce the following flag. The flag is public as we will check its state from outside the parser once the parsing has terminated.

```
public bool HasError { get; private set; }
```

Currently, to report an error, we would have to call the `ParseError` method with an appropriate message and then return a bad expression. A given error scenario may look something like this:

```
if (errorHasOccurred) {
    ParseError("Error has occurred");
    return new BadExpression();
}
```

Since this is going to be quite a common pattern in our code, we can abstract away the `ParseError` method into a new method that will return an expression while also taking in a error message and calling `ParseError` for us.

This will allow us to transform the snippet above into the following.

```
if (errorHasOccurred) {
    return ExpressionError("Error has occurred");
}
```

This is much cleaner, easier to read, and removes the overall amount of code we are writing, while still keeping the robust error handling functionality.

The implementation for this method is as follows.

```
private BadExpression ExpressionError(string? message = null) {
    if (message is not null) {
        ParseError(message);
    }

    HasError = true;
    return new BadExpression();
}
```

We pass in the message as normal, however we make it nullable and provide a default parameter. The reason for this is because an error may have already been reported but we still want to use this method to return an error without raising another message in the output.

Take the following snippet as an example.

```
if (!Expect(TokenType.SingleEquals, "=")) {
    return ExpressionError();
}
```

Assume the `Expect` method returns false. An error message will be outputted for this. We then should not have to provide `ExpressionError` with an error message as it has already been handled.

<br/>

Lets return to parsing the variable declaration. We can update the snippet to call our new error reporting method instead of returning a new bad expression instance directly.

```
private Expression ParseVariableDeclaration() {
    Current++; // we already know it is a 'Let' token, so we simply advance past it

    var identifier = Tokens[Current].Lexeme;
    if (!Expect(TokenType.Identifier, "identifier after 'let'")) {
        return ExpressionError();
    }

    return new BadExpression(); // temporary return
}
```

To test the error reporting, lets run the program with the input source below:

```
let
```

This should raise a parser error telling the user that an identifier was expected after the let.

```
parsing error: expected identifier after 'let' on line 1
```

As expected, the parser recognises that the token found after the 'let' was not the token expected when parsing a variable declaration. Now lets implement the rest of the variable parsing logic.

After the identifier has been advanced past, we expect to find a single equals symbol.

```
if (!Expect(TokenType.SingleEquals, "'=' after identifier")) {
    return ExpressionError();
}
```

Next, we parse the actual expression that the variable has been assigned to.

```
var value = ParseExpression();
if (HasError) {
    return ExpressionError("expected expression in variable declaration");
}
```

We perform a check on the `HasError` flag as something could have gone wrong in the `ParseExpression` method. Specifically when we are parsing a primary expression and the expression is not a valid primary expression.

For instance, the following input:

```
let x = ;;
```

Will result in the following error:

```
parsing error: expected expression in variable declaration on line 1
```

Finally, we ensure the declaration ends with a semicolon and return a new instance of a variable declaration.

```
if (!Expect(TokenType.SemiColon, "';' after expression")) {
    return ExpressionError();
}

Current--; // we decrement this as it will be incremented anyway in the main while loop
return new VariableDeclaration(identifier, value);
```

To ensure that the parser attempts to parse the correct statement we need to create a method that directs the parsing logic based on the type of the token currently being examined. This method acts as a 'router', determining how to handle each token it encounters.

If it finds a `Let` token, we attempt to parse a variable declaration, which is currently the only valid statement in our language. There will be others, but for now, it will attempt to parse an expression if any other tokens are encountered.

The top level method `ParseAst` will now call `ParseStatement` instead of `ParseExpression` in the while loop.

To implement this logic, we modify the top-level parsing method to call a new method, ParseStatement, in its while loop. This ensures that the parser correctly delegates token handling to the appropriate method. The reason we are choosing to implement a routing method like this is because it is easily extendible and gives a clear overview of which tokens map to what statements and expressions.

```
private Expression ParseStatement() {
    return Tokens[Current].Type switch {
        TokenType.Let => ParseVariableDeclaration(),
        _ => ParseExpression()
    };
}
```

Now, lets try out our parser with the following input:

```
let x = "Hello, World!";
```

This results in the following expression;

```
Expr: x = "Hello, World!"
```

We are now going to revisit the concept of precedence, which you may recall from the expression evaluator section. The only difference this time is that rather than creating the expressions and then evaluating them, we are just creating them. The evaluation will be done by the interpreter rather than the parser. 

We'll modify our parser by introducing the `ParseTerm` method for handling binary expressions. This method will handle additive operators such as plus and minus as they of the same precedence level.

Hopefully you can recognise some of the code from the method below. We attempt to parse a number first, then continue to parse primary expressions as long as the current operator is a plus or minus. Finally, we return the generated binary expression.

```
private Expression ParseTerm() {
    var left = ParsePrimary();

    while (Match(TokenType.Plus) || Match(TokenType.Minus)) {
        Token op = Tokens[Current++];
        var right = ParsePrimary();
        left = new BinaryExpression(left, op, right);
    }

    return left;
}
```

Similarly, the implementation for multiplicative expressions is the same except we check for the star and slash tokens.

```
private Expression ParseFactor() {
    var left = ParsePrimary();

    while (Match(TokenType.Star) || Match(TokenType.Slash) || Match(TokenType.Modulo)) {
        Token op = Tokens[Current++];
        var right = ParsePrimary();
        left = new BinaryExpression(left, op, right);
    }

    return left;
}
```

We can also update the `ToString` implementation for our binary expression and add parentheses around the expression to show us the levels of precedence.

```
public override string ToString() {
    return $"({Left} {Operator.Lexeme} {Right})";
}
```

Let's test the precedence with the following expression.

```
let x = 2 + 4 - 4 * 32 + 1 * 2;
```

Outputs the following.

```
Expr: x = (((2 + 4) - (4 * 32)) + (1 * 2))
```

As you can see, the brackets show us the actual precedence of the expressions, with the multiplicative expressions being grouped separately.


Now we want our parser to recognise logical expressions involving `and` and `or` tokens. These are slightly simpler to implement as they involve checking the one token type. In boolean logic, logical `and` takes precedence over logical `or`, meaning that the method parsing `or` expressions will call the method parsing `and` expressions to enforce this precedence.

Here is the implementation for the logical `and` parsing.

```
private Expression ParseLogicalAnd() {
    var left = ParseTerm();

    while (Match(TokenType.And)) {
        Token op = Tokens[Current++];
        var right = ParseTerm();
        left = new BinaryExpression(left, op, right);
    }

    return left;
}
```

Likewise, logical `or`:

```
private Expression ParseLogicalOr() {
    var left = ParseLogicalAnd();

    while (Match(TokenType.Or)) {
        Token op = Tokens[Current++];
        var right = ParseLogicalAnd();
        left = new BinaryExpression(left, op, right);
    }

    return left;
}
```

Logical or takes precedence over logical and mainly because this is how it works in modern programming languages. This stems from C++, which stemmed from C, which stemmed from mathematics.

Take the following expression:

```
let x = 2 == 2 and true;
```

You would expect this particular expression to be evaluated with the following precedence:

```
Expr: x = ((2 == 2) and True)
```

However, if we place logical and precedence over equality, we would get this:

```
Expr: x = (2 == (2 and True))
```

The first expression is a lot more natural and makes more sense to the programmer.

Next, lets implement the equality expression parsing.

```
private Expression ParseEquality() {
    var left = ParseComparison();

    while (Match(TokenType.DoubleEquals) || Match(TokenType.NotEquals)) {
        Token op = Tokens[Current++];
        var right = ParseComparison();
        left = new BinaryExpression(left, op, right);
    }

    return left;
}
```

Can you see the pattern in all of these? Once you can visualise and comprehend the recursive nature of the methods, it starts to all fits together.

The next type of binary expression we will parse is the comparison expression. This involves the four greater/less than operators and will take precedence over equality expressions.

```
private Expression ParseComparison() {
    var left = ParseTerm();

    while (Match(TokenType.GreaterThan) || Match(TokenType.LessThan) || 
            Match(TokenType.GreaterThanEquals) || Match(TokenType.LessThanEquals)) {
        Token op = Tokens[Current++];
        var right = ParseTerm();
        left = new BinaryExpression(left, op, right);
    }

    return left;
}
```


Lets test it with a group of expression

```
let x = 2 > 4 + 1 or 2 == 4 and 2 != 2;
```

We can see it has correctly parsed it in accordance with the precedence defined.

```
Expr: x = ((2 > (4 + 1)) or ((2 == 4) and (2 != 2)))
```