using System.Text.RegularExpressions;
using MipsEmu;
namespace MipsEmu.Assembler.Parser;

public struct Symbol {
    public string value;
    public SymbolType type;

    public Symbol(string value, SymbolType type) {
        this.value = value;
        this.type = type;
    }

    public override string ToString() {
        return value + ": " + type;
    }
}

public struct SymbolForm {
    public SymbolType type;
    public string form;

    public SymbolForm(SymbolType type, string formRegex) {
        this.type = type;
        form = formRegex;
    } 

    public Match Match(string text) {
        return Regex.Match(text, form);
    }

}

public enum SymbolType {
    QUOTE, NUMBER, STRING, COMMENT, DOT, WHITESPACE, COLON, REGISTER, COMMA
}
class LexicalAnalyzer {
    private static readonly List<SymbolForm> SYMBOL_FORMS = new List<SymbolForm>() {
        new SymbolForm(SymbolType.QUOTE, "\""), // regex, priority
        new SymbolForm(SymbolType.NUMBER, "[0-9]+"),
        new SymbolForm(SymbolType.COMMENT, "#.*\n"),
        new SymbolForm(SymbolType.DOT, "\\."),
        new SymbolForm(SymbolType.WHITESPACE, "\\s+"),
        new SymbolForm(SymbolType.STRING, "([A-z]|[0-9])+"),
        new SymbolForm(SymbolType.REGISTER, "\\$[A-z]{1,2}[0-9]?"),
        new SymbolForm(SymbolType.COLON, ":"),
        new SymbolForm(SymbolType.COMMA, ",")
    };

    /// <summary>Traverses the string looking for the longest matches.</summary>
    /// <returns>Returns a list containing the longest symbol matches.</returns>
    /// <throws>ParseException when encountering text that cannot be parsed.</throws>
    /// <param name=text>The text to be broken into symbols.</param>
    public static List<Symbol> FindSymbols(string text) {
        int position = 0;
        var symbols = new List<Symbol>();
        while (position < text.Length) {
            Console.WriteLine(position);
            string? longestMatch = null;
            SymbolType? longestMatchType = null;
            foreach(var form in SYMBOL_FORMS) {
                var match = form.Match(text.Substring(position));
                if (match.Index == 0 && (longestMatch == null || longestMatch.Length < match.Length)) {
                    longestMatch = match.Value;
                    longestMatchType = form.type;
                }
            }
            if (longestMatch == null || longestMatch.Length == 0 || longestMatchType == null) {
                Console.WriteLine(text.Substring(position));
                throw new ParseException("Failed to find symbol in remaining text.");
            } else {
                symbols.Add(new Symbol(longestMatch, longestMatchType.Value));
                position += longestMatch.Length;
            }
        }
        return symbols;
        
    }
}

public struct TokenForm {
    private SymbolType[] form;
    private Func<Symbol[], Token> generator;
    
    public TokenForm(SymbolType[] form, Func<Symbol[], Token> generator) {
        this.form = form;
        this.generator = generator;
    }

    public Token Create(Symbol[] symbols) {
        return generator.Invoke(symbols);
    }

    public bool Matches(Symbol[] symbols, int begin) {
        if (symbols.Length < form.Length) {
            return false;
        }
        for(int s = 0; s < form.Length; s++) {
            if (!form[s].Equals(symbols[begin + s])) {
                return false;
            }
        }
        return true;
    }
}

class ParseException : Exception {

    public ParseException(string message) : base(message) { }
}   


