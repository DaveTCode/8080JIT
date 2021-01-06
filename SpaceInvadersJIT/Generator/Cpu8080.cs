using System.Reflection;

namespace SpaceInvadersJIT.Generator
{
    /// <summary>
    /// Represents the internals of an 8080 that do not 1-1 map to CLR IL concepts
    /// </summary>
    internal class Cpu8080
    {
        internal object Emulator;

        internal MethodInfo Run;

        internal Cpu8080Internals Internals;
    }

    internal class Cpu8080Internals
    {
        internal FieldInfo A;
        internal FieldInfo B;
        internal FieldInfo C;
        internal FieldInfo D;
        internal FieldInfo E;
        internal FieldInfo H;
        internal FieldInfo L;

        internal MethodInfo BC;
        internal MethodInfo DE;
        internal MethodInfo HL;

        internal FieldInfo StackPointer;

        // TODO - Do I need to track a program counter as well?

        // TODO - Are any of these flags replaceable with native BR instructions in CLR IL? Seems like no but not 100% sure.
        internal FieldInfo SignFlag;
        internal FieldInfo ZeroFlag;
        internal FieldInfo AuxCarryFlag;
        internal FieldInfo ParityFlag;
        internal FieldInfo CarryFlag;

        internal MethodInfo GetFlagRegister;
        internal MethodInfo SetFlagRegister;

        internal FieldInfo InterruptEnable;
    }
}
