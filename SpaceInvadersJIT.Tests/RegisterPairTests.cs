using System;
using SpaceInvadersJIT._8080;
using SpaceInvadersJIT.Generator;
using Xunit;

namespace SpaceInvadersJIT.Tests
{
    public class RegisterPairTests
    {
        [Fact]
        public void TestHL()
        {
            var rom = new byte[] { 0x76 };
            var emulator = Emulator.CreateEmulator(rom, new MemoryBus8080(rom), new IOHandler());

            Assert.Equal((ushort)0, emulator.Internals.HL.Invoke(emulator.Emulator, Array.Empty<object>()));
            emulator.Internals.H.SetValue(emulator.Emulator, (byte)1);
            Assert.Equal((ushort)256, emulator.Internals.HL.Invoke(emulator.Emulator, Array.Empty<object>()));
            emulator.Internals.L.SetValue(emulator.Emulator, (byte)1);
            Assert.Equal((ushort)257, emulator.Internals.HL.Invoke(emulator.Emulator, Array.Empty<object>()));
        }

        [Fact]
        public void TestBC()
        {
            var rom = new byte[] { 0x76 };
            var emulator = Emulator.CreateEmulator(rom, new MemoryBus8080(rom), new IOHandler());

            Assert.Equal((ushort)0, emulator.Internals.BC.Invoke(emulator.Emulator, Array.Empty<object>()));
            emulator.Internals.B.SetValue(emulator.Emulator, (byte)1);
            Assert.Equal((ushort)256, emulator.Internals.BC.Invoke(emulator.Emulator, Array.Empty<object>()));
            emulator.Internals.C.SetValue(emulator.Emulator, (byte)1);
            Assert.Equal((ushort)257, emulator.Internals.BC.Invoke(emulator.Emulator, Array.Empty<object>()));
        }

        [Fact]
        public void TestDE()
        {
            var rom = new byte[] { 0x76 };
            var emulator = Emulator.CreateEmulator(rom, new MemoryBus8080(rom), new IOHandler());

            Assert.Equal((ushort)0, emulator.Internals.DE.Invoke(emulator.Emulator, Array.Empty<object>()));
            emulator.Internals.D.SetValue(emulator.Emulator, (byte)1);
            Assert.Equal((ushort)256, emulator.Internals.DE.Invoke(emulator.Emulator, Array.Empty<object>()));
            emulator.Internals.E.SetValue(emulator.Emulator, (byte)1);
            Assert.Equal((ushort)257, emulator.Internals.DE.Invoke(emulator.Emulator, Array.Empty<object>()));
        }
    }
}
