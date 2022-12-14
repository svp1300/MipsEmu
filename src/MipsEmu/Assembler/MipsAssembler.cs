namespace MipsEmu.Assembler;

using MipsEmu.Emulation.Instructions;

public class ProgramAssembler {
    private SyntaxAnalyzer syntaxAnalyzer;
    private List<SyntaxParseResult> programSections;
    private Dictionary<string, int> symbolTable;
    private Dictionary<int, long> dataStartTable, textStartTable;
    private List<bool> dataMemory;
    private List<IInstruction> instructions;
    
    private object dataLock;

    public ProgramAssembler(SyntaxAnalyzer syntaxAnalyzer) {
        this.syntaxAnalyzer = syntaxAnalyzer;
        symbolTable = new Dictionary<string, int>();
        programSections = new List<SyntaxParseResult>();
        dataMemory = new List<bool>();
        dataStartTable = new Dictionary<int, long>();
        textStartTable = new Dictionary<int, long>();
        instructions = new List<IInstruction>();
        dataLock = new object();
    }

    private void JoinTables(List<Label> labels, int sectionId) {
        foreach(var l in labels) {
            if (symbolTable.ContainsKey(l.Name)) {
                throw new ParseException("Duplicate global symbol: " + l.Name);
            } else {
                symbolTable.Add(l.Name, sectionId);
            }
        }
    }

    public void ParseSection(string section) {
        // Parse
        Symbol[] symbols = LexicalAnalyzer.FindSymbols(section);
        SyntaxParseResult result = syntaxAnalyzer.BuildProgram(symbols);
    
        Console.WriteLine(result);
        lock (dataLock) {
            // register
            int id = programSections.Count;
            programSections.Add(result);
            // make symbol table
            JoinTables(result.GetExternalDataReferences(), id);
            JoinTables(result.GetExternalTextReferences(), id);
        }
    }

    private void BuildStartAddressTables() {
        long dataSum = 0;
        long textSum = 0;
        for (int programId = 0; programId < programSections.Count; programId++) {
            dataStartTable[programId] = dataSum;
            textStartTable[programId] = textSum;
            SyntaxParseResult programData = programSections[programId];
            dataSum += programData.dataLength;
            textSum += programData.textLength;
        }
    }//TODO fix label positions
    public void Parse(string[] program) {
        foreach (string section in program) { // TODO async
            ParseSection(section);
        }
        foreach(var programData in programSections) {
            foreach(var token in programData.instructionTokens) {
                var instruction = (InstructionToken) token;
                instruction.CreateInstruction();
            }
        }
    }


}