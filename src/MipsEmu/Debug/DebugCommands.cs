namespace MipsEmu.Debug;

using MipsEmu.UserInterface;

public abstract class DebugCommand : Command {

    public override void Run(List<string> arguments, List<string> options, Dictionary<string, object> supportingObjects) {
        RunDebug(arguments, options, (DebugProgram) supportingObjects["debugger"]);
    }

    public abstract void RunDebug(List<string> arguments, List<string> options, DebugProgram debugger);

}
public class BreakpointCommand : DebugCommand {

    public override void RunDebug(List<string> arguments, List<string> options, DebugProgram debugger) {
        if (arguments.Count != 2) {
            Console.WriteLine("Expected one argument: instruction_address");
            return;
        }
        debugger.AddBreakpoint(long.Parse(arguments[1]));
    }

}

public class RemoveBreakpointCommand : DebugCommand {

    public override void RunDebug(List<string> arguments, List<string> options, DebugProgram debugger) {
        if (arguments.Count != 2) {
            Console.WriteLine("Expected one argument: instruction_address");
            return;
        }
        debugger.RemoveBreakpoint(Int32.Parse(arguments[1]));
    }

}

public class CycleCommand : DebugCommand {

    public override void RunDebug(List<string> arguments, List<string> options, DebugProgram debugger) {
        if (!debugger.Cycle()) {
            debugger.UpdateCloseFlag();
        }
    }

}

public class RepeatCommand : DebugCommand {

    public override void RunDebug(List<string> arguments, List<string> options, DebugProgram debugger) {
        if (arguments.Count != 2) {
            Console.WriteLine("Expected one argument: count");
            return;
        }   
        int count = Int32.Parse(arguments[1]);
        for (int i = 0; i < count; i++) {
            if (debugger.ShouldPause())
                break;
            else if (!debugger.Cycle()) {
                debugger.UpdateCloseFlag();
                break;
            }
        }
    }

}

public class RegFileCommand : DebugCommand {

    public override void RunDebug(List<string> arguments, List<string> options, DebugProgram debugger) {
        debugger.PrintRegisters();
    }
}

