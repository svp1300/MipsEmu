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

    public Symbol[] GetSymbols(int start, int end, bool ignoreWhitespace) {
        var result = new Symbol[end - start];
        int index = 0;
        int skip = 0;
        while (index < end && index + skip < match.Length) {
            if (ignoreWhitespace && match[index + skip].type.Equals(SymbolType.WHITESPACE)) {
                skip++;
            } else {
                if (index >= start) {
                    result[index - start] = match[index + skip];
                }
                index++;
            }
        }
        return result;
    }

    public Symbol GetSymbol(int location, bool ignoreWhitespace) {
        return GetSymbols(location, location + 1, ignoreWhitespace)[0];
    }

    public abstract Bits MakeValueBits(UnlinkedProgram sections, int sectionId);

    public string GetSymbolString(int index, Boolean ignoreWhitespace) => GetSymbol(index, ignoreWhitespace).value;
    public string GetSymbolString(int index) => GetSymbolString(index, true);
    public int GetSymbolCount(bool ignoreWhitespace) {
        int count = 0;
        foreach(var sym in match) {
            if (ignoreWhitespace && sym.type.Equals(SymbolType.WHITESPACE))
                continue;
            count++;
        }
        return count;
    }

    // Data/

    public abstract long GetBitLength(int alignment);
    public abstract void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results);
    public abstract TokenType GetTokenType();
    
}


public class LabelToken : Token {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.COLON}, true);
    public LabelToken(Symbol[] match) : base(match) { }

    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        throw new ParseException("Only dot directives can change the state of the assembler.");
    }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        throw new NotImplementedException();
    }
    public override long GetBitLength(int alignment) => 0;
    public override TokenType GetTokenType() => TokenType.LABEL;
}
