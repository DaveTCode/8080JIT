namespace JIT8080._8080
{
    /// <summary>
    /// This interface is passed to the CPU emulator and is fired on each
    /// VBlank interrupt allowing the calling code to perform draw (or other)
    /// functions whilst the CPU is halted
    /// </summary>
    public interface IRenderer
    {
        public void VBlank();
    }
}
