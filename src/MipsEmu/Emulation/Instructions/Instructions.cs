using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Memory;

namespace MipsEmu.Emulation.Instructions {

    interface IInstruction {

        void Run(Hardware hardware, Bits bits);

    }

    abstract class InstructionIType : IInstruction {

        public void Run(Hardware hardware, Bits bits) {
            var imm = bits.Subset(0, 16);
            var rs = bits.GetIntFromRange(16, 5);
            var rt = bits.GetIntFromRange(21, 5);

            Bits sourceValue = hardware.registers.GetValue(rs);

            Run(hardware, sourceValue, rt, imm);
        }

        public int GetStoreAddress(Bits rs, int imm) {
            return hardware.alu.Add(rs, Bits.SignExtend16(imm)).GetInt();
        }

        public abstract void Run(Hardware hardware, Bits rsValue, int rt, Bits imm);
    }

    abstract class InstructionRType : IInstruction {


        public void Run(Hardware hardware, Bits bits) {
            // var rs = bits.GetIntFromRange(6, 5);
            // var rt = bits.GetIntFromRange(11, 5);
            // var imm = bits.GetIntFromRange(16, 16);
            // Run(hardware, rs, rt, imm);
        }

        public abstract void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd);
    }

    // class AddInstruction : InstructionRType {
    //     public override void Run(Hardware hardware, int rs, int rt, int rd) {
    //         Bits sourceValue = hardware.registers.GetValue(rs);
    //         Bits targetValue = hardware.registers.GetValue(rt);
            
        
    //     }
    // }

    

    

}