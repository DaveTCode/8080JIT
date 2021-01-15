using System;
using JIT8080._8080;

namespace JIT8080.Tests.Mocks
{
    internal class TestMemoryBus : IMemoryBus8080
    {
        private readonly byte[] _memory = new byte[0x10000];

        internal TestMemoryBus(byte[] rom)
        {
            Array.Copy(rom, _memory, Math.Min(_memory.Length, rom.Length));
        }

        public byte ReadByte(ushort address) => _memory[address];

        public void WriteByte(byte value, ushort address)
        {
            _memory[address] = value;
        }
    }
}
