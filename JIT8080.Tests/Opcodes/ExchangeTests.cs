using System;
using JIT8080.Generator;
using JIT8080.Tests.Mocks;
using Xunit;

namespace JIT8080.Tests.Opcodes
{
    public class ExchangeTests
    {
        [Theory]
        [InlineData(1, 2, 3, 4)]
        [InlineData(255, 254, 253, 252)]
        public void TestXCHGOpcode(byte h, byte l, byte d, byte e)
        {
            var rom = new byte[] {0xEB, 0x76};
            var emulator = Emulator.CreateEmulator(rom, new TestMemoryBus(rom), new TestIOHandler(), new TestRenderer(), new TestInterruptUtils());
            emulator.Internals.H.SetValue(emulator.Emulator, h);
            emulator.Internals.L.SetValue(emulator.Emulator, l);
            emulator.Internals.D.SetValue(emulator.Emulator, d);
            emulator.Internals.E.SetValue(emulator.Emulator, e);

            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal(h, emulator.Internals.D.GetValue(emulator.Emulator));
            Assert.Equal(l, emulator.Internals.E.GetValue(emulator.Emulator));
            Assert.Equal(d, emulator.Internals.H.GetValue(emulator.Emulator));
            Assert.Equal(e, emulator.Internals.L.GetValue(emulator.Emulator));
        }

        [Theory]
        [InlineData(1, 2, 3, 4)]
        [InlineData(255, 254, 253, 252)]
        public void TestXTHLOpcode(byte h, byte l, byte mem1, byte mem2)
        {
            var rom = new byte[] {0xE3, 0x76};
            var memoryBus = new TestMemoryBus(rom);
            memoryBus.WriteByte(mem1, 0x3500);
            memoryBus.WriteByte(mem2, 0x3501);
            var emulator = Emulator.CreateEmulator(rom, memoryBus, new TestIOHandler(), new TestRenderer(), new TestInterruptUtils());
            emulator.Internals.H.SetValue(emulator.Emulator, h);
            emulator.Internals.L.SetValue(emulator.Emulator, l);
            emulator.Internals.StackPointer.SetValue(emulator.Emulator, (ushort)0x3500);

            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal(h, memoryBus.ReadByte(0x3501));
            Assert.Equal(l, memoryBus.ReadByte(0x3500));
            Assert.Equal(mem2, emulator.Internals.H.GetValue(emulator.Emulator));
            Assert.Equal(mem1, emulator.Internals.L.GetValue(emulator.Emulator));
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(0xFF, 0xFF, 0xFFFF)]
        [InlineData(0x50, 0x6C, 0x506C)]
        public void TestSPHLOpcode(byte h, byte l, ushort result)
        {
            var rom = new byte[] {0xF9, 0x76};
            var memoryBus = new TestMemoryBus(rom);
            var emulator = Emulator.CreateEmulator(rom, memoryBus, new TestIOHandler(), new TestRenderer(), new TestInterruptUtils());
            emulator.Internals.H.SetValue(emulator.Emulator, h);
            emulator.Internals.L.SetValue(emulator.Emulator, l);
            emulator.Internals.StackPointer.SetValue(emulator.Emulator, (ushort)0x1234);

            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal(h, emulator.Internals.H.GetValue(emulator.Emulator));
            Assert.Equal(l, emulator.Internals.L.GetValue(emulator.Emulator));
            
            Assert.Equal(result, emulator.Internals.StackPointer.GetValue(emulator.Emulator));
        }
    }
}
