using Xunit;
using MipsEmu;

using MipsEmu.Emulation.Devices;

using System;
namespace MipsEmuTest;

public class AluTest {
        
        /// <summary>Get the signed integer representation of a and b, find the sum, and return the result in Bits.
        ///Assumes a and b are the same size.</summary>
        /// <param name="a">First value of the sum.</param>
        /// <param name="b">Second value of the sum.</param>
        /// <returns>The sum in Bits.</returns>
        public Bits AddSigned(Bits a, Bits b) {
            var aValue = a.GetAsSignedInt();
            var bValue = b.GetAsSignedInt();
            var result = new Bits(a.GetLength());
            result.SetFromSignedInt(aValue + bValue);
            return result;
        }

        [Fact]
        public void AddUnsignedTest() {
            
        }

}