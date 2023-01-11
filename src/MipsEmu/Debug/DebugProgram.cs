namespace MipsEmu.Debug;

using MipsEmu.UserInterface;
using System.Text.RegularExpressions;

public class DebugProgram : MipsProgram {
    private List<long> breakpoints;
    private CommandParser debugParser;
    private bool closeFlag;
    private Dictionary<string, object> supporting;

    public DebugProgram(long memorySize) : base(memorySize, true) {
        breakpoints = new List<long>();
        debugParser = new CommandParser();
        debugParser.AddCommand("breakpoint", new BreakpointCommand());
        debugParser.AddCommand("cycle", new CycleCommand());
        debugParser.AddCommand("many", new RepeatCommand());
        debugParser.AddCommand("regfile", new RegFileCommand());
        supporting = new Dictionary<string, object> {{"debugger", this}};
    }

    public void AddBreakpoint(long line) {
        if (!breakpoints.Contains(line))
            breakpoints.Add(line);
    }

    public bool HitBreakpoint() => breakpoints.Contains(GetPCValue());

    public void RemoveBreakpoint(int line) => breakpoints.Remove(line);
    
    public bool ShouldPause() => HitBreakpoint() || GetHardware().exit;
    public void UpdateCloseFlag() => closeFlag = true;

    public void PrintRegisters() => Console.WriteLine(GetHardware().registers);

    public override void RunProgram() {
        while (!closeFlag && !GetHardware().exit) {
            if (HitBreakpoint()) {
                Console.WriteLine("Breakpoint hit at " + GetPCValue());
            }
            Console.Write("(debug) ");
            var line = Console.ReadLine();
            if (line != null) {
                debugParser.ParseAndRun(Regex.Split(line, "\\s+"), supporting);
            } else {
                return;
            }
        }
    }

    
}