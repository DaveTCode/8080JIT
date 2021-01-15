using System.Reflection;
using System.Reflection.Emit;
using JIT8080.Generator;

namespace JIT8080._8080
{
    /// <summary>
    /// Interrupts are handled in different ways on different systems,
    /// to support firing interrupts at the right moment this interface can
    /// be overridden to inject appropriate IL stream commands.
    /// </summary>
    public interface IInterruptUtils
    {
        /// <summary>
        /// This is typically used to declare locals and set their values and
        /// occurs at the start of the method
        /// </summary>
        public void PreProgramEmit(ILGenerator methodIL);

        /// <summary>
        /// This is called after each instruction executes and is used to
        /// check whether an interrupt should be fired.
        /// </summary>
        public void PostInstructionEmit(ILGenerator methodIL, CpuInternalBuilders internals, ushort programCounter);
    }
}