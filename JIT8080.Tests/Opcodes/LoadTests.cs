using System;
using JIT8080.Generator;
using Xunit;

namespace JIT8080.Tests.Opcodes
{
    /// <summary>
    /// Test simple instructions to read/write registers to memory
    /// </summary>
    public class LoadTests
    {
        [Fact]
        public void TestLDAAndSTA()
        {
            var rom = new byte[] {0x32, 0x00, 0x01, 0xAF, 0x3A, 0x00, 0x01, 0x76};
            var memoryBus = new TestMemoryBus(rom);
            var emulator =
                Emulator.CreateEmulator(rom, memoryBus, new TestIOHandler(), new TestRenderer());
            emulator.Internals.A.SetValue(emulator.Emulator, (byte)0x10);

            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal((byte)0x10, emulator.Internals.A.GetValue(emulator.Emulator));
            Assert.Equal((byte)0x10, memoryBus.ReadByte(0x100));
        }
    }
}