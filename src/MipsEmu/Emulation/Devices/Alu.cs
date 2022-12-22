using MipsEmu;

namespace MipsEmu.Emulation.Devices {

    public class Alu {

        public static Bits AddUnsigned(Bits a, Bits b) {
            var aValue = (ulong) a.GetAsUnsignedLong();
            var bValue = (ulong) b.GetAsUnsignedLong();
            // if (aValue.Length != bValue.Length) {
            //     throw new Exception("Mismatched lengths unsupported by ALU.");
            // }
            var result = new Bits(a.GetLength());
            result.SetFromUnsignedLong((long) (aValue + bValue));
            return result;
        }

        public static Bits AddSigned(Bits a, Bits b) {
            var aValue = a.GetAsSignedLong();
            var bValue = b.GetAsSignedLong();
            var result = new Bits(a.GetLength());
            result.SetFromSignedLong(aValue + bValue);
            return result;
        }

        public static Bits SubtractSigned(Bits a, Bits b) {
            var aValue = a.GetAsSignedInt();
            var bValue = b.GetAsSignedInt();
            var result = new Bits(a.GetLength());
            result.SetFromSignedInt(aValue - bValue);
            return result;
        }

        private static Bits BitwiseOperation(Bits a, Bits b, Func<bool,bool,bool> operation) {
            bool[] result = new bool[a.GetLength()];
            for(int i = 0; i < result.Length; i++) {
                result[i] = operation.Invoke(a.GetBit(i), b.GetBit((i)));
            }
            return new Bits(result);
        }

        public static Bits And(Bits a, Bits b) => BitwiseOperation(a, b, (c, d) => c && d);
        public static Bits Or(Bits a, Bits b) => BitwiseOperation(a, b, (c, d) => c || d);
        
        public static Bits Nor(Bits a, Bits b) => BitwiseOperation(a, b, (c, d) => !c && !d);

        public static Bits Xor(Bits a, Bits b) => BitwiseOperation(a, b, (c, d) => (c || d) && !(c && d));

    }

    public class AluOverflowException : Exception {
        
        public AluOverflowException(String message) : base(message) {
            
        }
    }

}