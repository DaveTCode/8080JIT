using JIT8080._8080;

namespace JIT8080.Tests.Mocks
{
    internal class TestIOHandler : IIOHandler
    {
        public void Out(byte port, byte value)
        {
            
        }

        public byte In(byte port)
        {
            return 0x0;
        }
    }
}
