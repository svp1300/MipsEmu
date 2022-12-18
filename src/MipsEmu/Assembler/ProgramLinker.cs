namespace MipsEmu.Assembler;

using MipsEmu.Assembler.Tokens;
using System.Text;

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

    private void AddProgramToString(int sectionId, StringBuilder builder) {
        var program = programSections[sectionId];
        long textAddress = 0;
        builder.AppendLine("~~~~SECTION #" + sectionId + "~~~~");
        foreach (var textLine in program.instructionTokens) {
            builder.AppendLine(textAddress + "\t" + "TEXT" + "\t" + textLine);
            textAddress += textLine.GetBitLength(2);
        }
    }

    public override string ToString() {
        var builder = new StringBuilder();
        for(int sectionId = 0; sectionId < programSections.Count; sectionId++) {
            AddProgramToString(sectionId, builder);
            builder.AppendLine();
        }
        return builder.ToString();
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

    public UnlinkedProgram Parse(string[] program) {
        ParseSection("j main li $v0, 10 syscall"); // enter and exit first
        foreach (string section in program) { // TODO async
            ParseSection(section);
        }
        return new LinkedProgram(programSections, symbolTable);
    }
    
    public LinkedProgram Link(UnlinkedProgram unlinked) {
        var data = new Bits(unlinked.GetDataLength());
        var text = new Bits(unlinked.GetTextLength());
        for (int sectionId = 0; sectionId < unlinked.GetSectionCount(); sectionId++) {
            StoreTextSection(sectionId, unlinked, text);
            StoreDataSection(sectionId, unlinked, data);
        }
        return new LinkedProgram(data, text);
    }

    private void StoreTextSection(int sectionId, UnlinkedProgram unlinked, Bits text) {
        var textBitsList = new LinkedList<Bits>();
        foreach (var textToken in programSections[sectionId].instructionTokens) {
            textBitsList.AddLast(((InstructionToken) textToken).MakeValueBits(unlinked, sectionId));
        }
        long address = unlinked.GetTextStartAddress(sectionId);
        while (textBitsList.Count > 0) {
            var front = textBitsList.First;
            text.Store(address, front.Value);
            textBitsList.RemoveFirst();
            address += 32;
        }
    }

    private void StoreDataSection(int sectionId, UnlinkedProgram unlinked, Bits data) {
        Console.WriteLine(unlinked);
        var dataBitsList = new LinkedList<Bits>();
        foreach (var dataToken in programSections[sectionId].directiveTokens) {
            dataBitsList.AddLast((dataToken).MakeValueBits(unlinked, sectionId));
        }
        long address = unlinked.GetTextStartAddress(sectionId);
        while (dataBitsList.Count > 0) {
            var front = dataBitsList.First;
            if (front == null) {
                throw new ParseException("Null value in data bits list.");
            } else {
                data.Store(address, front.Value);
                dataBitsList.RemoveFirst();
                address += front.Value.GetLength();
            }
        }
    }


}