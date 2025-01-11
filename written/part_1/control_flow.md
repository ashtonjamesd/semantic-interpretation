## Control Flow

The stack machine is not very useful at this point as it lacks the core concepts when dealing with written logic: conditionals, and loops. We won’t be adding conditionals and loops in the conventional way you are familiar with, but rather with ‘jump’ statements and labels.

Conditional jump statements are a type of jump statement that will execute only when a certain condition is true. 

The syntax for thisinstructions will be as follows:

Jump <label_name> when <numeric>

The interpreter will only jump to the label specified if the numeric value is present at the top of the stack. The interpreter will also support a default, non-conditional jump statement that is just ‘jump’ followed by the label_name.

We can start by adding the ‘Jump’ statement, and then work on the conditionals afterwards. Add a new value in the TokenType enum and in the Lexer Keywords dictionary.

ENUM

Since the label is not a keyword, it does not get added to the Keywords dictionary. Instead, we alter the ‘ParseIdentifier’ method to have a special case for when the keyword for a token is not present. We check if the token after the identifier is a colon character. If so, then we tokenize it as a valid label definition.

IF == COLON PARSEIDENTIFIER

Next we need to add the implementation for the ‘Jump’ instruction in the ‘Machine’ class. First we need to find the label the code is trying to jump to. We do this by taking the lexeme of the token after the ‘Jump’ keyword and attempting to find a matching label token in the ‘Tokens’ List. We then set the index of  the instruction pointer to the index of the matching label.

EXECUTE JUMP CODE

Let’s try creating a loop in our program to test if it works.

STACK_CODE LOOP

OUTPUT

The interpreter is working pretty nicely, however the code has been left in a purposefully vulnerable state and attention needs to be drawn to how it handles exceptions.
