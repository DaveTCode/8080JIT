using System;
using System.Collections.Generic;
using System.Linq;
using JIT8080.Generator;
using Xunit;

namespace JIT8080.Tests
{
    /// <summary>
    /// This class ensures that all possible combinations of opcode generate 
    /// valid (but not necessarily correct) IL
    /// </summary>
    public class ValidOpcodeTests
    {
        [Theory]
        [MemberData(nameof(AllBytes))]
        public void TestAllOpcodesGenerateValidIL(byte opcode)
        {
            var program = new byte[0x50];
            program[0] = 0x21;
            program[1] = 0x0A;
            program[2] = 0x00;
            program[3] = opcode;
            program[4] = 0x6;
            program[0x49] = 0x76; // HLT
            var emulator = Emulator.CreateEmulator(program, new TestMemoryBus(program), new TestIOHandler(), new TestRenderer());
            emulator.Run.Invoke(emulator.Emulator, Array.Empty<object>());
        }

        public static IEnumerable<object[]> AllBytes =>
            Enumerable.Range(0, byte.MaxValue)
                .Where(b => b != 0xC7) // Don't test RST 0 as it causes an infinite loop
                .Select(b => new object[] { b });
    }
}
