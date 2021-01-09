using System.Reflection;

namespace JIT8080.Generator
{
    /// <summary>
    /// Represents the internals of an 8080 that do not 1-1 map to CLR IL concepts
    /// </summary>
    public class Cpu8080
    {
        public object Emulator;

        public MethodInfo Run;

        public Cpu8080Internals Internals;
    }

    public class Cpu8080Internals
    {
        public FieldInfo A;
        public FieldInfo B;
        public FieldInfo C;
        public FieldInfo D;
        public FieldInfo E;
        public FieldInfo H;
        public FieldInfo L;

        public MethodInfo BC;
        public MethodInfo DE;
        public MethodInfo HL;

        public FieldInfo StackPointer;

        // TODO - Do I need to track a program counter as well?

        // TODO - Are any of these flags replaceable with native BR instructions in CLR IL? Seems like no but not 100% sure.
        public FieldInfo SignFlag;
        public FieldInfo ZeroFlag;
        public FieldInfo AuxCarryFlag;
        public FieldInfo ParityFlag;
        public FieldInfo CarryFlag;
        
        public FieldInfo Result;

        public MethodInfo GetFlagRegister;
        public MethodInfo SetFlagRegister;

        public FieldInfo InterruptEnable;
    }
}
