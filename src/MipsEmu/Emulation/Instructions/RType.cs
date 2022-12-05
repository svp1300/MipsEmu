using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Devices;

namespace MipsEmu.Emulation.Instructions {

    public class AddInstruction : InstructionRType {

        /// <summary>Store the sum of $rs and $rt in $rd.</summary>
        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            Bits sum = Alu.AddSigned(rsValue, rtValue);
            hardware.registers.SetRegisterBits(rd, sum);
        }
    }
    
    public class SubtractInstruction : InstructionRType {

        /// <summary>Store the sum of $rs and $rt in $rd.</summary>
        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            Bits sum = Alu.AddSigned(rsValue, rtValue);
            hardware.registers.SetRegisterBits(rd, sum);
        }
    }

    public class AndInstruction : InstructionRType {

        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            hardware.registers.SetRegisterBits(rd, Alu.And(rsValue, rtValue));
        }
    }

    public class NorInstruction : InstructionRType {

        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            hardware.registers.SetRegisterBits(rd, Alu.Nor(rsValue, rtValue));
        }
    }

    public class XorInstruction : InstructionRType {

        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            hardware.registers.SetRegisterBits(rd, Alu.Xor(rsValue, rtValue));
        }
    }

    public class OrInstruction : InstructionRType {

        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            hardware.registers.SetRegisterBits(rd, Alu.Or(rsValue, rtValue));
        }
    }

    public class JumpRegisterInstruction : InstructionRType {

        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            hardware.programCounter.SetBits(rsValue);
        }
    }

    public class JumpAndLinkRegisterInstruction : InstructionRType {

        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            Bits currentPosition = hardware.programCounter.GetBits();
            hardware.registers.SetRegisterBits(RegisterFile.RA_INDEX, currentPosition);
            hardware.programCounter.SetBits(rsValue);
        }
    }

}