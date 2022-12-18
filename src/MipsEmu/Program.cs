using System;

namespace MipsEmu;

using MipsEmu.Assembler;

public sealed class Program {

    public static void Main(string[] args) {
        var text = "main:\n\tjr $ra";
        text = "main: addi $t0, $t0, 43 jr $ra";
        // text = ".data values:.byte 5,4,3, 2, 1 beep: .word 16.text .globl main main: addi $t0, $t0, 43 add $t0, $t0, $t1 sub $t4, $s0, $t1 jr $ra";
        
        var syntaxAnalyzer = SyntaxAnalyzer.CreateDefaultSyntaxAnalyzer();
        var assembler = new ProgramLinker(syntaxAnalyzer);
        var unlinked = assembler.Parse(new string[]{text});
        var program = assembler.Link(unlinked);

        var emulated = new MipsProgram(0x20000000);
        emulated.LoadProgram(program.text, program.data);
        emulated.Cycle();

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

    }
}