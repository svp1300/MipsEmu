namespace MipsEmu.Emulation.Instructions {


    public abstract class LoadInstruction : InstructionIType {  

        /// <summary>Retrieve the contents at $rs + imm and store it in $rt.</summary>
        public void Load(Hardware hardware, Bits rsValue, int rt, Bits imm, int amount) {
            int address = CalculateStoreAddress(hardware, rsValue, imm).GetAsSignedInt();
            Bits contents = hardware.memory.LoadBits(address, amount);
            hardware.registers.SetRegisterBits(rt, contents.SignExtend(32 - amount));
        }

    }

    public class LoadByteInstruction : LoadInstruction {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Load(hardware, rsValue, rt, imm, 8);
        }

    }

    public class LoadHalfWordInstruction : LoadInstruction {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Load(hardware, rsValue, rt, imm, 16);
        }
        
    }

    public class LoadWordInstruction : LoadInstruction {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Load(hardware, rsValue, rt, imm, 32);
        }
        
    }

    public abstract class StoreInstruction : InstructionIType {

        /// <summary>Store the contents of $rt into $rs + imm.</summary>
        public void Store(Hardware hardware, Bits rsValue, int rt, Bits imm, int storeAmount) {
            int address = CalculateStoreAddress(hardware, rsValue, imm).GetAsSignedInt();
            Bits rtValue = hardware.registers.GetRegisterBits(rt);
            hardware.memory.StoreBits(address, rtValue, storeAmount);
        }
    }

    public class StoreByteInstruction : StoreInstruction {

        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Store(hardware, rsValue, rt, imm, 8);
        }
    }

    public class StoreHalfWordInstruction : StoreInstruction {

        /// <summary>Store the contents of $rt into $rs + imm.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Store(hardware, rsValue, rt, imm, 16);
        }
    }

    public class StoreWordInstruction : StoreInstruction {

        /// <summary>Store the contents of $rt into $rs + imm.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Store(hardware, rsValue, rt, imm, 32);
        }
    }

    public class AddImmediateInstruction : InstructionIType {

        /// <summary>Add $rs and imm then store in rt.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Bits sum = hardware.alu.AddSigned(rsValue, imm);
            hardware.registers.SetRegisterBits(rt, sum);
        }
    }

    public class SubImmediateInstruction : InstructionIType {

        /// <summary>Subtract $rs and imm then store in rt.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Bits result = hardware.alu.SubtractSigned(rsValue, imm);
            hardware.registers.SetRegisterBits(rt, result);
        }
    }

}