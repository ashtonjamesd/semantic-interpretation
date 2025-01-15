## Evaluating Expression

To create a mathematical expression evaluator, we first need to understand the concepts of precedence and the order of operations.

The order of operations is a set of rules that determines the sequence in which mathematical operations should be performed in an expression. Each operation is assigned a rank of precedence, which dictates its priority relative to other operations. Operators that have lower precedence are executed after those with higher precedence.

You may have heard of the acronym 'PEMDAS', which stands for Parentheses, Exponents, Multiplication, Division, Addition, and Subtraction. It describes the order of operations in a way that is easy to remember.

We are going to model the precedence of our expression evaluator with an algorithm called recursive descent parsing, in which each level of operator precedence will have a separate recursive method. In recursive descent parsing, the lower precedence methods call the higher precedence methods so that they are executed first, therefore taking precedence.

It can help to visualise precedence using parentheses that are implicitly placed around the expression. For instance, the expression `2 + 2 * 4` is evaluated as `2 + (2 * 4)`, as the `2 * 4` is evaluated first, and then added with the `2`.

```
2 + 2 * 4  // 10
```

However, we can insert brackets around the additive expression to increase the precedence it has over the multiplicative expression.

```
(2 + 2) * 4  // 16
```

Likewise, the following expression is evaluated with these implicit parentheses:

```
2 - (3 * 4) + 2 + (4 * 2)  // 0
```

<br/>

With a good grasp on mathematical precedence and the order of operations, we can begin to write the string parser for our expression evaluator. To be able to evaluate the string input, we first need to parse it into tokens.

For instance, given this input:

```
754 + 23
```

We need to extract the following tokens:

```
(754) (+) (23)
```

Our evaluator also needs to correctly identify and parse numbers, regardless of their format. Numbers in our evaluator can be:


1. Whole numbers
```
754, 23, 0
```

<br/>

2. Decimal Numbers
```
3.14, 0.5, 123.456
```

<br/>

3. Negative numbers
```
-42 -3.14
```

To correctly handle all of these cases, we need a strategy to parse the numbers from the input string. We will start by creating a class to encapsulate the behavior of the evaluator and a method for parsing the number tokens.

The source will be provided through the constructor and will remain immutable thereafter. Additionally, all whitespace in the source string will be removed to simplify the parsing process.

```
public class Evaluator {
    private readonly string Source;
    private int Current = 0;

    public Evaluator(string source) {
        Source = source.Replace(" ", "");
    }

    public double Parse() {
        return 0;
    }
}
```

The `Source` field contains the input expression to be evaluated, and the `Current` variable serves as a pointer for iterating through each character in this string.

We can start the tokenization by parsing the numbers within the expression. To do this, we begin at the first digit and continue iterating until we encounter a non-numeric character. We also ensure that the pointer is within the bounds of the string to avoid index-out-of-range errors.

Once the number has been traversed, we can slice a section of the input from the starting index to the current pointer position. We then convert this substring into a numerical value. The implementation for this is below:

```
private double ParseNumeric() {
    var start = Current;

    while (Current < Source.Length) {
        if (char.IsDigit(Source[Current])) {
            Current++;
        } else break;
    }

    var val = Source[start..Current];
    return double.Parse(val);
}
```

Let's test it with the following input:

```
var evaluator = new Evaluator("754");

var result = evaluator.Parse();
Console.WriteLine(result); // 754
```

<br/>

Great, now we can add functionality for decimal numbers. This change is relatively simple and involves only accounting for an optional dot character in the number.

Since we only allow for one dot in any given number, we can declare a flag to keep track of whether the decimal point has been encountered yet:

```
bool hasDecimal = false;
```

We can adjust the inside of the loop to check if the current character is a dot, if so, we check if the flag has already been set to true, in which an error will be thrown. Otherwise, we set the flag to true and continue to parse the number.

```
char c = Source[Current];

if (char.IsDigit(c)) {
    Current++;
} else if (c is '.' && !hasDecimal) {
    Current++;
    hasDecimal = true;
} else break;
```

Next, we want to be able to parse basic additive expressions with plus and minus operations. We can create another method called `ParseTerm` which will handle the parsing for us.

We first attempt to parse a numeric token, as that is how all expressions will begin. Then, as the parser continues to encounter plus and minus operators, we parse the next token in the expression. We then cumulatively add the result to `left`.

```
private double ParseTerm() {
    var left = ParseNumeric();

    while (Match('+') || Match('-')) {
        char op = Source[Current - 1];
        var right = ParseNumeric();
        left = op == '+' ? left + right : left - right;
    }

    return left;
}
```

We also need to adjust the `Parse` method to call `ParseTerm` rather than `ParseNumeric`.

```
public double Parse() {
    return ParseTerm();
}
```

Let's test our expression parser so far with the following input:

```
654.3 + 32 - 84 + 2.1
```

And we correctly get `604.4`.

Now we want to add functionality for multiplicative operations such as times and divide. For this, we can create a new method called `ParseFactor` and handle the logic in there.

```
private double ParseFactor() {
    var left = ParseNumeric();

    while (Match('*') || Match('/')) {
        char op = Source[Current - 1];
        var right = ParseNumeric();
        left = op == '*' ? left * right : left / right;
    }

    return left;
}
```

We do the exact same thing, except we check for the star and slash symbols and call the `ParseNumeric` method. `ParseTerm` will now call the `ParseFactor` method instead of `ParseNumeric` to account for the precedence in the expression.

Brackets can also be nested within an expression, changing the order of precedence temporarily. This is fairly simple to implement and involves checking if a numeric is surrounded by brackets before it is parsed as a number. `ParseFactor` will now call the `ParseGroup` method.

```
private double ParseGroup() {
    if (Match('(')) {
        var val = ParseTerm();
        if (!Match(')')) {
            throw new Exception("expected ')'");
        }

        return val;
    }

    return ParseNumeric();
}
```

First, we check if there is an open parenthesis character, indicating the beginning of a grouping expression. We then evaluate the expression inside of the grouping by calling `ParseTerm`. We also check for a closing parenthesis to complete the expression. If the bracket has not been closed, then we throw an error as it will not correctly parse the expression. If there are no parentheses we simply continue to parse and return a numeric token as normal.

Negative numbers are straightforward to parse since they are simply preceded by a `-` symbol. We can adjust the `ParseGroup` method to check for this symbol, in which case we negate and return the result of parsing a group expression.

```
private double ParseGroup() {
    // ...

    if (Match('-')) {
        return -ParseGroup();
    }
    
    return ParseNumeric();
}
```

And that's it. We have successfully created a mathematical expression evaluator that can handle the four basic operators, along with manual precedence, decimal numbers, negative numbers, and has whitespace tolerance. 

Why don't you experiment and use what you have learnt to implement exponentiation in the evaluator. For instance:

```
2 + 2^4 * 2 // 34

  // the precedence is as follows:
  // 2 + ((2^4) * 2)
```

Next, we're going to look more closely at lexical analysis and write our own simplified language interpreter.