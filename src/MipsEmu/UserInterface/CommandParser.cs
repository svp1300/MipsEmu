namespace MipsEmu.UserInterface;


public class CommandParser {
    private Dictionary<string, Command> commands;

    public CommandParser() {
        commands = new Dictionary<string, Command>();
    }

    public void AddCommand(string name, Command command) {
        commands.Add(name, command);
    }
    public void ParseAndRun(string[] commandArguments, Dictionary<string, object> supportingObjects) {
        var arguments = new List<string>();
        var options = new List<string>();
        foreach (var token in commandArguments) {
            if (token.StartsWith("-"))
                options.Add(token.Substring(1));
            else
                arguments.Add(token);
        }
        string command = arguments[0];
        if (commands.ContainsKey(command)) {
            try {
                commands[command].Run(arguments, options, supportingObjects);
            } catch(FormatException) {
                Console.WriteLine("Improper command formatting.");
            }
        } else {
            Console.WriteLine("Unrecognized command.");
        }
    }
}

public abstract class Command {

    public abstract void Run(List<string> arguments, List<string> options, Dictionary<string, object> supportingObjects);

}
