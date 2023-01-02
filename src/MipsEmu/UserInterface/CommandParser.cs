namespace MipsEmu.UserInterface;

using MipsEmu;

using MipsEmu.Assembler;
public class CommandParser {
    private static readonly Dictionary<string, Command> COMMANDS = new Dictionary<string, Command> {
        {"assemble", new AssembleCommand()}
    };

    public static void ParseAndRun(string[] commandLineArguments) {
        var arguments = new List<string>();
        var options = new List<string>();
        foreach (var token in commandLineArguments) {
            if (token.StartsWith("-"))
                options.Add(token.Substring(1));
            else
                arguments.Add(token);
        }
        string command = arguments[0];
        if (COMMANDS.ContainsKey(command)) {
            COMMANDS[command].Run(arguments, options);
        } else {
            Console.WriteLine("Unrecognized command.");
        }
    }
}

public abstract class Command {

    public abstract void Run(List<string> arguments, List<string> options);

}

public class AssembleCommand : Command {

    public override void Run(List<string> arguments, List<string> options) {
        string path;
        if (arguments.Count == 1)
            path = "./";
        else if (arguments.Count == 2)
            path = arguments[1];
        else {
            Console.WriteLine("Assemble expects one argument at most.");
            return;
        }
        string[]? unassembledProgram = ProgramReader.ReadProgram(path);
        if (unassembledProgram == null) {
            Console.WriteLine("Unable to read program!");
            return;
        }
        // text = ".data values:.byte 5,4,3, 2, 1 beep: .word 16.text .globl main main: addi $t0, $t0, 43 add $t0, $t0, $t1 sub $t4, $s0, $t1 jr $ra";
        // text = ".globl main main: add $t0, $t0, $t1 add $t0, $t0, $t1 add $t0, $t0, $t1 lw $s0, 0($t0)";
        var text = ".data var: .space 8 second: .byte 7 .text .globl main main: sub $t0, $t0, 43 lw $t0, 0($sp) jr $ra";
        var syntaxAnalyzer = SyntaxAnalyzer.CreateDefaultSyntaxAnalyzer();
        var pseudoExpander = PseudoInstructionExpander.CreateDefaultPseudoExpander();
        var assembler = new ProgramLinker(syntaxAnalyzer, pseudoExpander);
        var unlinked = assembler.Parse(unassembledProgram);//new string[] {text}); //
        var assembledProgram = assembler.Link(unlinked);
        // var emulated = new MipsProgram(0x20000000);
        // emulated.LoadProgram(program.text, program.data);
        // emulated.RunProgram();
        if (options.Contains("print")) {
            Console.WriteLine($"~~~Unlinked~~~\n{unlinked}\n\n~~~Linked~~~\n{assembledProgram}");
        } else {
            Console.WriteLine($"~~~Unlinked~~~\n{unlinked}\n\n~~~Linked~~~\n{assembledProgram}");
        }
    }
}

public class RunCommand {

}