namespace MipsEmu.UserInterface;

using MipsEmu;
using MipsEmu.Debug;
using MipsEmu.Assembler;

/// <summary>Assembles from the path and either prints or writes the results.</summary>
public class AssembleCommand : Command {

    public static LinkedProgram? AssembleProgram(string path, bool print) {
       string[]? unassembledProgram = ProgramReader.ReadAllWithExtension(path, ".asm");
        if (unassembledProgram == null) {
            Console.WriteLine("Unable to read program!");
            return null;
        }
        var syntaxAnalyzer = SyntaxAnalyzer.CreateDefaultSyntaxAnalyzer();
        var pseudoExpander = PseudoInstructionExpander.CreateDefaultPseudoExpander();
        var assembler = new ProgramLinker(syntaxAnalyzer, pseudoExpander);
        var unlinked = assembler.Parse(unassembledProgram);
        var assembledProgram = assembler.Link(unlinked);
        if (print)
            Console.WriteLine($"~~~Unlinked~~~\n{unlinked}\n\n~~~Linked~~~\n{assembledProgram}\n~~~Program~~~\n"); 
        return assembledProgram;
    }

    public override void Run(List<string> arguments, List<string> options, Dictionary<string, object> supportingObjects) {
        string path;
        if (arguments.Count == 1)
            path = "./";
        else if (arguments.Count == 2)
            path = arguments[1];
        else {
            Console.WriteLine("Assemble expects one or two arguments.");
            return;
        }
        
        bool print = options.Contains("print");
        var assembled = AssembleProgram(path, print);
        if (!print) {
            // ... write ...
        }
    }
}

/// <summary>Assembles the program fom a path and runs it in stdout/stdin.</summary>
public class AssembleRunCommand : Command {

    public override void Run(List<string> arguments, List<string> options, Dictionary<string, object> supportingObjects) {
        string path;
        if (arguments.Count == 1)
            path = "./";
        else if (arguments.Count == 2)
            path = arguments[1];
        else {
            Console.WriteLine("Assemblerun expects one or no arguments.");
            return;
        }
        var assembledProgram = AssembleCommand.AssembleProgram(path, true);
        if (assembledProgram != null) {
            var emulator = new MipsProgram(0x15000000);
            emulator.LoadProgram(assembledProgram.text, assembledProgram.data);
            emulator.RunProgram();
        }
    }
}

/// <summary>Assembles the program fom a path and runs the debugging tool.</summary>
public class AssembleDebugCommand : Command {

    public override void Run(List<string> arguments, List<string> options, Dictionary<string, object> supportingObjects) {
        string path;
        if (arguments.Count == 1)
            path = "./";
        else if (arguments.Count == 2)
            path = arguments[1];
        else {
            Console.WriteLine("Assemblerun expects one or no arguments.");
            return;
        }
        var assembledProgram = AssembleCommand.AssembleProgram(path, true);
        if (assembledProgram != null) {
            var emulator = new DebugProgram(0x15000000);
            emulator.LoadProgram(assembledProgram.text, assembledProgram.data);
            emulator.RunProgram();
        }
    }
}

/// <summary>Runs the mips program at the given path.</summary>
public class RunCommand : Command {
    public override void Run(List<string> arguments, List<string> options, Dictionary<string, object> supportingObjects) {
        if (arguments.Count != 3) {
            Console.WriteLine("Run expects one or no arguments: textfile datafile");
        } else {
            var reader = new ProgramReader();
            var emulator = new MipsProgram(0x15000000);
            var text = ProgramReader.ReadBits(arguments[0]);
            var data = ProgramReader.ReadBits(arguments[1]);
            emulator.LoadProgram(text, data);
            emulator.RunProgram();
        }
    }

}

/// <summary>Runs the debugging tool on the program at the given path.</summary>
public class DebugCommand : Command {
    public override void Run(List<string> arguments, List<string> options, Dictionary<string, object> supportingObjects) {
        if (arguments.Count != 3) {
            Console.WriteLine("Debug expects two arguments: textfile datafile");
        } else {
            var reader = new ProgramReader();
            var emulator = new DebugProgram(0x15000000);
            var text = ProgramReader.ReadBits(arguments[0]);
            var data = ProgramReader.ReadBits(arguments[1]);
            emulator.LoadProgram(text, data);
            emulator.RunProgram();
        }
    }

}