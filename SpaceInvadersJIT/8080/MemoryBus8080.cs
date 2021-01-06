using System;

namespace SpaceInvadersJIT._8080
{
    /// <summary>
    /// Provides an interface for the CPU to access whatever is connected to 
    /// it's address bus
    /// </summary>
    /// <remarks>
    /// Due to the simplicity of space invaders (no MMIO like GB, NES etc) this
    /// _could_ really just be a bare array in the generated IL and access to 
    /// it would not require the function call.
    /// 
    /// However, this architecture better demonstrates the emulation technique
    /// and would allow for it to be used in MMIO based architectures.
    /// </remarks>
    public class MemoryBus8080
    {
        private readonly byte[] _memory = new byte[0x4000];

        public MemoryBus8080(byte[] rom)
        {
            if (rom == null || rom.Length > 0x2000)
            {
                throw new ArgumentException("Invalid ROM, must be < 0x2000 bytes and non null", nameof(rom));
            }

            Array.Copy(rom, _memory, rom.Length);
        }

        private static ushort MirroredAddress(ushort address) => (ushort) (((address - 0x2000) & 0x1FFF) + 0x2000);

        public byte ReadByte(ushort address)
        {
            if (address < 0x2000)
            {
                return _memory[address];
            }
            else
            {
                return _memory[MirroredAddress(address)];
            }
        } 

        public void WriteByte(byte value, ushort address)
        {
            if (address < 0x2000) return;
            
            _memory[MirroredAddress(address)] = value;
        }
    }
}
