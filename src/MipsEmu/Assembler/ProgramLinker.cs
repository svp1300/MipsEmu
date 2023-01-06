namespace MipsEmu.Assembler;

using MipsEmu.Assembler.Tokens;
using MipsEmu.Emulation.Instructions;
using System.Text;

public enum MemorySection {
    DATA, TEXT
}

public class UnlinkedProgram  {

    private Dictionary<string, int> symbolTable;

    private List<SyntaxParseResult> programSections;
    private Dictionary<int, long> dataStartTable, textStartTable;
    private long dataSum, textSum;

    public UnlinkedProgram(List<SyntaxParseResult> programSections, Dictionary<string, int> symbolTable) {
        symbolTable = new Dictionary<string, int>();
        dataStartTable = new Dictionary<int, long>();
        textStartTable = new Dictionary<int, long>();
        this.programSections = programSections;
        this.symbolTable = symbolTable;
        AddStartAddressOffsets();
    }

    public void CreateSymbolTable() {
        for(int sectionId = 0; sectionId < programSections.Count; sectionId++) {
            foreach(string labelName in programSections[sectionId].globals) {
                if (programSections[sectionId].IsExternalDefinition(labelName)) {
                    if (symbolTable.ContainsKey(labelName)) {
                        throw new ParseException("Duplicate global symbol: " + labelName);
                    } else {
                        symbolTable.Add(labelName, sectionId);
                    }
                }
            }
        }
    }

    public void AddStartAddressOffsets() {
        dataSum = 0;
        textSum = 0;
        for (int programId = 0; programId < programSections.Count; programId++) {
            SyntaxParseResult programData = programSections[programId];
            dataStartTable[programId] = dataSum;
            textStartTable[programId] = textSum;
            programData.directiveLabels.ForEach((l) => l.AddAddressOffset(dataSum));
            programData.instructionLabels.ForEach((l) => l.AddAddressOffset(textSum));
            dataSum += programData.dataLength;
            textSum += programData.textLength;
        }
    }

    public void IncreaseTextSum(int instructionAmount) {
        textSum += 32 * instructionAmount;
    }

    public long GetDataLength() => dataSum;
    public long GetTextLength() => textSum;

    public bool IsKnown(string labelName) => symbolTable.ContainsKey(labelName);
    
    public long GetDataStartAddress(int sectionId) => dataStartTable[sectionId];
    public long GetTextStartAddress(int sectionId) => textStartTable[sectionId];

    public int GetSectionCount() => programSections.Count;
    public SyntaxParseResult GetSection(int sectionId) => programSections[sectionId];

    public long GetLabelAddress(string name, int sectionId) {
        try {
            return GetLabelAddress(name, sectionId, false);
        } catch(ParseException) {
            return GetLabelAddress(name, sectionId, true);
        }
    }

    public long GetLabelAddress(string name, int sectionId, bool text) {
        Label? localLabel = programSections[sectionId].GetLabel(name, text);
        if (localLabel == null && symbolTable.ContainsKey(name))
            localLabel = programSections[symbolTable[name]].GetLabel(name, text);
        if (localLabel == null)
            throw new ParseException("Unknown label: " + name);
        else {
            return localLabel.GetAddress();
        }
    }

    private void AddProgramToString(int sectionId, StringBuilder builder) {
        var program = programSections[sectionId];
        long textAddress = 0;
        builder.AppendLine("~~~~SECTION #" + sectionId + "~~~~");
        foreach (var textLine in program.instructionTokens) {
            builder.AppendLine(textAddress + "\t" + "TEXT" + "\t" + textLine);
            textAddress += textLine.GetByteLength(2);
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

public class LinkedProgram {
    public Bits text, data;

    public LinkedProgram(Bits data, Bits text) {
        this.data = data;
        this.text = text;
    }


    public override string ToString() {
        var builder = new StringBuilder();
        for (int i = 0; i < text.GetLength(); i += 32) {
            var address = 0x00400000 + i / 8;
            var bits = text.LoadBits(i, 32);
            var instruction = InstructionParser.ParseInstruction(bits);
            if (instruction == null) {
                builder.AppendLine($"{address}\t!!!!");
            } else {
                builder.AppendLine($"{address}\t{instruction.InfoString(bits)}");
            }
        }
        return builder.ToString();
    }

}

public class ProgramLinker {
    private SyntaxAnalyzer syntaxAnalyzer;
    private List<SyntaxParseResult> programSections;
    private PseudoInstructionExpander pseudoExpander;
    private Dictionary<string, int> symbolTable;
    
    private object dataLock;

    public ProgramLinker(SyntaxAnalyzer syntaxAnalyzer, PseudoInstructionExpander pseudoExpander) {
        this.syntaxAnalyzer = syntaxAnalyzer;
        this.pseudoExpander = pseudoExpander;
        symbolTable = new Dictionary<string, int>();
        programSections = new List<SyntaxParseResult>();
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
        lock (dataLock) {
            // register
            int id = programSections.Count;
            programSections.Add(result);
            // make symbol table
            JoinTables(result.GetExternalDataReferences(), id);
            JoinTables(result.GetExternalTextReferences(), id);
        }
    }

    /// <summary>Parse each section of the program and replace the pseudoinstructions.</summary>
    public UnlinkedProgram Parse(string[] program) {
        ParseSection("jal main li $v0, 10 syscall"); // enter and exit first
        foreach (string section in program) { // TODO async
            ParseSection(section);
        }
        var unlinked = new UnlinkedProgram(programSections, symbolTable);
        return pseudoExpander.ReplacePseudoInstructions(unlinked);
    }
    

    /// <summary>Link each section to create a runnable assembly program.</summary>
    public LinkedProgram Link(UnlinkedProgram unlinked) {
        var data = new Bits(8 * unlinked.GetDataLength());
        var text = new Bits(8 * unlinked.GetTextLength());
        unlinked.CreateSymbolTable();
        for (int sectionId = 0; sectionId < unlinked.GetSectionCount(); sectionId++) {
            StoreTextSection(sectionId, unlinked, text);
            StoreDataSection(sectionId, unlinked, data);
        }
        return new LinkedProgram(data, text);
    }

    private void StoreTextSection(int sectionId, UnlinkedProgram unlinked, Bits text) {
        var textBitsList = new LinkedList<Bits>();
        long pcPsuedoAddress = unlinked.GetTextStartAddress(sectionId);
        foreach (var textToken in programSections[sectionId].instructionTokens) {
            textBitsList.AddLast(((InstructionToken) textToken).MakeValueBits(unlinked, sectionId, pcPsuedoAddress));
            pcPsuedoAddress += 4;
        }
        long address = unlinked.GetTextStartAddress(sectionId);
        while (textBitsList.Count > 0) {
            var front = textBitsList.First;
            if (front == null) {
                throw new ParseException("Null value in data bits list.");
            }
            text.Store(address * 8, front.Value);
            textBitsList.RemoveFirst();
            address += 4;
        }
    }

    private void StoreDataSection(int sectionId, UnlinkedProgram unlinked, Bits data) {
        var dataBitsList = new LinkedList<Bits>();  
        long sum = 0;
        foreach (var dataToken in programSections[sectionId].directiveTokens) {
            dataBitsList.AddLast((dataToken).MakeValueBits(unlinked, sectionId, 0));
            sum += (dataToken).MakeValueBits(unlinked, sectionId, 0).GetLength();
        }
        long address = unlinked.GetDataStartAddress(sectionId);
        while (dataBitsList.Count > 0) {
            var front = dataBitsList.First;
            if (front == null) {
                throw new ParseException("Null value in data bits list.");
            } else if (front.Value.GetLength() > 0) {
                data.Store(address * 8, front.Value);
                dataBitsList.RemoveFirst();
                address += front.Value.GetLength();
            } else {
                dataBitsList.RemoveFirst();
            }
        }
    }

}