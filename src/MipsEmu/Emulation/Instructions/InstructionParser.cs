using MipsEmu.Emulation.Instructions;

using System.Collections.Generic;

namespace MipsEmu.Emulation.Instructions {

    public class InstructionParser {
        // Credit to https://opencores.org/projects/plasma/opcodes for the list of opcodes.
        private static readonly Dictionary<int,IInstruction> OPCODE_INSTRUCTIONS = new Dictionary<int, IInstruction>{
            {0, new AddInstruction()},
            {2, new JumpInstruction()},
            {3, new JumpAndLinkInstruction()},
            {4, new BranchOnEqualInstruction()},
            {8, new AddImmediateInstruction()},
            {12, new AndImmediateInstruction()},
            {13, new OrImmediateInstruction()},
            {14, new XorImmediateInstruction()},
            {34, new SubtractInstruction()},
            {43, new StoreWordInstruction()},
        };

        private static readonly Dictionary<int,IInstruction> FUNC_INSTRUCTIONS = new Dictionary<int, IInstruction>{
            {8, new JumpRegisterInstruction()},
            {12, new SyscallInstruction()},
            {32, new AddInstruction()},
        };


        /// <summary> Parses and returns the instruction. </summary>
        /// <throws> IndexOutOfRangeException when the instruction can't be parsed, found, or is unsupported. </throws>
        public static IInstruction? ParseInstruction(Bits instruction) {
            try {
                int opcode = instruction.GetUnsignedIntFromRange(Bits.OPCODE_INTERVAL);
                if (opcode == 0) {
                    var func = instruction.GetUnsignedIntFromRange(Bits.FUNC_INTERVAL);
                    return FUNC_INSTRUCTIONS[func];
                } else {
                    return OPCODE_INSTRUCTIONS[opcode];
                }
            } catch (IndexOutOfRangeException) {
                Console.WriteLine("Parsing failure! Unsupported or invalid instruction.");  
                return null;
            }

        }

    }
}