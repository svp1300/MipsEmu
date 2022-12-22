using MipsEmu.Emulation.Instructions;

using System.Collections.Generic;

namespace MipsEmu.Emulation.Instructions {

    public class InstructionParser {
        // Credit to https://opencores.org/projects/plasma/opcodes for the list of opcodes.
        private static readonly Dictionary<int,IInstruction> OPCODE_INSTRUCTIONS = new Dictionary<int, IInstruction>{
            {2, new JumpInstruction()},
            {3, new JumpAndLinkInstruction()},
            {8, new AddImmediateInstruction()},
            {12, new AndImmediateInstruction()},
            {13, new OrImmediateInstruction()},
            {14, new XorImmediateInstruction()},
            {34, new SubtractInstruction()},
            // {0b001111, new LoadUpperImmediateInstruction()},
            // {0b100000, new LoadByteInstruction()},
            // {0b100001, new LoadHalfWordInstruction()},
            // {0b100011, new LoadWordInstruction()},
            // {0b101000, new StoreHalfWordInstruction()},
            // {0b101001, new StoreHalfWordInstruction()},
            {43, new StoreWordInstruction()},
        };

        private static readonly Dictionary<int,IInstruction> FUNC_INSTRUCTIONS = new Dictionary<int, IInstruction>{
            {8, new JumpRegisterInstruction()},
            {12, new SyscallInstruction()},
            {32, new AddInstruction()},
            // {0b100100, new AndInstruction()},
            // {0b100111, new NorInstruction()},
            // {0b100110, new XorInstruction()},
            // {0b100101, new OrInstruction()},
            // {0b100010, new SubtractInstruction()},
            // {0b001001, new JumpAndLinkRegisterInstruction()},
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