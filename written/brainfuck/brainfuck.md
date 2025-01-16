## An Esoteric Interpreter

The word esoteric refers to something that is only understood by a small group of people with specialised knowledge. This accurately describes the language that is Brainfuck.


Brainfuck is an esoteric programming language created in 1993 by Urban Muller. It is renowned for being extremely minimalist and consists of only 8 basic commands. The language operates with a very basic memory model - a linear tape of cells, each cell initially set to zero, and a data pointer for traversing the tape.

The eight commands in Brainfuck are:

1. `>` - Move the data pointer one cell to the right.
2. `<` - Move the data pointer one cell to the left.
3. `+` - Increment the value at the current cell.
4. `-` - Decrement the value at the current cell.
5. `[` - If the value at the data pointer is zero, jump to the command after the matching ']'
6. `]` - If the value at the data pointer is not zero, jump back to the command after the matching ']'
7. `.` - Output the value at the data pointer
8. `,` - Accept input and store at the current data pointer position