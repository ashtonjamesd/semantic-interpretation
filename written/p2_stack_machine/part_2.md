## Part 2 - Stack Machine

We’re going to write a simple interpreter for a very basic instruction set. The interpreter will use a stack data structure for operations like storing and retrieving data, making it an implementation of a stack machine. We will interact with the stack primarily through ‘push’ and ‘pop’ operations.

A stack is an abstract data type that stores a collection of items in a last-in-first-out (LIFO) order. This means that the most recent item added to the stack with a ‘push’ operation is the first one to come off the stack with a ‘pop’ operation. Imagine a pile of plates on top of each other. Reasonably, to get to the bottom of the pile you have to take all the ones above off first. Additionally, a stack also provides a read-only ‘peek’ operation as part of its interface which returns the item at the top of the stack without altering the values.

We will again be using C-Sharp for this project implementation.