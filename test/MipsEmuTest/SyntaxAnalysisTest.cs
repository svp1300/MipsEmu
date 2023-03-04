using Xunit;

using System.Collections.Generic;
using MipsEmu.Assembler;
using MipsEmu.Assembler.Tokens;

public class SyntaxAnalysisTest {

    [Fact]
    public void TestLAExample() {
        var example = ".data message:.asciiz \"Hello world!\\n\" .text #hello\n.globl main main: la $a0, message li $v0, 4 syscall jr $ra";
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

    [Fact]
    public void TestSumTestExample() {
        var example = ".globl main main: li $t0, 8 move $t1, $zero sum_loop: beq $t0, $zero, sum_done addi $t0, $t0, -1 addi $t1, $t1, 2 j sum_loop sum_done: li $v0, 1 move $a0, $t1 syscall jr $ra";
        var symbols = LexicalAnalyzer.FindSymbols(example);
        var syntax = SyntaxAnalyzer.CreateDefaultSyntaxAnalyzer();
        var result = syntax.BuildProgram(symbols);
        var space = new Symbol(" ", SymbolType.WHITESPACE);
        var comma = new Symbol(",", SymbolType.COMMA);
        Assert.Equal(new List<Label>(), result.directiveLabels);
        Assert.Equal(new List<Token>() {
            new TextArgumentDirectiveToken(new Symbol[] {new Symbol(".", SymbolType.DOT), new Symbol("globl", SymbolType.NAME), space, new Symbol("main", SymbolType.NAME)})
        }, result.directiveTokens);
        Assert.Equal(new List<Label>() {new Label("main", 0), new Label("sum_loop", 8), new Label("sum_done", 24)}, result.instructionLabels);
        Assert.Equal(new List<Token>() {
            new PseudoInstructionToken(new Symbol[] {space, new Symbol("li", SymbolType.NAME), space, new Symbol("$t0", SymbolType.REGISTER), comma, space, new Symbol("8", SymbolType.NUMBER)}),
            new PseudoInstructionToken(new Symbol[] {space, new Symbol("move", SymbolType.NAME), space,  new Symbol("$t1", SymbolType.REGISTER), comma, space, new Symbol("$zero", SymbolType.REGISTER)}),
            new BranchInstructionToken(new Symbol[] {space, new Symbol("beq", SymbolType.NAME), space, new Symbol("$t0", SymbolType.REGISTER), comma, space, new Symbol("$zero", SymbolType.REGISTER), comma, space, new Symbol("sum_done", SymbolType.NAME)}),
            new TypeIInstructionToken(new Symbol[] {space, new Symbol("addi", SymbolType.NAME), space, new Symbol("$t0", SymbolType.REGISTER), comma, space, new Symbol("$t0", SymbolType.REGISTER), comma, space, new Symbol("-1", SymbolType.NUMBER)}),
            new TypeIInstructionToken(new Symbol[] {space, new Symbol("addi", SymbolType.NAME), space, new Symbol("$t1", SymbolType.REGISTER), comma, space, new Symbol("$t1", SymbolType.REGISTER), comma, space, new Symbol("2", SymbolType.NUMBER)}),
            new JumpInstructionToken(new Symbol[] {space, new Symbol("j", SymbolType.NAME), space, new Symbol("sum_loop", SymbolType.NAME)}),
            new PseudoInstructionToken(new Symbol[] {space, new Symbol("li", SymbolType.NAME), space, new Symbol("$v0", SymbolType.REGISTER), comma, space, new Symbol("1", SymbolType.NUMBER)}),
            new PseudoInstructionToken(new Symbol[] {space, new Symbol("move", SymbolType.NAME), space,  new Symbol("$a0", SymbolType.REGISTER), comma, space, new Symbol("$t1", SymbolType.REGISTER)}),
            new SyscallInstructionToken(new Symbol[] {space, new Symbol("syscall", SymbolType.NAME)}),
            new SingleRegisterInstructionToken(new Symbol[] {space, new Symbol("jr", SymbolType.NAME), space, new Symbol("$ra", SymbolType.REGISTER)})
        }, result.instructionTokens);

    }

}