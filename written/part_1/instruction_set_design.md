The first step that takes place when creating a language is the designing of the syntax. This refers to a set of rules defined in a language that dictate what is valid source code and what is not. A uniform method for representing language grammar is Backus Naur Form (BNF), however, the syntax of this language is simple enough that we can create a simplified representation of the instruction set with no particular notation.

The initial instructions defined in our stack machine are as follows:

```
push <value>  // pushes a value to the top of the stack
pop           // pops the value off the top of the stack
print         // prints the value at the top of the stack

add           // adds the two values at the top of the stack and pushes the result
sub           // subtracts the second value from the top of the stack from the top value and pushes the result
jump <label>  // jumps to a defined label in the source code

<label>:      // label definition for control flow
```

The syntax of these instructions dictate the name of the instruction comes first, followed by any arguments the instruction expects to receive.