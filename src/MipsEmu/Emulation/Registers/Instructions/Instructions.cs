using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Devices;

namespace MipsEmu.Emulation.Instructions {

    public interface IInstruction {

        void Run(Hardware hardware, Bits bits);

    }

    public abstract class InstructionIType : IInstruction {

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

    public abstract class InstructionRType : IInstruction {


        public void Run(Hardware hardware, Bits bits) {
            var rs = bits.Subset(21, 5);
            var rt = bits.Subset(16, 5);
            var rd = bits.GetIntFromRange(11, 5);
            Run(hardware, rs, rt, imm);
        }

        public abstract void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd);
    }

    

    

}