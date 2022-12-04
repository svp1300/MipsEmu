namespace MipsEmu.Emulation.Instructions {


    public class LoadWordInstruction : InstructionIType {  

        /// <summary>Retrieve the contents at $rs + imm and store it in $rt.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            int address = CalculateStoreAddress(hardware, rsValue, imm).GetAsSignedInt();
            Bits contents = hardware.memory.LoadBits(address, Bits.WORD_SIZE);
            hardware.registers.SetRegisterBits(rt, contents);
        }

    }

    public class StoreWordInstruction : InstructionIType {

        /// <summary>Store the contents of $rt into $rs + imm.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            int address = CalculateStoreAddress(hardware, rsValue, imm).GetAsSignedInt();
            Bits rtValue = hardware.registers.GetRegisterBits(rt);
            hardware.memory.StoreBits(address, rtValue);
        }
    }

    public class AddImmediateInstruction : InstructionIType {

        /// <summary>Add $rs and imm then store in rt.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Bits sum = hardware.alu.Add(rsValue, imm);
            hardware.registers.SetRegisterBits(rt, sum);
        }
    }

    // public class SubImmediateInstruction : InstructionIType {

    //     /// <summary>Subtract $rs and imm then store in rt.</summary>
    //     public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
    //         Bits result = hardware.alu.Subtract(rsValue, imm);
    //         hardware.registers.SetRegisterBits(rt, result);
    //     }
    // }

}