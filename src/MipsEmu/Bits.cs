using System.Text;

namespace MipsEmu {

    public class Bits {
        public static readonly Interval OPCODE_INTERVAL = new Interval(26, 6);
        public static readonly Interval FUNC_INTERVAL = new Interval(0, 6);

        public static readonly int WORD_SIZE = 32;
        public static readonly int HALFWORD_SIZE = 16;

        private bool[] values;


        public Bits(long size) {
            values = new bool[size];
        }

        public Bits(bool[] values) {
            this.values = new bool[values.Length];
            for (long l = 0; l < values.Length; l++) {
                this.values[l] = values[l];
            }
        }

        public void Store(long offset, bool[] bits) {
            if (offset < 0 || offset + bits.Length > values.Length) {
                throw new IndexOutOfRangeException("Specified bit(s) are out of the available range.");
            } else {
                for (long b = 0; b < bits.Length; b++) {
                    values[offset + b] = bits[b];
                }
            }
        }

        public void Store(long offset, Bits bits) => Store(offset, bits.GetValues());

        public void SetBits(long offset, Bits bits) => Store(offset, bits.values);
        

        public bool[] Load(long offset, long size) {
            if (offset < 0 || offset + size > values.Length) {
                throw new IndexOutOfRangeException("Specified bit(s) are out of the available range.");
            } else {
                bool[] read = new bool[size];
                for (long b = 0; b < size; b++) {
                    read[b] = values[b + offset];
                }
                return read;
            }
        }

        public float GetAsFloat() {
            return 0;
        }

        public string GetAsString() {
            return "";
        }
        
        public Bits LoadBits(long offset, int size) {
            return new Bits(Load(offset, size));
        }

        public bool GetBit(int index) {
            return values[index];
        }

        /// <summary>Grabs a subset from the bits and returns the signed integer representation of that subset.</summary>
        public int GetSignedIntFromRange(int offset, int size) => LoadBits(offset, size).GetAsSignedInt();

        /// <summary>Grabs a subset from the bits and returns the signed integer representation of that subset.</summary>
        public int GetSignedIntFromRange(Interval interval) => GetSignedIntFromRange(interval.start, interval.length);

        /// <summary>Grabs a subset from the bits and returns the unsigned integer representation of that subset.</summary>
        public int GetUnsignedIntFromRange(int offset, int duration) => LoadBits(offset, duration).GetAsUnsignedInt();

        /// <summary>Grabs a subset from the bits and returns the unsigned integer representation of that subset.</summary>
        public int GetUnsignedIntFromRange(Interval interval) => GetUnsignedIntFromRange(interval.start, interval.length);

      
        /// <summary>Get the value as an unsigned integer.</summary>
        public int GetAsUnsignedInt() => (int) GetAsUnsignedLong();
        /// <summary>Get the value as an unsigned long.</summary>
        public long GetAsUnsignedLong() {
            long sum = 0;
            for (long index = 0; index < values.Length; index++) {
                if (values[index]) {
                    sum += (int) Math.Pow(2, values.Length - index - 1);
                }
            }
            return sum;
        }

        /// <summary>Get the signed integer value using two's compliment.</summary>
        public int GetAsSignedInt() => (int) GetAsSignedLong();
        /// <summary>Get the signed long value using two's compliment.</summary>
        public long GetAsSignedLong() {
            long sum = 0;
            if (values[values.Length - 1]) {
                sum = -((int) Math.Pow(2, values.Length - 1));
            }
            for (long index = 0; index <= values.Length - 2; index++) {
                if (values[index])
                    sum += (int) Math.Pow(2, index);
            }
            return sum;
        }

        public void SetFromUnsignedLong(long number) {
            int endianOffset = values.Length - 1;
            int i = 0;
            while(i < values.Length && number > 0) {
                long pow = (long) Math.Pow(2, endianOffset - i);
                var fits = number - pow >= 0;
                values[i] = fits;
                if (fits) {
                    number -= pow;
                }
                i++;
            }
            while(i < values.Length) {
                values[i++] = false;
            }
        }

        public void SetFromSignedLong(long number) {
            int endianOffset = values.Length - 1;
            if (number < 0) {
                values[0] = true;
                number = (long) Math.Pow(2, values.Length - 1) - Math.Abs(number);
            } else {
                values[0] = false;
            }
            int p;
            for (p = 1; p < values.Length && number > 0; p++) {
                var pow = (long) Math.Pow(2, endianOffset - p);
                var geq = number - pow >= 0;
                values[p] = geq;
                if (geq) {
                    number -= pow;
                }
            }
            while(p < values.Length) {
                values[p++] = false;
            }
        }

        public void SetFromSignedInt(int number) => SetFromSignedLong((int) number);
        public void SetFromUnsignedInt(int number) => SetFromUnsignedLong((int) number);
        
        public override string ToString() {
            var builder = new StringBuilder(values.Length);
            for(int bit = values.Length - 1; bit >= 0; bit--) {
                builder.Append(values[bit] ? "1" : "0");
            }
            return builder.ToString();
        }
        
        public Bits SignExtend(int amount) {
            var result = new Bits(GetLength() + amount);
            result.Store(amount, values);
            return result;
        }

        public Bits SignExtend16() {
            return SignExtend(16);
        }
        
        public long GetLength() {
            return values.Length;
        }
  
        public bool[] GetValues() {
            return values;
        }


        public static int BitsToUnsignedInt(bool[] bits) =>  new Bits(bits).GetAsUnsignedInt();
            

    }

}