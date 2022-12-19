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
        {"jr", IntBits(0b001000)},
        // coprocessor unsupported
        {"syscall", new bool[] {false, false, true, true, false, false}},
    };

    public static readonly Dictionary<string, bool[]> OPCODE_INSTRUCTION_BITS = new Dictionary<string, bool[]>() {
        {"addi", IntBits(0b001000)},
        {"addiu", new bool[] {false, false, true, false, false, true}},
        {"andi", new bool[] {false, false, true, true, false, false}},
        {"lui", new bool[] {false, false, true, true, true, true}},
        {"ori", new bool[] {false, false, true, true, false, true}},
        {"slti", new bool[] {false, false, true, false, true, false}},
        {"sltiu", new bool[] {false, false, true, false, true, true}},
        {"xori", new bool[] {false, false, true, true, true, false}},
        {"beq", new bool[] {false, false, false, true, false, false}},
        {"bne", new bool[] {false, false, false, true, false, true}},
        {"lb", IntBits(0b100000)},
        {"lbu", IntBits(0b100100)},
        {"lh", IntBits(0b100001)},
        {"lw", IntBits(0b100011)},
        {"j", IntBits(0b000010)},
        {"jal", IntBits(0b000011)}

    };
    public static readonly Dictionary<string, bool[]> REGISTER_BITS = new Dictionary<string, bool[]>() {
        {"$zero", IntBits(0)}, {"$0", IntBits(0)},
        {"$at", IntBits(1)}, {"$1", IntBits(1)},
        {"$v0", IntBits(2)}, {"$2", IntBits(2)},
        {"$v1", IntBits(3)}, {"$3", IntBits(3)},
        {"$a0", IntBits(4)}, {"$4", IntBits(4)},
        {"$a1", IntBits(5)}, {"$5", IntBits(5)},  
        {"$a2", IntBits(6)}, {"$6", IntBits(6)},
        {"$a3", IntBits(7)}, {"$7", IntBits(7)},
        {"$t0", IntBits(8)}, {"$8", IntBits(8)},
        {"$t1", IntBits(9)}, {"$9", IntBits(9)},
        {"$t2", IntBits(10)}, {"$10", IntBits(10)},
        {"$t3", IntBits(11)}, {"$11", IntBits(11)},
        {"$t4", IntBits(12)}, {"$12", IntBits(12)},
        {"$t5", IntBits(13)}, {"$13", IntBits(13)},
        {"$t6", IntBits(14)}, {"$14", IntBits(14)},
        {"$t7", IntBits(15)}, {"$15", IntBits(15)},
        {"$s0", IntBits(16)}, {"$16", IntBits(16)},
        {"$s1", IntBits(17)}, {"$17", IntBits(17)},
        {"$s2", IntBits(18)}, {"$18", IntBits(18)},
        {"$s3", IntBits(19)}, {"$19", IntBits(19)},
        {"$s4", IntBits(20)}, {"$20", IntBits(20)},
        {"$s5", IntBits(21)}, {"$21", IntBits(21)},
        {"$s6", IntBits(22)}, {"$22", IntBits(22)},
        {"$s7", IntBits(23)}, {"$23", IntBits(23)},
        {"$t8", IntBits(24)}, {"$24", IntBits(24)},
        {"$t9", IntBits(25)}, {"$25", IntBits(25)},
        {"$k0", IntBits(26)}, {"$26", IntBits(26)},
        {"$k1", IntBits(27)}, {"$27", IntBits(27)},
        {"$gp", IntBits(28)}, {"$28", IntBits(28)},
        {"$sp", IntBits(29)}, {"$29", IntBits(29)},
        {"$fp", IntBits(30)}, {"$30", IntBits(30)},
        {"$ra", IntBits(31)}, {"$31", IntBits(31)},
    };

    private static bool[] IntBits(int value) {
        var bits = new Bits(6);
        bits.SetFromUnsignedInt(value);
        return bits.GetValues();
    }

    public InstructionToken(Symbol[] match) : base(match) { }
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;

    public override long GetBitLength(int alignment) => 32;

    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        throw new ParseException("Only dot directives can change the state of the assembler.");
    }
}


public class TypeRInstructionToken : InstructionToken {
    
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER}, true); // add $rs, $rt, $rd

    public TypeRInstructionToken(Symbol[] match) : base(match) { }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        var instruction = new Bits(32);
        instruction.Store(21, InstructionToken.REGISTER_BITS[GetSymbolString(3)]);
        instruction.Store(16, InstructionToken.REGISTER_BITS[GetSymbolString(5)]);
        instruction.Store(11, InstructionToken.REGISTER_BITS[GetSymbolString(1)]);
        instruction.Store(0, InstructionToken.FUNC_INSTRUCTION_BITS[GetSymbolString(0)]);
        return instruction;
    }

}
public class TypeIInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER}, true); // addi $rs, $rt, imm
    
    public TypeIInstructionToken(Symbol[] match) : base(match) { }
    
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
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
public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.NAME}, true);
    public JumpInstructionToken(Symbol[] match) : base(match) { }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        Bits targetBits = new Bits(26);
        long address = sections.GetAddress(GetSymbolString(1), sectionId, true);
        targetBits.SetFromUnsignedLong(address >> 2);
        Bits instruction = new Bits(32);
        instruction.Store(0, targetBits.GetValues());
        instruction.Store(26, OPCODE_INSTRUCTION_BITS[GetSymbolString(0)]);
        return instruction;

    }
}
public class MemoryInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER, SymbolType.OPEN_PAREN, SymbolType.REGISTER, SymbolType.CLOSE_PAREN}, true);
    
    public MemoryInstructionToken(Symbol[] match) : base(match) { }
    
    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
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
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER}, true);
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
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER}, true);
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
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME}, true);
    public SyscallInstructionToken(Symbol[] match) : base(match) { }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        Bits instruction = new Bits(32);
        
        return instruction;
    }

}