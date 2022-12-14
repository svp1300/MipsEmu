namespace MipsEmu.Emulation;

using MipsEmu.Emulation.Instructions;

using System.IO;

public class ProgramReader {

    public static List<Bits>? ReadProgram(string path) {
        var program = new List<Bits>();
        if (!File.Exists(path)) {
            Console.WriteLine("No file found at " + path);
            return null;
        } else {
            try {
                using (var fi = File.OpenRead(path)) {
                    int readLength;
                    var buffer = new byte[1024];
                        while((readLength = fi.Read(buffer, 0, buffer.Length)) > 0) {
                            for (int b = 0; b < readLength; b += 4) {
                                // program.Add(InstructionParser.InstructionToBits(new byte[]{buffer[b], buffer[b + 1], buffer[b + 2], buffer[b + 3]}));
                            }
                        }
                }
                return program;
            } catch(IndexOutOfRangeException) {
                return null;
            }
        }
    }
    
}