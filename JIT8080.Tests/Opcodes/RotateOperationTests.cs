using System;
using JIT8080.Generator;
using Xunit;

namespace JIT8080.Tests.Opcodes
{
    public class RotateOperationTests
    {
        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(1, 2, false)]
        [InlineData(0b1000_0000, 1, true)]
        public void TestRLCOpcode(byte original, byte expected, bool carryFlag)
        {
            var rom = new byte[] {0x3E, original, 0x07, 0x76};
            var emulator = Emulator.CreateEmulator(rom, new TestMemoryBus(rom), new TestIOHandler(), new TestRenderer());
            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal(expected, emulator.Internals.A.GetValue(emulator.Emulator));
            Assert.Equal(carryFlag, emulator.Internals.CarryFlag.GetValue(emulator.Emulator));
        }

        [Theory]
        [InlineData(0, 0, false, false)]
        [InlineData(0, 1, true, false)]
        [InlineData(1, 2, false, false)]
        [InlineData(0b1000_0000, 0, false, true)]
        [InlineData(0b1000_0000, 1, true, true)]
        public void TestRALOpcode(byte original, byte expected, bool originalCarryFlag, bool carryFlag)
        {
            var rom = new byte[] {0x3E, original, 0x17, 0x76};
            var emulator = Emulator.CreateEmulator(rom, new TestMemoryBus(rom), new TestIOHandler(), new TestRenderer());
            emulator.Internals.CarryFlag.SetValue(emulator.Emulator, originalCarryFlag);
            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal(expected, emulator.Internals.A.GetValue(emulator.Emulator));
            Assert.Equal(carryFlag, emulator.Internals.CarryFlag.GetValue(emulator.Emulator));
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(1, 0b1000_0000, true)]
        [InlineData(0b1000_0000, 0b0100_0000, false)]
        public void TestRRCOpcode(byte original, byte expected, bool carryFlag)
        {
            var rom = new byte[] {0x3E, original, 0x0F, 0x76};
            var emulator = Emulator.CreateEmulator(rom, new TestMemoryBus(rom), new TestIOHandler(), new TestRenderer());
            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal(expected, emulator.Internals.A.GetValue(emulator.Emulator));
            Assert.Equal(carryFlag, emulator.Internals.CarryFlag.GetValue(emulator.Emulator));
        }

        [Theory]
        [InlineData(0, 0, false, false)]
        [InlineData(1, 0, false, true)]
        [InlineData(1, 0b1000_0000, true, true)]
        [InlineData(0b1000_0000, 0b0100_0000, false, false)]
        [InlineData(0b1000_0000, 0b1100_0000, true, false)]
        public void TestRAROpcode(byte original, byte expected, bool originalCarryFlag, bool carryFlag)
        {
            var rom = new byte[] {0x3E, original, 0x1F, 0x76};
            var emulator = Emulator.CreateEmulator(rom, new TestMemoryBus(rom), new TestIOHandler(), new TestRenderer());
            emulator.Internals.CarryFlag.SetValue(emulator.Emulator, originalCarryFlag);
            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal(expected, emulator.Internals.A.GetValue(emulator.Emulator));
            Assert.Equal(carryFlag, emulator.Internals.CarryFlag.GetValue(emulator.Emulator));
        }
    }
}
