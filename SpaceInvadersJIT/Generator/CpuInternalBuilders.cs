using System.Reflection;
using System.Reflection.Emit;

namespace SpaceInvadersJIT.Generator
{
    internal class CpuInternalBuilders
    {
        internal FieldBuilder A;
        internal FieldBuilder B;
        internal FieldBuilder C;
        internal FieldBuilder D;
        internal FieldBuilder E;
        internal FieldBuilder H;
        internal FieldBuilder L;
        internal FieldBuilder StackPointer;
        internal FieldBuilder SignFlag;
        internal FieldBuilder ZeroFlag;
        internal FieldBuilder AuxCarryFlag;
        internal FieldBuilder ParityFlag;
        internal FieldBuilder CarryFlag;
        internal MethodBuilder HL;
        internal MethodBuilder BC;
        internal MethodBuilder DE;
        internal MethodBuilder SetHL;
        internal MethodBuilder SetBC;
        internal MethodBuilder SetDE;
        internal MethodBuilder GetFlagRegister;
        internal MethodBuilder SetFlagRegister;
        internal FieldInfo InterruptEnable;

        internal Label[] ProgramLabels;
    }
}
