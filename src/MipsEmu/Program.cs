using System;

namespace MipsEmu;

public sealed class Program {

    public static void Main(string[] args) {
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