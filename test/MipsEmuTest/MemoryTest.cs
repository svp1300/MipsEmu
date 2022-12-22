using Xunit;

using MipsEmu.Emulation.Devices;
using MipsEmu;

public class MemoryTest {

    [Fact]
    public void TestReadWrite() {
        var writeBits = new Bits(64);
        writeBits.SetFromUnsignedLong(0x562AFFA47742EE2D);
        var testBits = new Bits(64);
        var memory = new Ram(testBits);
        memory.StoreBits(0, writeBits, 64);
        Assert.Equal(writeBits, testBits);
        var readBits = memory.LoadBits(0, 64);
        Assert.Equal(writeBits, readBits);
    }

}