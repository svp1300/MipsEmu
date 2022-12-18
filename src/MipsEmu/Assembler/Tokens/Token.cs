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
        while (index < end) {
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

    public override long GetBitLength(int alignment) => 0;
    public override TokenType GetTokenType() => TokenType.LABEL;
}




public class ArgumentlessDirectiveToken : Token {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.DOT, SymbolType.STRING}, true); // .data
    
    public ArgumentlessDirectiveToken(Symbol[] match) : base(match) { }

    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        string directive = GetSymbol(1, true).value.ToLower();
        if (directive.Equals("data"))
            state.InText = false;
        else if (directive.Equals("text"))
            state.InText = true; 
    }
    
    public override long GetBitLength(int alignment) => 0;
    public override TokenType GetTokenType() => TokenType.DIRECTIVE;
}


public class NumberArgumentDirective : Token {
    public static readonly ITokenForm FORM = new CompositeTokenForm(new ITokenForm[] {
        new FixedTokenForm(new SymbolType[] {SymbolType.DOT, SymbolType.STRING, SymbolType.NUMBER}, true),
        new RepeatableTokenForm(new SymbolType[] {SymbolType.COMMA, SymbolType.NUMBER}, true)
    });

    public NumberArgumentDirective(Symbol[] symbols) : base(symbols) { }

    public override TokenType GetTokenType() => TokenType.DIRECTIVE;

    public override long GetBitLength(int alignment) {
        string directive = GetSymbol(1, true).value.ToLower();
        int symbolCount = GetSymbolCount(true);
        if (directive.Equals("byte"))
            return 8 * ((symbolCount - 1)/2);
        else if (directive.Equals("half"))
            return 16 * ((symbolCount - 1)/2);
        else if (directive.Equals("word"))
            return 32 * ((symbolCount - 1)/2);
        else
            return 0;
    }

    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        string directive = GetSymbol(1, true).value.ToLower();
        if (directive.Equals("align")) {
            state.Alignment = Int32.Parse(GetSymbol(2, true).value);
        }

    }
}
public class TextArgumentDirectiveToken : Token {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.DOT, SymbolType.STRING, SymbolType.STRING}, true);
    public TextArgumentDirectiveToken(Symbol[] match) : base(match) { }

    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        string directive = GetSymbol(1, true).value.ToLower();
        if (directive.Equals("globl")) {
            results.globals.Add(GetSymbol(2, true).value);
        }
    }

    public override long GetBitLength(int alignment) {
        return 0;
    }
    
    public override TokenType GetTokenType() => TokenType.DIRECTIVE;
}

public class SingleRegisterInstruction {}


