namespace JIT8080._8080
{
    internal enum Register
    {
        A = 0b111,
        B = 0b000,
        C = 0b001,
        D = 0b010,
        E = 0b011,
        H = 0b100,
        L = 0b101,
        M = 0b110, // Memory address referred to by contents of HL
    }
}
