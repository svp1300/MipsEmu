using Xunit;

using MipsEmu.Emulation.Devices;
using MipsEmu;

public class MemoryTest {

    [Fact]
    public void TestReadWrite() {
        var writeBits = new Bits(64);
        writeBits.SetFromUnsignedLong(0x562AFFA47742EE2D);
        var testBits = new Bits(64);
        var memory = new Ram(8);
        memory.StoreBytes(0, writeBits);
        var readBits = memory.LoadBytes(0, 8);
        Assert.Equal(writeBits, readBits);
    }

}