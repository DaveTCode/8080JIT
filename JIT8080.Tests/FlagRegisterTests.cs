using System;
using JIT8080.Generator;
using JIT8080.Tests.Mocks;
using Xunit;

namespace JIT8080.Tests
{
    public class FlagRegisterTests
    {
        [Theory]
        [InlineData(0b0000_0010, false, false, false, false, false)]
        [InlineData(0b1101_0111, true, true, true, true, true)]
        [InlineData(0b0000_0011, false, false, false, false, true)]
        [InlineData(0b0000_0110, false, false, false, true, false)]
        [InlineData(0b0001_0010, false, false, true, false, false)]
        [InlineData(0b0100_0010, false, true, false, false, false)]
        [InlineData(0b1000_0010, true, false, false, false, false)]
        public void TestGetFlagRegister(byte expected, bool sign, bool zero, bool aux, bool parity, bool carry)
        {
            var rom = new byte[] {0};
            var emulator = Emulator.CreateEmulator(rom, new TestMemoryBus(rom), new TestIOHandler(), new TestRenderer(), new TestInterruptUtils());
            emulator.Internals.SignFlag.SetValue(emulator.Emulator, sign);
            emulator.Internals.ZeroFlag.SetValue(emulator.Emulator, zero);
            emulator.Internals.AuxCarryFlag.SetValue(emulator.Emulator, aux);
            emulator.Internals.ParityFlag.SetValue(emulator.Emulator, parity);
            emulator.Internals.CarryFlag.SetValue(emulator.Emulator, carry);

            Assert.Equal(expected, emulator.Internals.GetFlagRegister.Invoke(emulator.Emulator, Array.Empty<object>()));
        }

        [Theory]
        [InlineData(0b0000_0010, false, false, false, false, false)]
        [InlineData(0b1101_0111, true, true, true, true, true)]
        [InlineData(0b0000_0011, false, false, false, false, true)]
        [InlineData(0b0000_0110, false, false, false, true, false)]
        [InlineData(0b0001_0010, false, false, true, false, false)]
        [InlineData(0b0100_0010, false, true, false, false, false)]
        [InlineData(0b1000_0010, true, false, false, false, false)]
        public void TestSetFlagRegister(byte initial, bool sign, bool zero, bool aux, bool parity, bool carry)
        {
            var rom = new byte[] {0};
            var emulator = Emulator.CreateEmulator(rom, new TestMemoryBus(rom), new TestIOHandler(), new TestRenderer(), new TestInterruptUtils());
            emulator.Internals.SetFlagRegister.Invoke(emulator.Emulator, new object[] {initial});

            Assert.Equal(sign, emulator.Internals.SignFlag.GetValue(emulator.Emulator));
            Assert.Equal(zero, emulator.Internals.ZeroFlag.GetValue(emulator.Emulator));
            Assert.Equal(aux, emulator.Internals.AuxCarryFlag.GetValue(emulator.Emulator));
            Assert.Equal(parity, emulator.Internals.ParityFlag.GetValue(emulator.Emulator));
            Assert.Equal(carry, emulator.Internals.CarryFlag.GetValue(emulator.Emulator));
        }

        [Fact]
        public void TestParityFlagImplementation()
        {
            var parityLookupTable = new[]
            {
                true, false, false, true, false, true, true, false, false, true, true, false, true, false, false, true,
                false, true, true, false, true, false, false, true, true, false, false, true, false, true, true, false,
                false, true, true, false, true, false, false, true, true, false, false, true, false, true, true, false,
                true, false, false, true, false, true, true, false, false, true, true, false, true, false, false, true,
                false, true, true, false, true, false, false, true, true, false, false, true, false, true, true, false,
                true, false, false, true, false, true, true, false, false, true, true, false, true, false, false, true,
                true, false, false, true, false, true, true, false, false, true, true, false, true, false, false, true,
                false, true, true, false, true, false, false, true, true, false, false, true, false, true, true, false,
                false, true, true, false, true, false, false, true, true, false, false, true, false, true, true, false,
                true, false, false, true, false, true, true, false, false, true, true, false, true, false, false, true,
                true, false, false, true, false, true, true, false, false, true, true, false, true, false, false, true,
                false, true, true, false, true, false, false, true, true, false, false, true, false, true, true, false,
                true, false, false, true, false, true, true, false, false, true, true, false, true, false, false, true,
                false, true, true, false, true, false, false, true, true, false, false, true, false, true, true, false,
                false, true, true, false, true, false, false, true, true, false, false, true, false, true, true, false,
                true, false, false, true, false, true, true, false, false, true, true, false, true, false, false, true
            };

            var rom = new byte[] {0x04, 0x76}; // INR B -> HLT
            var emulator = Emulator.CreateEmulator(rom, new TestMemoryBus(rom), new TestIOHandler(), new TestRenderer(), new TestInterruptUtils());
            emulator.Internals.B.SetValue(emulator.Emulator, (byte)0xFF);

            for (var ii = 0; ii < 0xFF; ii++)
            {
                emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());
                Assert.Equal(parityLookupTable[ii], emulator.Internals.ParityFlag.GetValue(emulator.Emulator));
            }
        }
    }
}
