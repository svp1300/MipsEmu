using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using MipsEmu;
namespace MipsEmu.Assembler;



/// <summary>Contains function(s) to break a string into lexical symbols.</summary>
public class LexicalAnalyzer {
    private static readonly List<SymbolForm> SYMBOL_FORMS = new List<SymbolForm>() {
       // new SymbolForm(SymbolType.QUOTE, "\""), // regex, priority
        new SymbolForm(SymbolType.NUMBER, "-?[0-9]+"),
        new SymbolForm(SymbolType.COMMENT, "#.*\n"),
        new SymbolForm(SymbolType.DOT, "\\."),
        new SymbolForm(SymbolType.WHITESPACE, "\\s+"),
        new SymbolForm(SymbolType.STRING, "\\\"(\\\\\\\"|.)*\\\""),
        new SymbolForm(SymbolType.NAME, "([A-z]|[0-9])+"),
        new SymbolForm(SymbolType.REGISTER, "\\$([A-z]|[0-9])+"),
        new SymbolForm(SymbolType.COLON, ":"),
        new SymbolForm(SymbolType.COMMA, ","),
        new SymbolForm(SymbolType.OPEN_PAREN, "\\("),
        new SymbolForm(SymbolType.CLOSE_PAREN, "\\)"),
        new SymbolForm(SymbolType.EQUALS, "="),
        new SymbolForm(SymbolType.MULTIPLY, "\\*")
    };

    /// <summary>Traverses the string looking for the longest matches.</summary>
    /// <returns>Returns a list containing the longest symbol matches.</returns>
    /// <throws>ParseException when encountering text that cannot be parsed.</throws>
    /// <param name=text>The text to be broken into symbols.</param>
    public static Symbol[] FindSymbols(string text) {
        int position = 0;
        var symbols = new List<Symbol>();
        while (position < text.Length) {
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
                throw new ParseException("Failed to find symbol in remaining text.");
            } else {
                if (!longestMatchType.Equals(SymbolType.COMMENT)) {
                    symbols.Add(new Symbol(longestMatch, longestMatchType.Value));
                }
                position += longestMatch.Length;
            }
        }
        
        return symbols.ToArray();
        
    }
}

class ParseException : Exception {

    public ParseException(string message) : base(message) { }
}   


