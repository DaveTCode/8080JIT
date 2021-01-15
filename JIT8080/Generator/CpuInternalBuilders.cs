using System.Reflection;
using System.Reflection.Emit;

namespace JIT8080.Generator
{
    public class CpuInternalBuilders
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
        public FieldBuilder InterruptEnable;
        public FieldBuilder CycleCounter;

        public FieldBuilder MemoryBusField;
        public FieldBuilder IOHandlerField;
        public FieldBuilder RendererField;

        /// <summary>
        /// We emit a label for each 8080 opcode to make it possible to jump
        /// to arbitrary addresses
        /// </summary>
        public Label[] ProgramLabels;

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
