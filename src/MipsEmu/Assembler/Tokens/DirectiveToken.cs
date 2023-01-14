namespace MipsEmu.Assembler.Tokens;

using System.Text.RegularExpressions;

public abstract class DirectiveToken : Token {
    private string[] validNames;

    public DirectiveToken(Symbol[] match, string[] validNames) : base(match) {
        this.validNames = validNames;
    }

    public override bool CheckValidMatch() {
        var name = GetSymbolString(1);
        foreach(var directiveName in validNames) {
            if (directiveName.Equals(name))
                return true;
        }
        return false;
    }
}
public class ArgumentlessDirectiveToken : DirectiveToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.DOT, SymbolType.NAME}, true); // .data
    public static readonly string[] VALID_NAMES = new string[] {"data", "text"};
    public ArgumentlessDirectiveToken(Symbol[] match) : base(match, VALID_NAMES) { }

    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        string directive = GetSymbol(1, true).value.ToLower();
        if (directive.Equals("data"))
            state.InText = false;
        else if (directive.Equals("text"))
            state.InText = true; 
    }
    
    public override long GetByteLength(int alignment) => 0;
    public override TokenType GetTokenType() => TokenType.DIRECTIVE;

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId, long psuedoAddress) {
        return new Bits(0);
    }
}


public class NumberArgumentDirectiveToken : DirectiveToken {
    public static readonly string[] VALID_NAMES = new string[] {"align", "byte", "word", "half", "space"};
    public static readonly ITokenForm FORM = new CompositeTokenForm(new ITokenForm[] {
        new FixedTokenForm(new SymbolType[] {SymbolType.DOT, SymbolType.NAME, SymbolType.NUMBER}, true),
        new RepeatableTokenForm(new SymbolType[] {SymbolType.COMMA, SymbolType.NUMBER}, true)
    });

    public NumberArgumentDirectiveToken(Symbol[] symbols) : base(symbols, VALID_NAMES) { }

    public override TokenType GetTokenType() => TokenType.DIRECTIVE;

    public override long GetByteLength(int alignment) {
        string directive = GetSymbol(1, true).value.ToLower();
        int symbolCount = GetSymbolCount(true);
        if (directive.Equals("byte"))
            return (symbolCount - 1)/2;
        else if (directive.Equals("half"))
            return 2 * ((symbolCount - 1)/2);
        else if (directive.Equals("word"))
            return 4 * ((symbolCount - 1)/2);
        else if (directive.Equals("space"))
            return Int32.Parse(GetSymbolString(2));
        else
            return 0;
    }


    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        string directive = GetSymbol(1, true).value.ToLower();
        if (directive.Equals("align")) {
            state.Alignment = Int32.Parse(GetSymbol(2, true).value);
        }
    }

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId, long psuedoAddress) {
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
public class TextArgumentDirectiveToken : DirectiveToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.DOT, SymbolType.NAME, SymbolType.NAME}, true);
    public static readonly string[] VALID_NAMES = new string[] {"globl"};
    public TextArgumentDirectiveToken(Symbol[] match) : base(match, VALID_NAMES) { }

    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) {
        string directive = GetSymbol(1, true).value.ToLower();
        if (directive.Equals("globl")) {
            results.globals.Add(GetSymbol(2, true).value);
        }
    }

    public override long GetByteLength(int alignment) {
        return 0;
    }
    
    public override TokenType GetTokenType() => TokenType.DIRECTIVE;

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId, long psuedoAddress) {
        return new Bits(0);
    }
}

public class StringArgumentDirectiveToken : DirectiveToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.DOT, SymbolType.NAME, SymbolType.STRING}, true);
    public static readonly string[] VALID_NAMES = new string[] {"asciiz", "ascii"};
    private string value;
    public StringArgumentDirectiveToken(Symbol[] match) : base(match, VALID_NAMES) {
        value = GetSymbolString(2);
        value = value.Substring(1, value.Length - 2);
        value = Regex.Replace(value, "\\\\\\\\", "\\");
        value = Regex.Replace(value, "\\\\n", "\n");
        value = Regex.Replace(value, "\\\\t", "\t");
    }

    public override void UpdateAssemblerState(AnalyzerState state, SyntaxParseResult results) { }

    public override long GetByteLength(int alignment) {// TODO alignment support
        string directive = GetSymbolString(1);
        int length = value.Length;
        if (directive.Equals("asciiz"))
            return length + 1;
        else if (directive.Equals("ascii"))
            return length;
        else
            throw new ParseException($"Unrecognized dot directive {directive} in string argument form.");
    }
    
    public override TokenType GetTokenType() => TokenType.DIRECTIVE;

    public override Bits MakeValueBits(UnlinkedProgram sections, int sectionId, long psuedoAddress) {
        string directive = GetSymbolString(1);
        int size;
        if (directive.Equals("asciiz"))
            size = 8 * (value.Length + 1);
        else if (directive.Equals("ascii"))
            size = 8 * value.Length;
        else
            throw new ParseException($"Unrecognized dot directive {directive} in string argument form.");
        var data = new Bits(size);
        for (int i = 0; i < value.Length; i++) {
            var b = new Bits(8);
            b.SetFromUnsignedInt(value[i]);
            data.Store(i * 8, b);
        }
        return data;
        
    }
}

