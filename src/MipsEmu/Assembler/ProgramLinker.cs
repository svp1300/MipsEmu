namespace MipsEmu.Assembler;

using MipsEmu.Emulation.Instructions;

public class LinkedProgram  {

    private Dictionary<string, int> symbolTable;

    private List<SyntaxParseResult> programSections;
    private Dictionary<int, long> dataStartTable, textStartTable;

    public LinkedProgram(List<SyntaxParseResult> programSections, Dictionary<string, int> symbolTable) {
        symbolTable = new Dictionary<string, int>();
        dataStartTable = new Dictionary<int, long>();
        textStartTable = new Dictionary<int, long>();
        this.programSections = programSections;
        this.symbolTable = symbolTable;
        AddStartAddressOffsets();
    }

    public void CreateSymbolTable(List<Label> labels, int sectionId) {
        foreach(var l in labels) {
            if (symbolTable.ContainsKey(l.Name)) {
                throw new ParseException("Duplicate global symbol: " + l.Name);
            } else {
                symbolTable.Add(l.Name, sectionId);
            }
        }
    }

    public void AddStartAddressOffsets() {
        long dataSum = 0;
        long textSum = 0;
        for (int programId = 0; programId < programSections.Count; programId++) {
            // dataStartTable[programId] = dataSum;
            // textStartTable[programId] = textSum;
            SyntaxParseResult programData = programSections[programId];
            dataSum += programData.dataLength;
            textSum += programData.textLength;
            foreach(var label in programData.directiveLabels) {
                label.AddAddressOffset(dataSum);
            }
            foreach(var label in programData.instructionLabels) {
                label.AddAddressOffset(textSum);
            }
        }
    }
    public bool IsKnown(string labelName) => symbolTable.ContainsKey(labelName);
    
    public long GetAddress(string name, int sectionId, bool text) {
        Label? localLabel = programSections[sectionId].GetLabel(name, text);
        if (localLabel == null)
            localLabel = programSections[symbolTable[name]].GetLabel(name, text);
        if (localLabel == null)
            throw new ParseException("Unknown label: " + name);
        else {
            return localLabel.Value.Address;
        }
    }

}

public class ProgramLinker {
    private SyntaxAnalyzer syntaxAnalyzer;
    private List<SyntaxParseResult> programSections;
    private Dictionary<string, int> symbolTable;
    private List<IInstruction> instructions;
    
    private object dataLock;

    public ProgramLinker(SyntaxAnalyzer syntaxAnalyzer) {
        this.syntaxAnalyzer = syntaxAnalyzer;
        symbolTable = new Dictionary<string, int>();
        programSections = new List<SyntaxParseResult>();
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
        Symbol[] symbols = LexicalAnalyzer.FindSymbols(section).ToArray();
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

    public LinkedProgram Parse(string[] program) {
        foreach (string section in program) { // TODO async
            ParseSection(section);
        }
        return new LinkedProgram(programSections, symbolTable);
    }


}