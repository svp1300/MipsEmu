
namespace MipsEmu {

    class Bits {
        public static readonly int WORD_SIZE = 32;
        public static readonly int HALFWORD_SIZE = 16;
        private bool[] values;

        public Bits(int size) {
            values = new bool[size];
        }

        private void StoreBits(int offset, bool[] bits) {
            if (offset < 0 || offset + bits.Length >= values.Length) {
                throw new IndexOutOfRangeException("Specified bit(s) are out of the available range.");
            } else {
                for (int b = 0; b < bits.Length; b++) {
                    values[offset + b] = bits[b];
                }
            }
        }

        private bool[] LoadBits(int offset, int size) {
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

        /// TODO implement
        public int GetIntFromRange(int offset, int size) {
            return 0;
        }

        
        /// <summary>Create and return a duplicate of the values.</summary>
        public bool[] GetValues() {
            return LoadBits(0, GetLength()); // returned duplicated as modifications should be done through store
        }

        public int GetLength() {
            return values.Length;
        }

        public static Bits SignExtend(Bits source, int amount) {
            var result = new Bits(source.GetLength() + amount);
            result.StoreBits(0, source.values);
            return result;
        }

        public static Bits SignExtend16(Bits source) {
            return SignExtend(source, 16);
        }

    }
}