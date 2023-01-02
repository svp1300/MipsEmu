namespace MipsEmu;

using MipsEmu.UserInterface;


public sealed class Program {

    public static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("At least one parameter is required. (run, debug, assemble)");
        } else {
            CommandParser.ParseAndRun(args);
        }
    }

}