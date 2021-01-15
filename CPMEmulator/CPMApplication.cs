using System;
using System.Reflection.Emit;
using System.Text;
using JIT8080._8080;
using JIT8080.Generator;

namespace CPMEmulator
{
    internal class CPMApplication : IMemoryBus8080, IIOHandler, IRenderer, IInterruptUtils
    {
        private static readonly byte[] Bios = {
            0x76, 0x76, 0x76, 0x76, 0x76, // 00-04 all just HLT machine
            0xD3, 0x00, 0x00, 0xC9 // Treat BDOS entrypoint as OUT (0), RET
        };

        internal Cpu8080 Emulator { get; set; }

        private readonly byte[] _memory = new byte[0x10000];
        private readonly int _romLength;

        public CPMApplication(byte[] rom)
        {
            if (rom.Length > 0xFFFF - 0x100)
            {
                throw new ArgumentOutOfRangeException(nameof(rom), rom, "Rom too large");
            }

            Array.Copy(Bios, _memory, Bios.Length);
            for (var ii = Bios.Length; ii < 0x100; ii++)
            {
                _memory[ii] = 0x76; // Fill the rest of the bios with HLT so we find out about instructions that shouldn't be called
            }
            Array.Copy(rom, 0, _memory, 0x100, rom.Length);
            _romLength = rom.Length;
        }

        internal Span<byte> CompleteProgram() => _memory.AsSpan(0, Bios.Length + _romLength);

        public byte ReadByte(ushort address)
        {
#if  DEBUG
            Console.WriteLine($"    READ {address:X4}={_memory[address]:X2}");
#endif
            return _memory[address];
        }

        public void WriteByte(byte value, ushort address)
        {
#if DEBUG
            Console.WriteLine($"    WRITE {address:X4}={value:X2}");
#endif
            _memory[address] = value;
        }

        public void Out(byte port, byte _)
        {
            if (port != 0) return;

            var operation = (byte)Emulator.Internals.C.GetValue(Emulator.Emulator)!;
            // Value here is the BDOS function to call (that is, register C at point of call)
            switch (operation)
            {
                case 0: // System Reset
                    Environment.Exit(0);
                    break;
                case 1: // C_READ
                    var key = Console.ReadKey(false);
                    Emulator.Internals.A.SetValue(Emulator.Emulator, (byte)key.KeyChar);
                    break;
                case 2: // C_WRITE
                    Console.Write(Convert.ToChar(Emulator.Internals.E.GetValue(Emulator.Emulator)!));
                    break;
                case 9: // C_WRITESTR
                    var startIndex = (ushort)Emulator.Internals.DE.Invoke(Emulator.Emulator, Array.Empty<object>())!;
                    var length = 1;
                    while (startIndex + length < _memory.Length)
                    {
                        if (_memory[startIndex + length] == (byte) '$') break;
                        length++;
                    }

                    var stringInMemory = Encoding.ASCII.GetString(_memory.AsSpan(startIndex, length).ToArray());
                    Console.Write(stringInMemory);
                    break;
            }
        }

        public byte In(byte port) => 0x0;

        public void VBlank()
        {
            throw new NotImplementedException();
        }

        public void PreProgramEmit(ILGenerator methodIL)
        {
        }

        public void PostInstructionEmit(ILGenerator methodIL, CpuInternalBuilders internals, ushort programCounter)
        {
        }
    }
}