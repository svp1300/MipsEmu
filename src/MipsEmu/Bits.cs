using System;

namespace MipsEmu {

    public class Bits {
        public static readonly int WORD_SIZE = 32;
        public static readonly int HALFWORD_SIZE = 16;
        private bool[] values;

        public Bits(int size) {
            values = new bool[size];
        }

        public Bits(bool[] values) {
            this.values = values;
        }

        private void Store(int offset, bool[] bits) {
            if (offset < 0 || offset + bits.Length >= values.Length) {
                throw new IndexOutOfRangeException("Specified bit(s) are out of the available range.");
            } else {
                for (int b = 0; b < bits.Length; b++) {
                    values[offset + b] = bits[b];
                }
            }
        }

        public void SetBits(int offset, Bits bits) {
            Store(offset, bits.values);
        }

        public bool[] Load(int offset, int size) {
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

        public Bits GetBits(int offset, int size) {
            return new Bits(Load(offset, size));
        }

        /// <summary>Grabs a subset from the bits and returns the signed integer representation of that subset.</summary>
        public int GetSignedIntFromRange(int offset, int size) {
            return GetBits(offset, size).GetAsSignedInt();
        }

      
        /// <summary>Get the value as an unsigned integer.</summary>
        public int GetAsUnsignedInt() {
            int sum = 0;
            for (int index = 0; index < values.Length - 1; index++) {
                if (values[index])
                    sum += (int) Math.Pow(2, index);
            }
            return sum;
        }

        /// <summary>Get the signed integer value using two's compliment.</summary>
        public int GetAsSignedInt() {
            int sum = 0;
            if (values[values.Length - 1]) {
                sum = -((int) Math.Pow(2, values.Length - 1));
            }
            for (int index = 0; index < values.Length - 1; index++) {
                if (values[index])
                    sum += (int) Math.Pow(2, index);
            }
            return sum;
        }
        
        public static Bits SignExtend(Bits source, int amount) {
            var result = new Bits(source.GetLength() + amount);
            result.Store(0, source.values);
            return result;
        }

        public static Bits SignExtend16(Bits source) {
            return SignExtend(source, 16);
        }
        
        public int GetLength() {
            return values.Length;
        }
  
        /// <summary>Create and return a duplicate of the values.</summary>
        public bool[] GetValues() {
            return Load(0, GetLength());
        }

    }
}