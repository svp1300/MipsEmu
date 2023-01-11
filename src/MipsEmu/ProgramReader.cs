namespace MipsEmu;


using System.IO;

public class ProgramReader {

    public static Bits ReadBits(string path) {
        var bytes = new List<int>();
        if (File.Exists(path)) {
            using (var fi = File.OpenRead(path)) {
                int read;
                while ((read = fi.ReadByte()) != -1) {
                    bytes.Add(read);
                }

            }
        }
        var result = new Bits(8 * bytes.Count);
        for (int b = 0; b < bytes.Count; b++) {
            var bits = new Bits(8);
            bits.SetFromUnsignedInt(bytes[b]);
            result.Store(b * 8, bits);
        }
        return result;
    }

    public static string[]? ReadAllWithExtension(string path, string fileExtension) {
        if (Directory.Exists(path)) {
            var sections = new List<string>();
            foreach (var sectionPath in Directory.GetFiles(path)) {
                if (sectionPath.EndsWith(fileExtension)) {
                    Console.WriteLine("Reading " + sectionPath);
                    var result = ReadProgramSection(sectionPath);
                    if (result != null)
                        sections.Add(result);
                }
            }
            return sections.ToArray();
        } else if (File.Exists(path)) {
            var result = ReadProgramSection(path);
            if (result != null)
                return new string[] {result};
        }
        return null;
    }

    public static string? ReadProgramSection(string section) {
        try {
            string? program = null;
            using (var fi = new StreamReader(File.OpenRead(section))) {
                program = fi.ReadToEnd();
            }
            return program;
        } catch(IOException) {
            return null;
        }
    }
    
}