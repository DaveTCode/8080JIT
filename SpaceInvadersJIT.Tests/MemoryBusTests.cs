using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceInvadersJIT._8080;
using Xunit;

namespace SpaceInvadersJIT.Tests
{
    public class MemoryBusTests
    {
        [Fact]
        public void TestMemoryBusReadWrite()
        {
            var program = new byte[0x2000];
            var memoryBus = new MemoryBus8080(program);

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
