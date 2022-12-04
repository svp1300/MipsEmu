
namespace MipsEmu.Emulation.Devices {

    public class Ram {
        public static readonly int TEXT_START = 0x00400000;
        private Bits memory;

        public void StoreBits(int address, Bits data) {
            memory.SetBits(address, data);
        }

        public Bits LoadBits(int address, int size) {
            return memory.GetBits(address, size);
        }
        
    }
   
}