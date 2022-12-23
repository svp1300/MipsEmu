using MipsEmu.Emulation.Instructions;
using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Devices;

namespace MipsEmu {
    
    public class Hardware {
        public Register programCounter;
        public RegisterFile registers;
        public Ram memory;
        public Alu alu;
        public bool exit;

        public Hardware(long memorySize) {
            programCounter = new Register();
            programCounter.SetFromUnsignedLong(Ram.TEXT_START);
            registers = new RegisterFile();
            memory = new Ram(memorySize);
            alu = new Alu();
            exit = false;
        }
    }

    public class MipsProgram {
        private Hardware hardware;
        private Bits pcIncrementBits;

        public MipsProgram(long memorySize) {
            hardware = new Hardware(memorySize);
            pcIncrementBits = new Bits(new bool[] {true, false, false, false, false, false}).SignExtend(26);

        }

        public void LoadProgram(Bits text, Bits data) {
            hardware.memory.StoreBits(Ram.TEXT_START, text);
            hardware.memory.StoreBits(Ram.DATA_START, data);
            hardware.programCounter.SetFromUnsignedLong(Ram.TEXT_START);
            hardware.registers.SetRegisterBits(31, new Bits(32));
        }

        public void RunProgram() {
            while (!hardware.exit) {
                var result = Cycle();
                if (!result) {
                    Console.WriteLine("Runtime error!");
                    break;
                }
            }
            Console.WriteLine(hardware.registers);
        }
        
        public virtual bool Cycle() {
            long instructionAddress = hardware.programCounter.GetBits().GetAsUnsignedLong();
            if (instructionAddress % 32 != 0) {
                throw new Exception("Instructions must be on 32 aligned addresses.");
            } else {
                var pcBits = hardware.memory.LoadBits(instructionAddress, 32); // fetch
                // if (hardware.skipPCIncrement) {
                //     hardware.skipPCIncrement = false;
                // } else {
                Bits increment = Alu.AddUnsigned(hardware.programCounter.GetBits(), pcIncrementBits);
                hardware.programCounter.SetBits(increment);
                // }
                IInstruction? instruction = InstructionParser.ParseInstruction(pcBits); // decode
                Console.WriteLine(instructionAddress + "\t" + instruction);
                if (instruction == null) {
                    return false;
                } else {
                    try {
                        instruction.Run(hardware, pcBits);  // execute
                        return true;
                    } catch(Exception e) {
                        Console.WriteLine(e);
                        return false;
                    }
                }
            }
            
        }

    }

}