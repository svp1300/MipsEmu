using MipsEmu.Emulation.Devices;

namespace MipsEmu.Emulation.Instructions {

    public abstract class LoadInstruction : InstructionIType {  

        /// <summary>Retrieve the contents at $rs + imm and store it in $rt.</summary>
        public void Load(Hardware hardware, Bits rsValue, int rt, Bits imm, int amount) {
            int address = CalculateStoreAddress(hardware, rsValue, imm).GetAsSignedInt();
            Bits contents = hardware.memory.LoadBytes(address, amount);
            hardware.registers.SetRegisterBits(rt, contents.SignExtend(32 - amount * 8, false));
        }

    }

    public class LoadUpperImmediateInstruction : InstructionIType {
        
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            var upper = new Bits(32);
            upper.Store(0, imm.Load(0, 16));
            hardware.registers.SetRegisterBits(rt, upper);
        }

    }
    
    public class LoadByteInstruction : LoadInstruction {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Load(hardware, rsValue, rt, imm, 1);
        }

    }

    public class LoadHalfWordInstruction : LoadInstruction {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Load(hardware, rsValue, rt, imm, 2);
        }
        
    }

    public class LoadWordInstruction : LoadInstruction {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Load(hardware, rsValue, rt, imm, 4);
        }
        
    }

    public abstract class StoreInstruction : InstructionIType {

        /// <summary>Store the contents of $rt into $rs + imm.</summary>
        public void Store(Hardware hardware, Bits rsValue, int rt, Bits imm, int storeAmount) {
            Bits rtValue = hardware.registers.GetRegisterBits(rt);
            var address = CalculateStoreAddress(hardware, rsValue, imm).GetAsUnsignedLong();
            hardware.memory.StoreBytes(address, rtValue, storeAmount);
        }
    }

    public class StoreByteInstruction : StoreInstruction {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Store(hardware, rsValue, rt, imm, 1);
        }
    }

    public class StoreHalfWordInstruction : StoreInstruction {

        /// <summary>Store the contents of $rt into $rs + imm.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Store(hardware, rsValue, rt, imm, 2);
        }
    }

    public class StoreWordInstruction : StoreInstruction {

        /// <summary>Store the contents of $rt into $rs + imm.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Store(hardware, rsValue, rt, imm, 4);
        }
    }

    public class AddImmediateInstruction : InstructionIType {

        /// <summary>Add $rs and imm then store in rt.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Bits sum = Alu.AddSigned(rsValue, imm);
            hardware.registers.SetRegisterBits(rt, sum);
        }
    }

    public class AndImmediateInstruction : InstructionIType {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            hardware.registers.SetRegisterBits(rt, Alu.And(rsValue, imm.SignExtend16(false)));
        }
    }

    public class OrImmediateInstruction : InstructionIType {
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            hardware.registers.SetRegisterBits(rt, Alu.Or(rsValue, imm.SignExtend16(false)));
        }
    }

    public class XorImmediateInstruction : InstructionIType {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            hardware.registers.SetRegisterBits(rt, Alu.Xor(rsValue, imm.SignExtend16(false)));
        }
    }

    public class SetOnLessThanImmediateInstruction : InstructionIType {
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Bits result = new Bits(32);
            if (rsValue.GetAsSignedLong() < imm.GetAsSignedLong()) {
                result.SetBit(31, true);
            }
            hardware.registers.SetRegisterBits(rt, result);    
        }
    }

}