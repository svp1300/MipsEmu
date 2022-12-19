using Xunit;

using MipsEmu.Assembler;

namespace MipsEmuTest;

public class LexicalAnalysisTest {

    [Fact]
    public void TestFindSymbols() {
        var text = ".data instruction:.asciiz \"addi $3, $s0, 7\" .text main: lw $s0, 0($sp) add $t0, $zero,$3 jr $ra";
        var given = LexicalAnalyzer.FindSymbols(text);
        var expected = new Symbol[] {
            new Symbol(".", SymbolType.DOT),
            new Symbol("data", SymbolType.NAME),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("instruction", SymbolType.NAME),
            new Symbol(":", SymbolType.COLON),
            new Symbol(".", SymbolType.DOT),
            new Symbol("asciiz", SymbolType.NAME),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("\"addi $3, $s0, 7\"", SymbolType.STRING),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol(".", SymbolType.DOT),
            new Symbol("text", SymbolType.NAME),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("main", SymbolType.NAME),
            new Symbol(":", SymbolType.COLON),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("lw", SymbolType.NAME),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("$s0", SymbolType.REGISTER),
            new Symbol(",", SymbolType.COMMA),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("0", SymbolType.NUMBER),
            new Symbol("(", SymbolType.OPEN_PAREN),
            new Symbol("$sp", SymbolType.REGISTER),
            new Symbol(")", SymbolType.CLOSE_PAREN),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("add", SymbolType.NAME),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("$t0", SymbolType.REGISTER),
            new Symbol(",", SymbolType.COMMA),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("$zero", SymbolType.REGISTER),
            new Symbol(",", SymbolType.COMMA),
            new Symbol("$3", SymbolType.REGISTER),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("jr", SymbolType.NAME),
            new Symbol(" ", SymbolType.WHITESPACE),
            new Symbol("$ra", SymbolType.REGISTER)
        };
        Assert.Equal(expected.Length, given.Length);
        for (int index = 0; index < expected.Length; index++) {
            Assert.Equal(expected[index], given[index]);
        }
    }
}