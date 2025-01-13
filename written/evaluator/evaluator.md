Let's start with the simplest part of the expression evaluator - parsing the actual numbers inside of the expression.

To be able to evaluate the string expression, we first need to parse the expression into tokens.

For instance, given this input:

```
754 + 23
```

We need to extract the following tokens:

```
(754) (+) (23)
```

Our evaluator needs to correctly identify and parse numbers, regardless of their format. Numbers in our evaluator can be:


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

To correctly handle all of these cases, we need a strategy to parse the numbers from the input string.

We will start by creating a class to encapsulate the behavior of the evaluator and a method for parsing the number tokens.


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

The `Source` field holds the input expression passed to the evaluator. We will loop through each character in this string using `Current` as a pointer.

To successfully tokenize a number, we start at the first digit and keep looping until we encounter a non-numeric character. We also need to make sure that the value of the pointer is less than the length of the string to prevent index out of range errors.

Once the number has been traversed, we can slice a section of the input, from the index of the first character to the current pointer value.

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
var evaluator = new Evaluator("754);

var result = evaluator.Parse();
Console.WriteLine(result); // 754
```

Great, now we can add functionality for decimal numbers. This is a relatively simple change and involves only accounting for an optional singular dot character in the number.

We can declare a flag to keep track of whether the decimal point has been encountered yet:

```
bool hasDecimal = false;
```

We check if the current character is a dot, if so, we check if the flag has already been set to true, in which an error will be thrown. Otherwise, we set the flag to true and continue to parse the number.

We can adjust the inside of the loop with the following:

```
char c = Source[Current];

if (char.IsDigit(c)) {
    Current++;
} else if (c == '.' && !hasDecimal) {
    hasDecimal = true;
    Current++;
} else {
    break;
}
```

Next, we want to be able to parse basic additive expressions with plus and minus operations. We can create another method called `ParseTerm` which will handle the parsing for us.

We first attempt to parse a number, as that is how all expressions will begin. Then, as the parser continues to encounter plus and minus operators, we parse the next token in the expression. We then accumulatively add the result to `left`.

```
private double ParseTerm() {
    var left = ParseNumeric();

    while (Match('+') || Match('-')) {
        char op = Source[Current - 1];
        var right = ParseNumeric();
        Console.WriteLine(right);
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

Now we want to add support for multiplicative operations such as times and divide.

For this, we can create a new method called 'ParseFactor' and handle the logic in there.

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

We do the exact same thing, except we check for the times and divide operators and call the `ParseNumeric` method.

`ParseTerm` will now call the `ParseFactor` method instead of `ParseNumeric`.