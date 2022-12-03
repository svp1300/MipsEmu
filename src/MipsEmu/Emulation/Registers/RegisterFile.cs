
namespace MipsEmu.Emulation.Registers {

    public class RegisterFile {
        public static readonly int RA_INDEX = 31;
        private Register[] registers;

        public RegisterFile() {
            registers = new Register[32]; 
            for (int r = 0; r < 32; r++) {
                registers[r] = new Register();
            }
        }

        public Bits GetBits(int index) {
            if (index < 0 || index > 31) {
                throw new Exception("Index out of range for register file.");
            } else {
                return registers[index].GetBits();
            }
        }
        
        
    }

}