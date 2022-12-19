using Xunit;

using System;
using MipsEmu.Emulation.Instructions;
using MipsEmu;

public class InstructionParserTest {

    [Fact]
    public void TestIType() {
        var bits = new Bits(32);
        bits.Store(26, new bool[] {false, false, true, false, false, false}); // 001000
        var result = InstructionParser.parseInstruction(bits);
        Assert.NotNull(result);
        Assert.True(result is AddImmediateInstruction);
    }

    [Fact]
    public void TestJType() {
        var bits = new Bits(32);
        bits.Store(26, new bool[] {false, false, false, false, true, false});
        var result = InstructionParser.parseInstruction(bits);
        Assert.NotNull(result);
        Assert.True(result is JumpInstruction);
    }

    [Fact]
    public void TestRType() {
        var bits = new Bits(32);
        bits.Store(26, new bool[] {false, false, false, false, false, false});
        bits.Store(0, new bool[] {true, false, false, false, false, false});
        var result = InstructionParser.parseInstruction(bits);
        Assert.NotNull(result);
        Assert.True(result is AddInstruction);
    }
}