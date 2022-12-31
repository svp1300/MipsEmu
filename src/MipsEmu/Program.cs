using System;

namespace MipsEmu;

using MipsEmu.Assembler;

using MipsEmu.Emulation.Instructions;

using MipsEmu.Assembler.Tokens;

public sealed class Program {

    public static void Main(string[] args) {
        // text = 
        // text = ".data values:.byte 5,4,3, 2, 1 beep: .word 16.text .globl main main: addi $t0, $t0, 43 add $t0, $t0, $t1 sub $t4, $s0, $t1 jr $ra";
        // text = ".globl main main: add $t0, $t0, $t1 add $t0, $t0, $t1 add $t0, $t0, $t1 lw $s0, 0($t0)";
        var text = ".globl main main: sub $t0, $t0, 43 jr $ra";
        var syntaxAnalyzer = SyntaxAnalyzer.CreateDefaultSyntaxAnalyzer();
        var pseudoExpander = PseudoInstructionExpander.CreateDefaultPseudoExpander();
        var assembler = new ProgramLinker(syntaxAnalyzer, pseudoExpander);
        var unlinked = assembler.Parse(new string[]{text});
        var program = assembler.Link(unlinked);
        Console.WriteLine("~~~Program~~~\n" + unlinked);
        var emulated = new MipsProgram(0x20000000);
        emulated.LoadProgram(program.text, program.data);
        emulated.RunProgram();

        if (args.Length == 0) {
            Console.WriteLine("At least one parameter is required. (run, debug, assemble)");
        } else {
            switch (args[0].ToLower()) {
                case "run":
                    RunProgram(args);
                    break;  
                case "debug":
                    DebugProgram(args);
                    break;
                case "assemble":
                    AssembleProgram(args);
                    break;
                default:
                    Console.WriteLine("Unrecognized option. Choices: run, debug, assemble.");
                    break;
            }
        }
    }

    public static void RunProgram(string[] args) {
        
    }

    public static void DebugProgram(string[] args) {

    }

    public static void AssembleProgram(string[] args) {
        string programPath;
        if (args.Length == 2) {
            programPath = "./";
        } else if (args.Length > 2) {
            
        }
    }
}