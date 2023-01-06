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
            var spValue = new Bits(32);
            spValue.SetFromUnsignedLong(Math.Min(memorySize, 0x7fffffff));
            registers.SetRegisterBits(29, spValue);
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
            pcIncrementBits = new Bits(new bool[] {false, false, false, true, false, false}).SignExtend(26, false);

        }

        public bool LoadProgram(Bits text, Bits data) {
            try {
                hardware.memory.StoreBytes(Ram.TEXT_START, text);
                hardware.memory.StoreBytes(Ram.DATA_START, data);
                hardware.programCounter.SetFromUnsignedLong(Ram.TEXT_START);
                return true;
            } catch(IndexOutOfRangeException) {
                Console.WriteLine("Error! Not enough memory.");
                return false;
            }
        }

        public void RunProgram() {
            while (!hardware.exit) {
                var result = Cycle();
                if (!result) {
                    Console.WriteLine("Runtime error!");
                    break;
                }
            }
            // Console.WriteLine(hardware.registers);
        }
        
        public virtual bool Cycle() {
            long instructionAddress = hardware.programCounter.GetBits().GetAsUnsignedLong();
            if (instructionAddress % 4 != 0) {
                throw new Exception("Instructions must be on 32 aligned addresses.");
            } else {
                var pcBits = hardware.memory.LoadBytes(instructionAddress, 4); // fetch
                // if (hardware.skipPCIncrement) {
                //     hardware.skipPCIncrement = false;
                // } else {
                Bits increment = Alu.AddUnsigned(hardware.programCounter.GetBits(), pcIncrementBits);
                hardware.programCounter.SetBits(increment);
                // }
                IInstruction? instruction = InstructionParser.ParseInstruction(pcBits); // decode
                Console.WriteLine(instruction.InfoString(pcBits));
                if (instruction == null) {
                    return false;
                } else {
                    // try {
                        instruction.Run(hardware, pcBits);  // execute
                        return true;
                    // } catch(Exception e) {
                        // Console.WriteLine(e);
                        // return false;
                    // }
                }
            }
            
        }

    }

}

public class EmulationException : Exception {

    public EmulationException(string message) : base(message) { }

}