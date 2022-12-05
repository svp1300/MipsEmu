using MipsEmu.Emulation.Instructions;

using System.Collections.Generic;

namespace MipsEmu.Emulation.Instructions {

    class InstructionParser {
        // Credit to https://opencores.org/projects/plasma/opcodes for the list of opcodes.
        private static readonly Dictionary<int,IInstruction> opcodeInstructions = new Dictionary<int, IInstruction>{
            {0b001000, new AddImmediateInstruction()},
            {0b100000, new LoadByteInstruction()},
            {0b100001, new LoadHalfWordInstruction()},
            {0b100011, new LoadWordInstruction()},
            {0b101000, new StoreHalfWordInstruction()},
            {0b101001, new StoreHalfWordInstruction()},
            {0b101011, new StoreWordInstruction()}
        };

        private static readonly Dictionary<int,InstructionRType> rType = new Dictionary<int, InstructionRType>{
            {0b100000, new AddInstruction()},
        };

        public IInstruction parseInstruction(Bits instruction) {
            int opcode = instruction.GetUnsignedIntFromRange(Bits.OPCODE_INTERVAL);
            if (opcode == 0) {
                var func = instruction.GetUnsignedIntFromRange(Bits.FUNC_INTERVAL);
                return opcodeInstructions[func];
            } else {
                return rType[opcode];
            }
        }

    }
}