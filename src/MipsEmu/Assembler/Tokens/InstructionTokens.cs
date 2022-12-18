namespace MipsEmu.Assembler.Tokens;

using MipsEmu.Emulation.Instructions;

public abstract class InstructionToken : Token {
    public static readonly Dictionary<string, bool[]> FUNC_INSTRUCTION_BITS = new Dictionary<string, bool[]>() {
        {"add", new bool[] {true, false, false, false, false, false}},
        {"addu", new bool[] {true, false, false, false, false, true}},
        {"and", new bool[] {true, false, false, true, false, false}},
        {"nor", new bool[] {true, false, false, true, true, true}},
        {"or", new bool[] {true, false, false, true, false, true}},
        {"slt", new bool[] {true, false, true, false, true, false}},
        {"sltu", new bool[] {true, false, true, false, true, true}},
        {"sub", new bool[] {true, false, false, false, true, false}},
        {"subu", new bool[] {true, false, false, false, true, true}},
        {"xor", new bool[] {true, false, false, true, true, false}},
        {"sll", new bool[] {false, false, false, false, false, false}},
        {"sllv", new bool[] {false, false, false, true, false, false}},
        {"sra", new bool[] {false, false, false, false, true, true}},
        {"srav", new bool[] {false, false, false, true, true, true}},
        {"srl", new bool[] {false, false, false, false, true, false}},
        {"srlv", new bool[] {false, false, false, true, true, false}},
        
        {"div", new bool[] {false, true, true, false, true, false}},
        {"divu", new bool[] {false, true, true, false, true, true}},
        {"mfhi", new bool[] {false, true, false, false, false, false}},
        {"mflo", new bool[] {false, true, false, false, true, false}},
        {"mthi", new bool[] {false, true, false, false, false, true}},
        {"mtlo", new bool[] {false, true, false, false, true, true}},
        {"mult", new bool[] {false, true, true, false, false, false}},
        {"multu", new bool[] {false, true, true, false, false, true}},

        {"break", new bool[] {false, false, true, true, false, true}},
        {"jalr", new bool[] {false, false, true, false, false, true}},
        {"jr", new bool[] {false, false, true, false, false, false}},
        // coprocessor unsupported
        {"syscall", new bool[] {false, false, true, true, false, false}},
    };

    public static readonly Dictionary<string, bool[]> OPCODE_INSTRUCTION_BITS = new Dictionary<string, bool[]>() {
        {"addi", new bool[] {false, false, true, false, false, false}},
        {"addiu", new bool[] {false, false, true, false, false, true}},
        {"andi", new bool[] {false, false, true, true, false, false}},
        {"lui", new bool[] {false, false, true, true, true, true}},
        {"ori", new bool[] {false, false, true, true, false, true}},
        {"slti", new bool[] {false, false, true, false, true, false}},
        {"sltiu", new bool[] {false, false, true, false, true, true}},
        {"xori", new bool[] {false, false, true, true, true, false}},
        {"beq", new bool[] {false, false, false, true, false, false}},
        {"bne", new bool[] {false, false, false, true, false, true}},
        {"", new bool[] {}},

    };
    public static readonly Dictionary<string, bool[]> REGISTER_BITS = new Dictionary<string, bool[]>() {

    };

    public InstructionToken(Symbol[] match) : base(match) { }
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;

    public override long GetBitLength(int alignment) => 32;

    
    public abstract Bits CreateInstruction(Dictionary<string, long> symbolTextTable, Dictionary<string, long> symbolDataTable);
    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        throw new ParseException("Only dot directives can change the state of the assembler.");
    }
}


public class TypeRInstructionToken : InstructionToken {
    
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER}, true); // add $rs, $rt, $rd

    public TypeRInstructionToken(Symbol[] match) : base(match) { }

    public override Bits CreateInstruction(Dictionary<string, long> symbolTextTable, Dictionary<string, long> symbolDataTable) {
        var instruction = new Bits(32);
        instruction.Store(21, InstructionToken.REGISTER_BITS[GetSymbolString(3)]);
        instruction.Store(16, InstructionToken.REGISTER_BITS[GetSymbolString(5)]);
        instruction.Store(11, InstructionToken.REGISTER_BITS[GetSymbolString(1)]);
        instruction.Store(0, InstructionToken.FUNC_INSTRUCTION_BITS[GetSymbolString(0)]);
        return instruction;
    }

}
public class TypeIInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER}, true); // addi $rs, $rt, imm
    
    public TypeIInstructionToken(Symbol[] match) : base(match) { }
    
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;

    public override Bits CreateInstruction(Dictionary<string, long> symbolTextTable, Dictionary<string, long> symbolDataTable) {
        var instruction = new Bits(32);
        instruction.Store(26, InstructionToken.OPCODE_INSTRUCTION_BITS[GetSymbolString(0)]);
        instruction.Store(21, InstructionToken.REGISTER_BITS[GetSymbolString(3)]);
        instruction.Store(16, InstructionToken.REGISTER_BITS[GetSymbolString(1)]);
        Bits imm = new Bits(16);
        imm.SetFromSignedLong(long.Parse(GetSymbolString(5))); // todo check for signed/unsigned
        instruction.Store(0, imm.GetValues());
        return instruction;
    }
    
}

public class JumpInstructionToken : InstructionToken {

    public JumpInstructionToken(Symbol[] match) : base(match) { }

    public override Bits CreateInstruction(Dictionary<string, long> symbolTextTable, Dictionary<string, long> symbolDataTable) {
        Bits targetBits = new Bits(26);
        long address = symbolTextTable[GetSymbolString(1)];
        targetBits.SetFromUnsignedLong(address >> 2);
        Bits instruction = new Bits(32);
        instruction.Store(0, targetBits.GetValues());
        instruction.Store(26, OPCODE_INSTRUCTION_BITS[GetSymbolString(0)]);
        return instruction;

    }
}
public class MemoryInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER, SymbolType.OPEN_PAREN, SymbolType.NUMBER, SymbolType.CLOSE_PAREN}, true);
    
    public MemoryInstructionToken(Symbol[] match) : base(match) { }
    
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;

    public override Bits CreateInstruction(Dictionary<string, long> symbolTextTable, Dictionary<string, long> symbolDataTable) {
        var instruction = new Bits(32);
        
        // imm 0 GetSymbolString(3)
        Bits imm = new Bits(16);
        imm.SetFromSignedLong(long.Parse(GetSymbolString(3)));
        instruction.Store(0, imm.GetValues());
        // rt 16 GetSymbolString(1)
        instruction.Store(16, InstructionToken.REGISTER_BITS[GetSymbolString(1)]);
        // rs 21 GetSymbolString(5)
        instruction.Store(21, InstructionToken.REGISTER_BITS[GetSymbolString(5)]);
        instruction.Store(26, InstructionToken.OPCODE_INSTRUCTION_BITS[GetSymbolString(0)]);
        return instruction;
    }
}

public class SingleRegisterInstruction : InstructionToken {    
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER}, true);
    public SingleRegisterInstruction(Symbol[] match) : base(match) { }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        var instruction = new Bits(32);
        string func = GetSymbol(0, true).value.ToLower();
        instruction.Store(0, FUNC_INSTRUCTION_BITS[func]);
        if (func.Equals("jalr")) {
            instruction.Store(11, REGISTER_BITS["$ra"]);
            instruction.Store(21, REGISTER_BITS[GetSymbolString(1)]);
        } else if(func.Equals("jr")) {
            instruction.Store(21, REGISTER_BITS[GetSymbolString(1)]);
        }
        return instruction;
    }
}

public class LoadImmediateInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER}, true);
    public LoadImmediateInstructionToken(Symbol[] match) : base(match) { }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        Bits instruction = new Bits(32);
        instruction.Store(26, InstructionToken.OPCODE_INSTRUCTION_BITS["addi"]);
        var register = InstructionToken.REGISTER_BITS[GetSymbolString(1)];
        instruction.Store(21, register);
        instruction.Store(16, register);   
        Bits imm = new Bits(16);
        imm.SetFromSignedLong(long.Parse(GetSymbolString(3)));
        instruction.Store(0, imm);
        return instruction;
    }

}

public class SyscallInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING}, true);
    public SyscallInstructionToken(Symbol[] match) : base(match) { }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        Bits instruction = new Bits(32);
        
        return instruction;
    }

}
