namespace MipsEmu.Emulation.Instructions {


    public class LoadWordInstruction : InstructionIType {  

        /// <summary>Retrieve the contents at $rs + imm and store it in $rt.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            int address = GetStoreAddress(rs, imm);
            Bits contents = hardware.memory.LoadBits(address, Bits.WORD_SIZE);
            hardware.registers.SetBits(rt, contents);
        }

    }

    public class StoreWordInstruction : InstructionIType {

        /// <summary>Store the contents of $rt into $rs + imm.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            int address = GetStoreAddress(rs, imm);
            Bits rtValue = hardware.memory.registers.GetBits(rt);
            hardware.memory.StoreBits(address, rtValue);
        }
    }

    public class AddImmediateInstruction : InstructionIType {

        /// <summary>Add $rs and imm then store in rt.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Bits sum = hardware.alu.Add(rsValue, imm);
            hardware.registers.SetBits(rt, sum);
        }
    }

}