using System.Reflection;
using System.Reflection.Emit;

namespace JIT8080.Generator
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

        /// <summary>
        /// The result of an operation is cached off so that we can calculate
        /// the flags (zero, sign, carry & parity at point of use) instead
        /// of on each arithmetic operation
        /// </summary>
        /// <remarks>
        /// Note that this is stored as an int so that we can get the carry bit
        /// </remarks>
        internal FieldBuilder Result;

        /// <summary>
        /// We emit a label for each 8080 opcode to make it possible to jump
        /// to arbitrary addresses
        /// </summary>
        internal Label[] ProgramLabels;

        /// <summary>
        /// Common variable to hold the destination of a runtime jump (e.g. PCHL)
        ///
        /// Used by the jump table to determine where to load
        /// </summary>
        internal LocalBuilder DestinationAddress;

        /// <summary>
        /// The jump table is a static area of code at the start of the main method to
        /// avoid reproducing a large block on each jump operation.
        /// </summary>
        internal Label JumpTableStart;
    }
}
