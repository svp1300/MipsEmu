using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

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