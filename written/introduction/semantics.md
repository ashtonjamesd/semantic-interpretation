## Semantics

While syntax is about the structure and grammar of the language, semantics is concerned with the meaning behind the language and what they actually signify in context.

Semantics are all about understanding and validating the behaviour of the program based on the combinations of tokens being executed. For instance, consider the following C statement. It is syntactically valid, but does it still make sense when put in context? It follows the correct rules for declaring a variable and assigning a value, however, it has semantic errors as x is an integer and therefore cannot be assigned to a string.


```
int x = "Hello, World!";
```

In simple terms, syntax tells you how to write the program, and semantics tells you if the program makes sense when put in context.