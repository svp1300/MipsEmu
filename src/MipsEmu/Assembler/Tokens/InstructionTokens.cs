namespace MipsEmu.Assembler.Tokens;

using MipsEmu.Emulation.Instructions;

public abstract class InstructionToken : Token {
    public static readonly Dictionary<string, bool[]> FUNC_INSTRUCTION_BITS = new Dictionary<string, bool[]>() {
        {"add", IntBits(32, 6)},
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
        {"jalr", IntBits(9, 6)},
        {"jr", IntBits(8, 6)},
        // coprocessor unsupported
        {"syscall", IntBits(12, 6)},
    };

    public static readonly Dictionary<string, bool[]> OPCODE_INSTRUCTION_BITS = new Dictionary<string, bool[]>() {
        {"addi", IntBits(8, 6)},
        {"sub", IntBits(34, 6)},
        {"addiu", new bool[] {false, false, true, false, false, true}},
        {"andi", new bool[] {false, false, true, true, false, false}},
        {"lui", new bool[] {false, false, true, true, true, true}},
        {"ori", new bool[] {false, false, true, true, false, true}},
        {"slti", new bool[] {false, false, true, false, true, false}},
        {"sltiu", new bool[] {false, false, true, false, true, true}},
        {"xori", new bool[] {false, false, true, true, true, false}},
        {"beq", new bool[] {false, false, false, true, false, false}},
        {"bne", new bool[] {false, false, false, true, false, true}},
        {"lb", IntBits(0b100000, 6)},
        {"sw", IntBits(0b101011, 6)},
        {"lbu", IntBits(0b100100, 6)},
        {"lh", IntBits(0b100001, 6)},
        {"lw", IntBits(35, 6)},
        {"j", IntBits(2, 6)},
        {"jal", IntBits(3, 6)}

    };
    public static readonly Dictionary<string, bool[]> REGISTER_BITS = new Dictionary<string, bool[]>() {
        {"$zero", IntBits(0, 5)}, {"$0", IntBits(0, 5)},
        {"$at", IntBits(1, 5)}, {"$1", IntBits(1, 5)},
        {"$v0", IntBits(2, 5)}, {"$2", IntBits(2, 5)},
        {"$v1", IntBits(3, 5)}, {"$3", IntBits(3, 5)},
        {"$a0", IntBits(4, 5)}, {"$4", IntBits(4, 5)},
        {"$a1", IntBits(5, 5)}, {"$5", IntBits(5, 5)},  
        {"$a2", IntBits(6, 5)}, {"$6", IntBits(6, 5)},
        {"$a3", IntBits(7, 5)}, {"$7", IntBits(7, 5)},
        {"$t0", IntBits(8, 5)}, {"$8", IntBits(8, 5)},
        {"$t1", IntBits(9, 5)}, {"$9", IntBits(9, 5)},
        {"$t2", IntBits(10, 5)}, {"$10", IntBits(10, 5)},
        {"$t3", IntBits(11, 5)}, {"$11", IntBits(11, 5)},
        {"$t4", IntBits(12, 5)}, {"$12", IntBits(12, 5)},
        {"$t5", IntBits(13, 5)}, {"$13", IntBits(13, 5)},
        {"$t6", IntBits(14, 5)}, {"$14", IntBits(14, 5)},
        {"$t7", IntBits(15, 5)}, {"$15", IntBits(15, 5)},
        {"$s0", IntBits(16, 5)}, {"$16", IntBits(16, 5)},
        {"$s1", IntBits(17, 5)}, {"$17", IntBits(17, 5)},
        {"$s2", IntBits(18, 5)}, {"$18", IntBits(18, 5)},
        {"$s3", IntBits(19, 5)}, {"$19", IntBits(19, 5)},
        {"$s4", IntBits(20, 5)}, {"$20", IntBits(20, 5)},
        {"$s5", IntBits(21, 5)}, {"$21", IntBits(21, 5)},
        {"$s6", IntBits(22, 5)}, {"$22", IntBits(22, 5)},
        {"$s7", IntBits(23, 5)}, {"$23", IntBits(23, 5)},
        {"$t8", IntBits(24, 5)}, {"$24", IntBits(24, 5)},
        {"$t9", IntBits(25, 5)}, {"$25", IntBits(25, 5)},
        {"$k0", IntBits(26, 5)}, {"$26", IntBits(26, 5)},
        {"$k1", IntBits(27, 5)}, {"$27", IntBits(27, 5)},
        {"$gp", IntBits(28, 5)}, {"$28", IntBits(28, 5)},
        {"$sp", IntBits(29, 5)}, {"$29", IntBits(29, 5)},
        {"$fp", IntBits(30, 5)}, {"$30", IntBits(30, 5)},
        {"$ra", IntBits(31, 5)}, {"$31", IntBits(31, 5)},
    };

    private string[] validNames;

    public InstructionToken(Symbol[] match, string[] validNames) : base(match) {
        this.validNames = validNames;
    }
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;

    public override long GetBitLength(int alignment) => 32;

    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        throw new ParseException("Only dot directives can change the state of the assembler.");
    }

    public override bool CheckValidMatch() {
        var name = GetSymbolString(0);
        foreach(var directiveName in validNames) {
            if (directiveName.Equals(name))
                return true;
        }
        return false;
    }

    private static bool[] IntBits(int value, int size) {
        var bits = new Bits(size);
        bits.SetFromUnsignedInt(value);
        return bits.GetValues();
    }
}


public class TypeRInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER}, true); // add $rs, $rt, $rd
    public static readonly string[] VALID_NAMES = new string[] {"add", "sub"};
    public TypeRInstructionToken(Symbol[] match) : base(match, VALID_NAMES) { }

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
    public static readonly string[] VALID_NAMES = new string[] {"addi"};
    public TypeIInstructionToken(Symbol[] match) : base(match, VALID_NAMES) { }
    
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
    public static readonly string[] VALID_NAMES = new string[] {"j", "jal"};
    public JumpInstructionToken(Symbol[] match) : base(match, VALID_NAMES) { }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        Bits targetBits = new Bits(26);
        long address = sections.GetLabelAddress(GetSymbolString(1), sectionId, true);
        if (address == -1)
            throw new ParseException($"Unable to find label {GetSymbolString(1)}");
        targetBits.SetFromUnsignedLong(address >> 2);
        Bits instruction = new Bits(32);
        instruction.Store(0, targetBits.GetValues());
        instruction.Store(26, OPCODE_INSTRUCTION_BITS[GetSymbolString(0)]);
        return instruction;

    }
}
public class MemoryInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER, SymbolType.OPEN_PAREN, SymbolType.REGISTER, SymbolType.CLOSE_PAREN}, true);
    public static readonly string[] VALID_NAMES = new string[] {"lw", "lh", "lb", "sw", "sh", "sb"};
    public MemoryInstructionToken(Symbol[] match) : base(match, VALID_NAMES) { }
    
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

public class RegisterImmediateInstructionToken :InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER}, true);
    public static readonly string[] VALID_NAMES = new string[] {"lui"};

    public RegisterImmediateInstructionToken(Symbol[] match) : base(match, VALID_NAMES) { }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        var instruction = new Bits(32);
        instruction.Store(26, OPCODE_INSTRUCTION_BITS[GetSymbolString(0)]);
        instruction.Store(16, InstructionToken.REGISTER_BITS[GetSymbolString(1)]);
        var immediate = new Bits(16);
        immediate.SetFromSignedLong(long.Parse(GetSymbolString(3)));
        instruction.Store(0, immediate);
        return instruction;
    }
}
public class SingleRegisterInstructionToken : InstructionToken {    
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER}, true);
    public static readonly string[] VALID_NAMES = new string[] {"jr", "jalr"};
    public SingleRegisterInstructionToken(Symbol[] match) : base(match, VALID_NAMES) { }

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
        instruction.Store(26, new bool[] {false, false, false, false, false, false});
        return instruction;
    }
}

public class PseudoInstructionToken : InstructionToken {
    public static readonly string[] VALID_NAMES = new string[] {"la", "mult", "move", "li"};
    public PseudoInstructionToken(Symbol[] match) : base(match, VALID_NAMES) { }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        throw new NotImplementedException();
    }

}

public class SyscallInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME}, true);
    public SyscallInstructionToken(Symbol[] match) : base(match, new string[]{"syscall"}) { }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        Bits instruction = new Bits(32);
        instruction.Store(0, InstructionToken.FUNC_INSTRUCTION_BITS[GetSymbolString(0, true)]);
        return instruction;
    }

}

public class BranchInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NAME}, true);

    public static readonly string[] VALID_NAMES = new string[] {"beq", "bne", "bge"};
    public BranchInstructionToken(Symbol[] match) : base(match, VALID_NAMES) { }

    public override Bits MakeValueBits(UnlinkedProgram unlinked, int sectionId) {
        Bits instruction = new Bits(32);
        instruction.Store(26, InstructionToken.OPCODE_INSTRUCTION_BITS[GetSymbolString(0, true)]);
        // TODO finish
        instruction.Store(21, REGISTER_BITS[GetSymbolString(1)]);
        instruction.Store(16, REGISTER_BITS[GetSymbolString(3)]);
        long address = unlinked.GetLabelAddress(GetSymbolString(5), sectionId, false);
        return instruction;
    }
}