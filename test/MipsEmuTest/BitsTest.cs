using Xunit;
using MipsEmu;

using System;
namespace MipsEmuTest;

public class BitsTest {

    // private void TestTools.CheckEquivalent(bool[] a, bool[] b) {
    //     Assert.Equal(a.Length, b.Length);
    //     for (int i = 0; i < b.Length; i++) {
    //         Assert.Equal(a[i], b[i]);
    //     }
    // }

    [Fact]
    public void WrapperTest() {
        var cut = new Bits(new bool[] {true, false, false, true, true, false, true});
        TestTools.CheckEquivalent(new bool[] {true, false, false, true, true, false, true}, cut.GetValues());
    }

    [Fact]
    public void SizeTest() {
        for(int size = 1; size < 5; size++) {
            Bits cut = new Bits(size);
            Assert.Equal(size, cut.GetLength());
        }
    }

    [Fact]
    public void StoreTest() {
        var cut = new Bits(new bool[] {false, false, false, false});
        cut.Store(1, new bool[] {true, true});
        TestTools.CheckEquivalent(cut.GetValues(), new bool[] {false, true, true, false});
        // test touching end
        cut = new Bits(new bool[] {false, false, false, false});
        cut.Store(2, new bool[] {true, true});
        TestTools.CheckEquivalent(cut.GetValues(), new bool[] {false, false, true, true});
    }

    [Fact]
    public void SignExtendTest() {
        var cut = new Bits(new bool[] {true, true, true, true});
        Assert.Equal(4, cut.GetLength());
        cut = cut.SignExtend(4);
        Assert.Equal(8, cut.GetLength());
        var result = cut.GetValues();
        var target = new bool[] {false, false, false, false, true, true, true, true};
        TestTools.CheckEquivalent(result, target);
    }

    [Fact]
    public void LoadTest() {
        var cut = new Bits(new bool[] {true, false, true, false});
        TestTools.CheckEquivalent(new bool[]{false, true}, cut.Load(1, 2));
        TestTools.CheckEquivalent(new bool[]{true, false, true}, cut.Load(0, 3));
        TestTools.CheckEquivalent(new bool[]{true, false}, cut.Load(2, 2));
    }

    [Fact]
    public void GetSignedIntTest() {
        var cut = new Bits(new bool[] {false, true, false, true});
        Assert.Equal(-6, cut.GetAsSignedInt());
        cut = new Bits(new bool[] {true, true, true, false});
        Assert.Equal(7, cut.GetAsSignedInt());
    }

    // 010101100
    [Fact]
    public void GetSignedIntFromRangeTest() {
                                    //   0      0      1     1     0      1     0     1      0
        var cut = new Bits(new bool[] {false, false, true, true, false, true, false, true, false});
        Assert.Equal(-5, cut.GetSignedIntFromRange(2, 4));
        Assert.Equal(5, cut.GetSignedIntFromRange(3, 4));
        Assert.Equal(5, cut.GetSignedIntFromRange(5, 4));

    }

    [Fact]
    public void GetUnsignedIntTest() {
        var cut = new Bits(new bool[] {false, true, false, true});
        Assert.Equal(5, cut.GetAsUnsignedInt());
        cut = new Bits(new bool[] {true, true, true, false});
        Assert.Equal(14, cut.GetAsUnsignedInt());       
    } 

    [Fact]
    public void SetFromSignedIntTest() {
        var cut = new Bits(5);
        cut.SetFromSignedInt(5);  
        TestTools.CheckEquivalent(new bool[]{false, false, true, false, true}, cut.GetValues());
        cut.SetFromSignedInt(-16 + 4);
        TestTools.CheckEquivalent(new bool[]{true, false, true, false, false}, cut.GetValues());
        cut.SetFromSignedInt(7);
        TestTools.CheckEquivalent(new bool[]{false, false, true, true, true}, cut.GetValues());
        cut.SetFromSignedInt(0);
        TestTools.CheckEquivalent(new bool[]{false, false, false, false, false}, cut.GetValues());
        cut.SetFromSignedInt(10);
        TestTools.CheckEquivalent(new bool[]{false, true, false, true, false}, cut.GetValues());
    }

    [Fact]
    public void SetFromUnsignedIntTest() {
        var cut = new Bits(4);
        cut.SetFromUnsignedInt(0);
        TestTools.CheckEquivalent(new bool[]{false, false, false, false}, cut.GetValues());
        cut.SetFromUnsignedInt(15);
        TestTools.CheckEquivalent(new bool[]{true, true, true, true}, cut.GetValues());
        cut.SetFromUnsignedInt(6);
        TestTools.CheckEquivalent(new bool[]{false, true, true, false}, cut.GetValues());
    }
}