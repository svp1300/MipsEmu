using MipsEmu;

namespace MipsEmu.Emulation.Devices {

    public class Alu {

        public Bits Add(Bits a, Bits b) {
            bool[] aValue = a.GetValues();
            bool[] bValue = b.GetValues();
            if (aValue.Length != bValue.Length) {
                throw new Exception("Mismatched lengths unsupported by ALU.");
            }
            bool carryover = false;
            bool[] result = new bool[a.GetLength()];
            for (int index = 0; index < aValue.Length; index++) {
                result[index] = aValue[index] ^ bValue[index] ^ carryover;
                carryover = aValue[index] && bValue[index];
            }
            if (carryover) {
                throw new AluOverflowException("Overflow during addition");
            } else {
                return new Bits(result);
            }
        }

    }

    public class AluOverflowException : Exception {
        
        public AluOverflowException(String message) : base(message) {
            
        }
    }

}