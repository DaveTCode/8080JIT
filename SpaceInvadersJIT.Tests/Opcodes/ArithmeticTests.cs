using System;
using SpaceInvadersJIT._8080;
using SpaceInvadersJIT.Generator;
using Xunit;

namespace SpaceInvadersJIT.Tests.Opcodes
{
    public class ArithmeticTests
    {
        [Theory]
        [InlineData(0x09, 0x33, 0x9F, 0xA1, 0x7B, 0xD51A, false)]
        [InlineData(0x09, 0xFF, 0xFF, 0x00, 0x01, 0, true)]
        public void TestDADOpcode(byte opcode, byte highByte, byte lowByte, byte h, byte l, ushort expected,
            bool carryFlag)
        {
            var rom = new byte[] {opcode, 0x76};
            var emulator = Emulator.CreateEmulator(rom, new MemoryBus8080(rom), new IOHandler());
            switch (opcode)
            {
                case 0x09:
                    emulator.Internals.B.SetValue(emulator.Emulator, highByte);
                    emulator.Internals.C.SetValue(emulator.Emulator, lowByte);
                    break;
                case 0x19:
                    emulator.Internals.D.SetValue(emulator.Emulator, highByte);
                    emulator.Internals.E.SetValue(emulator.Emulator, lowByte);
                    break;
                case 0x29:
                    emulator.Internals.H.SetValue(emulator.Emulator, highByte);
                    emulator.Internals.L.SetValue(emulator.Emulator, lowByte);
                    break;
                case 0x39:
                    emulator.Internals.StackPointer.SetValue(emulator.Emulator, (ushort) ((highByte << 8) | lowByte));
                    break;
            }

            emulator.Internals.H.SetValue(emulator.Emulator, h);
            emulator.Internals.L.SetValue(emulator.Emulator, l);
            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal(expected, emulator.Internals.HL.Invoke(emulator.Emulator, Array.Empty<object>()));
            Assert.Equal(carryFlag, emulator.Internals.CarryFlag.GetValue(emulator.Emulator));
        }

        [Theory]
        [InlineData(0x42, 0x3D, false, 0x7F, false, false, false, false, false)]
        [InlineData(0x42, 0x3D, true, 0x80, true, false, false, false, true)]
        public void TestADCOpcode(byte a, byte operand, bool carryFlag, byte result, bool expectedSignFlag,
            bool expectedZeroFlag, bool expectedCarryFlag, bool expectedParityFlag, bool expectedAuxCarryFlag)
        {
            var rom = new byte[] {0x88, 0x76};
            var emulator = Emulator.CreateEmulator(rom, new MemoryBus8080(rom), new IOHandler());
            emulator.Internals.A.SetValue(emulator.Emulator, a);
            emulator.Internals.B.SetValue(emulator.Emulator, operand);
            emulator.Internals.CarryFlag.SetValue(emulator.Emulator, carryFlag);

            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());
            Assert.Equal(result, emulator.Internals.A.GetValue(emulator.Emulator));
            Assert.Equal(expectedSignFlag, emulator.Internals.SignFlag.GetValue(emulator.Emulator));
            Assert.Equal(expectedZeroFlag, emulator.Internals.ZeroFlag.GetValue(emulator.Emulator));
            Assert.Equal(expectedCarryFlag, emulator.Internals.CarryFlag.GetValue(emulator.Emulator));
            //Assert.Equal(expectedParityFlag, emulator.Internals.ParityFlag.GetValue(emulator.Emulator));
            //Assert.Equal(expectedAuxCarryFlag, emulator.Internals.AuxCarryFlag.GetValue(emulator.Emulator));
        }
    }
}