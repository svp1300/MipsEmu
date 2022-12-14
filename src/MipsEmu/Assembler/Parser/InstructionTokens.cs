namespace MipsEmu.Assembler;

using MipsEmu.Emulation.Instructions;

public abstract class InstructionToken : Token {
    public static readonly Dictionary<string, bool[]> REGISTER_BITS = new Dictionary<string, bool[]>() {

    };

    public InstructionToken(Symbol[] match) : base(match) { }
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;

    public override long GetBitLength(int alignment) => 32;

    
    public abstract Bits CreateInstruction();
    public override void UpdateAssemblerState(AssemblerState state, SyntaxParseResult results) {
        throw new ParseException("Only dot directives can change the state of the assembler.");
    }
}


public class TypeRInstructionToken : InstructionToken {
    public static readonly Dictionary<string, bool[]> FUNC_INSTRUCTION_BITS = new Dictionary<string, bool[]>() {

    };
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER}, true); // add $rs, $rt, $rd

    public TypeRInstructionToken(Symbol[] match) : base(match) { }

    public override Bits CreateInstruction() {
        var instruction = new Bits(32);
        instruction.Store(21, InstructionToken.REGISTER_BITS[GetSymbolString(3)]);
        instruction.Store(16, InstructionToken.REGISTER_BITS[GetSymbolString(5)]);
        instruction.Store(11, InstructionToken.REGISTER_BITS[GetSymbolString(1)]);
        instruction.Store(0, FUNC_INSTRUCTION_BITS[GetSymbolString(0)]);
        return instruction;
    }

}
public class TypeIInstructionToken : InstructionToken {
    public static readonly Dictionary<string, bool[]> OPCODE_INSTRUCTION_BITS = new Dictionary<string, bool[]>() {

    };
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER}, true); // addi $rs, $rt, imm
    
    public TypeIInstructionToken(Symbol[] match) : base(match) { }
    
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;

    public override Bits CreateInstruction() {
        var instruction = new Bits(32);
        instruction.Store(26, OPCODE_INSTRUCTION_BITS[GetSymbolString(0)]);
        instruction.Store(21, InstructionToken.REGISTER_BITS[GetSymbolString(3)]);
        instruction.Store(16, InstructionToken.REGISTER_BITS[GetSymbolString(1)]);
        Bits imm = new Bits(16);
        imm.SetFromSignedLong(long.Parse(GetSymbolString(5))); // todo check for signed/unsigned
        instruction.Store(0, imm.GetValues());
        return instruction;
    }
    
}
// public class MemoryInstructionToken : InstructionToken {
//     public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER, SymbolType.OPEN_PAREN, SymbolType.NUMBER, SymbolType.CLOSE_PAREN}, true);
    
//     public MemoryInstructionToken(Symbol[] match) : base(match) { }
    
//     public override TokenType GetTokenType() => TokenType.INSTRUCTION;
// }
