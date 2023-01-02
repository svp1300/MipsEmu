
namespace MipsEmu.Assembler.Tokens;

public interface ITokenForm {

    int Match(Symbol[] symbols, int begin);
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

public class TokenDefinition {
    private ITokenForm form;
    private Dictionary<string, Func<Symbol[], Token>> possibleMatches;

    public TokenDefinition(ITokenForm form) {
        this.form = form;
        possibleMatches = new Dictionary<string, Func<Symbol[], Token>>();
    }

    /// <summary> Checks if the symbols matches the lexical pattern definition. If so, returns a token if the operation is known and throws an exception otherwise.</summary>
    public Token? TryCreate(Symbol[] symbols, int begin) {
        int length = form.Match(symbols, begin);
        if (length == 0)
            return null;
        else {
            var operation = Symbol.GetSymbolString(symbols, begin, true);
            if (possibleMatches.ContainsKey(operation)) {
                return possibleMatches[operation].Invoke(Symbol.GetSymbols(symbols, begin, length, true));
            } else {
                throw new ParseException($"Unknown/unregistered operation: {operation}");
            }
        }
    }

    public void AddOperation(string name, Func<Symbol[], Token> tokenCreator) {
        possibleMatches.Add(name, tokenCreator);
    }

}
