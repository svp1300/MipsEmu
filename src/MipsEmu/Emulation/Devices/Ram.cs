using System.Text;
namespace MipsEmu.Emulation.Devices {

    public class Ram {
        public static readonly long TEXT_START = 0x00400000;
        public static readonly long DATA_START = 0x10000000;
        public static readonly long USER_ACCESSIBLE_START = TEXT_START;
        public static readonly long USER_ACCESSIBLE_END = 0x80000000;
        private Bits?[] memory;

        public Ram(long userMemorySize) {
            memory = new Bits[userMemorySize];
        }

        public void StoreBytes(long address, Bits bits, long amount) {
            for (int i = 0; i < amount; i++) {
                memory[address + i] = bits.LoadBits(8 * i, 8);

            }
        }

        public void StoreBytes(long address, Bits bits) => StoreBytes(address, bits, bits.GetLength() / 8);

        public Bits LoadBytes(long address, int amount) {
            var read = new Bits(amount * 8);
            for (int i = 0; i < amount; i++) {
                var contents = memory[address + i];
                if (contents != null)
                    read.Store(8 * i, contents);
            }
            return read;
        }
        
        public string ReadString(long address) {
            var builder = new StringBuilder();
            char character = 'a';
            for (int index = 0; character != 0; index++) {
                character = (char) LoadBytes(address + index, 1).GetAsUnsignedInt();
                builder.Append(character);
            }
            return builder.ToString();
        }
    }
   


    public class DisjointNode {
        private long address;
        private List<Bits> data;

        public DisjointNode(long address) {
            this.data = new List<Bits>();
            this.address = address;
        }

        public bool CanJoin(DisjointNode other, long mergeDistance) {
            if (this.address < other.address)
                return false;
            else
                return mergeDistance < Math.Abs(other.address  - address);
        }

        public bool CanStore(long address, long mergeDistance) {
            if (this.address < address)
                return false;
            else
                return false;
        }

        public void Join(DisjointNode other) {
            while (data.Count < other.address)
                data.Add(new Bits(8));
            foreach(var b in other.data)
                data.Add(b);
        }

        public Bits ReadBytes(long address, int amount) {
            var bytes = new Bits(amount * 8);
            for (int i = 0; i < amount; i++) {
                bytes.Store(i * 8, data[(int) (address - this.address)]);
            }
            return bytes;
        }

        public void Store(long address, Bits storeData) { 
            for (int i = 0; i < storeData.GetLength() / 8; i++) {
                if (i < data.Count)
                    data[i] = storeData.LoadBits(8 * i, 8);
                else // assumed continuous
                    data.Add(storeData.LoadBits(8 * i, 8));
            }
        }

        public long GetStartAddress() => address;
    }

    public class DisjointMemory {

        private List<DisjointNode> memoryNodes;
        private long capacity;

        public DisjointMemory(long capacity) {
            this.capacity = capacity;
            memoryNodes = new List<DisjointNode>();
        }

        private int FindClosestNode(long address) {
            var low = 0;
            var high = memoryNodes.Count - 1;
            while (low != high) {
                int mid = (low + high) / 2;
                // if ()
            }
            return low;
        }
        // public Bits Load()

        
    }

}