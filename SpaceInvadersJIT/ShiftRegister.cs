namespace SpaceInvadersJIT
{
    internal struct ShiftRegister
    {
        internal ushort Register;
        internal byte Offset;

        internal byte Value() => (byte) (Register >> (8 - Offset));
    }
}