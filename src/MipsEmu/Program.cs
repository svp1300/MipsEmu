using System;

namespace MipsEmu;

using MipsEmu.Assembler;

public sealed class Program {

    public static void Main(string[] args) {
        var text = ".data beep:.asciiz \"beep\".text\nmain:\n\tjr $ra";
        text = ".data values:.byte 5,4,3, 2 beep: .word 16.text .globl main main: addi $t0, $t0, 43 add $t0, $t0, $t1";
        // text = ".byte 5,4,3,2";
        var syntaxAnalyzer = new SyntaxAnalyzer();
        syntaxAnalyzer.AddTokenForm(TypeIInstructionToken.FORM, (s) => new TypeIInstructionToken(s));
        syntaxAnalyzer.AddTokenForm(TypeRInstructionToken.FORM, (s) => new TypeRInstructionToken(s));

        syntaxAnalyzer.AddTokenForm(ArgumentlessDirectiveToken.FORM, (s) => new ArgumentlessDirectiveToken(s));
        syntaxAnalyzer.AddTokenForm(TextArgumentDirectiveToken.FORM, (s) => new TextArgumentDirectiveToken(s));
        syntaxAnalyzer.AddTokenForm(NumberArgumentDirective.FORM, (s) => new NumberArgumentDirective(s));
        syntaxAnalyzer.AddTokenForm(LabelToken.FORM, (s) => new LabelToken(s));
        var assembler = new ProgramAssembler(syntaxAnalyzer);
        assembler.ParseSection(text);

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