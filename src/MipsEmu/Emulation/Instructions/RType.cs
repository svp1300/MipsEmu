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
            if (rsValue.Equals(hardware.programCounter.GetBits())) { 
                throw new Exception("Infinite loop reached.");
            }
            hardware.programCounter.SetBits(rsValue);
        }
    }

    public class JumpAndLinkRegisterInstruction : InstructionRType {

        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            Bits currentPosition = hardware.programCounter.GetBits();
            if (rsValue.Equals(currentPosition)) { 
                throw new Exception("Infinite loop reached.");
            }
            hardware.registers.SetRegisterBits(RegisterFile.RA_INDEX, currentPosition);
            hardware.programCounter.SetBits(rsValue);
        }
    }

    public class SetOnLessThanInstruction : InstructionRType {
        public override void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd) {
            Bits result = new Bits(32);
            if (rsValue.GetAsSignedLong() < rtValue.GetAsSignedLong()) {
                result.SetBit(31, true);
            }
            hardware.registers.SetRegisterBits(rd, result);    
        }
    }
    
    public class BranchOnEqualInstruction :  BranchingInstruction {

        public override bool ShouldBranch(long rsValue, long rtValue) {
            return rsValue == rtValue;
        }
        
    }

    public class BranchOnNotEqualInstruction :  BranchingInstruction {

        public override bool ShouldBranch(long rsValue, long rtValue) {
            return rsValue != rtValue;
        }
        
    }
}

