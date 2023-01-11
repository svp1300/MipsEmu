namespace MipsEmu.Emulation.Instructions;

using MipsEmu.Emulation.Registers;

public class SyscallInstruction : IInstruction {
    public void Run(Hardware hardware, Bits bits) {
        var code = hardware.registers.GetRegisterBits(RegisterFile.REGISTER_INDICES["v0"]).GetAsUnsignedInt();
        switch (code) {
            case 1:
                Console.WriteLine(hardware.registers.GetRegisterBits(RegisterFile.REGISTER_INDICES["a0"]).GetAsSignedInt());
                break;
            case 2:
            case 3:
                Console.WriteLine(hardware.registers.GetRegisterBits(RegisterFile.REGISTER_INDICES["f12"]).GetAsFloat());
                break;
            case 4:
                var address = hardware.registers.GetRegisterBits(RegisterFile.REGISTER_INDICES["a0"]).GetAsUnsignedLong();
                Console.WriteLine(hardware.memory.ReadString(address));
                break;
            case 5:
                var input = new Bits(32);
                string? line = Console.ReadLine();
                if (line == null)
                    throw new Exception("Couldn't read line.");
                input.SetFromSignedInt(Int32.Parse(line));
                hardware.registers.SetRegisterBits(RegisterFile.REGISTER_INDICES["v0"], input);
                break; 
            case 10:
                hardware.exit = true;
                break;
            default:
                Console.WriteLine("Unsupported syscall");
                break;
        }
    }

    public string InfoString(Bits bits) {
        return "S\tSyscallInstruction";
    }
    
}