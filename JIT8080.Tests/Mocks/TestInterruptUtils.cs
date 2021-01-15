using System.Reflection.Emit;
using JIT8080._8080;
using JIT8080.Generator;

namespace JIT8080.Tests.Mocks
{
    public class TestInterruptUtils : IInterruptUtils
    {
        public void PreProgramEmit(ILGenerator methodIL)
        {
        }

        public void PostInstructionEmit(ILGenerator methodIL, CpuInternalBuilders internals, ushort programCounter)
        {
            
        }
    }
}