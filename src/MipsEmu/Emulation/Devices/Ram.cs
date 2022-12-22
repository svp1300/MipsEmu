
namespace MipsEmu.Emulation.Devices {

    public class Ram {
        public static readonly long TEXT_START = 0x00400000;
        public static readonly long DATA_START = 0x10000000;
        public static readonly long USER_ACCESSIBLE_START = TEXT_START;
        public static readonly long USER_ACCESSIBLE_END = 0x80000000;
        private Bits memory;

        public Ram(Bits wrap) {
            memory = wrap;
        }
        public Ram(long userMemorySize) {
            memory = new Bits(userMemorySize);
        }

        public void StoreBits(long address, Bits bits, long amount) {
            memory.SetBits(address, new Bits(bits.Load(0, amount)));
        }

        public void StoreBits(long address, Bits bits) => memory.SetBits(address, bits);

        public Bits LoadBits(long address, int size) {
            return memory.LoadBits(address, size);
        }
        
    }
   
}