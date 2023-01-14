using Xunit;

using System.Collections.Generic;
using MipsEmu.Assembler;
using MipsEmu.Assembler.Tokens;

public class SyntaxAnalysisTest {

    [Fact]
    public void TestLAExample() {
        var example = ".data message:.asciiz \"Hello world!\\n\" .text .globl main main: la $a0, message li $v0, 4 syscall jr $ra";
        var symbols = LexicalAnalyzer.FindSymbols(example);
        var syntax = SyntaxAnalyzer.CreateDefaultSyntaxAnalyzer();
        var result = syntax.BuildProgram(symbols);
        var space = new Symbol(" ", SymbolType.WHITESPACE);
        var comma = new Symbol(",", SymbolType.COMMA);
        Assert.Equal(new List<Label>() {new Label("message", 0)}, result.directiveLabels);
        Assert.Equal(new List<Token>() {
            new ArgumentlessDirectiveToken(new Symbol[] {new Symbol(".", SymbolType.DOT), new Symbol("data", SymbolType.NAME)}),
            new StringArgumentDirectiveToken(new Symbol[] {new Symbol(".", SymbolType.DOT), new Symbol("asciiz", SymbolType.NAME), space, new Symbol("\"Hello world!\\n\"", SymbolType.STRING)}),
            new ArgumentlessDirectiveToken(new Symbol[] {space, new Symbol(".", SymbolType.DOT), new Symbol("text", SymbolType.NAME)}),
            new TextArgumentDirectiveToken(new Symbol[] {space, new Symbol(".", SymbolType.DOT), new Symbol("globl", SymbolType.NAME), space, new Symbol("main", SymbolType.NAME)})
            
        }, result.directiveTokens);
        Assert.Equal(new List<Label>() {new Label("main", 0)}, result.instructionLabels);
        Assert.Equal(new List<Token>() {
            new PseudoInstructionToken(new Symbol[] {space, new Symbol("la", SymbolType.NAME), space, new Symbol("$a0", SymbolType.REGISTER), comma, space, new Symbol("message", SymbolType.NAME)}),
            new PseudoInstructionToken(new Symbol[] {space, new Symbol("li", SymbolType.NAME), space, new Symbol("$v0", SymbolType.REGISTER), comma, space, new Symbol("4", SymbolType.NUMBER)}),
            new SyscallInstructionToken(new Symbol[] {space, new Symbol("syscall", SymbolType.NAME)}),
            new SingleRegisterInstructionToken(new Symbol[] {space, new Symbol("jr", SymbolType.NAME), space, new Symbol("$ra", SymbolType.REGISTER)})
        }, result.instructionTokens);
    }

}