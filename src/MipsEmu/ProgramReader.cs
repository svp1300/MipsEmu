namespace MipsEmu;


using System.IO;

public class ProgramReader {

    public static string[]? ReadProgram(string path) {
        if (Directory.Exists(path)) {
            var sections = new List<string>();
            foreach (var sectionPath in Directory.GetFiles(path)) {
                if (sectionPath.EndsWith(".asm")) {
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