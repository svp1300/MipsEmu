
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
        var matchSymbols = GetSubArray(symbols, begin, match.Item2);
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
    public abstract TokenType GetTokenType();
    
}

public class TypeRInstructionToken : Token {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER}, true); // add $rs, $rt, $rd

    public TypeRInstructionToken(Symbol[] match) : base(match) { }

    public override TokenType GetTokenType() => TokenType.INSTRUCTION;


}
public class TypeIInstructionToken : Token {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER}, true); // addi $rs, $rt, imm
    
    public TypeIInstructionToken(Symbol[] match) : base(match) { }
    
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;
    
}

public class SingleRegisterInstruction {}

public class MemoryInstructionToken : Token {
    public static readonly ITokenForm FORM = new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.NUMBER, SymbolType.OPEN_PAREN, SymbolType.NUMBER, SymbolType.CLOSE_PAREN}, true);
    
    public MemoryInstructionToken(Symbol[] match) : base(match) { }
    
    public override TokenType GetTokenType() => TokenType.INSTRUCTION;
}
// // public class InstructionToken : IToken {
//     public static readonly ITokenForm[] TOKEN_FORMS = new ITokenForm[] {
//         new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, SymbolType.COMMA, SymbolType.STRING}, true), // la $rs, label
//         // new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER, Sym}, true), // lw $t0, 0($t1)
//  done       new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.REGISTER}, true), // jr $ra
//         new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.NUMBER}, true), // j pseudoaddress
//         new FixedTokenForm(new SymbolType[] {SymbolType.STRING, SymbolType.STRING}, true), // j label
//         new FixedTokenForm(new SymbolType[] {SymbolType.STRING}, true) // syscall
//     };
// }
