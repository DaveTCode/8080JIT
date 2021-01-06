using System;
using SpaceInvadersJIT._8080;
using SpaceInvadersJIT.Generator;
using Xunit;

namespace SpaceInvadersJIT.Tests
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
            var emulator = Emulator.CreateEmulator(rom, new MemoryBus8080(rom), new IOHandler());
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
            var emulator = Emulator.CreateEmulator(rom, new MemoryBus8080(rom), new IOHandler());
            emulator.Internals.SetFlagRegister.Invoke(emulator.Emulator, new object[] {initial});

            Assert.Equal(sign, emulator.Internals.SignFlag.GetValue(emulator.Emulator));
            Assert.Equal(zero, emulator.Internals.ZeroFlag.GetValue(emulator.Emulator));
            Assert.Equal(aux, emulator.Internals.AuxCarryFlag.GetValue(emulator.Emulator));
            Assert.Equal(parity, emulator.Internals.ParityFlag.GetValue(emulator.Emulator));
            Assert.Equal(carry, emulator.Internals.CarryFlag.GetValue(emulator.Emulator));
        }
    }
}
