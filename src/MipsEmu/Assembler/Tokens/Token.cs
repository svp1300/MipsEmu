namespace MipsEmu.Assembler.Tokens;

public class TokenFactory {
    private Dictionary<ITokenForm, Func<Symbol[], Token>> templates;
    
    public TokenFactory() {
        templates = new Dictionary<ITokenForm, Func<Symbol[], Token>>();
    }

    public List<Tuple<ITokenForm, int>> FindMatches(Symbol[] symbols, int begin) {
        var matches = new List<Tuple<ITokenForm, int>>();
        foreach (var form in templates.Keys) {
            var length = form.Match(symbols, begin);
            if (length > 0) {
                matches.Add(new Tuple<ITokenForm, int>(form, length));
            }
        }
        return matches;
    }

    public Token Generate(Symbol[] symbols, Tuple<ITokenForm, int> match, int begin) {
        var matchSymbols = GetSubArray(symbols, begin, begin + match.Item2);
        return templates[match.Item1].Invoke(matchSymbols);
    }

    public void AddTokenForm(ITokenForm form, Func<Symbol[], Token> generator) {
        templates[form] = generator;
    }

    private Symbol[] GetSubArray(Symbol[] symbols, int start, int end) {
        var sub = new Symbol[end - start];
        for(int i = 0; i < sub.Length; i++) {
            sub[i] = symbols[start + i];
        }
        return sub;
    }
}

public enum TokenType {
    INSTRUCTION, DIRECTIVE, LABEL

}
public abstract class Token {
    private Symbol[] match;
    
    public Token(Symbol[] match) {
        this.match = match;
    }

    public abstract bool CheckValidMatch();

    public Symbol[] GetSymbols(int start, int end, bool ignoreWhitespace) => Symbol.GetSymbols(match, start, end, ignoreWhitespace);

    public Symbol[] GetAllSymbols() => match;
    
    public Symbol GetSymbol(int location, bool ignoreWhitespace) => Symbol.GetSymbol(match, location, ignoreWhitespace);

    public abstract Bits MakeValueBits(UnlinkedProgram sections, int sectionId);

    public string GetSymbolString(int index, Boolean ignoreWhitespace) => Symbol.GetSymbolString(match, index, ignoreWhitespace);
    public string GetSymbolString(int index) => GetSymbolString(index, true);

    public int GetSymbolCount(bool ignoreWhitespace) => Symbol.GetSymbolCount(match, ignoreWhitespace);
    
    // Data/

    public abstract long GetBitLength(int alignment);
    public abstract void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results);
    public abstract TokenType GetTokenType();
    
}


public class LabelToken : Token {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.NAME, SymbolType.COLON}, true);
    public LabelToken(Symbol[] match) : base(match) { }

    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        throw new ParseException("Only dot directives can change the state of the assembler.");
    }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        throw new NotImplementedException();
    }
    
    public override long GetBitLength(int alignment) => 0;

    public override bool CheckValidMatch() => true;
    public override TokenType GetTokenType() => TokenType.LABEL;
}
