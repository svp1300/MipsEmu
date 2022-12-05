
namespace MipsEmu.Emulation.Devices {

    public class Ram {
        public static readonly int TEXT_START = 0x00400000;
        private Bits memory;

        public Ram(int memorySize) {
            memory = new Bits(memorySize);
        }

        public void StoreBits(int address, Bits data, int amount) {
            memory.SetBits(address, new Bits(data.Load(0, amount)));
        }

        public Bits LoadBits(int address, int size) {
            return memory.GetBits(address, size);
        }
        
    }
   
}