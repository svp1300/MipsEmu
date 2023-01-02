namespace MipsEmu.Assembler;

using MipsEmu.Assembler.Tokens;

public class PseudoInstructionExpander {
    private List<PseudoInstruction> pseudoInstructions;

    public PseudoInstructionExpander() {
        pseudoInstructions = new List<PseudoInstruction>();
    }

    private void FixSectionLabels(int instructionIndex, int expansionOffset, SyntaxParseResult section) {
        for (int labelIndex = 0; labelIndex < section.instructionLabels.Count; labelIndex++) {
            var label = section.instructionLabels[labelIndex];
            if (label.GetAddress() > instructionIndex * 32)  {
                label.AddAddressOffset(expansionOffset * 32);
            }
        }
    }

    private void FixSection(UnlinkedProgram unlinked, int sectionId) {
        SyntaxParseResult section = unlinked.GetSection(sectionId);
        int index = 0;
        // int lowestLabel = 0; // TODO start label iterator from this
        while(index < section.instructionTokens.Count) {
            Token instruction = section.instructionTokens[index];
            var update = new Token[0];
            foreach(var pseudo in pseudoInstructions) {
                if (pseudo.MatchToken(instruction)) {
                    update = pseudo.Expand(instruction, unlinked, sectionId);
                    break;
                }
            }
            if (update.Length != 0) {
                section.instructionTokens.RemoveAt(index);
                section.instructionTokens.InsertRange(index, update);
                FixSectionLabels(index, update.Length, section);
                index += update.Length;
            } else {
                index++;
            }
        }
    }

    public void AddPseudoInstruction(PseudoInstruction pseudoInstruction) {
        pseudoInstructions.Add(pseudoInstruction);
    }

    public UnlinkedProgram ReplacePseudoInstructions(UnlinkedProgram program) {
        for(int sectionId = 0; sectionId < program.GetSectionCount(); sectionId++) {
            FixSection(program, sectionId);
        }
        return program;
    }

    public static PseudoInstructionExpander CreateDefaultPseudoExpander() {
        var expander = new PseudoInstructionExpander();
        expander.AddPseudoInstruction(new LoadImmediatePseudoInstruction());
        return expander;
    }

}
public abstract class PseudoInstruction {
    public string Name {get;}
    private ITokenForm form;

    public PseudoInstruction(string name, ITokenForm form) {
        Name = name;
        this.form = form;
    }

    /// <summary>Convert the psuedoinstruction into a group of real instructions.</summary>
    public abstract Token[] Expand(Token pseudoToken, UnlinkedProgram unlinked, int sectionId);

    public bool MatchToken(Token instruction) {
        return instruction.GetSymbolString(0, true).Equals(Name) && form.Match(instruction.GetAllSymbols(), 0) > 0;
    }

}

public class LoadImmediatePseudoInstruction : PseudoInstruction {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER}, true);
    
    public LoadImmediatePseudoInstruction() : base("li", FORM) { }

    public override Token[] Expand(Token pseudoToken, UnlinkedProgram unlinked, int sectionId) {
        var register = pseudoToken.GetSymbol(1, true);
        var immediate = pseudoToken.GetSymbol(3, true);
        var comma = new Symbol(",", SymbolType.COMMA);
        var symbols = new Symbol[] {new Symbol("addi", SymbolType.NAME),
                                    register,
                                    comma,
                                    new Symbol("$zero", SymbolType.REGISTER),
                                    comma,
                                    immediate};
        return new Token[] {new TypeIInstructionToken(symbols)};

    }
}

public class MovePseudoInstruction : PseudoInstruction {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA,SymbolType.REGISTER}, true);

    public MovePseudoInstruction() : base("move", FORM) { }

    public override Token[] Expand(Token pseudoToken, UnlinkedProgram unlinked, int sectionId) {
        var toRegister = pseudoToken.GetSymbol(1, true);
        var fromRegister = pseudoToken.GetSymbol(3, true);
        var symbols = new Symbol[] {new Symbol("addi", SymbolType.NAME),
                                    toRegister,
                                    new Symbol(",", SymbolType.COMMA),
                                    new Symbol("$zero", SymbolType.REGISTER),
                                    new Symbol(",", SymbolType.COMMA),
                                    fromRegister};
        return new Token[] {new TypeIInstructionToken(symbols)};
    }
    
}

public class LoadAddressPseudoInstruction : PseudoInstruction {
public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA,SymbolType.NAME}, true);

public LoadAddressPseudoInstruction() : base("la", FORM) { }

    public override Token[] Expand(Token pseudoToken, UnlinkedProgram unlinked, int sectionId) {
        var tokens = new Token[2];
        var register = pseudoToken.GetSymbol(1, true);
        var labelName = pseudoToken.GetSymbol(3, true);
        var comma = new Symbol(",", SymbolType.COMMA);
        tokens[0] = new TypeIInstructionToken(new Symbol[] {new Symbol("lui", SymbolType.NAME),
                                                            new Symbol("$at", SymbolType.REGISTER),
                                                            comma,
                                                            new Symbol("0x1001", SymbolType.NUMBER)});
        
        long address = unlinked.GetLabelAddress(labelName.value, sectionId, false); // not offset by 0x1001<<16, so it's fine without subtraction
        tokens[1] = new TypeIInstructionToken(new Symbol[] {new Symbol("ori", SymbolType.NAME),
                                                            register,
                                                            comma,
                                                            new Symbol("$at", SymbolType.REGISTER),
                                                            comma,
                                                            new Symbol(address.ToString(), SymbolType.NUMBER)});
        return tokens;
    }
}