using MipsEmu.Emulation.Registers;

namespace MipsEmu.Emulation.Instructions {

    public class AddInstruction : InstructionRType {

        /// <summary>Store the sum of $rs and $rt in $rd.</summary>
        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            Bits sum = hardware.alu.AddSigned(rsValue, rtValue);
            hardware.registers.SetRegisterBits(rd, sum);
        }
    }

    public class SubtractInstruction : InstructionRType {

        /// <summary>Store the sum of $rs and $rt in $rd.</summary>
        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            Bits sum = hardware.alu.SubtractSigned(rsValue, rtValue);
            hardware.registers.SetRegisterBits(rd, sum);
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