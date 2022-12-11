

namespace MipsEmu.Assembler;




public struct Label {
    public string name;
    public int line;
}

public class ProgramAssembler {

    public static List<Token> ParseProgram(string program) {
        Symbol[] symbols = LexicalAnalyzer.FindSymbols(program);
        var analyzer = new SyntaxAnalyzer();
        analyzer.AddTokenForm(TypeIInstructionToken.FORM, (s) => new TypeIInstructionToken(s));
        List<Token> tokens = analyzer.BuildTree(symbols);

        return tokens;
        
    }


}