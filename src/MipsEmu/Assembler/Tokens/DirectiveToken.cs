namespace MipsEmu.Assembler.Tokens;


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

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        return new Bits(0);
    }
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

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        string directive = GetSymbol(1, true).value.ToLower();
        bool isWord = directive.Equals("word");
        bool isHalf = directive.Equals("half");
        bool isByte = directive.Equals("byte");
        if (isWord || isHalf || isByte) {
            int size = isWord ? 32 : (isHalf ? 16 : 8);
            var cleanedSymbols = GetSymbols(2, GetSymbolCount(true), true);
            var valueCount = (cleanedSymbols.Length + 1) / 2;
            var result = new Bits(size * valueCount);
            for (int symbolIndex = 0; symbolIndex < valueCount; symbolIndex += 2) {
                Bits symbolValue = new Bits(size);
                symbolValue.SetFromSignedInt(Int32.Parse(cleanedSymbols[symbolIndex].value));
                result.Store(symbolIndex * size, symbolValue);
            }
            return result;
        } else {
            return new Bits(0);
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

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId) {
        return new Bits(0);
    }
}

