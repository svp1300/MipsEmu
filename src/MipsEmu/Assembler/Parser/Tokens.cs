
namespace MipsEmu.Assembler;
using System.Text;

// ~~~~~~~~~~~~~~~~~~~~~~~~ Token Forms ~~~~~~~~~~~~~~~~~~~~~~~
public interface ITokenForm {

    int Match(Symbol[] symbols, int begin);

}
public class CompositeTokenForm : ITokenForm {

    private ITokenForm[] tokenForms;
    public CompositeTokenForm(ITokenForm[] forms) {
        tokenForms = forms;
    }

    public int Match(Symbol[] symbols, int begin) {
        int matchLength = 0;
        foreach(var form in tokenForms) {
            matchLength += form.Match(symbols, begin + matchLength);
        }
        return matchLength;
    }
}

public class RepeatableTokenForm : FixedTokenForm {

    public RepeatableTokenForm(SymbolType[] form, bool ignoreWhitespace) : base(form, ignoreWhitespace) { }

    public override int Match(Symbol[] symbols, int begin) {
        int match;
        int length = 0;
        while ((match = base.Match(symbols, begin + length)) != 0) {
            length += match;
        }
        return length;
    }

}
public class FixedTokenForm : ITokenForm {
    private bool ignoreWhitespace;
    public static readonly FixedTokenForm[] TOKEN_FORMS = new FixedTokenForm[] {

    };

    private SymbolType[] form;
    
    public FixedTokenForm(SymbolType[] form, bool ignoreWhitespace) {
        this.form = form;
        this.ignoreWhitespace = ignoreWhitespace;
        // this.generator = generator;
    }

    public int GetFormLength() => form.Length;

    public virtual int Match(Symbol[] symbols, int begin) {
        int s = 0;
        int skipped = 0;
        while (s < form.Length) {
            var current = symbols[begin + s + skipped];
            if (ignoreWhitespace && current.type.Equals(SymbolType.WHITESPACE)) {
                skipped++;
                continue;
            }
            if (!form[s].Equals(current.type)) {
                return 0;
            } else {
                s++;
            }

        }
        return s + skipped;
    }
}

// ~~~~~~~~~~~~~~~~~~~~~~~~ Tokens ~~~~~~~~~~~~~~~~~~~~~~~


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

    // Data/
    public abstract void UpdateAssemblerState(ref bool inText, ParseResult state);
    public abstract TokenType GetTokenType();
    
}

public abstract class InstructionToken : Token {

    public InstructionToken(Symbol[] match) : base(match) { }
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;


    public override void UpdateAssemblerState(ref bool inText, ParseResult state) {
        throw new ParseException("Only dot directives can change the state of the assembler.");
    }
}


public class TypeRInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER}, true); // add $rs, $rt, $rd

    public TypeRInstructionToken(Symbol[] match) : base(match) { }



}

public class LabelToken : Token {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.COLON}, true);
    public LabelToken(Symbol[] match) : base(match) { }

    public override void UpdateAssemblerState(ref bool inText, ParseResult state) {
        throw new ParseException("Only dot directives can change the state of the assembler.");
    }

    public override TokenType GetTokenType() => TokenType.LABEL;
}
public class TypeIInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER}, true); // addi $rs, $rt, imm
    
    public TypeIInstructionToken(Symbol[] match) : base(match) { }
    
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;
    
}

public class ArgumentlessDirectiveToken : Token {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.DOT, SymbolType.STRING}, true); // .data
    
    public ArgumentlessDirectiveToken(Symbol[] match) : base(match) { }

    public override void UpdateAssemblerState(ref bool inText, ParseResult state) {
        string directive = GetSymbol(1, true).value.ToLower();
        if (directive.Equals("data"))
            inText = false;
        else if (directive.Equals("text"))
            inText = true; 
    }
    
    public override TokenType GetTokenType() => TokenType.DIRECTIVE;
}

public class StringArgumentDirectiveToken : Token {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.DOT, SymbolType.STRING, SymbolType.STRING}, true);
    public StringArgumentDirectiveToken(Symbol[] match) : base(match) { }

    public override void UpdateAssemblerState(ref bool inText, ParseResult state) {
        string directive = GetSymbol(1, true).value.ToLower();
        if (directive.Equals("globl")) {
            state.globals.Add(GetSymbol(2, true).value);
        }
    }
    
    public override TokenType GetTokenType() => TokenType.DIRECTIVE;
}
public class SingleRegisterInstruction {}

public class MemoryInstructionToken : InstructionToken {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER, SymbolType.OPEN_PAREN, SymbolType.NUMBER, SymbolType.CLOSE_PAREN}, true);
    
    public MemoryInstructionToken(Symbol[] match) : base(match) { }
    
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;
}
