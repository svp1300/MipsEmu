using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Memory;
namespace MipsEmu.Emulation.Instructions {

    interface Instruction {

        public void Run(Hardware hardware, Bits bits);

    }

    abstract class ITypeInstruction : Instruction {

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

        public abstract void Run(Hardware hardware, Bits rsVal, Bits rt, Bits imm);
    }

    abstract class RTypeInstruction : Instruction {


        public void Run(Hardware hardware, Bits bits) {
            // var rs = bits.GetIntFromRange(6, 5);
            // var rt = bits.GetIntFromRange(11, 5);
            // var imm = bits.GetIntFromRange(16, 16);
            // Run(hardware, rs, rt, imm);
        }

        public abstract void Run(Hardware hardware, int rs, int rt, int rd);
    }

    // // TODO move
    // class AddInstruction : RTypeInstruction {
    //     public override void Run(Hardware hardware, int rs, int rt, int rd) {
    //         Bits sourceValue = hardware.registers.GetValue(rs);
    //         Bits targetValue = hardware.registers.GetValue(rt);
            
    //         // Bits source = hardware.regis
            
    //     }
    // }

    class LoadWordInstruction : ITypeInstruction {  

        /// <summary>Retrieve the contents at $rs + imm and store it in $rt.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            int address = GetStoreAddress(rs, imm);
            Bits contents = hardware.memory.LoadBits(address, Bits.WORD_SIZE);
            hardware.registers.StoreValue(rt, contents);
        }

    }

    class StoreWordInstruction {

        /// <summary>Store the contents of $rt into $rs + imm.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            int address = GetStoreAddress(rs, imm);
            Bits rtValue = hardware.memory.registers.GetValue(rt);
            hardware.memory.StoreBits(address, rtValue);
        }
    }

    class AddImmediateInstruction {

        /// <summary>Add $rs and imm then store in rt.</summary>
        public override void Run(Hardware hardware, Bits rsValue, int rt, Bits imm) {
            Bits sum = hardware.alu.Add(rsValue, imm);
            hardware.registers.StoreValue(rt, sum);
        }
    }

}