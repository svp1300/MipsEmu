using Xunit;

namespace MipsEmuTest;

public class TestTools {
    
    public static void CheckEquivalent(bool[] a, bool[] b) {
        Assert.Equal(a.Length, b.Length);
        for (int i = 0; i < b.Length; i++) {
            Assert.Equal(a[i], b[i]);
        }
    }
}