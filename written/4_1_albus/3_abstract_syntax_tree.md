## Abstract Syntax Tree

The next step in building an interpreter after completing the lexer is constructing the Abstract Syntax Tree (AST). An AST is a tree-like data structure that represents the source code in a way that is easy to traverse and  manipulate.

It abstracts away the raw tokens and syntax and instead creates more a more meaningful representation of your program with more expressive components such as statements and declarations.

Let's look at an example. The following code snippet is a simple variable declaration.

```
let x = 42;
```

The generated AST structure may look like this:

```
VariableDeclaration
  |- Identifier: x
  |- Literal: 42
```

The AST removes the redundant parts of the variable declaration such as the equals and semicolon symbols.