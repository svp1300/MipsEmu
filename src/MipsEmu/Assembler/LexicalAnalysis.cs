using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using MipsEmu;
namespace MipsEmu.Assembler;

/// <summary>Structure representing lowest level tokens. Represents registers, comments, and various symbols. NOT to be confused with symbols from assembly.</summary>
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

    public override bool Equals([NotNullWhen(true)] object? obj) {
        if (obj is Symbol) {
            var other = (Symbol) obj;
            return other.type.Equals(type) && other.value.Equals(value);
        } else
            return false;
    }

    public override int GetHashCode() => value.GetHashCode();
}

/// <summary>Represents the lexical specification for a symbol and its categorization.</sumary>
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
    QUOTE, NUMBER, NAME, COMMENT, DOT, WHITESPACE, COLON, REGISTER, COMMA, OPEN_PAREN, CLOSE_PAREN, STRING
}

/// <summary>Contains function(s) to break a string into lexical symbols.</summary>
public class LexicalAnalyzer {
    private static readonly List<SymbolForm> SYMBOL_FORMS = new List<SymbolForm>() {
       // new SymbolForm(SymbolType.QUOTE, "\""), // regex, priority
        new SymbolForm(SymbolType.NUMBER, "[0-9]+"),
        new SymbolForm(SymbolType.COMMENT, "#.*\n"),
        new SymbolForm(SymbolType.DOT, "\\."),
        new SymbolForm(SymbolType.WHITESPACE, "\\s+"),
        new SymbolForm(SymbolType.STRING, "\\\"(\\\\\\\"|.)*\\\""),
        new SymbolForm(SymbolType.NAME, "([A-z]|[0-9])+"),
        new SymbolForm(SymbolType.REGISTER, "\\$([A-z]|[0-9])+"),
        new SymbolForm(SymbolType.COLON, ":"),
        new SymbolForm(SymbolType.COMMA, ","),
        new SymbolForm(SymbolType.OPEN_PAREN, "\\("),
        new SymbolForm(SymbolType.CLOSE_PAREN, "\\)")
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
                symbols.Add(new Symbol(longestMatch, longestMatchType.Value));
                position += longestMatch.Length;
            }
        }
        return symbols.ToArray();
        
    }
}

class ParseException : Exception {

    public ParseException(string message) : base(message) { }
}   


