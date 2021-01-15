using System;
using JIT8080.Generator;
using JIT8080.Tests.Mocks;
using Xunit;

namespace JIT8080.Tests.Opcodes
{
    public class JumpTests
    {
        [Theory]
        [InlineData(0xC3)]
        [InlineData(0xCB)]
        public void TestJMP(byte opcode)
        {
            var rom = new byte[] {opcode, 0x04, 0x00, 0x04, 0x76};
            var emulator =
                Emulator.CreateEmulator(rom, new TestMemoryBus(rom), new TestIOHandler(), new TestRenderer(), new TestInterruptUtils());

            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal((byte)0, emulator.Internals.B.GetValue(emulator.Emulator));
        }

        [Theory]
        [InlineData(0xC2, false, 0)] // JNZ
        [InlineData(0xC2, true, 1)] // JNZ
        [InlineData(0xCA, false, 1)] // JZ
        [InlineData(0xCA, true, 0)] // JZ
        
        [InlineData(0xD2, false, 0)] // JNC
        [InlineData(0xD2, true, 1)] // JNC
        [InlineData(0xDA, false, 1)] // JC
        [InlineData(0xDA, true, 0)] // JC

        [InlineData(0xE2, false, 0)] // JPO
        [InlineData(0xE2, true, 1)] // JPO
        [InlineData(0xEA, false, 1)] // JPE
        [InlineData(0xEA, true, 0)] // JPE

        [InlineData(0xF2, false, 0)] // JP
        [InlineData(0xF2, true, 1)] // JP
        [InlineData(0xFA, false, 1)] // JM
        [InlineData(0xFA, true, 0)] // JM
        public void TestJwFlag(byte opcode, bool flagValue, byte expectedResult)
        {
            var rom = new byte[] {opcode, 0x04, 0x00, 0x04, 0x76};
            var emulator =
                Emulator.CreateEmulator(rom, new TestMemoryBus(rom), new TestIOHandler(), new TestRenderer(), new TestInterruptUtils());
            switch (opcode)
            {
                case 0xC2:
                case 0xCA:
                    emulator.Internals.ZeroFlag.SetValue(emulator.Emulator, flagValue);
                    break;
                case 0xD2:
                case 0xDA:
                    emulator.Internals.CarryFlag.SetValue(emulator.Emulator, flagValue);
                    break;
                case 0xE2:
                case 0xEA:
                    emulator.Internals.ParityFlag.SetValue(emulator.Emulator, flagValue);
                    break;
                case 0xF2:
                case 0xFA:
                    emulator.Internals.SignFlag.SetValue(emulator.Emulator, flagValue);
                    break;
            }

            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());

            Assert.Equal(expectedResult, emulator.Internals.B.GetValue(emulator.Emulator));
        }
    }
}
