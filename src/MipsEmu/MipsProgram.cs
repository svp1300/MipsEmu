using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Devices;

namespace MipsEmu {
    
    public struct Hardware {
        public Register programCounter;
        public RegisterFile registers;
        public Ram memory;
        public Alu alu;
    }

    public class MipsProgram {
        private Hardware hardware;

        public MipsProgram() {
            hardware = new Hardware();
        }

    }
}