
namespace MipsEmu.Assembler.Tokens;

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
        if (begin >= symbols.Length)
            return 0;
        int matchLength = 0;
        int skipped = 0;
        while (matchLength < form.Length) {
            if (begin + matchLength + skipped >= symbols.Length) {
                return 0;
            }
            var current = symbols[begin + matchLength + skipped];
            if (ignoreWhitespace && current.type.Equals(SymbolType.WHITESPACE)) {
                skipped++;
                continue;
            }
            if (!form[matchLength].Equals(current.type)) {
                return 0;
            } else {
                matchLength++;
            }

        }
        return matchLength + skipped;
    }
}
