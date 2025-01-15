## Syntax

Programming languages have rules that must be followed in order to build a working program in that language. The language may consist of words, numbers, and symbols that must be correctly structured for the program to be considered valid and able to execute.

A language may be written such that certain words are built-in and considered part of the language syntax. Examples of this are the `for` and `while` keywords in C, which both symbolise the start of a loop control flow statement. These are known as reserved words and cannot be used as identifiers (such as variable or function names).

The primary goal of syntax is to enforce a standard procedure of writing code in a specific language. It ensures that code can be understood by the computer and executed without errors. If a programmer writes code that contains invalid characters or something against the language rules (syntactically invalid), then a syntax error will occur, preventing the program from running.

A common example of syntax in languages from the C-family is the semicolon statement terminator. This syntax rule enforces that statements written in the language must end with a semicolon character to inform the C compiler that the statement is finished and anything after is a new statement. Missing semicolons can confuse the compiler, such with the example below.

```
int x = 10; // valid
int y = 10  // syntax error
```

Part of the syntax of a programming language is checked by a special program called a lexer, in a process called lexical analysis. This is the process of breaking down source code into more meaningful units of data known as tokens. Each token will represent a valid piece of syntax in the programming language, such as a keyword, operator, symbol, or identifier. Lexical analysis is typically the first stage in both compilation and interpretation.

Consider the following code snippet.

```
if (isNull) {
    return;
}
```

This will be tokenized into the following: `if` `(` `isNull` `)` `{` `return` `;` `}`. In this example, the `if` and `return` are keywords as they operate with the behaviour and flow of the language itself. These tokens will then be passed to the next stage of the interpretation process - the parser.

While the lexer performs syntax checks, it primarily focuses on identifying where specific characters are expected to appear, whereas the parser is responsible for recognizing and organizing specific tokens based on the grammar of the language. For instance, the following code would likely be considered valid grammar by the lexer, however this expression is very illogical and likely does not follow the grammar of the language.

```
let 1 { 'a' =
```

