using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Devices;
using MipsEmu.Emulation;

namespace MipsEmu.Emulation.Instructions {

    public interface IInstruction {

        void Run(Hardware hardware, Bits bits);

    }

    public abstract class InstructionIType : IInstruction {

        public void Run(Hardware hardware, Bits bits) {
            var imm = bits.GetBits(0, 16);
            var rs = bits.GetSignedIntFromRange(16, 5);
            var rt = bits.GetSignedIntFromRange(21, 5);

            Bits sourceValue = hardware.registers.GetRegisterBits(rs);

            Run(hardware, sourceValue, rt, imm);
        }

        public Bits CalculateStoreAddress(Hardware hardware, Bits rsValue, Bits imm) {
            return hardware.alu.Add(rsValue, imm.SignExtend16());
        }

        public abstract void Run(Hardware hardware, Bits rsValue, int rt, Bits imm);
    }

    public abstract class InstructionRType : IInstruction {


        public void Run(Hardware hardware, Bits bits) {
            var rs = bits.GetBits(21, 5);
            var rt = bits.GetBits(16, 5);
            var rd = bits.GetSignedIntFromRange(11, 5);
            Run(hardware, rs, rt, rd);
        }

        public abstract void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd);
    }

    

    

}