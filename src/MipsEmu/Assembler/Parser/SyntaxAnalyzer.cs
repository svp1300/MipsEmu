
namespace MipsEmu.Assembler;

using System.Text;
public class SyntaxAnalyzer {
    private TokenFactory factory;

    public SyntaxAnalyzer() {
        factory = new TokenFactory();
    }
    public List<Token> BuildTree(Symbol[] symbols) {
        List<Token>? result = BuildTree(symbols, 0);
        if (result == null) {
            throw new ParseException("Unable to build parse tree.");
        } else {
            return result;
        }
    }

    private List<Token>? BuildTree(Symbol[] symbols, int startIndex) {
            PrintRemaining(symbols, startIndex);
        if (startIndex >= symbols.Length) {
            return new List<Token>();
        } else {
            var matches = factory.FindMatches(symbols, startIndex);
            foreach (var match in matches) {
                Token token = factory.Generate(symbols, match, startIndex);
                var next = BuildTree(symbols, startIndex + match.Item2);
                if (next != null) {
                    var result = new List<Token>();
                    result.Add(token);
                    result.Concat(next);
                    return result;
                }
            }
            return null;
        }
    }


    private void PrintRemaining(Symbol[] symbols, int start) {
        var builder = new StringBuilder();
        for (int i = start; i < symbols.Length; i++)
            builder.Append(symbols[i] + " ~~ ");
        Console.WriteLine(builder.ToString());
    }

    public void AddTokenForm(ITokenForm form, Func<Symbol[], Token> generator) {
        factory.AddTokenForm(form, generator);
    }
    // private static Token[] ExpandArray(Token[] tokens, Token added) {
    //     var expanded = new Token[tokens.Length + 1];
    //     for (int index = 0; index < tokens.Length; index++) {
    //         expanded[index] = tokens[index];
    //     }
    //     expanded[expanded.Length - 1] = added;
    //     return expanded; 
    // }
}


