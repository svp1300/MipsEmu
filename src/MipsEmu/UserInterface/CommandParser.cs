namespace MipsEmu.UserInterface;

using MipsEmu;

using MipsEmu.Assembler;
public class CommandParser {
    private static readonly Dictionary<string, Command> COMMANDS = new Dictionary<string, Command> {
        {"assemble", new AssembleCommand()}, {"assemblerun", new AssembleRunCommand()}
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
            Console.WriteLine("Assemble expects one or two arguments.");
            return;
        }
        string[]? unassembledProgram = ProgramReader.ReadProgram(path);
        if (unassembledProgram == null) {
            Console.WriteLine("Unable to read program!");
            return;
        }
        var syntaxAnalyzer = SyntaxAnalyzer.CreateDefaultSyntaxAnalyzer();
        var pseudoExpander = PseudoInstructionExpander.CreateDefaultPseudoExpander();
        var assembler = new ProgramLinker(syntaxAnalyzer, pseudoExpander);
        var unlinked = assembler.Parse(unassembledProgram); //new string[] {text}); //
        
        var assembledProgram = assembler.Link(unlinked);
        if (options.Contains("print")) {
            Console.WriteLine($"~~~Unlinked~~~\n{unlinked}\n\n~~~Linked~~~\n{assembledProgram}");
        } else {
            Console.WriteLine($"~~~Unlinked~~~\n{unlinked}\n\n~~~Linked~~~\n{assembledProgram}");
        }
    }
}

public class AssembleRunCommand : Command {

    public override void Run(List<string> arguments, List<string> options) {
        string path;
        if (arguments.Count == 1)
            path = "./";
        else if (arguments.Count == 2)
            path = arguments[1];
        else {
            Console.WriteLine("Assemblerun expects one or no arguments.");
            return;
        }
        string[]? unassembledProgram = ProgramReader.ReadProgram(path);
        if (unassembledProgram == null) {
            Console.WriteLine("Unable to read program!");
            return;
        }
        var syntaxAnalyzer = SyntaxAnalyzer.CreateDefaultSyntaxAnalyzer();
        var pseudoExpander = PseudoInstructionExpander.CreateDefaultPseudoExpander();
        var assembler = new ProgramLinker(syntaxAnalyzer, pseudoExpander);
        var unlinked = assembler.Parse(unassembledProgram);
        var assembledProgram = assembler.Link(unlinked);
        Console.WriteLine($"~~~Unlinked~~~\n{unlinked}\n\n~~~Linked~~~\n{assembledProgram}\n~~~Program~~~\n");
        var emulator = new MipsProgram(0x15000000);
        emulator.LoadProgram(assembledProgram.text, assembledProgram.data);
        emulator.RunProgram();
    }
}
public class RunCommand {

}