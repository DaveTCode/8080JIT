using Xunit;

namespace SpaceInvadersJIT.Tests
{
    public class ShiftRegisterTests
    {
        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(0, 0xFF, 0, 0xFF)]
        [InlineData(0xFF, 0x0, 0, 0x0)]
        [InlineData(0b1111_1111, 0, 1, 0b1)]
        [InlineData(0b1111_1111, 0, 2, 0b11)]
        [InlineData(0b1111_1111, 0, 3, 0b111)]
        [InlineData(0b1111_1111, 0, 4, 0b1111)]
        [InlineData(0b1111_1111, 0, 5, 0b1111_1)]
        [InlineData(0b1111_1111, 0, 6, 0b1111_11)]
        [InlineData(0b1111_1111, 0, 7, 0b1111_111)]
        [InlineData(0b1111_1111, 0, 8, 0)]
        public void TestShiftRegister(byte lowByte, byte highByte, byte offset, byte expectedResult)
        {
            var app = new SpaceInvadersApplication(System.Array.Empty<byte>());
            app.Out(2, offset);
            app.Out(4, lowByte);
            app.Out(4, highByte);
            Assert.Equal(expectedResult, app.In(3));
        }
    }
}
