namespace MipsEmu.Assembler;

using MipsEmu.Assembler.Tokens;

public class PsuedoInstructionExpander {
    private List<PseudoInstruction> pseudoInstructions;

    public PsuedoInstructionExpander() {
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

    private void FixSection(SyntaxParseResult section) {
        int index = 0;
        // int lowestLabel = 0; // TODO start label iterator from this
        while(index < section.instructionLabels.Count) {
            Token instruction = section.instructionTokens[index];
            var update = new Token[0];
            foreach(var pseudo in pseudoInstructions) {
                if (pseudo.MatchInstructionName(instruction)) {
                    update = pseudo.Expand(instruction);
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
            FixSection(program.GetSection(sectionId));
        }
        return program;
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
    public abstract Token[] Expand(Token pseudoToken);

    public bool MatchInstructionName(Token instruction) {
        return instruction.GetSymbolString(0, true).Equals(Name);
    }

}