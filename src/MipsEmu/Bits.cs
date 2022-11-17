
namespace MipsEmu {

    class Bits {
        private bool[] values;

        public Bits(int size) {
            values = new bool[size];
        }

        private void storeBits(int offset, bool[] bits) {
            if (offset < 0 || offset + bits.Length >= values.Length) {
                throw new IndexOutOfRangeException("Specified bit(s) are out of the available range.");
            } else {
                for (int b = 0; b < bits.Length; b++) {
                    values[offset + b] = bits[b];
                }
            }
        }

        private bool[] loadBits(int offset, int size) {
            if (offset < 0 || offset + size >= values.Length) {
                throw new IndexOutOfRangeException("Specified bit(s) are out of the available range.");
            } else {
                bool[] read = new bool[size];
                for (int b = 0; b < size; b++) {
                    read[b] = values[b + offset];
                }
                return read;
            }
        }

        public int getIntFromRange(int offset, int size) {
            return 0;
        }

        
    }
}