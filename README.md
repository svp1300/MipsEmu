# MipsEmu

A tool for running, assembling, and debugging MIPS assembly.

Assembling is done as following:
1. Passing strings of MIPS assembly code into the Lexical Analyzer to break the string down into vaguely categorized symbols. Matches are made by choosing the longest of all possible matches.
2. Passing the symbols into the Syntax Analyzer creates a parse tree. The path from the deepest leaf to the root yields a list of Tokens symbolizing various directives and instructions.
3. Passing the tokenized program into the program linker to create runnable machine code. First the symbol table and reference tables are made, so that labels inside Tokens can be replaced with addresses. An array of bits is created and each instruction and directive is inserted into their respective block.
4. The bits containing the text and data blocks are returned.

Running is done as following:
1. The text and data blocks are loaded into emulated memory inside a MipsProgram object.
2. The $sp is set to the starting address.
3. The program cycles until an exit flag is true.
5. The cycle function fetches the instruction, decodes it using the InstructionParser, and executes the instruction's Run method.

Debugging uses a subclass of MipsProgram that queries the user for debugging commands that traverse the program.

The project is under active development and has almost reached the minimum viable product for a release.