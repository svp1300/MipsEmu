

namespace MipsEmu.Assembler;






public class ProgramAssembler {

    public static void ParseProgram(string program) {
        Symbol[] symbols = LexicalAnalyzer.FindSymbols(program);
        var analyzer = new SyntaxAnalyzer();
        analyzer.AddTokenForm(TypeIInstructionToken.FORM, (s) => new TypeIInstructionToken(s));
        analyzer.AddTokenForm(TypeRInstructionToken.FORM, (s) => new TypeRInstructionToken(s));

        analyzer.AddTokenForm(ArgumentlessDirectiveToken.FORM, (s) => new ArgumentlessDirectiveToken(s));
        analyzer.AddTokenForm(StringArgumentDirectiveToken.FORM, (s) => new StringArgumentDirectiveToken(s));

        analyzer.AddTokenForm(LabelToken.FORM, (s) => new LabelToken(s));

        // List<Token> tokens = analyzer.BuildTree(symbols);
        ParseResult result = analyzer.BuildProgram(symbols);
        Console.WriteLine(result);
        // return tokens;
        
    }


}