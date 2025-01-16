## Instruction Set Design

The first step that takes place when creating a language is to define the grammar. This refers to a set of rules defined in a language that dictate what is valid source code and what is not. A uniform method for representing language grammar is Backus Naur Form (BNF), a notation for describing the syntax and rules of a language. 

BNF is used in compiler and interpreter design as it provides a precise and unambiguous method for defining the grammar of a programming language. BNF is a type of metalanguage - a language used to describe other languages. The rules that are defined with BNF describe how valid strings (statements) can be formed from a set of symbols (such as words, numbers, and operators, etc).

BNF consists of two major components: non-terminal symbols, and terminal symbols (constants). Non-terminal symbols represent the more abstract parts of the language, such as expressions and statements. They are written within enclosed angle brackets, like `<expression>`, `<statement>`, etc. These symbols are not part of the final string in the language, but are instead used to define the structure of valid statements in that language.

In the statement below, the `<expression` and `<term>` symbols are non-terminal syntactic constructs and represent the categories that the terminal symbols fall into.

```
<expression> ::= <term> "+" <expression> | <term>
```

Terminal symbols are usually very basic symbols or literal values of the language. These symbols are unable to be broken down further as they are the builidng blocks of the language itself. For instance, in a given programming language, terminals might be digits, operators, keywords, or other punctuation marks. The notation for writing terminal symbol is within enclosed double quotes or just plain text.

The statement below demonstrates the creation of a non-terminal symbol that can be either "1", "2", or "3".

```
<term> ::= "1" | "2" | "3"

 // '::=' is the 'definition' symbol
```

The instructions defined in our stack machine with BNF are as follows:

```
<instruction> ::= "push" <value>
                | "pop"
                | "print"
                | "read"
                | "add"
                | "sub"
                | "jump" <label>

<value>       ::= <integer>
<label>       ::= <string>

<integer>     ::= <digit> | <digit> <integer>
<digit>       ::= "0" | "1" | ... | "9"

<string>      ::= <char> | <char> <string>
<char>        ::= "a" | "b" | "c" | ... | "z"
```

Let's break down each part of the notation.

The following statement declares a non-terminal that can by any letter from "a" to "z". We obviously don't have to list every single letter in the alphabet, so we can imply that is the case with the "..." symbol.

```
<char>        ::= "a" | "b" | "c" | ... | "z"
```

<br/>

This definition means that the `<string>` non-terminal can be a single `<char>` or a `<char>` followed by `<string>`. This essentially creates a recursive definition where the `<string>` can be expanded again, repeating the process and allowing the construction of strings of arbitrary length. 

```
<string>      ::= <char> | <char> <string>
```

<br/>

We effectively do the same thing here except it is with numbers rather than letters.

```
<integer>     ::= <digit> | <digit> <integer>
<digit>       ::= "0" | "1" | ... | "9"
```

We now need to define what values and labels actually are, as these will be part of the instructions in our stack machine. We do this by introducing customised non-terminals. These are simply non-terminals that have a one-to-one correspondence with other non-terminals, but are given more meaningful names for clarity and context.

```
<value>       ::= <integer>
<label>       ::= <string>
```

<br/>

Lastly, we define the `<instruction>` non-terminal as any one of the following patterns. For instance, an instruction can be "push" followed by a `<value>` non-terminal, which can be any number. Similarly, the "jump" instruction is that keyword followed by a `<label>` non-terminal symbol.

```
<instruction> ::= "push" <value>
                | "pop"
                | "print"
                | "read"
                | "add"
                | "sub"
                | "jump" <label>
```

With the instruction set clearly defined using Backus-Naur Form. These rules ensure that any program written for our language will be syntactically correct and unambiguous. We can now move onto writing the code that will actually parse the source into these instructions through a process called lexical analysis.