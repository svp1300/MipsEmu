using MipsEmu.Emulation.Instructions;
using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Devices;

namespace MipsEmu {
    
    public struct Hardware {
        public Register programCounter;
        public RegisterFile registers;
        public Ram memory;
        public Alu alu;

        public Hardware(long memorySize) {
            programCounter = new Register();
            programCounter.SetFromUnsignedLong(Ram.TEXT_START);
            registers = new RegisterFile();
            memory = new Ram(memorySize);
            alu = new Alu();
        }
    }

    public class MipsProgram {
        private Hardware hardware;
        private Bits one;

        public MipsProgram(long memorySize) {
            hardware = new Hardware(memorySize);
            one = new Bits(new bool[]{true, false, false, false, false}).SignExtend(27);
        }

        public void LoadProgram(Bits text, Bits data) {
            hardware.memory.StoreBits(Ram.TEXT_START, text);
            hardware.memory.StoreBits(Ram.DATA_START, data);
            hardware.registers.SetRegisterBits(31, new Bits(32));
        }

        public virtual bool Cycle() {
            int instructionAddress = hardware.programCounter.GetBits().GetAsSignedInt();
            var pcBits = hardware.memory.LoadBits(instructionAddress, 32); // fetch
            IInstruction? instruction = InstructionParser.parseInstruction(pcBits); // decode
            if (instruction == null) {
                return false;
            } else {
                try {
                    instruction.Run(hardware, pcBits);  // execute
                    Bits increment = Alu.AddSigned(hardware.programCounter.GetBits(), one);
                    hardware.programCounter.SetBits(increment);
                    return true;
                } catch(Exception e) {
                    Console.WriteLine(e);
                    return false;
                }
            }
        }

    }

}