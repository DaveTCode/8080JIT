namespace JIT8080._8080
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
    public interface IMemoryBus8080
    {
        public byte ReadByte(ushort address);

        public void WriteByte(byte value, ushort address);
    }
}
