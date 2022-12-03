namespace MipsEmu.Emulation.Instructions {

    class AddInstruction : InstructionRType {

        /// <summary>Store the sum of $rs and $rt in $rd.</summary>
        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            Bits sum = hardware.alu.Add(rsValue, rtValue);
            hardware.registers.StoreBits(rd, sum);
        }
    }

    class JumpRegisterInstruction : InstructionRType {

        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            hardware.programCounter.StoreBits(rsValue);
        }
    }

    class JumpAndLinkRegisterInstruction : InstructionRType {

        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            Bits currentPosition = hardware.programCounter.GetBits();
            hardware.registers.StoreBits(RegisterFile.RA_INDEX, currentPosition);
            hardware.programCounter.StoreBits(rsValue);
        }
    }

}