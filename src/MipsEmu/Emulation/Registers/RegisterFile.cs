using System.Text;

namespace MipsEmu.Emulation.Registers {

    public class RegisterFile {
        public static readonly Dictionary<string, int> REGISTER_INDICES = new Dictionary<string, int>() {
            {"v0", 2}, {"a0", 4}
        };
        public static readonly int RA_INDEX = 31;
        private Register[] registers;

        public RegisterFile() {
            registers = new Register[32]; 
            for (int r = 0; r < 32; r++) {
                registers[r] = new Register();
            }
        }

        public Bits GetRegisterBits(int index) {
            if (index < 0 || index > 31) {
                throw new Exception("Index out of range for register file.");
            } else {
                return registers[index].GetBits();
            }
        }

        public void SetRegisterBits(int index, Bits bits) {
            if (index < 0 || index > 31) {
                throw new Exception("Index out of range for register file.");
            } else {
                registers[index].SetBits(bits);
            }
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine("Register File:");
            for (int register = 0; register < 32; register++) {
                var registerValue = GetRegisterBits(register).GetAsUnsignedLong();
                builder.AppendLine($"\t{register}:\t{registerValue:X}");
            }
            return builder.ToString();
        }
        
        
    }

}