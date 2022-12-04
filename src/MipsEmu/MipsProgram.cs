using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Devices;

namespace MipsEmu {
    
    public struct Hardware {
        public Register programCounter;
        public RegisterFile registers;
        public Ram memory;
        public Alu alu;

        public Hardware(int memorySize) {
            programCounter = new Register();
            programCounter.SetFromSignedInt(Ram.TEXT_START);
            registers = new RegisterFile();
            memory = new Ram(memorySize);
            alu = new Alu();
        }
    }

    public class MipsProgram {
        private Hardware hardware;

        public MipsProgram(int memorySize) {
            hardware = new Hardware(memorySize);
        }

    }
}