using System;
using JIT8080.Generator;
using Xunit;

namespace JIT8080.Tests.Opcodes
{
    public class StackTests
    {
        [Theory]
        [InlineData(0x06, 0x0E, 0xC5, 0xC1)] // BC
        [InlineData(0x16, 0x1E, 0xD5, 0xD1)] // DE
        [InlineData(0x26, 0x2E, 0xE5, 0xE1)] // HL
        public void TestPushPopRegisterPair(byte firstRegOpcode, byte secondRegisterOpcode, byte pushOpcode, byte popOpcode)
        {
            var setupRom = new byte[]
            {
                0x31, 0x00, 0x01, // SP to 0x100 
                firstRegOpcode, 0x01, // MVI B, 0x01
                secondRegisterOpcode, 0x02, // MVI C, 0x02
                pushOpcode,
                popOpcode,
                0x76, // HLT 
            };
            var testMemoryBus = new TestMemoryBus(setupRom);
            var emulator =
                Emulator.CreateEmulator(setupRom, testMemoryBus, new TestIOHandler(), new TestRenderer());

            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            // All registers should still have their original values but the values pushed to the stack
            // should still be at the locations they were written.
            Assert.Equal((ushort)0x100, emulator.Internals.StackPointer.GetValue(emulator.Emulator));
            Assert.Equal((byte)0x01, testMemoryBus.ReadByte(0xFF));
            Assert.Equal((byte)0x02, testMemoryBus.ReadByte(0xFE));
            switch (firstRegOpcode)
            {
                case 0x06:
                    Assert.Equal((byte)0x01, emulator.Internals.B.GetValue(emulator.Emulator));
                    Assert.Equal((byte)0x02, emulator.Internals.C.GetValue(emulator.Emulator));
                    break;
                case 0x16:
                    Assert.Equal((byte)0x01, emulator.Internals.D.GetValue(emulator.Emulator));
                    Assert.Equal((byte)0x02, emulator.Internals.E.GetValue(emulator.Emulator));
                    break;
                case 0x26:
                    Assert.Equal((byte)0x01, emulator.Internals.H.GetValue(emulator.Emulator));
                    Assert.Equal((byte)0x02, emulator.Internals.L.GetValue(emulator.Emulator));
                    break;
            }
        }

        [Fact]
        public void TestPushPopPSW()
        {
            var setupRom = new byte[]
            {
                0x31, 0x00, 0x01, // SP to 0x100 
                0x3E, 0x01, // MVI A, 0x01
                0x37, // STC
                0xF5, // PUSH PSW
                0xF1, // POP PSW
                0x76, // HLT 
            };
            var testMemoryBus = new TestMemoryBus(setupRom);
            var emulator =
                Emulator.CreateEmulator(setupRom, testMemoryBus, new TestIOHandler(), new TestRenderer());

            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            // All registers should still have their original values but the values pushed to the stack
            // should still be at the locations they were written.
            Assert.Equal((ushort)0x100, emulator.Internals.StackPointer.GetValue(emulator.Emulator));
            Assert.Equal((byte)0x01, testMemoryBus.ReadByte(0xFF));
            Assert.Equal((byte)0b0000_0011, testMemoryBus.ReadByte(0xFE));
            Assert.Equal((byte)0x01, emulator.Internals.A.GetValue(emulator.Emulator));
            Assert.Equal(true, emulator.Internals.CarryFlag.GetValue(emulator.Emulator));
            Assert.Equal(false, emulator.Internals.SignFlag.GetValue(emulator.Emulator));
            Assert.Equal(false, emulator.Internals.ZeroFlag.GetValue(emulator.Emulator));
            Assert.Equal(false, emulator.Internals.AuxCarryFlag.GetValue(emulator.Emulator));
            Assert.Equal(false, emulator.Internals.ParityFlag.GetValue(emulator.Emulator));
        }
    }
}