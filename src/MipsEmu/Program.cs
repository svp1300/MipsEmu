namespace MipsEmu;

using MipsEmu.UserInterface;


public sealed class Program {

    public static void Main(string[] args) {
        var parser = new CommandParser();
        parser.AddCommand("assemblerun", new AssembleRunCommand());
        parser.AddCommand("assembledebug", new AssembleDebugCommand());
        parser.AddCommand("assemble", new AssembleCommand());
        parser.AddCommand("run", new RunCommand());
        parser.AddCommand("debug", new DebugCommand());
        if (args.Length == 0) {
            Console.WriteLine("At least one parameter is required. (run, debug, assemble)");
        } else {
            parser.ParseAndRun(args, new Dictionary<string, object>());
        }
    }

}