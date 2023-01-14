
namespace MipsEmu.Assembler.Tokens;

public interface ITokenForm {

    /// <summary>Finds the length of the form's match starting from an offset.</summary>
    int Match(Symbol[] symbols, int begin);
    /// <summary>Whether it should keep trying to match.<summary>
    bool ShouldStop(int matchLength);

}
public class CompositeTokenForm : ITokenForm {

    private ITokenForm[] tokenForms;
    public CompositeTokenForm(ITokenForm[] forms) {
        tokenForms = forms;
    }

    public int Match(Symbol[] symbols, int begin) {
        int matchLength = 0;
        foreach(var form in tokenForms) {
            var subMatch = form.Match(symbols, begin + matchLength);
            if (form.ShouldStop(subMatch))
                break;
            else
                matchLength += subMatch;
        }
        return matchLength;
    }
    
    public bool ShouldStop(int matchLength) => matchLength == 0;
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


    public new bool ShouldStop(int matchLength) => false;

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

    public bool ShouldStop(int matchLength) => matchLength == 0;
}
