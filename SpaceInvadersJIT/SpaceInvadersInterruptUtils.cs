using System.Reflection.Emit;
using JIT8080._8080;
using JIT8080.Generator;

namespace SpaceInvadersJIT
{
    public class SpaceInvadersInterruptUtils : IInterruptUtils
    {
        private LocalBuilder _interruptDownCounter;
        private LocalBuilder _oldInterruptDownCounter;
        
        public void PreProgramEmit(ILGenerator methodIL)
        {
            // Track half screen and full screen interrupts using cycle down counters
            _interruptDownCounter = methodIL.DeclareLocal(typeof(long));
            _oldInterruptDownCounter = methodIL.DeclareLocal(typeof(long));
            methodIL.Emit(OpCodes.Ldc_I8, SpaceInvadersApplication.CyclesPerScreen);
            methodIL.Emit(OpCodes.Stloc, _interruptDownCounter.LocalIndex);
        }

        public void PostInstructionEmit(ILGenerator methodIL, CpuInternalBuilders internals, ushort programCounter)
        {
            // Check whether to fire half screen interrupt
            var skipInterrupt = methodIL.DefineLabel();
            var skipHalfScreen = methodIL.DefineLabel();

            // Always decrement cycles to interrupt
            methodIL.Emit(OpCodes.Ldloc, _interruptDownCounter.LocalIndex);
            methodIL.Emit(OpCodes.Stloc, _oldInterruptDownCounter.LocalIndex);
            methodIL.Emit(OpCodes.Ldloc, _interruptDownCounter.LocalIndex);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.CycleCounter);
            methodIL.Emit(OpCodes.Sub);
            methodIL.Emit(OpCodes.Stloc, _interruptDownCounter.LocalIndex);
            
            // Check if we've just crossed the half screen boundary
            methodIL.Emit(OpCodes.Ldloc, _interruptDownCounter.LocalIndex);
            methodIL.Emit(OpCodes.Ldc_I8, SpaceInvadersApplication.HalfCyclesPerScreen + 1);
            methodIL.Emit(OpCodes.Clt);
            methodIL.Emit(OpCodes.Ldloc, _oldInterruptDownCounter.LocalIndex);
            methodIL.Emit(OpCodes.Ldc_I8, SpaceInvadersApplication.HalfCyclesPerScreen);
            methodIL.Emit(OpCodes.Cgt);
            methodIL.Emit(OpCodes.And);
            methodIL.Emit(OpCodes.Brfalse, skipHalfScreen);
#if DEBUG
            methodIL.EmitWriteLine("Half screen interrupt");
#endif
            // Actually _do_ the half screen interrupt (RST 0)
            // If IE flag set then skip interrupt check
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.InterruptEnable);
            methodIL.Emit(OpCodes.Brfalse, skipInterrupt);
#if DEBUG
            methodIL.EmitWriteLine("Half screen interrupt EI enabled");
#endif
            var halfEmitter = new CallEmitter(0xCF, programCounter, internals.ProgramLabels[0x08], 0x08);
            halfEmitter.Emit(methodIL, internals);
            methodIL.Emit(OpCodes.Br, skipInterrupt);

            // Skipped the half screen so now check for full screen interrupt
            methodIL.MarkLabel(skipHalfScreen);
            
            methodIL.Emit(OpCodes.Ldc_I8, 0L);
            methodIL.Emit(OpCodes.Ldloc, _interruptDownCounter);
            methodIL.Emit(OpCodes.Blt, skipInterrupt);
            
#if DEBUG
            methodIL.EmitWriteLine("Full screen interrupt");
#endif
            
            // Fire the vblank routine on the renderer
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.MemoryBusField);
            methodIL.Emit(OpCodes.Callvirt, internals.RendererField.FieldType.GetMethod("VBlank")!);
            
            // Reset the interrupt down counter
            methodIL.Emit(OpCodes.Ldc_I8, SpaceInvadersApplication.CyclesPerScreen);
            methodIL.Emit(OpCodes.Stloc, _interruptDownCounter.LocalIndex);
            
            // Fire the vblank interrupt routine
            // If IE flag not set then skip interrupt fire (but still do vblank call)
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, internals.InterruptEnable);
            methodIL.Emit(OpCodes.Brfalse, skipInterrupt);
            var fullEmitter = new CallEmitter(0xD7, programCounter, internals.ProgramLabels[0x10], 0x10);
            fullEmitter.Emit(methodIL, internals);
            
            methodIL.MarkLabel(skipInterrupt);
        }
    }
}