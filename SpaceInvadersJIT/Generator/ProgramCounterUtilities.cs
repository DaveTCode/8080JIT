using System.Linq;
using System.Reflection.Emit;

namespace SpaceInvadersJIT.Generator
{
    internal static class ProgramCounterUtilities
    {
        /// <summary>
        /// Various operations in an 8080 allow for jumping to an arbitrary
        /// address (which may be in ROM, RAM, VRAM, MMIO etc).
        ///
        /// This function supports that with a very naive linear search
        /// through a jump table.
        ///
        /// TODO - Improve this from O(n) to O(log(n)) complexity using a
        /// binary chop through the jump table
        /// </summary>
        /// 
        /// <param name="methodIL">
        /// The IL generator in which to place this macro
        /// </param>
        ///
        /// <param name="programLabels">
        /// A label per byte in the entire program space
        /// </param>
        ///
        /// <param name="destinationAddress">
        /// A local containing the destination program counter
        /// </param>
        internal static void JumpToDynamicAddress(ILGenerator methodIL, Label[] programLabels,
            LocalBuilder destinationAddress)
        {
            foreach (var (ix, programLabel) in programLabels.Select((l, ix) => (ix, l)))
            {
                methodIL.Emit(OpCodes.Ldc_I4, ix);
                methodIL.Emit(OpCodes.Ldloc, destinationAddress.LocalIndex);
                methodIL.Emit(OpCodes.Beq, programLabel);
            }

            // TODO [HARD] - Handle jumps outside of program ROM
        }
    }
}