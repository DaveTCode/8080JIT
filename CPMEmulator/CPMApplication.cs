using System;
using JIT8080._8080;

namespace CPMEmulator
{
    internal class CPMApplication : IMemoryBus8080, IIOHandler, IRenderer
    {
        private static readonly byte[] Bios = {
            0x76, 0x00, 0x01, 0x0, 0x0, // BOOT treated as HLT to exit if this is called
            0xDB, 0x03, 0xC9, // CONST treated as IN(3)
            0xDB, 0x02, 0xC9, // CONIN treated as IN(2)
            0xF5, 0x79, 0xD3, 0x02, 0xF1, 0xC9 // CONOUT treated as PUSH AF, MOV A,C, OUT (2), POP AF (which is more than the 3 bytes really allowed but I don't want to emulate LIST at 0x09 so whatever
        };

        private readonly byte[] _memory = new byte[0x10000];
        private readonly int _romLength;

        public CPMApplication(byte[] rom)
        {
            if (rom.Length > (0xFFFF - 0x100))
            {
                throw new ArgumentOutOfRangeException(nameof(rom), rom, "Rom too large");
            }

            Array.Copy(Bios, _memory, Bios.Length);
            for (var ii = Bios.Length; ii < 0x100; ii++)
            {
                _memory[ii] = 0xC9; // Fill the rest of the bios with RET so that we immediately return from any calls
            }
            Array.Copy(rom, 0, _memory, 0x100, rom.Length);
            _romLength = rom.Length;
        }

        internal Span<byte> CompleteProgram() => _memory.AsSpan(0, Bios.Length + _romLength);

        public byte ReadByte(ushort address) => _memory[address];

        public void WriteByte(byte value, ushort address) => _memory[address] = value;

        public void Out(byte port, byte value)
        {
            switch (port)
            {
                case 2: // CONOUT
                    Console.Write((char)value);
                    break;
            }
        }

        public byte In(byte port) => port switch
        {
            2 => (byte) Console.ReadKey(false).KeyChar, // Treat IN (2) as IO CONIN
            3 => 0xFF, // Treat IN (3) as CONST
            _ => throw new NotImplementedException($"Unexpected IN call to port {port}"),
        };

        public void VBlank()
        {
            throw new NotImplementedException();
        }
    }
}