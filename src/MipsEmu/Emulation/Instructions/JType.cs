using MipsEmu.Emulation.Registers;
using MipsEmu.Emulation.Devices;

namespace MipsEmu.Emulation.Instructions;

public class JumpInstruction : InstructionJType {

    public override void RunJump(Hardware hardware, Bits address) {
        hardware.programCounter.SetBits(address);
    }

}

public class JumpAndLinkInstruction : InstructionJType {

    public override void RunJump(Hardware hardware, Bits address) {
        Bits currentPosition = hardware.programCounter.GetBits();
        hardware.registers.SetRegisterBits(RegisterFile.RA_INDEX, currentPosition);
        hardware.programCounter.SetBits(address);
    }
    
}