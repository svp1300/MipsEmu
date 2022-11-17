using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Memory;
using MipsEmu.Emulation.Processing;

namespace MipsEmu.Emulation {
    
    struct Hardware {
        public Register programCounter;
        public RegisterFile registers;
        public Ram memory;
        public Alu alu;
    }

    class MipsProgram {
        private Hardware hardware;

        public MipsProgram() {
            hardware = new Hardware();
        }

    }
}