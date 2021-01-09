using System;
using JIT8080._8080;

namespace JIT8080.Tests
{
    internal class TestMemoryBus : IMemoryBus8080
    {
        internal byte[] Memory = new byte[0x10000];

        internal TestMemoryBus(byte[] rom)
        {
            Array.Copy(rom, Memory, Math.Min(Memory.Length, rom.Length));
        }

        public byte ReadByte(ushort address) => Memory[address];

        public void WriteByte(byte value, ushort address)
        {
            Memory[address] = value;
        }
    }
}
