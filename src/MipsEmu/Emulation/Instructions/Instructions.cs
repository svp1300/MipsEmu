using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Memory;
namespace MipsEmu.Emulation.Instructions {

    interface Instruction {

        public void Run(Hardware hardware, Bits bits);

    }

    abstract class ITypeInstruction : Instruction {

        public void Run(Hardware hardware, Bits bits) {
            var imm = bits.getIntFromRange(0, 16);
            var rs = bits.getIntFromRange(16, 5);
            var rt = bits.getIntFromRange(21, 5);
            Run(hardware, rs, rt, imm);
        }

        public abstract void Run(Hardware hardware, int rs, int rt, int imm);
    }

    abstract class RTypeInstruction : Instruction {


        public void Run(Hardware hardware, Bits bits) {
            var rs = bits.getIntFromRange(6, 5);
            var rt = bits.getIntFromRange(11, 5);
            var imm = bits.getIntFromRange(16, 16);
            Run(hardware, rs, rt, imm);
        }

        public abstract void Run(Hardware hardware, int rs, int rt, int rd);
    }

    // TODO move
    class AddInstruction : RTypeInstruction {
        public override void Run(Hardware hardware, int rs, int rt, int rd) {
            Bits sourceValue = hardware.registers.GetValue(rs);
            Bits targetValue = hardware.registers.GetValue(rt);
            
            // Bits source = hardware.regis
            
        }
    }

}