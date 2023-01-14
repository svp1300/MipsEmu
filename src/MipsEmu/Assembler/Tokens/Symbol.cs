using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

/// <summary>Structure representing lowest level tokens. Represents registers, comments, and various symbols. NOT to be confused with symbols from assembly.</summary>
public class Symbol {
    public string value;
    public SymbolType type;

    public Symbol(string value, SymbolType type) {
        this.value = value;
        this.type = type;
    }

    public override string ToString() {
        return value + ": " + type;
    }

    ///<summary>True when the SymbolType and value are equal.</summary>
    public override bool Equals([NotNullWhen(true)] object? obj) {
        if (obj is Symbol) {
            var other = (Symbol) obj;
            return other.type.Equals(type) && other.value.Equals(value);
        } else
            return false;
    }

    public override int GetHashCode() => value.GetHashCode();

    ///<summary>Returns the symbols in a range from a match.</summary>
    public static Symbol[] GetSymbols(Symbol[] match, int start, int end, bool ignoreWhitespace) {
        var result = new Symbol[end - start];
        int index = 0;
        int skip = 0;
        while (index < end && index + skip < match.Length) {
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

    ///<summary>Returns the symbol from an index in a match.</summary>
    public static Symbol GetSymbol(Symbol[] match, int location, bool ignoreWhitespace) {
        return GetSymbols(match, location, location + 1, ignoreWhitespace)[0];
    }


    ///<summary>Returns the text in a symbol at a location in the match.</summary>
    public static string GetSymbolString(Symbol[] match, int index, Boolean ignoreWhitespace) => GetSymbol(match, index, ignoreWhitespace).value;
    
    ///<summary>GetSymbolString, but with ignoreWhitespace as true.</summary>
    public static string GetSymbolString(Symbol[] match, int index) => GetSymbolString(match, index, true);

    ///<summary>Get the number of symbols in a match.</summary>
    public static int GetSymbolCount(Symbol[] match, bool ignoreWhitespace) {
        int count = 0;
        foreach(var sym in match) {
            if (ignoreWhitespace && sym.type.Equals(SymbolType.WHITESPACE))
                continue;
            count++;
        }
        return count;
    }
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
    QUOTE, NUMBER, NAME, COMMENT, DOT, WHITESPACE, COLON, REGISTER, COMMA, OPEN_PAREN, CLOSE_PAREN, STRING, EQUALS, MULTIPLY
}