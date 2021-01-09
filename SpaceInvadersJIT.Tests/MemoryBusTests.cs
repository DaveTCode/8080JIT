using Xunit;

namespace SpaceInvadersJIT.Tests
{
    public class MemoryBusTests
    {
        [Fact]
        public void TestMemoryBusReadWrite()
        {
            var program = new byte[0x2000];
            var memoryBus = new SpaceInvadersApplication(program);

            for (ushort ii = 0; ii < ushort.MaxValue; ii++)
            {
                memoryBus.WriteByte((byte)(ii + 1), ii);
                if (ii < 0x2000)
                {
                    Assert.Equal(0, memoryBus.ReadByte(ii));
                }
                else
                {
                    Assert.Equal((byte)(ii + 1), memoryBus.ReadByte(ii));
                }
            }
        }
    }
}
