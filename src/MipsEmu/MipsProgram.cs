using MipsEmu.Emulation.Instructions;
using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Devices;

using System;

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
        private Bits one;

        public MipsProgram(int memorySize) {
            hardware = new Hardware(memorySize);
            one = new Bits(new bool[]{true}).SignExtend(31);
        }

        public virtual bool Cycle() {
            int instructionAddress = hardware.programCounter.GetBits().GetAsSignedInt();
            var pcBits = hardware.memory.LoadBits(instructionAddress, 32); // fetch
            IInstruction instruction = InstructionParser.parseInstruction(pcBits); // decode
            try {
                instruction.Run(hardware, pcBits);  // execute
                Bits increment = hardware.alu.AddSigned(hardware.programCounter.GetBits(), one);
                hardware.programCounter.SetBits(increment);
                return true;
            } catch(Exception e) {
                Console.WriteLine(e);
                return false;
            }
        }

    }
}