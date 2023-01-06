using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Devices;
using MipsEmu.Emulation;

namespace MipsEmu.Emulation.Instructions {

    public interface IInstruction {

        void Run(Hardware hardware, Bits bits);
        string InfoString(Bits bits);
    }

    public abstract class InstructionIType : IInstruction {
        public static readonly Interval RS_BITS = new Interval(16, 5);
        public virtual void Run(Hardware hardware, Bits bits) {
            var imm = bits.LoadBits(0, 16);
            var rt = bits.GetUnsignedIntFromRange(16, 5);
            var rs = bits.GetUnsignedIntFromRange(21, 5);
            
            Bits rsValue = hardware.registers.GetRegisterBits(rs);

            Run(hardware, rsValue, rt, imm);
        }

        public Bits CalculateStoreAddress(Hardware hardware, Bits rsValue, Bits imm) {
            return Alu.AddSigned(rsValue, imm.SignExtend16(false));
        }

        public abstract void Run(Hardware hardware, Bits rsValue, int rt, Bits imm);

        public string InfoString(Bits bits) {
            var imm = bits.LoadBits(0, 16).GetAsSignedLong();
            var rt = bits.LoadBits(16, 5).GetAsUnsignedLong();
            var rs = bits.LoadBits(21, 5).GetAsUnsignedLong();
            return $"I\t{GetType().Name}\t\t{rs}\t{rt}\t{imm}";
        }
    }

    public abstract class InstructionRType : IInstruction {


        public virtual void Run(Hardware hardware, Bits bits) {
            var rs = bits.GetUnsignedIntFromRange(21, 5);
            var rt = bits.GetUnsignedIntFromRange(16, 5);
            var rd = bits.GetUnsignedIntFromRange(11, 5);
            Run(hardware, hardware.registers.GetRegisterBits(rs), hardware.registers.GetRegisterBits(rt), rd);
        }

        public abstract void Run(Hardware hardware, Bits rsValue, Bits rtValue, int rd);

        public string InfoString(Bits bits) {
            var rd = bits.LoadBits(11, 5).GetAsSignedLong();
            var rt = bits.LoadBits(16, 5).GetAsUnsignedLong();
            var rs = bits.LoadBits(21, 5).GetAsUnsignedLong();
            return $"R\t{GetType().Name}\t\t{rs}\t{rt}\t{rd}";
        }
    }

    public abstract class InstructionJType : IInstruction {

        public virtual void Run(Hardware hardware, Bits instruction) {
            long highOrder = 0xF00000 & hardware.programCounter.GetBits().GetAsUnsignedLong();
            long psuedoAddress = instruction.LoadBits(0, 26).GetAsUnsignedLong();
            var addressValue = (psuedoAddress << 2) + highOrder;
            if (addressValue == hardware.programCounter.GetBits().GetAsUnsignedLong()) {
                throw new Exception("Infinite loop reached.");
            }
            var address = new Bits(32);
            address.SetFromUnsignedLong(addressValue);
            RunJump(hardware, address);
        }

        public abstract void RunJump(Hardware hardware, Bits address);

        public string InfoString(Bits bits) {
            long psuedoAddress = bits.LoadBits(0, 26).GetAsUnsignedLong();
            return $"J\t{GetType().Name}\t\t{psuedoAddress}";
        }
    }

    public abstract class BranchingInstruction : InstructionIType {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            if (ShouldBranch(rsValue.GetAsUnsignedLong(), rt)) {
                var pcAddress = hardware.programCounter.GetBits().GetAsUnsignedLong();
                var offset = imm.GetAsSignedLong();
                hardware.programCounter.SetFromUnsignedLong(pcAddress + offset);
            }
        }

        public abstract bool ShouldBranch(long rsValue, long rtValue);
    }

}